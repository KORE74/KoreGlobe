    using System;
using System.Collections.Generic;

using Godot;


// FssPrimitiveFactory: Class that creates new primitive instances
public static class FssPrimitiveFactory
{

    // Usage:
    // - AddChild( FssPrimitiveFactory.CreateSphere(new Vector3(0f, 1f, 0f), 1.2f, new Color(0.5f, 1.0f, 0.5f, 0.7f)) );

    public static MeshInstance3D CreateSphere(Vector3 position, float radius, Color color)
    {
        MeshInstance3D sphere = new MeshInstance3D();
        sphere.Mesh = new SphereMesh
        {
            Radius = radius,
            Height = radius * 2.0f
        };
        sphere.Position = position;
        sphere.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(color);
        return sphere;
    }

    private static void CreateCylinder(Node3D node, Vector3 start, Vector3 end, float radius, Color color)
    {
        Vector3 direction = end - start;
        float length = direction.Length();
        direction = direction.Normalized();

        // Midpoint
        Vector3 midPoint = (start + end) / 2.0f;

        // above the midpoint
        Vector3 aboveMidPoint = midPoint * 1.1f;

        MeshInstance3D cylinder = new MeshInstance3D();
        CylinderMesh cylinderMesh = new CylinderMesh
        {
            TopRadius      = radius,
            BottomRadius   = radius,
            Height         = length,
            RadialSegments = 6,
            Rings          = 3
        };
        cylinder.Mesh             = cylinderMesh;
        cylinder.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(color);

        node.AddChild(cylinder);

        cylinder.Position = midPoint;
        cylinder.LookAtFromPosition(midPoint, end, Vector3.Up);
    }

    // FssPrimitiveFactory.AddAxisMarkers(ModelNode, 0.02f, 0.005f);

    public static void AddAxisMarkers(Node3D node, float offsetSize, float markerRadius)
    {
        // Core position - White
        MeshInstance3D core = CreateSphere(new Vector3(0f, 0f, 0f), markerRadius, new Color(1.0f, 1.0f, 1.0f, 1.0f));
        core.Name = "Core";
        node.AddChild(core);

        // X Axis - Red
        MeshInstance3D xMarker = CreateSphere(new Vector3(offsetSize, 0f, 0f), markerRadius, new Color(1.0f, 0.0f, 0.0f, 1.0f));
        xMarker.Name = "XMarker";
        node.AddChild(xMarker);

        // Y Axis - Green
        MeshInstance3D yMarker = CreateSphere(new Vector3(0f, offsetSize, 0f), markerRadius, new Color(0.0f, 1.0f, 0.0f, 1.0f));
        yMarker.Name = "YMarker";
        node.AddChild(yMarker);

        // Z Axis - Blue
        MeshInstance3D zMarker = CreateSphere(new Vector3(0f, 0f, offsetSize), markerRadius, new Color(0.0f, 0.0f, 1.0f, 1.0f));
        zMarker.Name = "ZMarker";
        node.AddChild(zMarker);

        // CreateCylinder(node, Vector3.Zero, new Vector3(offsetSize, 0f, 0f), markerRadius * 0.5f, new Color(1.0f, 0.0f, 0.0f, 1.0f));
        // CreateCylinder(node, Vector3.Zero, new Vector3(0f, offsetSize, 0f), markerRadius * 0.5f, new Color(0.0f, 1.0f, 0.0f, 1.0f));
        // CreateCylinder(node, Vector3.Zero, new Vector3(0f, 0f, offsetSize), markerRadius * 0.5f, new Color(0.0f, 0.0f, 1.0f, 1.0f));
    }

}