using System;

using Godot;

public partial class FssWedgeBuilder : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddStartupEntry("WedgeBuilder // _Ready");

        // Fix the position to line up with the sphere
        Position = new Vector3(0f, 0f, 0f);

        FssMeshBuilder meshBuilder = new ();
        FssMeshBuilder meshBuilder2 = new ();

        var matGround    = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matTrans     = FssMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));
        var matWire      = FssMaterialFactory.WireframeWhiteMaterial();

        for (float currLon = -180f; currLon < (180f - 10f); currLon += 30f)
        {
            for (float currLat = -90; currLat < 90; currLat += 30f)
            {
                meshBuilder.AddShellSegment (
                    currLat, currLat + 25, // elevationMin, elevationMax,
                    currLon, currLon + 25, // azimuthMin, azimuthMax,
                    1.2f, 1.22f,           // distanceMin, distanceMax,
                    4, 4 );                // resolutionAz, resolutionEl)

                ArrayMesh meshData = meshBuilder.Build("Wedge", false);

                // Add the mesh to the current Node3D
                MeshInstance3D meshInstance   = new();
                meshInstance.Mesh             = meshData;
                meshInstance.MaterialOverride = matGround; //matTransBlue;

                // Add the mesh to the current Node3D
                MeshInstance3D meshInstanceW   = new();
                meshInstanceW.Mesh             = meshData;
                meshInstanceW.MaterialOverride = matWire;

                AddChild(meshInstance);
                AddChild(meshInstanceW);

                meshBuilder.Init();
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
