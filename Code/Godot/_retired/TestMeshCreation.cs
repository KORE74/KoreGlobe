using Godot;
using System;

public partial class TestMeshCreation : Node3D
{
    public override void _Ready()
    {
        FssMeshBuilder meshBuilder = new FssMeshBuilder();
        meshBuilder.AddBox(Vector3.Zero, 0.1f, 0.2f, 0.3f);
        ArrayMesh meshData = meshBuilder.Build2("Mesh", false);

        var matWire      = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

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
