// using Godot;
// using System;

// public partial class TestMeshBuilder : Node
// {
//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         GloMeshBuilder meshBuilder = new GloMeshBuilder();
//         meshBuilder.AddCylinder(Vector3.Zero, new Vector3(1f, 0f, 0f), 0.2f, 0.2f, 12, true);
//         //meshBuilder.AddSphere(Vector3.Zero, 1, 36);

//         var matGrey = GloMaterialFactory.TransparentColoredMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));
//         var matWire = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);

//         ArrayMesh meshData = meshBuilder.Build("MeshBuilder", false);

//         // Add the mesh to the current Node3D
//         MeshInstance3D meshInstance   = new() { Name = "Mesh" };
//         meshInstance.Mesh             = meshData;
//         meshInstance.MaterialOverride = matGrey;

//         MeshInstance3D meshInstanceW   = new() { Name = "Wire" };
//         meshInstanceW.Mesh             = meshData;
//         meshInstanceW.MaterialOverride = matWire;

//         AddChild(meshInstance);
//         AddChild(meshInstanceW);

//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//     }
// }
