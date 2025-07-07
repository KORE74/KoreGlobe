#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using System.Text;

using Godot;

public partial class GloTextureLoader
{
    // Cache to store loaded textures
    private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();

    // Queue to manage texture loading requests
    private readonly Queue<string> _loadQueue = new Queue<string>();

    // Lock object for thread safety
    private readonly object _lock = new object();

    // ------------------------------------------------------------------------------------------------

    // Standard C# singleton pattern

    private static GloTextureLoader? _instance;

    public static GloTextureLoader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GloTextureLoader();
            }
            return _instance;
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Image
    // ------------------------------------------------------------------------------------------------

    // Usage: GloTextureLoader.LoadDirectWebp("res://images/texture.webp");
    public static Image? LoadDirectWebp(string filePath)
    {
        // Load image synchronously
        var image = new Image();
        var err = image.Load(filePath);

        if (err != Error.Ok) return null;
        if (image.IsEmpty()) return null;

        return image;
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Texture Loading
    // ------------------------------------------------------------------------------------------------

    public Texture2D? LoadTextureDirect(string filePath)
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
            GloCentralLog.AddEntry($"LoadTextureDirect: Failed to load image: {filePath}");
            return null;
        }

        if (image.IsEmpty())
        {
            GloCentralLog.AddEntry($"LoadTextureDirect: Empty image: {filePath}");
            return null;
        }

        // Create the texture synchronously - Note the create then set is thought to be required for webp
        var texture = new ImageTexture();
        texture.SetImage(image);

        // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
        image.Dispose();

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

    public bool LoadTextureDirectWebp(string filePath)
    {
        lock (_lock)
        {
            // Check if the texture is already loaded
            if (_textureCache.ContainsKey(filePath))
            {
                //GD.Print($"Texture already loaded: {filePath}");
                return true;
            }
        }

        // Load image synchronously
        var image = new Image();
        var err = image.Load(filePath);
        if (err != Error.Ok)
        {
            GloCentralLog.AddEntry($"LoadTextureDirect: Failed to load image: {filePath}");
            return false;
        }

        if (image.IsEmpty())
        {
            GloCentralLog.AddEntry($"LoadTextureDirect: Empty image: {filePath}");
            return false;
        }

        // Create the texture synchronously - Note the create then set is thought to be required for webp
        var texture = new ImageTexture();
        texture.SetImage(image);

        // Free CPU memory used by the image, it was only ever a vehicle for creating the texture
        image.Dispose();

        if (texture == null)
            return false;

        // Cache the texture
        lock (_lock)
        {
            if (!_textureCache.ContainsKey(filePath))
            {
                _textureCache[filePath] = texture;
            }
        }

        // GD.Print($"Loaded and created texture directly: {filePath}");

        return true;
    }

    // ------------------------------------------------------------------------------------------------

    // Load the WEBP and return a material, no delay, no caching

    public StandardMaterial3D? CreateDirectWebpMaterial(string filePath)
    {
        // Using block Dispose()'s of the image at the end of the block
        using (var image = new Image())
        {
            var err = image.Load(filePath);
            if (err != Error.Ok)
            {
                GloCentralLog.AddEntry($"CreateDirectWebpMaterial: Failed to load image: {filePath}");
                return null;
            }

            if (image.IsEmpty())
            {
                GloCentralLog.AddEntry($"CreateDirectWebpMaterial: Empty image: {filePath}");
                return null;
            }

            // Create the texture synchronously - Note the create then set is thought to be required for webp
            var texture = new ImageTexture();
            texture.SetImage(image);

            var material = new StandardMaterial3D
            {
                AlbedoTexture = texture,
                ShadingMode   = StandardMaterial3D.ShadingModeEnum.Unshaded
            };

            return material;
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

    public Texture2D? GetTexture(string filePath)
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

    // Usage: GD.Print(GloTextureLoader.Instance.TextureCacheList());
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
