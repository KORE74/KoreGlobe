using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Godot;

public partial class FssImageManager : Node
{
    // Cache to hold the textures for map tiles.
    private ConcurrentDictionary<string, Texture> TextureCache     = new ConcurrentDictionary<string, Texture>();
    private ConcurrentDictionary<string, Task>    LoadingTasksList = new ConcurrentDictionary<string, Task>();
    private ConcurrentDictionary<string, int>     ExpireTimeList   = new ConcurrentDictionary<string, int>();

    // Time for textures to keep alive or next check.
    private const int KeepAliveTime = 10;
    private const int ActionTimerIncrement = 2;

    // Periodic action timer.
    private int ActionTimer = 0;

    // --------------------------------------------------------------------------------------------
    // MARK: Node functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        Name = "ImageManager";
    }

    // Called every frame to handle the keep-alive functionality.
    public override void _Process(double delta)
    {
        // Periodically delete the oldest expired image, reduce memory footprint.
        if (ActionTimer < FssCentralTime.RuntimeIntSecs)
        {
            ActionTimer = FssCentralTime.RuntimeIntSecs + ActionTimerIncrement;
            DeleteOldestExpiredImage();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Start Image Loading
    // --------------------------------------------------------------------------------------------

    // Starts loading an image asynchronously. Non-blocking.
    public async void StartImageLoading(string imagePath)
    {
        Texture texture = await LoadTextureAsync(imagePath);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Background loading
    // --------------------------------------------------------------------------------------------

    // Loads a texture from a resource path asynchronously.
    private async Task<Texture> LoadTextureAsync(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture existingTexture))
        {
            ExpireTimeList[path] = FssCentralTime.RuntimeIntSecs;
            return existingTexture;
        }

        if (!LoadingTasksList.ContainsKey(path))
        {
            // Load texture in the background.
            Task loadingTask = Task.Run(() =>
            {
                try
                {
                    var image = (Image)ResourceLoader.Load(path);
                    if (image == null)
                    {
                        GD.PrintErr($"Failed to load image at path: {path}");
                        return;
                    }
                    TextureCache[path]   = ImageTexture.CreateFromImage(image); // Put the image in the cache as a texture.
                    ExpireTimeList[path] = FssCentralTime.RuntimeIntSecs + KeepAliveTime; // Set the expiration time.
                }
                catch (Exception ex)
                {
                    GD.PrintErr("", ex);
                }
            });

            LoadingTasksList[path] = loadingTask;
        }

        await LoadingTasksList[path];
        LoadingTasksList.TryRemove(path, out _);
        TextureCache.TryGetValue(path, out Texture loadedTexture);
        return loadedTexture;
    }

    // Checks if the image has been loaded and is available in the cache.
    public bool IsImageLoaded(string path)
    {
        return TextureCache.ContainsKey(path);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Ping
    // --------------------------------------------------------------------------------------------

    // Keeps a texture alive by updating its last access time.
    public void KeepAlive(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            ExpireTimeList[path] = FssCentralTime.RuntimeIntSecs + KeepAliveTime;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Expired Tile Deletion
    // --------------------------------------------------------------------------------------------

    private void DeleteExpiredImage()
    {
        foreach (var key in ExpireTimeList.Keys)
        {
            if ((ExpireTimeList.TryGetValue(key, out int expireTime)) &&
                (expireTime < FssCentralTime.RuntimeIntSecs))
            {
                // Remove the texture to free up memory.
                if (TextureCache.TryRemove(key, out Texture texture))
                {
                    texture.Dispose();
                    ExpireTimeList.TryRemove(key, out _);
                }
            }
        }
    }

    // Deletes the oldest, most out-of-date image from the cache.
    private void DeleteOldestExpiredImage()
    {
        // Delete the oldest image only if it has exceeded the KeepAliveTime.
        if (ExpireTimeList.Count == 0)
            return;

        var oldestEntry  = ExpireTimeList.OrderBy(kvp => kvp.Value).FirstOrDefault();
        string oldestKey = oldestEntry.Key;

        if ((!string.IsNullOrEmpty(oldestKey)) &&
            (oldestEntry.Value < FssCentralTime.RuntimeIntSecs))
        {
            // Only delete the oldest image if it has indeed expired.
            if (TextureCache.TryRemove(oldestKey, out Texture texture))
            {
                texture.Dispose();
                ExpireTimeList.TryRemove(oldestKey, out _);
            }
        }
    }
}
