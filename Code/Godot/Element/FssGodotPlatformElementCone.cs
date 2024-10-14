

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssGodotPlatformElementCone : FssGodotPlatformElement
{

    public string SourcePlatformName;
    public string TargetPlatformName;
    public float ConeAzDegs = 1.0f;
    public float ConeLengthM = 1.0f;

    FssLineMesh3D LineMesh   = new FssLineMesh3D();
    MeshInstance3D MeshInstance;

    Color ConeColor;
    Color WireColor;
    StandardMaterial3D MatCone;

    FssPolarOffset DirectionPolar = new FssPolarOffset();

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
        FssMeshBuilder meshBuilder  = new FssMeshBuilder();

        // Create the cone
        meshBuilder.AddCone(100, ConeLengthM, 32);
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