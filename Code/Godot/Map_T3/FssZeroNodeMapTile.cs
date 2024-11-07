using System;
using System.Collections.Generic;

using Godot;

// ZeroNode map tile: A tile placed at an offset from the zeronode.
public partial class FssZeroNodeMapTile : Node3D
{
    // Main tile data, the az-el box and the 2D elevation array we stretch across it.
    public GloAzElBox      RwAzElBox = GloAzElBox.Zero;
    public GloFloat2DArray RwEleData = new GloFloat2DArray();

    private ArrayMesh          TileMeshData;
    private Color              WireColor;
    private StandardMaterial3D SurfaceMat;

    private GloLineMesh3D LineMesh3D;


    private double RadiusFudgeM = 1000;

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
        if (GloZeroNode.ZeroNodeUpdateCycle)
            LocateTile();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
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

        RwEleData = new GloFloat2DArray(10, 10);
        RwEleData.SetRandomVals(1000f, 1100f);

        GloPolarDirection azElCenter  = RwAzElBox.Center;
        GloLLAPoint       rwLLACenter = new GloLLAPoint() { LatDegs = azElCenter.ElDegs, LonDegs = azElCenter.AzDegs, RadiusM = GloPosConsts.EarthRadiusM + RadiusFudgeM};
        GloXYZPoint       rwXYZCenter = rwLLACenter.ToXYZ();

        // Setup the loop control values
        int          pointCountAz = RwEleData.Width;
        int          pointCountEl = RwEleData.Height;
        List<double> azListRads   = GloValueUtils.CreateRangeList(pointCountAz, RwAzElBox.MinAzRads, RwAzElBox.MaxAzRads);
        List<double> azListRads2  = GloValueUtils.CreateRangeList(pointCountAz, -RwAzElBox.HalfArcAzRads, RwAzElBox.HalfArcAzRads);
        List<double> elListRads   = GloValueUtils.CreateRangeList(pointCountEl, RwAzElBox.MinElRads, RwAzElBox.MaxElRads);
        Vector3[,]   v3Data       = new Vector3[pointCountAz, pointCountEl];

        for (int i = 0; i < pointCountAz; i++)
        {
            for (int j = 0; j < pointCountEl; j++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double      azRads        = azListRads[i];
                double      elRads        = elListRads[j];
                double      ele           = rwLLACenter.RadiusM + RwEleData[i, j];
                GloLLAPoint rwLLAPointPos = new GloLLAPoint() { LatRads = elRads, LonRads = azRads, RadiusM = ele };
                GloXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // Find the offset from the tile center.
                GloXYZPoint rwXYZCenterOffset = rwXYZCenter.XYZTo(rwXYZPointPos);

                if ((i == 0) && (j == 0))
                {
                    GD.Print($"{rwXYZCenter} // {rwXYZPointPos} // {rwXYZCenterOffset}");
                    GD.Print($"azElCenter:{azElCenter:F3} // {RwAzElBox.MinAzDegs:F3}->{RwAzElBox.MaxAzDegs:F3}");
                }

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
        GloMeshBuilder meshBuilder = new GloMeshBuilder();
        meshBuilder.AddSurface(v3Data, false);

        // Build the mesh data and add it to the node
        MeshInstance3D tileMeshInstance   = new MeshInstance3D() { Name = "tileMesh" };
        tileMeshInstance.Mesh             = meshBuilder.BuildWithUV("Surface");
        tileMeshInstance.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        //tileMeshInstance.MaterialOverride = GloMaterialFactory.WaterMaterial();

        AddChild(tileMeshInstance);

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwAzElBox.MinAzRads + RwAzElBox.MaxAzRads); // WTF!! azElCenter.AzRads
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

    // Locate the tile in the game engine.

    private void LocateTile()
    {
        GloPolarDirection azElCenter  = RwAzElBox.Center;
        GloLLAPoint       rwLLACenter = new GloLLAPoint() { LatDegs = azElCenter.ElDegs, LonDegs = azElCenter.AzDegs, RadiusM = GloPosConsts.EarthRadiusM + RadiusFudgeM};

        Position = GloGeoConvOperations.RwToOffsetGe(rwLLACenter);
    }

}



