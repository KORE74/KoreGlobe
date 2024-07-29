using Godot;
using System;
using System.IO;


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

        CreateTestWedge();


        FssTileInfo tileInfo = new FssTileInfo() {
            TileCode = "CF",
            TileLLBounds = new FssLLBox() {
                MinLatDegs   =  0,
                MinLonDegs   =  0,
                DeltaLatDegs = 30,
                DeltaLonDegs = 30,
            }
        };

        LoadTile(tileInfo);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void LoadTile(FssTileInfo tileInfo)
    {
        // Setup the filepaths
        string rootDir = "res://Resources/Map/Lvl0_30x30";
        string ImageFilePath = Path.Combine(rootDir, tileInfo.TileCode + ".png");
        string EleFilePath   = Path.Combine(rootDir, tileInfo.TileCode + ".asc");

        ImageFilePath = $"res://Resources/Map/Lvl0_30x30/Sat_{tileInfo.TileCode}.png";
        EleFilePath   = $"res://Resources/Map/Lvl0_30x30/Ele_{tileInfo.TileCode}.asc";

        EleFilePath = FssGodotFileUtil.GetActualPath(EleFilePath);

        GD.Print("ImageFilePath: " + ImageFilePath);
        GD.Print("EleFilePath: "   + EleFilePath);

        // Load the image
        var image = Image.LoadFromFile(ImageFilePath);
        var texture = ImageTexture.CreateFromImage(image);
        var _material = new StandardMaterial3D();
        _material.AlbedoTexture = texture;
        _material.ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded;

        // Load the elevation data
        FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(EleFilePath);
        FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 10000f));
        FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(50, 50);

        // Create the wedge
        FssMeshBuilder meshBuilder  = new ();
        meshBuilder.AddSurface(
            (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs, //float azMinDegs, float azMaxDegs,
            (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs, //float elMinDegs, float elMaxDegs,
            2f, 0.000002f, //float surfaceRadius, float surfaceScale,
            croppedArray //FssFloat2DArray surfaceArray,
        ); //bool flipTriangles = false)
        ArrayMesh meshData = meshBuilder.BuildWithUV(tileInfo.TileCode);


        // Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();;
        AddChild(meshInstanceW);

        // Add the textured Mesh
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = _material;
        AddChild(meshInstance);
    }




    // --------------------------------------------------------------------------------------------
    // MARK: Debug Functions
    // --------------------------------------------------------------------------------------------

    private void CreateTestWedge()
    {
        FssMeshBuilder meshBuilder  = new ();


        //string filePath = FssGodotFileUtil.GetActualPath("res://Resources/Map/Lvl0_30x30/Ele_BG_1km.asc");
        string filePath = FssGodotFileUtil.GetActualPath("res://Resources/Map/Lvl0_30x30/Ele_BG.asc");

        FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(filePath);
        FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 10000f));
        FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(50, 50);

        FssFloat2DArray noiseSurface = new FssFloat2DArray(50, 50);
        noiseSurface.SetRandomVals(-1f, 1f);

        noiseSurface[0,0] = 10f;

        meshBuilder.AddSurface(
            0, 30, //float azMinDegs, float azMaxDegs,
            30, 60, //float elMinDegs, float elMaxDegs,
            2f, 0.000002f, //float surfaceRadius, float surfaceScale,
            croppedArraySubSample //FssFloat2DArray surfaceArray,
        ); //bool flipTriangles = false)

        meshBuilder.AddSurfaceWedgeSides(
            0, 30, //float azMinDegs, float azMaxDegs,
            30, 60, //float elMinDegs, float elMaxDegs,
            2f, 0.000002f, 1.95f,
            croppedArraySubSample //FssFloat2DArray surfaceArray,
        ); //bool flipTriangles = false)

        // meshBuilder.AddShellSegment (
        //     30, 60, // elevationMin, elevationMax,
        //     30, 60, // azimuthMin, azimuthMax,
        //     1.10f, 1.12f,          // distanceMin, distanceMax,
        //     3, 3 );                // resolutionAz, resolutionEl)

        ArrayMesh meshData = meshBuilder.BuildWithUV("Wedge");

        var image = Image.LoadFromFile("res://Resources/Map/Lvl0_30x30/Sat_BG.png");
        var texture = ImageTexture.CreateFromImage(image);

        //var _material = FssMaterialFactory.TexMaterial();


        var _material = new StandardMaterial3D();
        _material.AlbedoTexture = texture;
        _material.ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded;


        // load an image into a material

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = _material;

        AddChild(meshInstance);

        var matWire = FssMaterialFactory.WireframeWhiteMaterial();
        //matWire.Transparency = 0.85;

        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;
        meshInstanceW.Transparency     = 0.9f;
        //meshInstanceW.CastShadows      = false;

        AddChild(meshInstanceW);

    }


}
