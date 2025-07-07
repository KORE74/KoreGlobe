using Godot;
using System;

public partial class TestMeshCreation : Node3D
{
    public override void _Ready()
    {
        GloMeshBuilder meshBuilder = new GloMeshBuilder();
        meshBuilder.AddBox(Vector3.Zero, 0.1f, 0.2f, 0.3f);
        ArrayMesh meshData = meshBuilder.Build2("Mesh", false);

        var matWire      = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);
        var matTransBlue = GloMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

        MeshInstance3D meshInstance    = new() { Name = "Cube" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matTransBlue;

        MeshInstance3D meshInstanceW   = new() { Name = "CubeWire" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        AddChild(meshInstance);
        AddChild(meshInstanceW);
    }

    public override void _Process(double delta)
    {
    }
}
