using Godot;
using System;

public partial class FssTestGlobeCore : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Output a message to the internal logging system
        FssCentralLog.AddStartupEntry(FssGlobals.VersionString);

        // Add a standard primitive to the scene
        var sphereMesh = new SphereMesh();
        sphereMesh.Radius = 1.0f;
        sphereMesh.Height = 2.0f;

        {
            var testSphereInstance = new MeshInstance3D();
            testSphereInstance.Name             = "TestSphere";
            testSphereInstance.Mesh             = sphereMesh;
            testSphereInstance.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
            testSphereInstance.Position         = new Vector3(0.0f, 0.0f, 0.0f);
            testSphereInstance.Scale            = new Vector3(1f, 1f, 1f);
            AddChild(testSphereInstance);
        }
        {
            var testSphereInstance = new MeshInstance3D();
            testSphereInstance.Name             = "TestSphere";
            testSphereInstance.Mesh             = sphereMesh;
            testSphereInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0f, 1f));
            testSphereInstance.Position         = new Vector3(0.0f, 0.0f, 0.0f);
            testSphereInstance.Scale            = new Vector3(1f, 1f, 1f);
            AddChild(testSphereInstance);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
