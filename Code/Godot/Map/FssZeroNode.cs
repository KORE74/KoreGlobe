
using System;

using Godot;

// Class representing the zero anchor position in the game engine world.

public partial class FssZeroNode : Node3D
{
    // The root node for all entities, made static so it can be accessed from anywhere.
    public static Node3D EntityRootNode = new Node3D() { Name = "EntityRootNode" };

    private float MarkerSize  = 0.2f;

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
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------

    private void CreateDebugMarker()
    {
        float markerSize = 0.2f;

        Node3D zeroNodeMarker = FssPrimitiveFactory.CreateSphereNode("Marker", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffRed"], true);
        AddChild(zeroNodeMarker);

        zeroNodeMarker.AddChild( FssPrimitiveFactory.AxisMarkerNode(MarkerSize, MarkerSize/4) );
    }
}
