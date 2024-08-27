#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using System.Text;

using Godot;

public partial class FssTextureLoader
{
    // Cache to store loaded textures
    private readonly Dictionary<string, ImageTexture> _textureCache = new Dictionary<string, ImageTexture>();

    // Queue to manage texture loading requests
    private readonly Queue<string> _loadQueue = new Queue<string>();

    // Lock object for thread safety
    private readonly object _lock = new object();

    // ------------------------------------------------------------------------------------------------

    // Standard C# singleton pattern

    private static FssTextureLoader? _instance;

    public static FssTextureLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FssTextureLoader();
            }
            return _instance;
        }
    }

    // ------------------------------------------------------------------------------------------------

    public ImageTexture? LoadTextureDirect(string filePath)
    {
        lock (_lock)
        {
            // Check if the texture is already loaded
            if (_textureCache.ContainsKey(filePath))
            {
                //GD.Print($"Texture already loaded: {filePath}");
                return _textureCache[filePath];
            }
        }

        // Load image synchronously
        var image = new Image();
        var err = image.Load(filePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load image: {filePath}");
            return null;
        }

        // Create the texture synchronously
        var texture = ImageTexture.CreateFromImage(image);

        // Cache the texture
        lock (_lock)
        {
            if (!_textureCache.ContainsKey(filePath))
            {
                _textureCache[filePath] = texture;
            }
        }

        // GD.Print($"Loaded and created texture directly: {filePath}");

        return texture;
    }

    // ------------------------------------------------------------------------------------------------

    private void CreateTexture(string filePath, Image image)
    {
        lock (_lock)
        {
            if (!_textureCache.ContainsKey(filePath))
            {
                var texture = ImageTexture.CreateFromImage(image);
                _textureCache[filePath] = texture;

                // GD.Print($"Loaded and created texture: {filePath}");
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    public static bool IsFileLoadable(string filePath)
    {
        return File.Exists(filePath);
    }

    // ------------------------------------------------------------------------------------------------

    public void QueueTexture(string filePath)
    {
        if (IsFileLoadable(filePath))
        {
            lock (_lock)
            {
                if (!_textureCache.ContainsKey(filePath) && !_loadQueue.Contains(filePath))
                {
                    _loadQueue.Enqueue(filePath);

                    //GD.Print($"Queued texture_: {filePath} ({_loadQueue.Count})");
                }
                else
                {
                    //GD.Print($"Texture already loaded or queued: {filePath}");
                }
            }
        }
        else
        {
            GD.PrintErr($"File not loadable: {filePath}");
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void ReleaseTexture(string filePath)
    {
        lock (_lock)
        {
            if (_textureCache.ContainsKey(filePath))
            {
                _textureCache.Remove(filePath);
                GD.Print($"Released texture: {filePath}");
            }
            else
            {
                GD.Print($"Texture not found: {filePath}");
            }
        }
    }

    // ------------------------------------------------------------------------------------------------

    public bool IsTextureLoaded(string filePath)
    {
        lock (_lock)
        {
            return _textureCache.ContainsKey(filePath);
        }
    }

    // ------------------------------------------------------------------------------------------------

    public ImageTexture? GetTexture(string filePath)
    {
        lock (_lock)
        {
            return _textureCache.ContainsKey(filePath) ? _textureCache[filePath] : null;
        }
    }

    // ------------------------------------------------------------------------------------------------

    public StandardMaterial3D? GetMaterialWithTexture(string filePath)
    {
        var texture = GetTexture(filePath);
        if (texture != null)
        {
            var material = new StandardMaterial3D
            {
                AlbedoTexture = texture,
                ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
            };
            return material;
        }

        return null;
    }

    // ------------------------------------------------------------------------------------------------

    // Usage: GD.Print(FssTextureLoader.Instance.TextureCacheList());
    public string TextureCacheList()
    {
        lock (_lock)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Texture Cache List");
            sb.AppendLine("------------------");
            sb.AppendLine();
            sb.AppendLine($"Count: {_textureCache.Count}");
            sb.AppendLine();
            
            foreach (var key in _textureCache.Keys)
            {
                sb.AppendLine(key);
            }
            return sb.ToString();
        }
    }

}
