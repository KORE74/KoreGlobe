using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile: A tile placed at an offset from the zeronode.
public partial class FssZeroNodeMapTile : Node3D
{
    FssMapTileCode TileCode = FssMapTileCode.Zero;
    FssZeroNodeMapTile? ParentTile;

    // Working values
    private FssMapTileFilepaths?  Filepaths;

    // Main tile data, the az-el box and the 2D elevation array we stretch across it.
    public FssLLBox        RwLLBox = FssLLBox.Zero;
    public FssFloat2DArray RwEleData = new FssFloat2DArray();


    private FssLLAPoint        RwLLACenter = FssLLAPoint.Zero;

    private ArrayMesh          TileMeshData;
    private Color              WireColor;
    private StandardMaterial3D SurfaceMat;

    private FssLineMesh3D LineMesh3D;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateMaterials();
        CreateMesh();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update when the World000 is updated
        if (FssZeroNode.ZeroNodeUpdated)
            LocateTile();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Main Create Funcs
    // --------------------------------------------------------------------------------------------

    public void StartTileConstruction(FssMapTileCode tileCode, FssZeroNodeMapTile? parentTile)
    {
        // Set the core Tilecode and node name.
        TileCode = tileCode;
        Name     = TileCode.ToString();

        // Fire off the fully background task of creating/loading the mesh
        Task.Run(async () => await BackgroundTileCreation());
    }

    private async Task BackgroundTileCreation()
    {
        await Task.Run(() =>
        {
            // Start the image loading
            Filepaths = new FssMapTileFilepaths(TileCode); // Figure out the file paths for the tile

            // Start the mesh creation process
            FssAppFactory.Instance.EleManager.RequestTile(TileCode);

            // Start looking for the mail tile image
            SourceTileImage();
        });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void SourceTileImage()
    {


        // If we have no parent tile, we need to find the exact image for this tile.
        // Likely a Lvl0 tile, or a testing situation.
        if (ParentTile == null)
        {
            // If we have no texture, there is no option but to load a default image.
            if (!Filepaths!.ImageFileExists)
            {
                // Load a default image
            }
            else
            {
                // STart the load the exact image (or confirmation that it already exists)
                FssGodotFactory.Instance.ImageManager.StartImageLoading(Filepaths!.ImageFilepath);
            }
        }
    }


    public void CreateMaterials()
    {
        // Create the material for the mesh.
        WireColor  = FssColorUtil.Colors["Red"]; //StringToColor("default");
        SurfaceMat = FssMaterialFactory.TransparentColoredMaterial(WireColor);
    }

    // Inputs being the RwAzElBox and RwEleData, we create a mesh that represents the tile.
    // Context being the RwtoGe scaling factor.

    public void CreateMesh()
    {
        LineMesh3D = new() { Name = "LineMesh" };
        AddChild(LineMesh3D);

        // Setup the center position for the mesh and later positioning
        RwLLACenter = new FssLLAPoint() { LatDegs = RwLLBox.MidLatDegs, LonDegs = RwLLBox.MinLonDegs, RadiusM = FssPosConsts.EarthRadiusM};

        FssXYZPoint rwXYZCenter = RwLLACenter.ToXYZ();

        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile.
        FssLLAPoint       rwLLAZeroLonCenter = new FssLLAPoint() {
            LatDegs = RwLLACenter.LatDegs,
            LonDegs = 0,
            RadiusM = FssPosConsts.EarthRadiusM};
        FssXYZPoint       rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int          pointCountAz  = RwEleData.Width;
        int          pointCountEl  = RwEleData.Height;
        List<double> latListRads   = FssValueUtils.CreateRangeList(pointCountEl, RwLLBox.MinLatRads, RwLLBox.MaxLatRads);
        List<double> lonListRads   = FssValueUtils.CreateRangeList(pointCountAz, -RwLLBox.HalfDeltaLonRads, RwLLBox.HalfDeltaLonRads);
        Vector3[,]   v3Data        = new Vector3[pointCountAz, pointCountEl];

        for (int i = 0; i < pointCountAz; i++)
        {
            for (int j = 0; j < pointCountEl; j++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double      lonRads       = lonListRads[i];
                double      latRads       = latListRads[j];
                double      ele           = rwLLAZeroLonCenter.RadiusM + RwEleData[i, j];
                FssLLAPoint rwLLAPointPos = new FssLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = ele };
                FssXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // Find the offset from the tile center.
                FssXYZPoint rwXYZCenterOffset = rwXYZCenter.XYZTo(rwXYZPointPos);

                // if ((i == 0) && (j == 0))
                // {
                //     GD.Print($"{rwXYZCenter} // {rwXYZPointPos} // {rwXYZCenterOffset}");
                //     GD.Print($"azElCenter:{azElCenter:F3} // {RwAzElBox.MinAzDegs:F3}->{RwAzElBox.MaxAzDegs:F3}");
                // }

                // Convert the Real-World position to the Game Engine position.
                v3Data[i, j] = new Vector3(
                    (float)(rwXYZCenterOffset.X * FssZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(rwXYZCenterOffset.Y * FssZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(rwXYZCenterOffset.Z * FssZeroOffset.RwToGeDistanceMultiplierM));
            }
        }

        // Add the grid to the LineMesh
        LineMesh3D.AddSurface(v3Data, FssColorUtil.Colors["Red"]);

        // Create the game-engine mesh from the V3s
        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSurface(v3Data, false);

        // Build the mesh data and add it to the node
        MeshInstance3D tileMeshInstance   = new MeshInstance3D() { Name = "tileMesh" };
        tileMeshInstance.Mesh             = meshBuilder.BuildWithUV("Surface");
        tileMeshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        //tileMeshInstance.MaterialOverride = FssMaterialFactory.WaterMaterial();

        AddChild(tileMeshInstance);

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwLLACenter.LonRads);
        tileMeshInstance.Rotation = new Vector3(0, rotAz, 0);
        LineMesh3D.Rotation       = new Vector3(0, rotAz, 0);

        // Add a simple sphere marker for the tile center
        // Create a new SphereMesh instance with radius and height set
        {
            float sphereRad = 1f;
            var sphereMesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f };
            var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = sphereMesh };
            AddChild(sphereInstance);
        }

        LocateTile();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine.

    private void LocateTile()
    {
        Position = FssGeoConvOperations.RwToOffsetGe(RwLLACenter);
    }

}



