using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Godot;

#nullable enable

public partial class FssImageManager : Node
{
    // Cache to hold the textures for map tiles.
    private ConcurrentDictionary<string, Texture> TextureCache   = new ConcurrentDictionary<string, Texture>();
    private ConcurrentDictionary<string, Task>    LoadingTasks   = new ConcurrentDictionary<string, Task>();
    private ConcurrentDictionary<string, int>     LastAccessList = new ConcurrentDictionary<string, int>();

    // Time for textures to keep alive (in seconds).
    private const int KeepAliveTime = 10;
    private const int ActionTimerIncrement = 2;
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
        // Periodically delete the expired textures
        if (ActionTimer <= FssCentralTime.RuntimeIntSecs)
        {
            ActionTimer = FssCentralTime.RuntimeIntSecs + ActionTimerIncrement;
            DeleteOldestExpiredTextures();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Start Image Loading
    // --------------------------------------------------------------------------------------------

    // Starts loading an image asynchronously. Non-blocking.
    public async Task StartImageLoading(string imagePath)
    {
        await Task.Run(async () => await LoadTextureAsync(imagePath));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Background loading
    // --------------------------------------------------------------------------------------------

    // Loads a texture from a resource path asynchronously.
    private async Task LoadTextureAsync(string path)
    {
        // Update the last access time if the texture is already loaded, and return.
        if (TextureCache.TryGetValue(path, out Texture? texture) && texture != null)
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return;
        }

        // if we are not already loading the texture, load it in the background.
        if (!LoadingTasks.ContainsKey(path))
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

                    ImageTexture texture = ImageTexture.CreateFromImage(image);

                    TextureCache[path]   = texture;
                    LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
                }
                catch (Exception ex)
                {
                    GD.PrintErr("Error loading texture:", ex);
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
    }

    // Checks if the image has been loaded and is available in the cache.
    public bool HasImage(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    public Texture? GetTexture(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture? texture) && texture != null)
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return texture;
        }
        return null;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Ping
    // --------------------------------------------------------------------------------------------

    // Keeps a texture alive by updating its last access time.
    public void KeepAlive(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Expired Tile Deletion
    // --------------------------------------------------------------------------------------------

    private void DeleteExpiredTextures()
    {
        if (TextureCache.Count == 0)
            return;

        int currentTime = FssCentralTime.RuntimeIntSecs;
        foreach (var key in LastAccessList.Keys)
        {
            if (LastAccessList.TryGetValue(key, out int lastAccess) && currentTime - lastAccess > KeepAliveTime)
            {
                // Remove the texture to free up memory.
                if (TextureCache.TryRemove(key, out Texture? texture) && texture != null)
                {
                    texture.Dispose();
                    LastAccessList.TryRemove(key, out _);
                }
            }
        }
    }

    // Deletes the oldest, most out-of-date image from the cache.
    public void DeleteOldestExpiredTextures()
    {
        // Delete the oldest image only if it has exceeded the KeepAliveTime.
        if (LastAccessList.Count == 0)
            return;

        var currentTime = FssCentralTime.RuntimeIntSecs;
        var oldestEntry = LastAccessList.OrderBy(kvp => kvp.Value).FirstOrDefault();
        string oldestKey = oldestEntry.Key;

        if (!string.IsNullOrEmpty(oldestKey) &&
            (currentTime - oldestEntry.Value > KeepAliveTime))
        {
            // Only delete the oldest image if it has indeed expired.
            if (TextureCache.TryRemove(oldestKey, out Texture? texture) && texture != null)
            {
                texture.Dispose();
                LastAccessList.TryRemove(oldestKey, out _);
            }
        }
    }
}
