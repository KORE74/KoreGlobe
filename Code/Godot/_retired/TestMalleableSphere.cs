using Godot;
using System;

public partial class TestMalleableSphere : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GloColorRange colorRange = GloColorRange.RedYellowGreen();

        GloFloat2DArray radiusList = new GloFloat2DArray(32, 16);
        radiusList = GloFloat2DArray.AntennaPattern_001(32, 16);

        GloMeshBuilder meshBuilder = new();
        meshBuilder.AddMalleableSphere(new Vector3(0, 0, 0), radiusList, colorRange);

        var matWire        = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);
        var matVertexColor = GloMaterialFactory.VertexColorMaterial();

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
