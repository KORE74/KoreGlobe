using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class GloZeroNodeMapTile : Node3D
{
    // Externally specified data required to start tile construction
    public GloFloat2DArray       RwEleData  = new GloFloat2DArray();
    public GloMapTileCode        TileCode   = GloMapTileCode.Zero;
    public GloZeroNodeMapTile?   ParentTile = null;

    // Child accessible values
    public GloFloatRange         UVx = GloFloatRange.ZeroToOne;
    public GloFloatRange         UVy = GloFloatRange.ZeroToOne;
    public GloUVBox              UVBox = GloUVBox.Zero;

    // Working values
    private GloMapTileFilepaths  Filepaths;
    private GloLLAPoint          RwLLACenter = GloLLAPoint.Zero;

    // Materials and Meshes
    private ArrayMesh            TileMeshData;
    private Color                WireColor;
    private StandardMaterial3D   SurfaceMat;
    private GloLineMesh3D        LineMesh3D;

    // Construction Flags
    private bool ConstructionComplete = false;
    private bool MeshDone             = false;
    private bool MeshInstatiated      = false;
    private bool ImageDone            = false;

    // Flag set when the tile (or its children) should be visible. Gates the main visibility processing.
    public bool ActiveVisibility      = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    public GloZeroNodeMapTile(GloMapTileCode tileCode)
    {
        // Set the core Tilecode and node name.
        TileCode = tileCode;
        Name     = tileCode.ToString();

        // Fire off the fully background task of creating/loading the mesh
        Task.Run(() => BackgroundTileCreation(tileCode));



    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateMaterials();
        CreateMesh();

        // Start the image loading
        Filepaths = new GloMapTileFilepaths(TileCode); // Figure out the file paths for the tile
        if (Filepaths.ImageFileExists)
            GloGodotFactory.Instance.ImageManager.StartImageLoading(Filepaths.ImageFilepath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update when the World000 is updated
        if (GloZeroNode.ZeroNodeUpdateCycle)
            LocateTile();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private async void BackgroundTileCreation(GloMapTileCode tileCode)
    {


        GloGodotFactory.Instance.EleSystem.RequestTile(tileCode);)

        // // Starting: Set the flags that will be used later to determine activity around the tile wheil we construct it.
        // ConstructionComplete = false;
        // ActiveVisibility     = false;

        // // Setup some basic elements of the tile ahead of the mail elevation and image loading.
        // Filepaths = new GloMapTileFilepaths(TileCode); // Figure out the file paths for the tile

        // // Pause the thread, being a good citizen with lots of tasks around.
        // await Task.Yield();


        // // Load the image data - which will determine the UVBox it will need.
        // // - 1 - we find the image and load it.
        // // - 2 - We copy the parent tile image and UVBox, and subsample them.
        // if (Filepaths.ImageFileExists)
        //     LoadTileImage();
        // else
        //     SubsampleParentTileImage();
    }

    // --------------------------------------------------------------------------------------------

    public void CreateMaterials()
    {
        // Create the material for the mesh.
        WireColor  = GloColorUtil.Colors["Red"]; //StringToColor("default");
        SurfaceMat = GloMaterialFactory.TransparentColoredMaterial(WireColor);
    }

    // Inputs being the RwAzElBox and RwEleData, we create a mesh that represents the tile.
    // Context being the RwtoGe scaling factor.

    public void CreateMesh()
    {
        LineMesh3D = new() { Name = "LineMesh" };
        AddChild(LineMesh3D);

        // Setup the center position for the mesh and later positioning
        GloLLBox rwLLBox = TileCode.LLBox;
        RwLLACenter = new GloLLAPoint() { LatDegs = rwLLBox.MidLatDegs, LonDegs = rwLLBox.MinLonDegs, RadiusM = GloPosConsts.EarthRadiusM};

        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile.
        GloLLAPoint rwLLAZeroLonCenter = new GloLLAPoint() {
            LatDegs = RwLLACenter.LatDegs,
            LonDegs = 0,
            RadiusM = GloPosConsts.EarthRadiusM};
        GloXYZPoint rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int          pointCountLon   = RwEleData.Width;
        int          pointCountLat   = RwEleData.Height;
        List<double> lonZeroListRads = GloValueUtils.CreateRangeList(pointCountLon, -rwLLBox.HalfDeltaLonRads, rwLLBox.HalfDeltaLonRads); // Relative azimuth
        List<double> latListRads     = GloValueUtils.CreateRangeList(pointCountLat,  rwLLBox.MinLatRads,       rwLLBox.MaxLatRads);
        Vector3[,]   v3Data          = new Vector3[pointCountLon, pointCountLat];

        for (int i = 0; i < pointCountLon; i++)
        {
            for (int j = 0; j < pointCountLat; j++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double      lonRads       = lonZeroListRads[i];
                double      latRads       = latListRads[j];
                double      ele           = RwLLACenter.RadiusM + RwEleData[i, j];
                GloLLAPoint rwLLAPointPos = new GloLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = ele };
                GloXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // Find the offset from the tile center.
                GloXYZPoint rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // Convert the Real-World position to the Game Engine position.
                v3Data[i, j] = new Vector3(
                    (float)(rwXYZCenterOffset.X * GloZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(rwXYZCenterOffset.Y * GloZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(rwXYZCenterOffset.Z * GloZeroOffset.RwToGeDistanceMultiplierM));
            }
        }

        // Add the grid to the LineMesh
        LineMesh3D.AddSurface(v3Data, GloColorUtil.Colors["Red"]);

        // Create the game-engine mesh from the V3s
        GloMeshBuilder meshBuilder = new();
        meshBuilder.AddSurface(v3Data, UVx, UVy, false);

        // Build the mesh data and add it to the node
        MeshInstance3D tileMeshInstance   = new MeshInstance3D() { Name = "tileMesh" };
        tileMeshInstance.Mesh             = meshBuilder.BuildWithUV("Surface");
        tileMeshInstance.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        //tileMeshInstance.MaterialOverride = GloMaterialFactory.WaterMaterial();

        AddChild(tileMeshInstance);

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwLLACenter.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.
        tileMeshInstance.Rotation = new Vector3(0, rotAz, 0);
        LineMesh3D.Rotation       = new Vector3(0, rotAz, 0);

        // Add a simple sphere marker for the tile center
        // Create a new SphereMesh instance with radius and height set
        {
            float sphereRad = 10f;
            var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f } };
            AddChild(sphereInstance);
        }

        LocateTile();
    }

    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine.

    private void LocateTile()
    {
        Position = GloGeoConvOperations.RwToOffsetGe(RwLLACenter);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create Image
    // --------------------------------------------------------------------------------------------

    private void SourceTileImage()
    {
        // Request the image to be loaded in the background - non-blocking.


        // GloTextureLoader? TL = GloTextureLoader.Instance;
        // if (TL != null)
        // {
        //     ImageTexture? tex = TL.LoadTextureDirect(Filepaths.ImageFilepath);

        //     if (tex != null)
        //     {
        //         TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

        //         if (TileMaterial != null)
        //         {
        //             ImageDone = true;
        //         }
        //     }
        // }

        // // Setup the UV Box - new image, so a full 0,0 -> 1,1 range
        // UVBox = new GloUvBoxDropEdgeTile(GloUvBoxDropEdgeTile.UVTopLeft, GloUvBoxDropEdgeTile.UVBottomRight);
    }


    private void SubsampleParentTileImage()
    {
        // if (ParentTile != null)
        // {
        //     GloTextureLoader? TL = GloTextureLoader.Instance;

        //     Filepaths.ImageFilepath   = ParentTile.Filepaths.ImageFilepath;
        //     Filepaths.ImageFileExists = ParentTile.Filepaths.ImageFileExists;

        //     TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

        //     if (TileMaterial != null)
        //         ImageDone = true;

        //     // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
        //     // Get the grid position of this tile in its parent (eg [1x,2y] in a 5x5 grid).
        //     UVBox = new GloUvBoxDropEdgeTile(ParentTile.UVBox, TileCode.GridPos);

        // }
        // else
        // {
        //     // Subsmapling, but no parent. Setup a default.
        //     UVBox = GloUvBoxDropEdgeTile.Default(TileSizePointsPerLvl[TileCode.MapLvl], TileSizePointsPerLvl[TileCode.MapLvl]);
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create Elevation
    // --------------------------------------------------------------------------------------------

    private void SourceTileElevation()
    {

    }


}


