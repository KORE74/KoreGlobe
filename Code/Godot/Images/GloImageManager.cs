using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Godot;
using System.Linq;

#nullable enable

public partial class GloImageManager : Node
{
    // Cache to hold the textures for map tiles.
    private ConcurrentDictionary<string, Texture2D> TextureCache   = new ConcurrentDictionary<string, Texture2D>();
    private ConcurrentDictionary<string, Task>      LoadingTasks   = new ConcurrentDictionary<string, Task>();
    private ConcurrentDictionary<string, int>       LastAccessTime = new ConcurrentDictionary<string, int>();

    // Time for textures to keep alive (in seconds).
    private const float KeepAliveTime        = 10;
    private const float ActionTimerIncrement = 0.2f;

    private float ActionTimer = 0f;

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
        if (ActionTimer < GloCentralTime.RuntimeSecs)
        {
            ActionTimer = GloCentralTime.RuntimeSecs + ActionTimerIncrement;
            DeleteExpiredTextures();
            //DeleteOldestExpiredTextures();

            GloCentralLog.AddEntry($"Textures: {TextureCache.Count}");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Start Image Loading
    // --------------------------------------------------------------------------------------------

    // Starts loading an image asynchronously. Non-blocking.
    public async void StartImageLoading(string imagePath)
    {
        // Determine the file extension
        string extension = GloFileOperations.GetExtension(imagePath);

        Texture2D? texture = null;

        // Call the appropriate method based on the file type
        if      (extension == ".ktx2") texture = await LoadKtx2TextureAsync(imagePath);
        else if (extension == ".ctex") texture = await LoadCtexTextureAsync(imagePath);
        else if (extension == ".png")  texture = await LoadPngTextureAsync(imagePath);
        else if (extension == "webp")  texture = await LoadWebpTextureAsync(imagePath);
    }

    public Texture2D? LoadImage(string imagePath)
    {
        // Determine the file extension
        string extension = GloFileOperations.GetExtension(imagePath);

        Texture2D? texture = null;

        // Call the appropriate method based on the file type
        if      (extension == ".ktx2") texture = LoadKtx2Texture(imagePath);
        else if (extension == ".ctex") texture = LoadCtexTexture(imagePath);
        else if (extension == ".png")  texture = LoadPngTexture(imagePath);
        else if (extension == "webp")  texture = LoadWebpTexture(imagePath);
        return texture;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Access Texture
    // --------------------------------------------------------------------------------------------

    // Checks if the image has been loaded and is available in the cache.
    public bool HasTexture(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    public Texture2D? GetTexture(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture2D? existingTexture))
        {
            return existingTexture;
        }
        return null;
    }

    public StandardMaterial3D? GetMaterialWithTexture(string filePath)
    {
        Texture2D? tex = GetTexture(filePath);
        if (tex != null)
        {
            var material = new StandardMaterial3D
            {
                AlbedoTexture = tex!,
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
    public void KeepAlive(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Expired Tile Deletion
    // --------------------------------------------------------------------------------------------

    private void DeleteExpiredTextures()
    {
        if (TextureCache.Count == 0)
            return;

        int currentTime = GloCentralTime.RuntimeIntSecs;
        foreach (var key in LastAccessTime.Keys)
        {
            if (LastAccessTime.TryGetValue(key, out int lastAccess) && currentTime - lastAccess > KeepAliveTime)
            {
                // Remove the texture to free up memory.
                if (TextureCache.TryRemove(key, out Texture2D? texture))
                {
                    texture.Dispose();
                    LastAccessTime.TryRemove(key, out _);

                    GloCentralLog.AddEntry($"Deleted Expired {key} // {TextureCache.Count}");
                }
            }
        }
    }

    // Deletes the oldest, most out-of-date image from the cache.
    public void DeleteOldestExpiredTextures()
    {
        // Delete the oldest image only if it has exceeded the KeepAliveTime.
        if (LastAccessTime.Count == 0)
            return;

        var currentTime = GloCentralTime.RuntimeIntSecs;
        var oldestEntry = LastAccessTime.OrderBy(kvp => kvp.Value).FirstOrDefault();
        string oldestKey = oldestEntry.Key;

        if (!string.IsNullOrEmpty(oldestKey) &&
            (currentTime - oldestEntry.Value > KeepAliveTime))
        {
            // Only delete the oldest image if it has indeed expired.
            if (TextureCache.TryRemove(oldestKey, out Texture2D? texture))
            {
                texture.Dispose();
                LastAccessTime.TryRemove(oldestKey, out _);

                GloCentralLog.AddEntry($"Deleted Oldest Expired {oldestKey} // {TextureCache.Count}");
            }
        }
    }

}
