// using Godot;
// using System;

// public partial class GloGlobeCore2 : Node3D
// {
//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         // Output a message to the internal logging system
//         GloCentralLog.AddEntry($"GloTestGloCore // _Ready");

//         // Add a standard primitive to the scene
//         float radius = 1.0f;
//         var sphereMesh = new SphereMesh()
//         {
//             Radius = radius,
//             Height = radius * 2.0f
//         };

//         {
//             var testSphereInstance = new MeshInstance3D();
//             testSphereInstance.Name             = "TestSphere";
//             testSphereInstance.Mesh             = sphereMesh;
//             testSphereInstance.MaterialOverride = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);
//             testSphereInstance.Position         = new Vector3(0.0f, 0.0f, 0.0f);
//             testSphereInstance.Scale            = new Vector3(1f, 1f, 1f);
//             AddChild(testSphereInstance);
//         }
//         {
//             var testSphereInstance = new MeshInstance3D();
//             testSphereInstance.Name             = "TestSphere";
//             testSphereInstance.Mesh             = sphereMesh;
//             testSphereInstance.MaterialOverride = GloMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0f, 1f));
//             testSphereInstance.Position         = new Vector3(0.0f, 0.0f, 0.0f);
//             testSphereInstance.Scale            = new Vector3(1f, 1f, 1f);
//             AddChild(testSphereInstance);
//         }
//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//     }
// }
