using Godot;
using System;

public partial class TestMalleableSphere : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssColorRange   colorRange = FssColorRange.RedYellowGreen();

        FssFloat2DArray radiusList = new FssFloat2DArray(50, 50);
        radiusList.SetRandomVals(0.2f, 0.22f);

        FssMeshBuilder meshBuilder  = new ();

        meshBuilder.AddMalleableSphere(new Vector3(0, 0, 0), radiusList, colorRange);

        var matTrans     = FssMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 1.0f, 0.5f, 0.7f));
        var matWire      = FssMaterialFactory.WireframeWhiteMaterial();

        ArrayMesh meshData = meshBuilder.Build("Wedge", false);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = matTrans;

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire; // matTestWite; //

        AddChild(meshInstance);
        AddChild(meshInstanceW);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
