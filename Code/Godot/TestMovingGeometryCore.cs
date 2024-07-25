using Godot;
using System;

public partial class TestMovingGeometryCore : Node3D
{
    public Vector3 CorePos  = new Vector3(-3, 0, 0);
    public Vector3 FocusPos = new Vector3 (3, 0, 0);

    Node3D CoreNode;
    Node3D ZeroNode;
    Node3D FocusPointNode;
    Node3D LinkCylinderNode;

    MeshInstance3D LinkCylinderMesh;
    MeshInstance3D LinkCylinderWire;

    Material matColorRed;
    Material matColorBlue;
    Material matColorYellow;
    Material matColorWhite;
    Material matWire;

    private float MarkerSize  = 0.2f;

    private float AnimAzDegs  = 0f;
    private float AnimElDegs  = 0f;
    private float AnimAzDelta = 1.55f;
    private float AnimElDelta = 1.05f;

    private float UIPollTimer = 0.0f;
    private float UIPollTimer2 = 0.0f;

    private FssLLAPoint    PlatformPos;
    private FssCourse      PlatformCourse;
    private FssCourseDelta PlatformCourseDelta;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssEarthCore.EarthRadiusM = 10;

        matColorRed    = FssMaterialFactory.SimpleColoredMaterial(new Color(0.9f, 0.3f, 0.3f, 1f));
        matColorBlue   = FssMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.9f, 1f));
        matColorYellow = FssMaterialFactory.SimpleColoredMaterial(new Color(0.8f, 0.8f, 0.3f, 1f));
        matColorWhite  = FssMaterialFactory.SimpleColoredMaterial(new Color(1f, 1f, 1f, 1f));
        matWire        = FssMaterialFactory.WireframeWhiteMaterial();

        CreateCoreNode();
        CreateFocusNode();
        CreateZeroNode();
        CreateLinkCylinder();

        CreatePlatform();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (UIPollTimer < FssCoreTime.RuntimeSecs)
        {
            UIPollTimer = FssCoreTime.RuntimeSecs + 1f; // Update the timer to the next whole second

            GD.Print($"FocusPoint: Lat:{FssEarthCore.FocusPoint.LatDegs:0.00} Lon:{FssEarthCore.FocusPoint.LonDegs:0.00} RadiusM:{FssEarthCore.FocusPoint.RadiusM:0.00}");

            GD.Print($"PlatPos: Lat:{PlatformPos.LatDegs:0.00} Lon:{PlatformPos.LonDegs:0.00} RadiusM:{PlatformPos.RadiusM:0.00}");
        }


        float scale = 0.02f;

        AnimAzDegs += AnimAzDelta * ((float)delta * scale);
        AnimElDegs += AnimElDelta * ((float)delta * scale);

        if (AnimElDegs >  25f) AnimElDelta = -1.05f;
        if (AnimElDegs <   0f) AnimElDelta =  1.05f;
        if (AnimAzDegs >  25f) AnimAzDelta = -1.55f;
        if (AnimAzDegs <   0f) AnimAzDelta =  1.55f;

        FssEarthCore.FocusPoint = new FssLLAPoint() {
            LatDegs = AnimElDegs,
            LonDegs = AnimAzDegs,
            RadiusM = FssEarthCore.EarthRadiusM };

        FssEarthCore.UpdatePositions();

        // FocusPos = FssEarthCore.FocusPos;
        // CorePos  = FssEarthCore.CorePos;

        UpdatePositions();
        UpdateLinkCylinder();

        UpdatePlatform(delta);
    }

    // --------------------------------------------------------------------------------------------

    private void CreateCoreNode()
    {
        // Create the core node
        CoreNode = new Node3D() { Name = "CoreNode" };
        AddChild(CoreNode);

        // Add sphere for the core point
        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSphere(Vector3.Zero, MarkerSize, 16);
        ArrayMesh meshData = meshBuilder.Build2("Sphere", false);

        MeshInstance3D meshInstance    = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matColorBlue;

        MeshInstance3D meshInstanceW   = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        CoreNode.AddChild(meshInstance);
        CoreNode.AddChild(meshInstanceW);

        // Attach the script to the core node
        CoreNode.AddChild(new TestEarthCore((float)FssEarthCore.EarthRadiusM));

        CoreNode.AddChild(new TestLabelMaker());
    }

    private void CreateFocusNode()
    {
        // Create the focus point node
        FocusPointNode = new Node3D() { Name = "FocusPointNode" };
        AddChild(FocusPointNode);

        // Add sphere for the core point
        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSphere(Vector3.Zero, MarkerSize, 16);
        ArrayMesh meshData = meshBuilder.Build2("Sphere", false);

        MeshInstance3D meshInstance    = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matColorYellow;

        MeshInstance3D meshInstanceW   = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        FocusPointNode.AddChild(meshInstance);
        FocusPointNode.AddChild(meshInstanceW);
    }

    private void CreateZeroNode()
    {
        // Create the focus point node
        ZeroNode = new Node3D() { Name = "ZeroNode" };
        AddChild(ZeroNode);

        // Add sphere for the core point
        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddSphere(Vector3.Zero, MarkerSize, 16);
        ArrayMesh meshData = meshBuilder.Build2("Sphere", false);

        MeshInstance3D meshInstance    = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matColorRed;

        MeshInstance3D meshInstanceW   = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        ZeroNode.AddChild(meshInstance);
        ZeroNode.AddChild(meshInstanceW);
    }

    private void CreateLinkCylinder()
    {
        // Create the focus point node
        LinkCylinderNode = new Node3D() { Name = "LinkCylinder" };
        AddChild(LinkCylinderNode);

        LinkCylinderMesh                  = new MeshInstance3D() { Name = "Color" };
        LinkCylinderMesh.MaterialOverride = matColorWhite;
        LinkCylinderNode.AddChild(LinkCylinderMesh);

        LinkCylinderWire                  = new MeshInstance3D() { Name = "Wire" };
        LinkCylinderWire.MaterialOverride = matWire;
        LinkCylinderNode.AddChild(LinkCylinderWire);
    }

    private void UpdateLinkCylinder()
    {
        Vector3 frompoint = FssEarthCore.CorePos;
        Vector3 topoint   = FssEarthCore.FocusPos;
        Vector3 pointdiff = topoint - frompoint;

        float radius = MarkerSize * 0.5f;

        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddCylinder(Vector3.Zero, pointdiff, radius, radius, 12, true);
        ArrayMesh meshData = meshBuilder.Build2("Cylinder", false);

        LinkCylinderMesh.Position = frompoint;
        LinkCylinderMesh.Mesh     = meshData;

        LinkCylinderWire.Position = frompoint;
        LinkCylinderWire.Mesh     = meshData;
    }

    private void UpdatePositions()
    {
        CoreNode.Position       = FssEarthCore.CorePos;
        FocusPointNode.Position = FssEarthCore.FocusPos;
    }

    // --------------------------------------------------------------------------------------------

    private void CreatePlatform()
    {
        PlatformPos = new FssLLAPoint() {
            LatDegs = 10f,
            LonDegs = 10f,
            RadiusM = 10f };

        PlatformCourse = new FssCourse() {
            HeadingDegs = 0f,
            SpeedMps    = 0.04f };

        PlatformCourseDelta = new FssCourseDelta() {
            HeadingChangeClockwiseDegsSec = 5f,
            SpeedChangeMpMps              = 0f };
    }

    private void UpdatePlatform(double delta)
    {
        // Update real world position
        PlatformCourse = PlatformCourse.PlusDeltaForTime(PlatformCourseDelta, delta);
        PlatformPos    = PlatformPos.PlusDeltaForTime(PlatformCourse, delta);

        // Create the visual position
        FssEntityV3 platVecs = FssGeoConvOperations.ReadWorldToStruct(PlatformPos, PlatformCourse);
    }
}
