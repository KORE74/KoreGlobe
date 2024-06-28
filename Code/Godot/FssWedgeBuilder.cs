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

        meshBuilder.AddTriangle(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0));

        meshBuilder.AddShellSegment (
            0, 10, //  azimuthMin,  azimuthMax,
            0, 10, //  elevationMin,  elevationMax,
            1, 2, //  distanceMin,  distanceMax,
            5, 5 ); // resolutionAz,  resolutionEl)

        ArrayMesh meshData = meshBuilder.Build("Wedge", true);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance = new();
        meshInstance.Mesh = meshData;
        meshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 1.0f));

        AddChild(meshInstance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
