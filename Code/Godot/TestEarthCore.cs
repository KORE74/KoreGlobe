using Godot;
using System;

public partial class TestEarthCore : MeshInstance3D
{
    float radius = 6f;

    public TestEarthCore()
    {

    }

    public TestEarthCore(float radius)
    {
        this.radius = radius;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateShell();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void CreateShell()
    {
        FssMeshBuilder meshBuilder  = new ();

        Color colorBlue = new Color(0.5f, 0.5f, 1f, 0.85f);
        var matWire = FssMaterialFactory.WireframeWhiteMaterial();

        //var matTestWite   = FssMaterialFactory.WireframeShaderMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

        for (float currLon = -180f; currLon < (180f - 10f); currLon += 30f)
        {
            for (float currLat = -90; currLat < 90; currLat += 30f)
            {
                // Create a random variation on the green terrain color

                var matRandoBlue = FssMaterialFactory.TransparentColoredMaterial(FssColorUtil.ColorWithRGBNoise(colorBlue, 0.7f));

                meshBuilder.AddShellSegment (
                    currLat, currLat + 25, // elevationMin, elevationMax,
                    currLon, currLon + 25, // azimuthMin, azimuthMax,
                    radius-0.2f, radius,   // distanceMin, distanceMax,
                    5, 5 );                // resolutionAz, resolutionEl)

                ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

                // Add the mesh to the current Node3D
                MeshInstance3D meshInstance   = new();
                meshInstance.Mesh             = meshData;
                meshInstance.MaterialOverride = matRandoBlue;

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
}
