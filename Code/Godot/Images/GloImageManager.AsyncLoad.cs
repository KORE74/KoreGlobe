using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Godot;
using System.Linq;

#nullable enable

public partial class GloImageManager : Node
{

    // --------------------------------------------------------------------------------------------
    // MARK: WebP
    // --------------------------------------------------------------------------------------------

    // Loads a texture from a resource path asynchronously.
    private async Task<Texture2D?> LoadWebpTextureAsync(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture2D? existingTexture))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
            return existingTexture;
        }

        if (!LoadingTasks.ContainsKey(path))
        {
            // Load texture in the background.
            Task loadingTask = Task.Run(() =>
            {
                try
                {
                    // Load image synchronously
                    var image = new Image();
                    var err = image.Load(path);
                    if (err != Error.Ok)
                    {
                        GloCentralLog.AddEntry($"Failed to load image: {path}");
                        //return null;
                    }

                    // Create the texture synchronously
                    var texture = ImageTexture.CreateFromImage(image);

                    // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
                    image.Dispose();

                    TextureCache[path]   = texture;
                    LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;

                    GloCentralLog.AddEntry($"LOADED Async {path} // {TextureCache.Count}");
                }
                catch (Exception ex)
                {
                    GloCentralLog.AddEntry($"EXCEPTION: LoadTextureAsync: {ex.Message}");
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
        TextureCache.TryGetValue(path, out Texture2D? loadedTexture);
        return loadedTexture;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: PNG
    // --------------------------------------------------------------------------------------------

    // Loads a texture from a resource path asynchronously.
    private async Task<Texture2D?> LoadPngTextureAsync(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture2D? existingTexture))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
            return existingTexture;
        }

        if (!LoadingTasks.ContainsKey(path))
        {
            // Load texture in the background.
            Task loadingTask = Task.Run(() =>
            {
                try
                {
                    // Load image synchronously
                    var image = new Image();
                    var err = image.Load(path);
                    if (err != Error.Ok)
                    {
                        GloCentralLog.AddEntry($"Failed to load image: {path}");
                        //return null;
                    }

                    // Create the texture synchronously
                    var texture = ImageTexture.CreateFromImage(image);

                    // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
                    image.Dispose();

                    TextureCache[path]   = texture;
                    LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;

                    GloCentralLog.AddEntry($"LOADED {path} // {TextureCache.Count}");
                }
                catch (Exception ex)
                {
                    GloCentralLog.AddEntry($"EXCEPTION: LoadTextureAsync: {ex.Message}");
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
        TextureCache.TryGetValue(path, out Texture2D? loadedTexture);
        return loadedTexture;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: KTX2
    // --------------------------------------------------------------------------------------------

    // Load the KTX2 compressed image directly into a Texture2D object.

    private async Task<Texture2D?> LoadKtx2TextureAsync(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture2D? existingTexture))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
            return existingTexture;
        }

        if (!LoadingTasks.ContainsKey(path))
        {
            Task loadingTask = Task.Run(() =>
            {
                try
                {
                    var texture = ResourceLoader.Load<Texture2D>(path);
                    if (texture != null)
                    {
                        TextureCache[path]   = texture;
                        LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
                        GloCentralLog.AddEntry($"LOADED {path} // {TextureCache.Count}");
                    }
                    else
                    {
                        GloCentralLog.AddEntry($"Failed to load KTX2 texture: {path}");
                    }
                }
                catch (Exception ex)
                {
                    GloCentralLog.AddEntry($"EXCEPTION: LoadKtx2TextureAsync: {ex.Message}");
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
        TextureCache.TryGetValue(path, out Texture2D? loadedTexture);
        return loadedTexture;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: CTEX
    // --------------------------------------------------------------------------------------------

    private async Task<Texture2D?> LoadCtexTextureAsync(string path)
    {
        if (TextureCache.TryGetValue(path, out Texture2D? existingTexture))
        {
            LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;
            return existingTexture;
        }

        if (!LoadingTasks.ContainsKey(path))
        {
            // Load texture in the background.
            Task loadingTask = Task.Run(() =>
            {
                try
                {
                    CompressedTexture2D texture = GD.Load<CompressedTexture2D>(path);

                    TextureCache[path]   = texture;
                    LastAccessTime[path] = GloCentralTime.RuntimeIntSecs;

                    GloCentralLog.AddEntry($"LOADED {path} // {TextureCache.Count}");
                }
                catch (Exception ex)
                {
                    GloCentralLog.AddEntry($"EXCEPTION: LoadTextureDirectCtex: {ex.Message}");
                }
            });

            LoadingTasks[path] = loadingTask;
        }

        await LoadingTasks[path];
        LoadingTasks.TryRemove(path, out _);
        TextureCache.TryGetValue(path, out Texture2D? loadedTexture);
        return loadedTexture;

    }

}
