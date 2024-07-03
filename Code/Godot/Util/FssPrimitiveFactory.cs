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

}