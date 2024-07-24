using System;

using Godot;

public partial class FssWedgeBuilder : Node3D
{
    FssMapTileNode MapTileNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddStartupEntry("WedgeBuilder // _Ready");

        // Fix the position to line up with the sphere
        Position = new Vector3(0f, 0f, 0f);

        FssMeshBuilder meshBuilder  = new ();

        //var matGround    = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matTrans     = FssMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));
        var matWire      = FssMaterialFactory.WireframeWhiteMaterial();

        //var matTestWite   = FssMaterialFactory.WireframeShaderMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

        for (float currLon = -180f; currLon < (180f - 10f); currLon += 30f)
        {
            for (float currLat = -90; currLat < 90; currLat += 30f)
            {
                // Create a random variation on the green terrain color
                var matGround = FssMaterialFactory.SimpleColoredMaterial(FssColorUtil.ColorWithRGBNoise(new Color(0.5f, 1.0f, 0.5f, 0.7f), 0.5f));

                meshBuilder.AddShellSegment (
                    currLat, currLat + 25, // elevationMin, elevationMax,
                    currLon, currLon + 25, // azimuthMin, azimuthMax,
                    1.20f, 1.22f,          // distanceMin, distanceMax,
                    3, 3 );                // resolutionAz, resolutionEl)

                ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

                // Add the mesh to the current Node3D
                MeshInstance3D meshInstance   = new();
                meshInstance.Mesh             = meshData;
                meshInstance.MaterialOverride = matGround; //matTransBlue;

                // Add the mesh to the current Node3D
                MeshInstance3D meshInstanceW   = new();
                meshInstanceW.Mesh             = meshData;
                meshInstanceW.MaterialOverride = matWire; // matTestWite; //

                AddChild(meshInstance);
                AddChild(meshInstanceW);

                meshBuilder.Init();
            }
        }
        CreateTestWedge();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // -------------------------------------------------------------------------------------------- 

    private void CreateTestWedge()
    {
        FssMeshBuilder meshBuilder  = new ();

        meshBuilder.AddShellSegment (
            30, 60, // elevationMin, elevationMax,
            30, 60, // azimuthMin, azimuthMax,
            1.10f, 1.12f,          // distanceMin, distanceMax,
            3, 3 );                // resolutionAz, resolutionEl)

        ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

        MapTileNode = new FssMapTileNode("./Resources/Misc/SatImg.png") { Name = "MapTileNode" };
        AddChild(MapTileNode);
    }


}
