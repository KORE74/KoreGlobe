#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

public partial class FssTextureLoader : Node
{
    // Cache to store loaded textures
    private readonly Dictionary<string, ImageTexture> _textureCache = new Dictionary<string, ImageTexture>();

    // Queue to manage texture loading requests
    private readonly Queue<string> _loadQueue = new Queue<string>();

    // Task to manage the current texture loading
    private Task<ImageTexture?>? _loadingTask = null;

    // The file path of the currently loading texture
    private string? _currentLoadingFilePath = null;

    // Lock object for thread safety
    private readonly object _lock = new object();

    // ------------------------------------------------------------------------------------------------

   // public static FssTextureLoader Instance { get; private set; }

    public static FssTextureLoader? GetGlobal()
    {
         var tree = Engine.GetMainLoop() as SceneTree;

         if (tree == null)
         {
             GD.PrintErr("Failed to get SceneTree.");
             return null;
         }

        FssTextureLoader? TL = tree.Root.GetNodeOrNull<FssTextureLoader>("FssTextureLoader_Global");

        if (TL == null)
        {
            GD.PrintErr("Failed to get TextureLoader.");
        }

        return TL;
    }


    // ------------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {
        GD.Print($"TextureCacheList: {_loadQueue.Count}");

        // If there's no active loading task, start the next one in the queue
        lock (_lock)
        {
            GD.Print("1");

            if (_loadingTask == null) GD.Print("1.1");
            if (_loadQueue.Count > 0) GD.Print("1.2");

            if (_loadingTask == null && _loadQueue.Count > 0)
            {

                GD.Print("2");

                _currentLoadingFilePath = _loadQueue.Dequeue();
                _loadingTask            = LoadTextureAsync(_currentLoadingFilePath);

                GD.Print($"Loading texture: {_currentLoadingFilePath}");
            }
            else
            {
                GD.Print($"No textures to load. {_loadQueue.Count}");
            }
        }

        GD.Print("3");

        if (_loadingTask != null)
        {
            GD.Print("4");
            if ( _loadingTask.IsCompleted) GD.Print("5");
        }

        // Check if the current loading task is complete
        if (_loadingTask != null && _loadingTask.IsCompleted)
        {
            ImageTexture? texture = _loadingTask.Result;

            if (texture != null && _currentLoadingFilePath != null)
            {
                GD.Print($"Texture loaded successfully for: {_currentLoadingFilePath}");

                // Cache the texture
                lock (_lock)
                {
                    if (!_textureCache.ContainsKey(_currentLoadingFilePath))
                    {
                        _textureCache[_currentLoadingFilePath] = texture;
                    }
                }
            }
            else
            {
                GD.PrintErr("Failed to load texture.");
            }

            // Reset task and path for the next load
            _loadingTask = null;
            _currentLoadingFilePath = null;
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

                    GD.Print($"Queued texture_: {filePath} ({_loadQueue.Count})");
                }
                else
                {
                    GD.Print($"Texture already loaded or queued: {filePath}");
                }
            }
        }
        else
        {
            GD.PrintErr($"File not loadable: {filePath}");
        }
    }

    // ------------------------------------------------------------------------------------------------

    private async Task<ImageTexture?> LoadTextureAsync(string filePath)
    {
        var image = new Image();
        var err = await Task.Run(() => image.Load(filePath));

        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load image: {filePath}, Error: {err}");
            return null;
        }

        // Ensure this part runs on the main thread
        await ToSignal(GetTree(), "idle_frame");

        return ImageTexture.CreateFromImage(image);
    }

    // ------------------------------------------------------------------------------------------------

    public bool IsTextureAvailable(string filePath)
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

    public Material? GetMaterialWithTexture(string filePath)
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

    public string TextureCacheList()
    {
        lock (_lock)
        {
            return string.Join("\n", _textureCache.Keys);
        }
    }

}
