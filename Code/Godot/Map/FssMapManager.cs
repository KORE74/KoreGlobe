using Godot;
using System;
using System.IO;
using System.Threading.Tasks;

// Usage: FssTileInfo newTileInfo = new FssTileInfo() { TileCodeStruct = tilecodestr, TileLLBounds = llbox };

// public struct FssTileInfo
// {
//     public FssMapTileCode TileCodeStruct;
//     public FssLLBox TileLLBounds;
// }

public partial class FssMapManager : Node3D
{
    private string MapRootPath = "";

    // Debug
    private FssLLAPoint pos  = new FssLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    private FssCourse Course = new FssCourse() { HeadingDegs = 90, SpeedMps = 1 };
    Node3D ModelNode         = null;
    Node3D ModelResourceNode = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Read the config values
        var config = FssCentralConfig.Instance;
        MapRootPath = config.GetParameter<string>("MapRootPath", "C:/Util/Godot/MapLib/");
        config.WriteToFile();

        for (int lonId = 0; lonId < 12; lonId++)
        {
            for (int latId = 0; latId < 6; latId++)
            {
                FssMapTileCode currTileCode = new FssMapTileCode(lonId, latId);
                //currTileCode.AddLevelCode(lonId, latId, 0);

                // string tilecodestr   = FssMapTileCode.CodeForIndex(latId, lonId);
                // double minLatDegs = 60 - (latId * 30);
                // double minLonDegs = -180 + (lonId * 30);

                // FssLLBox llbox = new FssLLBox() {
                //     MinLatDegs   = minLatDegs,
                //     MinLonDegs   = minLonDegs,
                //     DeltaLatDegs = 30,
                //     DeltaLonDegs = 30,
                // };

                // GD.Print($"Tile: {tilecodestr}  LLBox: {llbox}");

                // FssTileInfo tileInfo = new FssTileInfo() {
                //     TileCode = tilecodestr,
                //     TileLLBounds = llbox
                // };

                FssMapTileNode newChildTile = new FssMapTileNode(currTileCode);
                AddChild(newChildTile);

                // LoadTile2(tileInfo);
            }
        }
        //DebugWedge();
        //DebugShip();

        CreateMapLibaryDirectories();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        //UpdateModelPosition();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Tile Prep Functions
    // --------------------------------------------------------------------------------------------

    // private async void LoadTile2(FssTileInfo tileInfo)
    // {
    //     // Setup the file paths
    //     string rootDir = "res://Resources/Map/Lvl0_30x30";
    //     string imageFilePath = Path.Combine(rootDir, $"Sat_{tileInfo.TileCode}.png");
    //     string eleFilePath = Path.Combine(rootDir, $"Ele_{tileInfo.TileCode}.asc");
    //     string meshFilePath = Path.Combine(rootDir, $"Mesh_{tileInfo.TileCode}.mesh");

    //     // Run file loading and processing on a background thread
    //     var meshData = await Task.Run(() =>
    //     {
    //         eleFilePath = FssGodotFileUtil.GetActualPath(eleFilePath);
    //         meshFilePath = FssGodotFileUtil.GetActualPath(meshFilePath);

    //         bool loadMesh = File.Exists(meshFilePath);
    //         bool saveMesh = !loadMesh;

    //         FssMeshBuilder meshBuilder = new();
    //         if (loadMesh)
    //         {
    //             meshBuilder.meshData = FssMeshDataIO.ReadMeshFromFile(meshFilePath);
    //             FssCentralLog.AddEntry($"Loaded mesh: {meshFilePath}");
    //         }
    //         else
    //         {
    //             // Load the elevation data
    //             FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(eleFilePath);
    //             FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 10000f));
    //             FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(20, 20);

    //             // Create the mesh
    //             meshBuilder.AddSurface(
    //                 (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
    //                 (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
    //                 5f, 0.000016f,
    //                 croppedArraySubSample
    //             );
    //             meshBuilder.AddSurfaceWedgeSides(
    //                 (float)tileInfo.TileLLBounds.MinLonDegs, (float)tileInfo.TileLLBounds.MaxLonDegs,
    //                 (float)tileInfo.TileLLBounds.MinLatDegs, (float)tileInfo.TileLLBounds.MaxLatDegs,
    //                 5f, 0.000016f, 4.5f,
    //                 croppedArraySubSample
    //             ); //bool flipTriangles = false)
    //         }
    //         ArrayMesh meshData = meshBuilder.BuildWithUV(tileInfo.TileCode);

    //         if (saveMesh)
    //         {
    //             // Save the mesh to a file
    //             FssMeshDataIO.WriteMeshToFile(meshBuilder.meshData, meshFilePath);
    //             FssCentralLog.AddEntry($"Saved mesh: {meshFilePath}");
    //         }

    //         return meshData;
    //     });

    //     // Load the image and create the texture on the main thread
    //     var image = new Image();
    //     var err = image.Load(imageFilePath);
    //     if (err != Error.Ok)
    //     {
    //         GD.PrintErr($"Failed to load image: {imageFilePath}");
    //         return;
    //     }
    //     var texture = ImageTexture.CreateFromImage(image);
    //     var material = new StandardMaterial3D
    //     {
    //         AlbedoTexture = texture,
    //         ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
    //     };

    //     // Add the mesh instances to the current Node3D
    //     var meshInstanceW = new MeshInstance3D { Name = $"{tileInfo.TileCode} wire" };
    //     meshInstanceW.Mesh = meshData;
    //     meshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
    //     AddChild(meshInstanceW);

    //     var meshInstance = new MeshInstance3D { Name = $"{tileInfo.TileCode} image" };
    //     meshInstance.Mesh = meshData;
    //     meshInstance.MaterialOverride = material;
    //     AddChild(meshInstance);
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug Functions
    // --------------------------------------------------------------------------------------------

    private void DebugWedge()
    {
        FssFloat2DArray eleArray = new FssFloat2DArray(50, 50);
        eleArray.SetAllVals(0.001f);

        var meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSurface(
            -10, 50,
            60, 20,
            5f, 0.000006f,
            eleArray,
            true);
        var meshData = meshBuilder.BuildWithUV("surface");

        // Add the mesh instances to the current Node3D
        var meshInstanceW = new MeshInstance3D { Name = $"Water wire" };
        meshInstanceW.Mesh = meshData;
        meshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
        AddChild(meshInstanceW);

        var meshInstance = new MeshInstance3D { Name = $"water image" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = (ShaderMaterial)ResourceLoader.Load("res://Materials/Water_002.tres"); // FssMaterialFactory.WaterMaterial();
        AddChild(meshInstance);
    }

    private void DebugShip()
    {
        string ModelPath = "res://Resources/Models/Ship/GenericSupportShip/GenericSupportShip.glb";

        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);
        if (importedModel != null)
        {
            // Root of the model and orientation
            ModelNode = new Node3D() { Name = "ModelNode" };
            ModelNode.LookAt(Vector3.Forward, Vector3.Up);
            AddChild(ModelNode);

            // Instance the model
            Node modelInstance     = importedModel.Instantiate();
            ModelResourceNode      = modelInstance as Node3D;
            ModelResourceNode.Name = "ModelResourceNode";
            ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up);

            ModelNode.AddChild(ModelResourceNode);
            ModelResourceNode.Scale    = new Vector3(0.05f, 0.05f, 0.05f); // Set the model scale
            //ModelResourceNode.Scale    = new Vector3(0.005f, 0.005f, 0.005f); // Set the model scale
            ModelResourceNode.Position = new Vector3(0f, 0f, 0f); // Set the model position

        }
    }

    private void UpdateModelPosition()
    {
        // --- Define positions -----------------------

        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;
        if (Course.IsStationary())
        {
            posAhead = pos;
            posAhead.LatDegs += 0.001;
        }
        else
        {
            posAhead = pos.PlusPolarOffset(Course.ToPolarOffset(-5));
        }

        // --- Define vectors -----------------------

        // Define the Vector3 Offsets
        Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
        Vector3 vecAbove = FssGeoConvOperations.RealWorldToGodot(posAbove);
        Vector3 vecAhead = FssGeoConvOperations.RealWorldToGodot(posAhead);

        FssEntityV3 platVecs = FssGeoConvOperations.ReadWorldToStruct(pos, Course);

        // Update node position and orientation
        ModelNode.Position = platVecs.Position;// vecPos;
        ModelNode.LookAt(platVecs.PosAhead, platVecs.PosAbove);

        // Update camera position and orientation
        //FssXYZPoint camOffsetXYZ = CameraOffset.ToXYZ();
        //ModelCamera.Position = new Vector3((float)camOffsetXYZ.X, -(float)camOffsetXYZ.Y, -(float)camOffsetXYZ.Z);
        //ModelCamera.LookAt(vecPos, vecAbove);
    }

    // --------------------------------------------------------------------------------------------

    private void CreateMapLibaryDirectories()
    {
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");

        for (int currLvl = 0; currLvl < 3; currLvl++)
        {
            string externalMapLvlFilePath = Path.Combine(externalRootPath, FssMapTileCode.PathPerLvl[currLvl]);

            // Create any top level dirs that don't exist
            if (!Directory.Exists(externalMapLvlFilePath))
                Directory.CreateDirectory(externalMapLvlFilePath);

            // Create the Lvl1 subdirectories
            if (currLvl == 1)
            {
                int numTilesHoriz = FssMapTileCode.NumTilesHorizPerLvl[currLvl];
                int numTilesVert  = FssMapTileCode.NumTilesVertPerLvl[currLvl];

                for (int lonId = 0; lonId < numTilesHoriz; lonId++)
                {
                    for (int latId = 0; latId < numTilesVert; latId++)
                    {
                        string tileCode = FssMapTileCode.CodeForIndex(latId, lonId);
                        string tileDir  = Path.Combine(externalMapLvlFilePath, tileCode);

                        // Create any top level dirs that don't exist
                        if (!Directory.Exists(tileDir))
                            Directory.CreateDirectory(tileDir);
                    }
                }
            }
        }


    }

}
