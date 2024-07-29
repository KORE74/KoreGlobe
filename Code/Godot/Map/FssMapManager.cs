using Godot;
using System;
using System.IO;
using System.Threading.Tasks;

public struct FssTileInfo
{
    public string TileCode;
    public FssLLBox TileLLBounds;
}

public partial class FssMapManager : Node3D
{

    private string MapRootPath = "";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Read the config values
        var config = FssCentralConfig.Instance;
        MapRootPath = config.GetParameter<string>("MapRootPath", "c:/GlobeLib/Maps/");

        for (int lonId = 0; lonId < 12; lonId++)
        {
            for (int latId = 0; latId < 6; latId++)
            {
                string tilecodestr   = FssMapTileCode.CodeForIndex(latId, lonId);
                double minLatDegs = 60 - (latId * 30);
                double minLonDegs = -180 + (lonId * 30);

                FssLLBox llbox = new FssLLBox() {
                    MinLatDegs   = minLatDegs,
                    MinLonDegs   = minLonDegs,
                    DeltaLatDegs = 30,
                    DeltaLonDegs = 30,
                };

                GD.Print($"Tile: {tilecodestr}  LLBox: {llbox}");

                FssTileInfo tileInfo = new FssTileInfo() {
                    TileCode = tilecodestr,
                    TileLLBounds = llbox
                };

                LoadTile2(tileInfo);
            }
        }
        DebugWedge();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Tile Prep Functions
    // --------------------------------------------------------------------------------------------

    private async void LoadTile2(FssTileInfo tileInfo)
    {
        // Setup the file paths
        string rootDir = "res://Resources/Map/Lvl0_30x30";
        string imageFilePath = Path.Combine(rootDir, $"Sat_{tileInfo.TileCode}.png");
        string eleFilePath = Path.Combine(rootDir, $"Ele_{tileInfo.TileCode}.asc");
        eleFilePath = FssGodotFileUtil.GetActualPath(eleFilePath);

        // Run file loading and processing on a background thread
        var meshData = await Task.Run(() =>
        {
            // Load the elevation data
            FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(eleFilePath);
            FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 10000f));
            FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(20, 20);

            // Create the mesh
            var meshBuilder = new FssMeshBuilder();
            meshBuilder.AddSurface(
                (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
                (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
                5f, 0.000006f,
                croppedArraySubSample
            );
            meshBuilder.AddSurfaceWedgeSides(
                (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
                (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
                5f, 0.000006f, 4.5f,
                croppedArraySubSample
            ); //bool flipTriangles = false)
            var meshData = meshBuilder.BuildWithUV(tileInfo.TileCode);

            return meshData;
        });

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

    // --------------------------------------------------------------------------------------------
    // MARK: Debug Functions
    // --------------------------------------------------------------------------------------------

    private void DebugWedge()
    {
        FssFloat2DArray eleArray = new FssFloat2DArray(50, 50);
        eleArray.SetAllVals(0.001f);

        var meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSurface(
            -10, 20, 
            50, 20, 
            5.1f, 0.000006f, 
            eleArray);
        var meshData = meshBuilder.BuildWithUV("surface");

        // Add the mesh instances to the current Node3D
        var meshInstanceW = new MeshInstance3D { Name = $"Water wire" };
        meshInstanceW.Mesh = meshData;
        meshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
        AddChild(meshInstanceW);

        var meshInstance = new MeshInstance3D { Name = $"water image" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = FssMaterialFactory.WaterMaterial();
        AddChild(meshInstance);
    }

}
