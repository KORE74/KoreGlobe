    using System;
using System.Collections.Generic;

using Godot;


// GloPrimitiveFactory: Class that creates new primitive instances
public static class GloPrimitiveFactory
{

    // Usage:
    // - AddChild( GloPrimitiveFactory.CreateGodotSphere(new Vector3(0f, 1f, 0f), 1.2f, new Color(0.5f, 1.0f, 0.5f, 0.7f)) );

    // --------------------------------------------------------------------------------------------
    // MARK: Godot Primitives
    // --------------------------------------------------------------------------------------------

    public static MeshInstance3D CreateGodotSphere(Vector3 position, float radius, Color color)
    {
        MeshInstance3D sphere = new MeshInstance3D();
        sphere.Mesh = new SphereMesh
        {
            Radius = radius,
            Height = radius * 2.0f
        };
        sphere.Position = position;
        sphere.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(color);
        return sphere;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Meshbuilder Primitives
    // --------------------------------------------------------------------------------------------

    // Usage: GloPrimitiveFactory.CreateSphereNode("FocusPoint", new Vector3(0f, 0f, 0f), 1.2f, new Color(0.5f, 1.0f, 0.5f, 0.7f));

    public static Node3D CreateSphereNode(string name, Vector3 position, float radius, Color color, bool WithWireframe = false)
    {
        // Create the focus point node
        Node3D newNode = new Node3D() { Name = name };
        newNode.Position = position;

        // Add sphere for the core point
        GloMeshBuilder meshBuilder = new GloMeshBuilder();
        meshBuilder.AddSphere(Vector3.Zero, radius, 16);
        ArrayMesh meshData = meshBuilder.Build2("Sphere", false);

        MeshInstance3D meshInstance    = new MeshInstance3D() { Name = "Color" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = GloMaterialFactory.SimpleColoredMaterial(color);
        newNode.AddChild(meshInstance);

        if (WithWireframe)
        {
            MeshInstance3D meshInstanceW   = new MeshInstance3D() { Name = "Wire" };
            meshInstanceW.Mesh             = meshData;
            meshInstanceW.MaterialOverride = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);
            newNode.AddChild(meshInstanceW);
        }

        return newNode;
    }


    // Usage:
    // - AddChild( GloPrimitiveFactory.CreateBox(new Vector3(0f, 1f, 0f), new Vector3(1.2f, 1.2f, 1.2f), new Color(0.5f, 1.0f, 0.5f, 0.7f)) );
    public static MeshInstance3D CreateCylinder(Vector3 toPoint, float radius, Color color)
    {
        // Implementation is to create a cylinder from the current point (assumed 0,0,0) to the end point.
        // The rotation of the cylinder is not considered.

        MeshInstance3D cylinder = new MeshInstance3D();
        CylinderMesh cylinderMesh = new CylinderMesh
        {
            TopRadius      = radius,
            BottomRadius   = radius,
            Height         = toPoint.Length(),
            RadialSegments = 12,
            Rings          = 3
        };

        cylinder.Mesh             = cylinderMesh;
        cylinder.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(color);

        // cylinder.Position = toPoint / 2.0f;
        // cylinder.LookAt(toPoint, Vector3.Up);

        return cylinder;
    }


    public static MeshInstance3D CreateGodotCylinder(Vector3 fromPoint, Vector3 toPoint, float radius, Color color)
    {
        // Implementation is to create a cylinder from the current point (assumed 0,0,0) to the end point.
        // The rotation of the cylinder is not considered.

        MeshInstance3D cylinder = new MeshInstance3D();
        CylinderMesh cylinderMesh = new CylinderMesh
        {
            TopRadius      = radius,
            BottomRadius   = radius,
            Height         = fromPoint.DistanceTo(toPoint),
            RadialSegments = 12,
            Rings          = 3
        };

        cylinder.Mesh             = cylinderMesh;
        cylinder.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(color);

        cylinder.Position = (fromPoint + toPoint) / 2.0f;
        cylinder.LookAt(toPoint, Vector3.Up);

        return cylinder;
    }

    // function to place an existingCylinder between the two points and orient it towards the toPoint
    // GloPrimitiveFactory.OrientGodotCylinder(cylinder, fromPoint, toPoint);
    public static void OrientGodotCylinder(MeshInstance3D cylinder, KoreEntityV3 platformV3)
    {
        cylinder.Position = (platformV3.Pos + platformV3.PosAhead) / 2.0f;
        cylinder.LookAt(platformV3.PosAhead, platformV3.VecUp);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Special Case Primitives
    // --------------------------------------------------------------------------------------------

    public static void AddAxisMarkers(Node3D node, float offsetSize, float markerRadius)
    {
        // Core position - White
        MeshInstance3D core = CreateGodotSphere(new Vector3(0f, 0f, 0f), markerRadius, GloColorUtil.Colors["White"]);
        core.Name = "Core";
        node.AddChild(core);

        // X Axis - Red
        MeshInstance3D xMarker = CreateGodotSphere(new Vector3(offsetSize, 0f, 0f), markerRadius, GloColorUtil.Colors["Red"]);
        xMarker.Name = "XMarker";
        node.AddChild(xMarker);

        // Y Axis - Green
        MeshInstance3D yMarker = CreateGodotSphere(new Vector3(0f, offsetSize, 0f), markerRadius, GloColorUtil.Colors["Green"]);
        yMarker.Name = "YMarker";
        node.AddChild(yMarker);

        // Z Axis - Blue
        MeshInstance3D zMarker = CreateGodotSphere(new Vector3(0f, 0f, offsetSize), markerRadius, GloColorUtil.Colors["Blue"]);
        zMarker.Name = "ZMarker";
        node.AddChild(zMarker);

        // CreateCylinder(node, Vector3.Zero, new Vector3(offsetSize, 0f, 0f), markerRadius * 0.5f, new Color(1.0f, 0.0f, 0.0f, 1.0f));
        // CreateCylinder(node, Vector3.Zero, new Vector3(0f, offsetSize, 0f), markerRadius * 0.5f, new Color(0.0f, 1.0f, 0.0f, 1.0f));
        // CreateCylinder(node, Vector3.Zero, new Vector3(0f, 0f, offsetSize), markerRadius * 0.5f, new Color(0.0f, 0.0f, 1.0f, 1.0f));
    }

    // AddChild( GloPrimitiveFactory.AxisMarkerNode(1.2f, 0.1f) );
    public static Node3D AxisMarkerNode(float offsetSize, float markerRadius)
    {
        Node3D node = new Node3D() { Name = "AxisMarkers" };
        AddAxisMarkers(node, offsetSize, markerRadius);
        return node;
    }

}
