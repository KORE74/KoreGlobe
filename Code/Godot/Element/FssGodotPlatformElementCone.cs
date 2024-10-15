

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssGodotPlatformElementCone : FssGodotPlatformElement
{

    public string SourcePlatformName;
    public string TargetPlatformName;
    public float ConeAzDegs = 1.0f;
    public float ConeLengthM = 1.0f;

    FssLineMesh3D LineMesh   = new FssLineMesh3D();

    Color ConeColor;
    Color WireColor;
    StandardMaterial3D MatCone;

    FssPolarOffset DirectionPolar = new FssPolarOffset();

    private Node3D RotatorNode = new Node3D();
    private MeshInstance3D ConeNode;

    private bool IsVisible = true;

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
        PlatformPositionLookup();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Target PositionLookup
    // --------------------------------------------------------------------------------------------

    public void PlatformPositionLookup()
    {
        // Find and validation the basic LLA positions
        FssLLAPoint? sourcepos = FssAppFactory.Instance.EventDriver.GetPlatformPosition(SourcePlatformName);
        FssLLAPoint? targetpos = FssAppFactory.Instance.EventDriver.GetPlatformPosition(TargetPlatformName);

        if (sourcepos == null || targetpos == null)
           return;

        FssLLAPoint sourcePos2 = (FssLLAPoint)sourcepos;
        FssLLAPoint targetPos2 = (FssLLAPoint)targetpos;

        // Find the polar offset between the two points
        FssPolarOffset sourceToTargetPolar = sourcePos2.StraightLinePolarOffsetTo(targetPos2);

        // Find the source heading, to offset the output
        double sourceHeadingDegs = 0;
        FssCourse? currCourse = FssAppFactory.Instance.EventDriver.PlatformCurrCourse(SourcePlatformName);
        if (currCourse != null)
            sourceHeadingDegs = ((FssCourse)currCourse).HeadingDegs;

        DirectionPolar.ElDegs = sourceToTargetPolar.ElDegs;
        DirectionPolar.AzDegs = sourceToTargetPolar.AzDegs - sourceHeadingDegs;
        DirectionPolar.RangeM = ConeLengthM;

        float geAzimuithDegs = (float)FssValueUtils.DegsToRads(DirectionPolar.AzDegs) * -1f;
        float geElevationDegs = (float)FssValueUtils.DegsToRads(DirectionPolar.ElDegs);

        // Set the rotation of the rotator node
        RotatorNode.Rotation = new Vector3(geElevationDegs, geAzimuithDegs, 0);

        // if (pos != null)
        //     return (FssLLAPoint)pos;
        // else
        //     return FssLLAPoint.Zero;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateMaterials()
    {
        WireColor = FssColorUtil.StringToColor(Name);

        ConeColor = WireColor;
        ConeColor.A = 0.4f;
        MatCone = FssMaterialFactory.TransparentColoredMaterial(ConeColor);
    }

    private void CreateCone()
    {
        FssMeshBuilder meshBuilder = new FssMeshBuilder();

        float coneLen   = ConeLengthM * (float)FssZeroOffset.RwToGeDistanceMultiplierM;
        float coneWidth = 100.0f      * (float)FssZeroOffset.RwToGeDistanceMultiplierM;

        // Create the cone
        meshBuilder.AddCone(coneWidth, coneLen, 32);
        ArrayMesh meshData = meshBuilder.Build2("Cone", false);

        // Create the mesh and add it.
        ConeNode = new MeshInstance3D() { Name = $"{Name}-Cone" };
        ConeNode.Mesh             = meshData;
        ConeNode.MaterialOverride = MatCone;

        FssLineMesh3D LineMesh = new FssLineMesh3D() { Name = $"{Name}-Wire" };
        LineMesh.AddCone(coneWidth, coneLen, 32, WireColor);

        // Create the rotator Node to align the cone and add it
        RotatorNode = new Node3D() { Name = $"{Name}-Rotator" };
        AddChild(RotatorNode);

        // Add the mesh to the current Node3D
        RotatorNode.AddChild(ConeNode);
        RotatorNode.AddChild(LineMesh);


    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool beamVisible)
    {
        IsVisible = beamVisible;

        Visible = IsVisible;
    }


}