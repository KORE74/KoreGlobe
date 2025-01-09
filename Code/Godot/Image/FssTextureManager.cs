using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

using Godot;
using System.IO;

#nullable enable

public partial class FssTextureManager : Node
{
    // Cache to hold the textures for map tiles.
    private ConcurrentDictionary<string, ImageTexture> TextureCache   = new ConcurrentDictionary<string, ImageTexture>();
    private ConcurrentDictionary<string, Task>         LoadingTasks   = new ConcurrentDictionary<string, Task>();
    private ConcurrentDictionary<string, int>          LastAccessList = new ConcurrentDictionary<string, int>();

    // Time for textures to keep alive (in seconds).
    private const int KeepAliveTimeSecs        = 10;
    private const int ActionTimerIncrementSecs = 2;
    private int ActionTimer = 0;

    // --------------------------------------------------------------------------------------------
    // MARK: Node functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        Name = "TextureManager";
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame to handle the keep-alive functionality.
    public override void _Process(double delta)
    {
        // Periodically delete the expired textures
        if (ActionTimer <= FssCentralTime.RuntimeIntSecs)
        {
            ActionTimer = FssCentralTime.RuntimeIntSecs + ActionTimerIncrementSecs;
            DeleteOldestExpiredTextures();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Start Image Loading
    // --------------------------------------------------------------------------------------------

    // Starts loading an image asynchronously. Non-blocking.
    public void StartTextureLoading(string imagePath)
    {
        _ = Task.Run(() => LoadTextureAsync(imagePath));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Background loading
    // --------------------------------------------------------------------------------------------

    // Loads a texture from a resource path asynchronously.
    private async Task LoadTextureAsync(string path)
    {
        await Task.Yield();

        // Update the last access time if the texture is already loaded, and return.
        if (TextureCache.TryGetValue(path, out ImageTexture? texture) && texture != null)
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
                    // Log the load request
                    FssCentralLog.AddEntry($"TextureManager: Loading request texture: {path}");

                    // If the file doesn't exist, fail the operation
                    if (!File.Exists(path))
                    {
                        FssCentralLog.AddEntry($"TextureManager: Texture file not found: {path}");
                        return;
                    }

                    // Fire off the actual inage loading.
                    var image = (Image)ResourceLoader.Load(path);
                    if (image == null)
                    {
                        FssCentralLog.AddEntry($"TextureManager: Failed to load image at path: {path}");
                        return;
                    }

                    ImageTexture newTexture = ImageTexture.CreateFromImage(image);

                    TextureCache[path]   = newTexture;
                    LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
                }
                catch (Exception ex)
                {
                    FssCentralLog.AddEntry($"TextureManager: Exception loading texture async: {path} Exception message: {ex.Message}");
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
    }

    // --------------------------------------------------------------------------------------------

    // Load a texture directly

    public void LoadTextureDirect(string path)
    {
        // Update the last access time if the texture is already loaded, and return.
        if (TextureCache.TryGetValue(path, out ImageTexture? cachedTexture) && cachedTexture != null)
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return;
        }

        try
        {
            // Log the load request
            FssCentralLog.AddEntry($"TextureManager: Loading request texture: {path}");

            // Check if the file exists
            if (!File.Exists(path))
            {
                FssCentralLog.AddEntry($"TextureManager: Texture file not found: {path}");
                return;
            }

            // Load the image using Godot's Image class (supports WebP directly)
            var image = new Image();
            var loadError = image.Load(path);
            if (loadError != Error.Ok)
            {
                FssCentralLog.AddEntry($"TextureManager: Failed to load image at path: {path}. Error: {loadError}");
                image.Dispose();
                return;
            }

            // Create a texture from the loaded image
            var newTexture = ImageTexture.CreateFromImage(image);
            image.Dispose(); // Release the image resource

            // Add the texture to the cache and update the last access time
            TextureCache[path] = newTexture;
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;

            FssCentralLog.AddEntry($"TextureManager: Successfully loaded and cached texture: {path}");
        }
        catch (Exception ex)
        {
            // Log any exceptions during the texture load process
            FssCentralLog.AddEntry($"TextureManager: Exception loading texture direct: {path}. Exception: {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    // Checks if the image has been loaded and is available in the cache.
    public bool HasTexture(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    // --------------------------------------------------------------------------------------------

    public ImageTexture? GetTexture(string path)
    {
        if (TextureCache.TryGetValue(path, out ImageTexture? texture) && texture != null)
        {
            LastAccessList[path] = FssCentralTime.RuntimeIntSecs;
            return texture;
        }
        return null;
    }

    // --------------------------------------------------------------------------------------------

    public StandardMaterial3D? GetMaterialWithTexture(string filePath)
    {
        ImageTexture? tex = GetTexture(filePath);
        if (tex != null)
        {
            var material = new StandardMaterial3D
            {
                AlbedoTexture = tex,
                ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
            };
            return material;
        }

        return null;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Ping
    // --------------------------------------------------------------------------------------------

    // Keeps a texture alive by updating its last access time.
    public void RefreshTextureTimeout(string path)
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
            if (LastAccessList.TryGetValue(key, out int lastAccess) && currentTime - lastAccess > KeepAliveTimeSecs)
            {
                // Remove the texture to free up memory.
                if (TextureCache.TryRemove(key, out ImageTexture? texture) && texture != null)
                {
                    texture.Dispose();
                    LastAccessList.TryRemove(key, out _);

                    FssCentralLog.AddEntry($"TextureManager: Deleted expired texture: {key}");
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------

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
            (currentTime - oldestEntry.Value > KeepAliveTimeSecs))
        {
            // Only delete the oldest image if it has indeed expired.
            if (TextureCache.TryRemove(oldestKey, out ImageTexture? texture) && texture != null)
            {
                texture.Dispose();
                LastAccessList.TryRemove(oldestKey, out _);

                FssCentralLog.AddEntry($"TextureManager: Deleted expired texture: {oldestKey}");
            }
        }
    }
}
