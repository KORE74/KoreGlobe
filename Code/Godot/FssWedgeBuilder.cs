using System;

using Godot;

public partial class FssWedgeBuilder : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddStartupEntry("WedgeBuilder // _Ready");

        // Fix the position to line up with the sphere
        Position = new Vector3(0f, 1f, 0f);

        FssMeshBuilder meshBuilder = new ();

        float az = 340;

        meshBuilder.AddSphere (
            new Vector3(2, 1.5f, 0), 0.6f, 36
        );
        // meshBuilder.AddShellSegment (
        //     0, 40, //  azimuthMin,  azimuthMax,
        //     0, 40, //  elevationMin,  elevationMax,
        //     1.5f, 2f, //  distanceMin,  distanceMax,
        //     3, 4 ); // resolutionAz,  resolutionEl)
        ArrayMesh meshData = meshBuilder.Build("Wedge", true);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance = new();
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = FssMaterialFactory.WireframeShaderMaterial(new Color(0.5f, 1.0f, 0.5f, 1.0f));
        //meshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 1.0f));

        AddChild(meshInstance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
