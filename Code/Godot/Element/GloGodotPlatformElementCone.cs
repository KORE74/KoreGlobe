

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class GloGodotPlatformElementCone : GloGodotPlatformElement
{
    public string SourcePlatformName;
    public string TargetPlatformName;
    public float  ConeAzDegs    = 1.0f;
    public float  TxConeLengthM = 1.0f;
    public float  RxConeLengthM = 1.0f;
    public bool   Targeted = false;

    GloLineMesh3D LineMesh = new GloLineMesh3D();

    Color ConeColor;
    Color WireColor;
    StandardMaterial3D MatCone;

    GloAzElRange DirectionPolar = new GloAzElRange();

    private Node3D RotatorNode = new Node3D();
    private MeshInstance3D RxConeNode;
    private MeshInstance3D TxConeNode;

    private GloLineMesh3D RxLineMesh;
    private GloLineMesh3D TxLineMesh;
    private GloLineMesh3D FullLineMesh;

    private bool RxVisible = true;
    private bool TxVisible = true;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateMaterials();
        CreateCone();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Targeted)
            PlatformPositionLookup();
        else
            RotatorNode.Rotation = new Vector3(0, 0, 0);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Target PositionLookup
    // --------------------------------------------------------------------------------------------

    public void PlatformPositionLookup()
    {
        // Find and validation the basic LLA positions
        GloLLAPoint? sourcepos = GloAppFactory.Instance.EventDriver.GetPlatformPosition(SourcePlatformName);
        GloLLAPoint? targetpos = GloAppFactory.Instance.EventDriver.GetPlatformPosition(TargetPlatformName);

        if (sourcepos == null || targetpos == null)
        {
           Targeted = false;
           return;
        }

        GloLLAPoint sourcePos2 = (GloLLAPoint)sourcepos;
        GloLLAPoint targetPos2 = (GloLLAPoint)targetpos;

        // Find the polar offset between the two points
        GloAzElRange sourceToTargetPolar = sourcePos2.StraightLinePolarOffsetTo(targetPos2);

        // Find the source heading, to offset the output
        double sourceHeadingDegs = 0;
        GloCourse? currCourse = GloAppFactory.Instance.EventDriver.PlatformCurrCourse(SourcePlatformName);
        if (currCourse != null)
            sourceHeadingDegs = ((GloCourse)currCourse).HeadingDegs;

        DirectionPolar.ElDegs = sourceToTargetPolar.ElDegs;
        DirectionPolar.AzDegs = sourceToTargetPolar.AzDegs - sourceHeadingDegs;
        DirectionPolar.RangeM = TxConeLengthM;

        float geAzimuithRads  = (float)GloValueUtils.DegsToRads(DirectionPolar.AzDegs) * -1f;
        float geElevationRads = (float)GloValueUtils.DegsToRads(DirectionPolar.ElDegs);

        // Set the rotation of the rotator node
        RotatorNode.Rotation = new Vector3(geElevationRads, geAzimuithRads, 0);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateMaterials()
    {
        WireColor = GloColorUtil.StringToColor(Name);

        ConeColor = WireColor;
        ConeColor.A = 0.4f;
        MatCone = GloMaterialFactory.TransparentColoredMaterial(ConeColor);
    }

    private void CreateCone()
    {
        // Functino working variables
        int numConeSideFaces = 16;
        //ConeAzDegs = 10f;
        GloMeshBuilder txMeshBuilder = new GloMeshBuilder();
        GloMeshBuilder rxMeshBuilder = new GloMeshBuilder();

        // Object creation
        TxConeNode   = new MeshInstance3D() { Name = $"{Name}-TxCone" };
        RxConeNode   = new MeshInstance3D() { Name = $"{Name}-RxCone" };
        TxLineMesh   = new GloLineMesh3D()  { Name = $"{Name}-TxWire" };
        RxLineMesh   = new GloLineMesh3D()  { Name = $"{Name}-RxWire" };
        FullLineMesh = new GloLineMesh3D()  { Name = $"{Name}-FullWire" };

        // Calc widths and lengths
        // Limit length to 500km

        double minConeM = 100;
        double maxConeM = 500000;
        bool txCropped = false;
        bool rxCropped = false;
        if ((TxConeLengthM < minConeM) || (maxConeM < TxConeLengthM)) txCropped = true;
        if ((RxConeLengthM < minConeM) || (maxConeM < RxConeLengthM)) rxCropped = true;
        float txConeLen   = (float)(GloValueUtils.LimitToRange(TxConeLengthM, 100, 500000) * KoreZeroOffset.RwToGeDistanceMultiplier);
        float rxConeLen   = (float)(GloValueUtils.LimitToRange(RxConeLengthM, 100, 500000) * KoreZeroOffset.RwToGeDistanceMultiplier);

        GloLineMesh3D.ConeStyle rxDrawStyle = GloLineMesh3D.ConeStyle.Cone;
        GloLineMesh3D.ConeStyle txDrawStyle = GloLineMesh3D.ConeStyle.Cone;
        if (rxCropped)          rxDrawStyle = GloLineMesh3D.ConeStyle.CroppedCone;
        if (txCropped)          txDrawStyle = GloLineMesh3D.ConeStyle.CroppedCone;

        // Use simple trig to turn the ConeAzDegs into a width, using the length as the adjacent side
        // tan(angle) = opposite / adjacent
        // opposite = tan(ConeAzDegs) * coneLen
        float txConeWidth = (float)System.Math.Tan(GloValueUtils.DegsToRads(ConeAzDegs)) * txConeLen;
        float rxConeWidth = (float)System.Math.Tan(GloValueUtils.DegsToRads(ConeAzDegs)) * rxConeLen;

        // - - - - Create the Tx cone - - - -
        txMeshBuilder.AddCone(txConeWidth, txConeLen, numConeSideFaces);
        ArrayMesh meshData = txMeshBuilder.Build2("TxCone", false);

        // Create the mesh and add it.
        TxConeNode.Mesh             = meshData;
        TxConeNode.MaterialOverride = MatCone;
        TxLineMesh.AddCone(txConeWidth, txConeLen, numConeSideFaces, WireColor, txDrawStyle);
        FullLineMesh.AddCone(txConeWidth, txConeLen, numConeSideFaces, WireColor, txDrawStyle);

        // - - - - Create the Rx cone - - - -
        rxMeshBuilder.AddCone(rxConeWidth, rxConeLen, numConeSideFaces);
        meshData = rxMeshBuilder.Build2("RxCone", false);

        // Create the mesh and add it.
        RxConeNode.Mesh             = meshData;
        RxConeNode.MaterialOverride = MatCone;
        RxLineMesh.AddCone(rxConeWidth, rxConeLen, numConeSideFaces, WireColor, rxDrawStyle);
        FullLineMesh.AddCone(rxConeWidth, rxConeLen, numConeSideFaces, WireColor, rxDrawStyle);

        // Create the rotator Node to align the cone and add it
        RotatorNode = new Node3D() { Name = $"{Name}-Rotator" };
        AddChild(RotatorNode);

        // Add the mesh to the current Node3D
        RotatorNode.AddChild(TxConeNode);
        RotatorNode.AddChild(RxConeNode);
        RotatorNode.AddChild(TxLineMesh);
        RotatorNode.AddChild(RxLineMesh);
        RotatorNode.AddChild(FullLineMesh);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool beamsVisible, bool rxVisible, bool txVisible)
    {
        RxVisible = rxVisible & beamsVisible;
        TxVisible = txVisible & beamsVisible;

        // Update the mesh (volume) visibility
        RxConeNode.Visible = RxVisible;
        TxConeNode.Visible = TxVisible;

        // Update the wireframe visibility
        RxLineMesh.Visible   = RxVisible && !TxVisible;
        TxLineMesh.Visible   = TxVisible && !RxVisible;
        FullLineMesh.Visible = RxVisible && TxVisible;
    }
}

