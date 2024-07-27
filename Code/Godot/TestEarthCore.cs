using Godot;
using System;

public partial class TestEarthCore : MeshInstance3D
{
    float radius = 6f;
    FssMapTileNode MapTileNode;

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
        CreateTestWedge();
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

    private void CreateTestWedge()
    {
        FssMeshBuilder meshBuilder  = new ();

        FssFloat2DArray noiseSurface = new FssFloat2DArray(50, 50);
        noiseSurface.SetRandomVals(-1f, 1f);

        noiseSurface[0,0] = 10f;

        meshBuilder.AddSurface(
            0, 30, //float azMinDegs, float azMaxDegs,
            30, 60, //float elMinDegs, float elMaxDegs,
            3.5f, 0.005f, //float surfaceRadius, float surfaceScale,
            noiseSurface //FssFloat2DArray surfaceArray,
        ); //bool flipTriangles = false)

        meshBuilder.AddSurfaceWedgeSides(
            0, 30, //float azMinDegs, float azMaxDegs,
            30, 60, //float elMinDegs, float elMaxDegs,
            3.5f, 0.005f, 3.0f,
            noiseSurface //FssFloat2DArray surfaceArray,
        ); //bool flipTriangles = false)

        // meshBuilder.AddShellSegment (
        //     30, 60, // elevationMin, elevationMax,
        //     30, 60, // azimuthMin, azimuthMax,
        //     1.10f, 1.12f,          // distanceMin, distanceMax,
        //     3, 3 );                // resolutionAz, resolutionEl)

        ArrayMesh meshData = meshBuilder.BuildWithUV("Wedge");

        var image = Image.LoadFromFile("C:/Util/Data/FssLibrary/Map/Sat/Lvl0/30x30/Sat_BF.png");
        //var image = Image.LoadFromFile("res://Resources/Misc/SatImg.png");
        var texture = ImageTexture.CreateFromImage(image);

        //var _material = FssMaterialFactory.TexMaterial();


        var _material = new StandardMaterial3D();
        _material.AlbedoTexture = texture;
        _material.ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded;


        // load an image into a material

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = _material;

        AddChild(meshInstance);

        var matWire = FssMaterialFactory.WireframeWhiteMaterial();
        //matWire.Transparency = 0.85;

        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;
        meshInstanceW.Transparency     = 0.9f;
        //meshInstanceW.CastShadows      = false;

        AddChild(meshInstanceW);

    }
}
