using Godot;
using System;
using System.Threading.Tasks;

public partial class FssMapTileNode : Node3D
{
    private string _imagePath;
    private bool _isDone;
    StandardMaterial3D _material;
    public ArrayMesh meshData;

    private bool MeshDone = false;
    private bool MaterialDone = false;

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

        Task.Run(() => LoadTile2(tileInfo));
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



    private async void LoadTile2(FssTileInfo tileInfo)
    {
        // Setup the file paths
        string rootDir = "res://Resources/Map/Lvl0_30x30";
        string imageFilePath = Path.Combine(rootDir, $"Sat_{tileInfo.TileCode}.png");
        string eleFilePath = Path.Combine(rootDir, $"Ele_{tileInfo.TileCode}.asc");
        string meshFilePath = Path.Combine(rootDir, $"Mesh_{tileInfo.TileCode}.mesh");

        // Run file loading and processing on a background thread
        {
            eleFilePath = FssGodotFileUtil.GetActualPath(eleFilePath);
            meshFilePath = FssGodotFileUtil.GetActualPath(meshFilePath);

            bool loadMesh = File.Exists(meshFilePath);
            bool saveMesh = !loadMesh;

            FssMeshBuilder meshBuilder = new();
            if (loadMesh)
            {
                meshBuilder.meshData = FssMeshDataIO.ReadMeshFromFile(meshFilePath);
                FssCentralLog.AddEntry($"Loaded mesh: {meshFilePath}");
            }
            else
            {
                // Load the elevation data
                FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(eleFilePath);
                FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 10000f));
                FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(20, 20);

                // Create the mesh
                meshBuilder.AddSurface(
                    (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
                    (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
                    5f, 0.000016f,
                    croppedArraySubSample
                );
                meshBuilder.AddSurfaceWedgeSides(
                    (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
                    (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
                    5f, 0.000016f, 4.5f,
                    croppedArraySubSample
                ); //bool flipTriangles = false)
            }
            meshData = meshBuilder.BuildWithUV(tileInfo.TileCode);
            MeshDone = true;

            if (saveMesh)
            {
                // Save the mesh to a file
                FssMeshDataIO.WriteMeshToFile(meshBuilder.meshData, meshFilePath);
                FssCentralLog.AddEntry($"Saved mesh: {meshFilePath}");
            }
        }
    }

    private void LoadMaterial()
    {
        // Setup the file paths
        string rootDir = "res://Resources/Map/Lvl0_30x30";
        string imageFilePath = Path.Combine(rootDir, $"Sat_{tileInfo.TileCode}.png");

        // Load the image and create the texture on the main thread
        var image = new Image();
        var err = image.Load(imageFilePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load image: {imageFilePath}");
            return;
        }
        var texture = ImageTexture.CreateFromImage(image);
        var material = new StandardMaterial3D
        {
            AlbedoTexture = texture,
            ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
        };

        // Add the mesh instances to the current Node3D
        var meshInstanceW = new MeshInstance3D { Name = $"{tileInfo.TileCode} wire" };
        meshInstanceW.Mesh = meshData;
        meshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
        AddChild(meshInstanceW);

        var meshInstance = new MeshInstance3D { Name = $"{tileInfo.TileCode} image" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = material;
        AddChild(meshInstance);
    }

}
