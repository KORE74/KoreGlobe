using Godot;
using System;
using System.Threading.Tasks;

public partial class FssMapTileNode : Node3D
{
    private string _imagePath;
    private bool _isDone;
    StandardMaterial3D _material;
    public ArrayMesh meshData;

    // Property to check if the material loading is done
    public bool IsDone => _isDone;

    private bool OneShotFlag = false;

    // Property to get the loaded texture
    public StandardMaterial3D LoadedMaterial => _material;

    // Constructor to initialize the texture path and start loading
    public FssMapTileNode(string imagePath)
    {
        _imagePath = imagePath;
        _isDone = false;
        StartLoading();
    }

    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (IsDone && OneShotFlag == false)
        {
            AddChild( new MeshInstance3D() {
                Name             = "MapTileMesh",
                Mesh             = meshData,
                MaterialOverride = _material
            });

            AddChild( new MeshInstance3D() {
                Name             = "MapTileMeshW",
                Mesh             = meshData,
                MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial()
            });

            OneShotFlag = true;

            GD.Print("=========> MapTileMesh added to FssMapTileNode");
        }
    }

    // --------------------------------------------------------------------------------------------

    // Async

    // Method to start the texture loading process
    private async void StartLoading()
    {
        var image = await LoadImageAsync(_imagePath);
        CallDeferred(nameof(GetMaterial), image);

    }

    // Asynchronous method to load the image
    private async Task<Image> LoadImageAsync(string path)
    {
        return await Task.Run(() => {
            // Load the image from the file system
            Image image = new Image();
            image.Load(path);
            return image;
        });
    }

    // --------------------------------------------------------------------------------------------

    // Image / texture / material functions

    // Method to create a material with the loaded texture
    private void GetMaterial(Image image)
    {
        _material = new StandardMaterial3D();
        _material.AlbedoTexture = ImageTexture.CreateFromImage(image);

        // Only set the done flag AFTER the material is created
        _isDone = true;

    }

}
