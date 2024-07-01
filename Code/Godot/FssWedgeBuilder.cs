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


        float az = 30;

        float left  = 0;
        float right = left + az;

        var matTrans = FssMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matWire = FssMaterialFactory.WireframeWhiteMaterial();

        for (int i = 0; i < 5; i++)
        {
            meshBuilder.AddShellSegment (
                left, right, //  azimuthMin,  azimuthMax,
                0, 30, //  elevationMin,  elevationMax,
                1.5f, 2f, //  distanceMin,  distanceMax,
                5, 5 ); // resolutionAz,  resolutionEl)


            ArrayMesh meshData = meshBuilder.Build("Wedge", true);

            // Add the mesh to the current Node3D
            MeshInstance3D meshInstance = new();
            meshInstance.Mesh = meshData;
            meshInstance.MaterialOverride = matTrans;

            // Add the mesh to the current Node3D
            MeshInstance3D meshInstanceW = new();
            meshInstanceW.Mesh = meshData;
            meshInstanceW.MaterialOverride = matWire;

            AddChild(meshInstance);
            AddChild(meshInstanceW);

            meshBuilder.Init();
            left = right + 5;
            right = left + az;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
