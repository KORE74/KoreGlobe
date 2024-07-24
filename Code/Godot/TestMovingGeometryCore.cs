using Godot;
using System;

public partial class TestMovingGeometryCore : Node3D
{
    float radius = 10f;

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
    private float AnimAzDelta = 2.5f;
    private float AnimElDelta = 2.5f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        matColorRed    = FssMaterialFactory.SimpleColoredMaterial(new Color(0.9f, 0.3f, 0.3f, 1f));
        matColorBlue   = FssMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.9f, 1f));
        matColorYellow = FssMaterialFactory.SimpleColoredMaterial(new Color(0.8f, 0.8f, 0.3f, 1f));
        matColorWhite  = FssMaterialFactory.SimpleColoredMaterial(new Color(1f, 1f, 1f, 1f));
        matWire        = FssMaterialFactory.WireframeWhiteMaterial();

        CreateCoreNode();
        CreateFocusNode();
        CreateZeroNode();
        CreateLinkCylinder();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        AnimAzDegs += AnimAzDelta * (float)delta;
        AnimElDegs += AnimElDelta * (float)delta;

        if (AnimElDegs > 60f)  AnimElDelta = -2.5f;
        if (AnimElDegs < -60f) AnimElDelta =  2.5f;

        //float radius = 6f;
        float backoffFraction = 0.2f;

        float CoreDist  = radius - (radius * backoffFraction);
        float FocusDist = (radius * backoffFraction);

        float azDegs = AnimAzDegs;
        float elDegs = AnimElDegs;
        float azRads = (float)FssValueUtils.DegsToRads(azDegs);
        float elRads = (float)FssValueUtils.DegsToRads(elDegs);

        Vector3 focusOffset = new Vector3(
            FocusDist * Mathf.Cos(azRads) * Mathf.Cos(elRads),
            FocusDist * Mathf.Sin(elRads),
            FocusDist * Mathf.Sin(azRads) * Mathf.Cos(elRads)
        );

        FocusPos = focusOffset;


        float backAzDegs = azDegs + 180f;
        float backElDegs = elDegs * -1f;
        float backAzRads = (float)FssValueUtils.DegsToRads(backAzDegs);
        float backElRads = (float)FssValueUtils.DegsToRads(backElDegs);

        Vector3 coreOffset = new Vector3(
            CoreDist * Mathf.Cos(backAzRads) * Mathf.Cos(backElRads),
            CoreDist * Mathf.Sin(backElRads),
            CoreDist * Mathf.Sin(backAzRads) * Mathf.Cos(backElRads)
        );

        CorePos = coreOffset;


        UpdatePositions();
        UpdateLinkCylinder();
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

        MeshInstance3D meshInstance = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = matColorBlue;

        MeshInstance3D meshInstanceW = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh = meshData;
        meshInstanceW.MaterialOverride = matWire;

        CoreNode.AddChild(meshInstance);
        CoreNode.AddChild(meshInstanceW);

        // Attach the script to the core node
        CoreNode.AddChild(new TestEarthCore(radius));
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

        MeshInstance3D meshInstance = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = matColorYellow;

        MeshInstance3D meshInstanceW = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh = meshData;
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

        MeshInstance3D meshInstance = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = matColorRed;

        MeshInstance3D meshInstanceW = new MeshInstance3D() { Name = "Wire" };
        meshInstanceW.Mesh = meshData;
        meshInstanceW.MaterialOverride = matWire;

        ZeroNode.AddChild(meshInstance);
        ZeroNode.AddChild(meshInstanceW);
    }

    private void CreateLinkCylinder()
    {
        // Create the focus point node
        LinkCylinderNode = new Node3D() { Name = "LinkCylinder" };
        AddChild(LinkCylinderNode);

        LinkCylinderMesh = new MeshInstance3D() { Name = "Color" };
        LinkCylinderMesh.MaterialOverride = matColorWhite;
        LinkCylinderNode.AddChild(LinkCylinderMesh);

        LinkCylinderWire = new MeshInstance3D() { Name = "Wire" };
        LinkCylinderWire.MaterialOverride = matWire;
        LinkCylinderNode.AddChild(LinkCylinderWire);
    }

    private void UpdateLinkCylinder()
    {
        Vector3 frompoint = CorePos;
        Vector3 topoint   = FocusPos;
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
        CoreNode.Position       = CorePos;
        FocusPointNode.Position = FocusPos;
    }

}
