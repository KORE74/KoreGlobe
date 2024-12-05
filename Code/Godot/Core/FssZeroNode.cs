
// using System;

// using Godot;

// // Class representing the zero anchor position in the game engine world.

// public partial class FssZeroNode : Node3D
// {
//     // The root node for all entities, made static so it can be accessed from anywhere.
//     public static Node3D EntityRootNode = new Node3D() { Name = "EntityRootNode" };

//     private static FssLLAPoint ZeroPosToApply = new FssLLAPoint() { LatDegs = 0, LonDegs = 0, RadiusM = FssPosConsts.EarthRadiusM };

//     private float Timer1Hz = 0.0f;

//     // Usage: FssZeroNode.ZeroNodeUpdateCycle
//     public static bool ZeroNodeUpdateCycle = false;

//     // --------------------------------------------------------------------------------------------
//     // MARK: Node Functions
//     // --------------------------------------------------------------------------------------------

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         Name = "ZeroNode";
//     }

//     public override void _Process(double delta)
//     {
//         ZeroNodeUpdateCycle = false;

//         if (Timer1Hz < FssCentralTime.RuntimeSecs)
//         {
//             Timer1Hz = FssCentralTime.RuntimeSecs + 1.0f;

//             SetZeroNodePositionDeferred();
//             ZeroNodeUpdateCycle = true;
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: ZeroNode Position
//     // --------------------------------------------------------------------------------------------

//     // Usage: FssZeroNode.SetZeroNodePosition(pos);
//     public static void SetZeroNodePosition(FssLLAPoint pos)
//     {
//         ZeroPosToApply = pos;
//     }

//     // Usage: FssZeroNode.SetZeroNodePosition(53.0, -6.0);
//     public static void SetZeroNodePosition(double lat, double lon)
//     {
//         // Set the position of the ZeroNode.
//         FssLLAPoint pos = new FssLLAPoint() {
//             LatDegs = lat,
//             LonDegs = lon,
//             RadiusM = FssPosConsts.EarthRadiusM };

//         ZeroPosToApply = pos;
//     }

//     // Internal function to actually apply the new zero offset position, of the node's own timeline.

//     private void SetZeroNodePositionDeferred()
//     {
//         // Set the position of the ZeroNode.
//         FssZeroOffset.SetLLA(ZeroPosToApply);

//         // Update the cnsequent positions of the ZeroNode and EarthCoreNode.
//         FssGodotFactory.Instance.ZeroNode.Position      = FssZeroOffset.GeZeroPoint();
//         // FssGodotFactory.Instance.EarthCoreNode.Position = FssZeroOffset.GeCorePoint();
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Helpers
//     // --------------------------------------------------------------------------------------------

//     private void CreateDebugMarker()
//     {
//         float markerSize  = 2f;

//         Node3D zeroNodeMarker = FssPrimitiveFactory.CreateSphereNode("Marker", new Vector3(0f, 0f, 0f), markerSize, FssColorUtil.Colors["OffRed"], true);
//         AddChild(zeroNodeMarker);

//         zeroNodeMarker.AddChild( FssPrimitiveFactory.AxisMarkerNode(markerSize, markerSize/4) );
//     }
// }
