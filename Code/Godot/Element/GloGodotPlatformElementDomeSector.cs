

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class GloGodotPlatformElementDomeSector : GloGodotPlatformElement
{
    public float      RxDistanceM   = 1.0f;
    public float      TxDistanceM   = 1.0f;
    public GloAzElBox SectorAzElBox = GloAzElBox.Zero;

    Color              DomeColor;
    Color              SegmentColor;
    Color              WireColor;
    StandardMaterial3D MatDome;
    StandardMaterial3D MatSegment;

    GloLineMesh3D      RxLineMesh;
    GloLineMesh3D      TxLineMesh;
    MeshInstance3D     RxMeshInstance;
    MeshInstance3D     TxMeshInstance;

    Node3D             RotatorNode;
    float              RotateDegs = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateMaterials();
        CreateSector();
        //CreateSegment();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // if (GloAppFactory.Instance.SimClock.IsRunning)
        // {
        //     RotateDegs += (float)delta * RotateDegsPerSec;
        //     if (RotateDegs >= 360.0f)
        //         RotateDegs -= 360.0f;

        //     float RotateRads = (float)GloValueUtils.DegsToRads(RotateDegs);

        //     RotatorNode.Rotation = new Vector3(0, RotateRads, 0);
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateMaterials()
    {
        DomeColor      = GloColorUtil.StringToColor(Name, 0.4f);
        DomeColor.A    = 0.4f;
        SegmentColor   = DomeColor;
        SegmentColor.A = 0.7f;
        WireColor      = DomeColor;
        WireColor.A    = 0.8f;

        MatDome    = GloMaterialFactory.TransparentColoredMaterial(DomeColor);
        MatSegment = GloMaterialFactory.TransparentColoredMaterial(SegmentColor);
    }

    // --------------------------------------------------------------------------------------------

    private void CreateSector()
    {
        GloMeshBuilder rxMeshBuilder = new();
        GloMeshBuilder txMeshBuilder = new();

        int numSegments = 20;

        if (true)
        {
            float minDistM = 100f;
            float maxDistM = 100000f;
            if (RxDistanceM < minDistM) RxDistanceM = minDistM;
            if (RxDistanceM > maxDistM) RxDistanceM = maxDistM;
            if (TxDistanceM < minDistM) TxDistanceM = minDistM;
            if (TxDistanceM > maxDistM) TxDistanceM = maxDistM;

            float rxDist = (float)(RxDistanceM * KoreZeroOffset.RwToGeDistanceMultiplier);
            float txDist = (float)(TxDistanceM * KoreZeroOffset.RwToGeDistanceMultiplier);


            ArrayMesh rxMeshData = rxMeshBuilder.Build2("RxSector", false);
            ArrayMesh txMeshData = txMeshBuilder.Build2("TxSector", false);

            // Add the mesh to the current Node3D
            RxMeshInstance                  = new() { Name = $"{Name}RxSector" };
            RxMeshInstance.Mesh             = rxMeshData;
            RxMeshInstance.MaterialOverride = MatDome;
            AddChild(RxMeshInstance);

            TxMeshInstance                  = new() { Name = $"{Name}TxSector" };
            TxMeshInstance.Mesh             = txMeshData;
            TxMeshInstance.MaterialOverride = MatDome;
            AddChild(TxMeshInstance);

            RxLineMesh = new GloLineMesh3D() { Name = $"{Name}RxWire" };
            RxLineMesh.AddHemisphereSector(
                Vector3.Zero, rxDist, numSegments,
                (float)SectorAzElBox.MinAzDegs, (float)SectorAzElBox.MaxAzDegs,
                WireColor);
            AddChild(RxLineMesh);

            TxLineMesh = new GloLineMesh3D() { Name = $"{Name}TxWire" };
            TxLineMesh.AddHemisphereSector(
                Vector3.Zero, txDist, numSegments,
                (float)SectorAzElBox.MinAzDegs, (float)SectorAzElBox.MaxAzDegs,
                WireColor);
            AddChild(TxLineMesh);
        }
    }

    // --------------------------------------------------------------------------------------------

    // Create the segment we can spin at the scan pattern rate
    private void CreateSegment()
    {
        GloMeshBuilder meshBuilder  = new();

        float azMin = -5f;
        float azMax =  5f;
        float elMin =  5f;
        float elMax = 80f;
        float distMin = (float)(200         * KoreZeroOffset.RwToGeDistanceMultiplier);
        float distMax = (float)(RxDistanceM * KoreZeroOffset.RwToGeDistanceMultiplier) * 0.95f;

        int resAz = 5;
        int resEl = 50;

        meshBuilder.AddShellSegment(
            azMin, azMax,
            elMin, elMax,
            distMin, distMax,
            resAz, resEl);

        ArrayMesh meshData = meshBuilder.Build2("Segment", false);

        MeshInstance3D rotateMeshInstance   = new() { Name = $"{Name}Segment" };
        rotateMeshInstance.Mesh             = meshData;
        rotateMeshInstance.MaterialOverride = MatSegment;

        RotatorNode = new Node3D() { Name = $"{Name}Rotator" };
        AddChild(RotatorNode);

        RotatorNode.AddChild(rotateMeshInstance);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool beamsVisible, bool rxVisible, bool txVisible)
    {
        bool finalRxVisible = rxVisible & beamsVisible;
        bool finalTxVisible = txVisible & beamsVisible;

        // Update the mesh (volume) visibility
        RxMeshInstance.Visible = finalRxVisible;
        RxLineMesh.Visible     = finalRxVisible;
        RotatorNode.Visible    = finalRxVisible;

        TxMeshInstance.Visible = finalTxVisible;
        TxLineMesh.Visible     = finalTxVisible;
    }
}

