using Godot;
using System;

public partial class FssTestObject : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Output a message to the internal logging system
        FssCentralLog.AddStartupEntry(FssGlobals.VersionString);

        // Add a standard primitive to the scene
        var sphereMesh = new SphereMesh();
        sphereMesh.Radius = 1.0f;
        var testSphereInstance = new MeshInstance3D();
        testSphereInstance.Mesh = sphereMesh;
        AddChild(testSphereInstance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
