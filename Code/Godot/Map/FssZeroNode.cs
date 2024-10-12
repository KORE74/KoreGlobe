
using System;

using Godot;

// Class representing the zero anchor position in the game engine world.

public partial class FssZeroNode : Node3D
{
    // The root node for all entities, made static so it can be accessed from anywhere.
    public static Node3D EntityRootNode = new Node3D() { Name = "EntityRootNode" };

    private static FssLLAPoint ZeroPosToApply = new FssLLAPoint() { LatDegs = 0, LonDegs = 0, RadiusM = FssPosConsts.EarthRadiusM };

    private float Timer1Hz = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "ZeroNode";

        // Setup the Zero Node.
        //FssZeroOffset.ZeroNode = this;

        // Add the EntityRootNode to the ZeroNode,
        //AddChild(EntityRootNode);

        CreateDebugMarker();

        //FssLineMesh3D lineCube = new FssLineMesh3D();
        //AddChild(lineCube);


        //AddChild(GodotEntityManager);

        //CallDeferred("SetZeroNodePositionDeferred");
    }

    public override void _Process(double delta)
    {
        if (Timer1Hz < FssCoreTime.RuntimeSecs)
        {
            Timer1Hz = FssCoreTime.RuntimeSecs + 1.0f;
            // CallDeferred("SetZeroNodePositionDeferred");

            SetZeroNodePositionDeferred();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: ZeroNode Position
    // --------------------------------------------------------------------------------------------

    // Usage: FssZeroNode.SetZeroNodePosition(pos);
    public static void SetZeroNodePosition(FssLLAPoint pos)
    {
        ZeroPosToApply = pos;
    }

    // Usage: FssZeroNode.SetZeroNodePosition(53.0, -6.0);
    public static void SetZeroNodePosition(double lat, double lon)
    {
        // Set the position of the ZeroNode.
        FssLLAPoint pos = new FssLLAPoint() {
            LatDegs = lat,
            LonDegs = lon,
            RadiusM = FssPosConsts.EarthRadiusM };

        ZeroPosToApply = pos;
    }


    // Usage: FssZeroNode.SetZeroNodePosition(pos);
    public static void SetZeroNodePositionDeferred()
    {
        // Set the position of the ZeroNode.
        FssZeroOffset.SetLLA(ZeroPosToApply);

        // Update the cnsequent positions of the ZeroNode and EarthCoreNode.
        FssGodotFactory.Instance.ZeroNode.Position      = FssZeroOffset.GeZeroPoint();
        FssGodotFactory.Instance.EarthCoreNode.Position = FssZeroOffset.GeCorePoint();
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------

    private void CreateDebugMarker()
    {
        float markerSize  = 2f;

        Node3D zeroNodeMarker = FssPrimitiveFactory.CreateSphereNode("Marker", new Vector3(0f, 0f, 0f), markerSize, FssColorUtil.Colors["OffRed"], true);
        AddChild(zeroNodeMarker);

        zeroNodeMarker.AddChild( FssPrimitiveFactory.AxisMarkerNode(markerSize, markerSize/4) );
    }
}
