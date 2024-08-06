using Godot;
using System;

public partial class TestMalleableSphere : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssColorRange colorRange = FssColorRange.RedYellowGreen();

        FssFloat2DArray radiusList = new FssFloat2DArray(32, 16);
        radiusList.SetRandomVals(1.01f, 1.05f);

        radiusList = FssFloat2DArray.AntennaPattern_001(32, 16);

        FssMeshBuilder meshBuilder = new();
        meshBuilder.AddMalleableSphere(new Vector3(0, 0, 0), radiusList, colorRange);

        var matWire        = FssMaterialFactory.WireframeWhiteMaterial();
        var matVertexColor = FssMaterialFactory.VertexColorMaterial();
        var matBlue        = FssMaterialFactory.SimpleColoredMaterial(new Color(0.0f, 0.5f, 1.0f, 1.0f));

        ArrayMesh meshData = meshBuilder.Build("Sphere", false);

        // Colored - Add the mesh to the current Node3D
        MeshInstance3D meshInstance    = new();
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matVertexColor;

        // Wirefrme - Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        AddChild(meshInstance);
        AddChild(meshInstanceW);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
