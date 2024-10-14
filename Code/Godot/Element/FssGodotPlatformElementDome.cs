

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementDome : FssGodotPlatformElement
{
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.0f;

    Color DomeColor;
    Color SegmentColor;
    Color WireColor;
    StandardMaterial3D MatDome;
    StandardMaterial3D MatSegment;

    FssLineMesh3D LineMesh;

    Node3D RotatorNode;
    public float RotateDegsPerSec = 10.0f;
    float RotateDegs = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateMaterials();
        CreateDome();
        CreateSegment();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        RotateDegs += (float)delta * RotateDegsPerSec;
        if (RotateDegs >= 360.0f)
            RotateDegs -= 360.0f;

        float RotateRads = (float)FssValueUtils.DegsToRads(RotateDegs);

        RotatorNode.Rotation = new Vector3(0, RotateRads, 0);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateMaterials()
    {
        DomeColor = FssColorUtil.StringToColor(Name);
        DomeColor.A = 0.4f;
        MatDome = FssMaterialFactory.TransparentColoredMaterial(DomeColor);

        SegmentColor = DomeColor;
        SegmentColor.A = 0.7f;
        MatSegment = FssMaterialFactory.TransparentColoredMaterial(SegmentColor);

        WireColor = FssColorUtil.StringToColor(Name);
        WireColor.A = 1.0f;
    }

    private void CreateDome()
    {

        FssMeshBuilder meshBuilder  = new ();

        int numSegments = 20;

        if (true)
        {
            float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddHemisphere(Vector3.Zero, rxDist, numSegments);

            ArrayMesh meshData = meshBuilder.Build2("Dome", false);

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstance   = new() { Name = $"{Name}Dome" };
            rxMeshInstance.Mesh             = meshData;
            rxMeshInstance.MaterialOverride = MatDome;
            AddChild(rxMeshInstance);

            LineMesh = new FssLineMesh3D() { Name = $"{Name}Wire" };
            LineMesh.AddHemisphere(Vector3.Zero, rxDist, numSegments, WireColor);
            AddChild(LineMesh);
        }
    }

    // Create the segment we can spin at the scan pattern rate
    private void CreateSegment()
    {
        FssMeshBuilder meshBuilder  = new ();

        float azMin = -5f;
        float azMax =  5f;
        float elMin =  5f;
        float elMax = 80f;
        float distMin = (float)(200         * FssZeroOffset.RwToGeDistanceMultiplierM);
        float distMax = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM) * 0.95f;

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

    public void SetVisibility(bool beamsVisible, bool isVisible)
    {
        foreach (Node3D child in GetChildren())
        {
            child.Visible = (isVisible & beamsVisible);
        }
    }
}