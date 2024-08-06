using Godot;
using System;

public partial class TestMovingGeometryCore : Node3D
{
    // public Vector3 CorePos  = new Vector3(-3, 0, 0);
    // public Vector3 FocusPos = new Vector3 (3, 0, 0);

    // Node3D CoreNode;
    // Node3D ZeroNode;
    // Node3D FocusPointNode;
    // Node3D LinkCylinderNode;

    // Node3D PlaformBaseNode;
    // Node3D ModelResourceNode;
    // Node3D TrailNode;

    // Node3D TestNode;
    // Node3D TestNodeAhead;
    // Node3D TestNodeAbove;



    // MeshInstance3D LinkCylinderMesh;
    // MeshInstance3D LinkCylinderWire;

    // Material matColorRed;
    // Material matColorBlue;
    // Material matColorYellow;
    // Material matColorWhite;
    // Material matWire;

    // private float MarkerSize  = 0.2f;
    // private bool  WithWire = true;

    // private float AnimAzDegs  = 0f;
    // private float AnimElDegs  = 0f;
    // private float AnimAzDelta = 1.55f;
    // private float AnimElDelta = 1.05f;

    // private float UIPollTimer = 0.0f;
    // private float UIPollTimer2 = 0.0f;
    // private float PollTimerTrailNode = 0.0f;

    // private FssLLAPoint    PlatformPos;
    // private FssCourse      PlatformCourse;
    // private FssCourseDelta PlatformCourseDelta;

    // private FssCyclicIdGenerator IdGen = new FssCyclicIdGenerator(250);
    // private string randomString = FssRandomStringGenerator.GenerateRandomString(5);

    // // --------------------------------------------------------------------------------------------
    // // MARK: Node _Ready and _Process
    // // --------------------------------------------------------------------------------------------

    // // Called when the node enters the scene tree for the first time.
    // public override void _Ready()
    // {
    //     FssEarthCore.EarthRadiusM = 10;

    //     matColorRed    = FssMaterialFactory.SimpleColoredMaterial(new Color(0.9f, 0.3f, 0.3f, 1f));
    //     matColorBlue   = FssMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.9f, 1f));
    //     matColorYellow = FssMaterialFactory.SimpleColoredMaterial(FssColorUtil.Colors["OffYellow"]);
    //     matColorWhite  = FssMaterialFactory.SimpleColoredMaterial(new Color(1f, 1f, 1f, 1f));
    //     matWire        = FssMaterialFactory.WireframeWhiteMaterial();

    //     CreateCoreNode();
    //     CreateLinkCylinder();

    //     CreatePlatform();
    //     CreatePlatformNodes();
    // }

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(double delta)
    // {
    //     if (UIPollTimer < FssCoreTime.RuntimeSecs)
    //     {
    //         UIPollTimer = FssCoreTime.RuntimeSecs + 1f; // Update the timer to the next whole second

    //         GD.Print($"FocusPoint: Lat:{FssEarthCore.RwFocusLLA.LatDegs:0.00} Lon:{FssEarthCore.RwFocusLLA.LonDegs:0.00} RadiusM:{FssEarthCore.RwFocusLLA.RadiusM:0.00}");
    //         GD.Print($"PlatPos: Lat:{PlatformPos.LatDegs:0.00} Lon:{PlatformPos.LonDegs:0.00} RadiusM:{PlatformPos.RadiusM:0.00} // Course: Heading:{PlatformCourse.HeadingDegs:0.00} Speed:{PlatformCourse.SpeedMps:0.00}");
    //         GD.Print($"FocusPos: {FssEarthCore.FocusPos} // CorePos: {FssEarthCore.CorePos} // CorePos Magnitude {FssEarthCore.CorePos.Length()}");
    //     }

    //     float scale = 0.7f;

    //     AnimAzDegs += AnimAzDelta * ((float)delta * scale);
    //     AnimElDegs += AnimElDelta * ((float)delta * scale);

    //     if (AnimElDegs >  25f) AnimElDelta = -1.05f;
    //     if (AnimElDegs <   0f) AnimElDelta =  1.05f;
    //     if (AnimAzDegs >  25f) AnimAzDelta = -1.55f;
    //     if (AnimAzDegs <   0f) AnimAzDelta =  1.55f;

    //     FssEarthCore.RwFocusLLA = new FssLLAPoint() {
    //         LatDegs = AnimElDegs,
    //         LonDegs = AnimAzDegs,
    //         RadiusM = FssEarthCore.EarthRadiusM };

    //     // Update the gloabel Focus Position, LLA and Vector3
    //     FssEarthCore.UpdatePositions();

    //     UpdatePositions();
    //     UpdateLinkCylinder();

    //     UpdatePlatform(delta);
    //     UpdatePlatformNodes(delta);
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Create Major moving geometry nodes.
    // // --------------------------------------------------------------------------------------------

    // private void CreateCoreNode()
    // {
    //     CoreNode = FssPrimitiveFactory.CreateSphereNode("CoreNode", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffBlue"], WithWire);
    //     AddChild(CoreNode);

    //     FocusPointNode = FssPrimitiveFactory.CreateSphereNode("FocusPointNode", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffYellow"], WithWire);
    //     AddChild(FocusPointNode);

    //     ZeroNode = FssPrimitiveFactory.CreateSphereNode("ZeroNode", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffRed"], WithWire);
    //     AddChild(ZeroNode);


    //     {
    //         Node3D tempNode1 = FssPrimitiveFactory.CreateSphereNode("tempNode1", new Vector3(0f, 1f, 0f), MarkerSize/3, FssColorUtil.Colors["Magenta"], WithWire);
    //         FocusPointNode.AddChild(tempNode1);

    //         Node3D tempNode2 = FssPrimitiveFactory.CreateSphereNode("tempNode2", new Vector3(0f, 2f, 0f), MarkerSize/3, FssColorUtil.Colors["Magenta"], WithWire);
    //         FocusPointNode.AddChild(tempNode2);

    //         Node3D tempNode3 = FssPrimitiveFactory.CreateSphereNode("tempNode3", new Vector3(1f, 1f, 0f), MarkerSize/3, FssColorUtil.Colors["Magenta"], WithWire);
    //         FocusPointNode.AddChild(tempNode3);
    //     }

    //     {
    //         TestNode = FssPrimitiveFactory.CreateSphereNode("TestNode", Vector3.Zero, MarkerSize/3, FssColorUtil.Colors["Yellow"], WithWire);
    //         FocusPointNode.AddChild(TestNode);

    //         TestNodeAbove = FssPrimitiveFactory.CreateSphereNode("TestNodeAbove", Vector3.Zero, MarkerSize/3, FssColorUtil.Colors["Yellow"], WithWire);
    //         FocusPointNode.AddChild(TestNodeAbove);

    //         TestNodeAhead = FssPrimitiveFactory.CreateSphereNode("TestNodeAhead", Vector3.Zero, MarkerSize/3, FssColorUtil.Colors["Yellow"], WithWire);
    //         FocusPointNode.AddChild(TestNodeAhead);
    //     }


    //     CoreNode.AddChild(new TestEarthCore((float)FssEarthCore.EarthRadiusM));
    //     CoreNode.AddChild(new TestLabelMaker());
    // }

    // private void CreateLinkCylinder()
    // {
    //     // Create the focus point node
    //     LinkCylinderNode = new Node3D() { Name = "LinkCylinder" };
    //     AddChild(LinkCylinderNode);

    //     LinkCylinderMesh                  = new MeshInstance3D() { Name = "Color" };
    //     LinkCylinderMesh.MaterialOverride = matColorWhite;
    //     LinkCylinderNode.AddChild(LinkCylinderMesh);

    //     LinkCylinderWire                  = new MeshInstance3D() { Name = "Wire" };
    //     LinkCylinderWire.MaterialOverride = matWire;
    //     LinkCylinderNode.AddChild(LinkCylinderWire);
    // }

    // private void UpdateLinkCylinder()
    // {
    //     Vector3 frompoint = FssEarthCore.CorePos;
    //     Vector3 topoint   = FssEarthCore.FocusPos;
    //     Vector3 pointdiff = topoint - frompoint;

    //     float radius = MarkerSize * 0.5f;

    //     FssMeshBuilder meshBuilder = new FssMeshBuilder();
    //     meshBuilder.AddCylinder(Vector3.Zero, pointdiff, radius, radius, 12, false);
    //     ArrayMesh meshData = meshBuilder.Build2("LinkCylinder", false);

    //     // Mesh
    //     LinkCylinderMesh.Position = frompoint;
    //     LinkCylinderMesh.Mesh     = meshData;

    //     // Wireframe
    //     LinkCylinderWire.Position = frompoint;
    //     LinkCylinderWire.Mesh     = meshData;
    // }

    // private void UpdatePositions()
    // {
    //     CoreNode.Position       = FssEarthCore.CorePos;
    //     FocusPointNode.Position = FssEarthCore.FocusPos;

    //     // Ensure the core and focus point rotations are zero
    //     CoreNode.Rotation       = new Vector3(0f, 0f, 0f);
    //     FocusPointNode.Rotation = new Vector3(0f, 0f, 0f);
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Create and update moving platform
    // // --------------------------------------------------------------------------------------------

    // private void CreatePlatform()
    // {
    //     PlatformPos = new FssLLAPoint() {
    //         LatDegs = 10f,
    //         LonDegs = 10f,
    //         RadiusM = 10f };

    //     PlatformCourse = new FssCourse() {
    //         HeadingDegs = 0f,
    //         SpeedMps    = 0.15f };

    //     PlatformCourseDelta = new FssCourseDelta() {
    //         HeadingChangeClockwiseDegsSec = -8f,
    //         SpeedChangeMpMps              = 0f };
    // }

    // private void CreatePlatformNodes()
    // {
    //     // Create the core for the platform, to place and orient
    //     PlaformBaseNode = new Node3D() { Name = "PlaformBaseNode" };
    //     FocusPointNode.AddChild(PlaformBaseNode);

    //     FssPrimitiveFactory.AddAxisMarkers(PlaformBaseNode, 0.2f, 0.05f);

    //     string ModelPath = "res://Resources/Plane_Paper/PaperPlanes_v002.glb";

    //     PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);
    //     if (importedModel != null)
    //     {
    //         // Instance the model
    //         Node modelInstance     = importedModel.Instantiate();
    //         ModelResourceNode      = modelInstance as Node3D;
    //         ModelResourceNode.Name = "ModelResourceNode";
    //         ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up);

    //         PlaformBaseNode.AddChild(ModelResourceNode);
    //         ModelResourceNode.Scale    = new Vector3(0.25f, 0.25f, 0.25f); // Set the model scale
    //         ModelResourceNode.Position = new Vector3(0f, 0f, 0f); // Set the model position
    //     }
    // }

    // private void UpdatePlatform(double delta)
    // {
    //     // Update real world position
    //     PlatformCourse = PlatformCourse.PlusDeltaForTime(PlatformCourseDelta, delta);
    //     PlatformPos    = PlatformPos.PlusDeltaForTime(PlatformCourse, delta);
    // }

    // private void UpdatePlatformNodes(double delta)
    // {
    //     FssRWPlatformPositions rwStruct = FssGeoConvOperations.RealWorldStruct(PlatformPos, PlatformCourse);

    //     // PlaformBaseNode.Position = rwStruct.vecPos;
    //     // PlaformBaseNode.LookAt(rwStruct.vecPosAhead, rwStruct.vecPosAbove, true);

    //     // Place the yellow nodes - local co-ordinate system to the focus node.
    //     TestNode.Position      = rwStruct.vecPos;
    //     TestNodeAbove.Position = rwStruct.vecPosAbove;
    //     TestNodeAhead.Position = rwStruct.vecPosAhead;

    //     // Vector3 pos   = new Vector3(0f, 1f, 0f);
    //     // Vector3 ahead = new Vector3(0f, 2f, 0f);
    //     // Vector3 above = new Vector3(1f, 1f, 0f);

    //     Vector3 adjustedPos   = rwStruct.vecPos + FssEarthCore.FocusPos;
    //     Vector3 adjustedAhead = ToGlobal(rwStruct.vecPosAhead + FssEarthCore.FocusPos);
    //     Vector3 adjustedAbove = ToGlobal(rwStruct.vecPosAbove + FssEarthCore.FocusPos);

    //     // ahead += FssEarthCore.FocusPos;
    //     // above += FssEarthCore.FocusPos;

    //     PlaformBaseNode.Position = adjustedPos;
    //     PlaformBaseNode.LookAt(adjustedAhead, adjustedAbove, true);

    //     PlaformBaseNode.LookAtFromPosition(
    //         adjustedPos,
    //         adjustedAhead,
    //         adjustedAbove,
    //         true);

    // }

}
