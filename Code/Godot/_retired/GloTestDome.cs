// using System;

// using Godot;

// // Class to add an entity from the PlatformModel.

// public partial class GloTestDome : Node3D
// {
//     private GloLLAPoint pos   = new GloLLAPoint() { LatDegs = 35, LonDegs = 30, AltMslM = 1.22f };
//     Node3D ModelNode         = null;

//     public override void _Ready()
//     {
//         GD.Print("GloTestSim._Ready");

//             ModelNode = new Node3D() { Name = "TestDome" };
//             ModelNode.LookAt(Vector3.Forward, Vector3.Up);
//             AddChild(ModelNode);

//             // ---------------------------------------

//             // Add a Dome
//             GloMeshBuilder meshBuilder = new GloMeshBuilder();
//             meshBuilder.AddHemisphere(Vector3.Zero, 0.15f, 25);

//             ArrayMesh meshData = meshBuilder.Build2("Dome", false);

//             var matWire      = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["White"]);
//             var matTransBlue = GloMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

//             MeshInstance3D meshInstance    = new() { Name = "Dome" };
//             meshInstance.Mesh              = meshData;
//             meshInstance.MaterialOverride  = matTransBlue;

//             MeshInstance3D meshInstanceW   = new() { Name = "DomeWire" };
//             meshInstanceW.Mesh             = meshData;
//             meshInstanceW.MaterialOverride = matWire;

//             ModelNode.AddChild(meshInstance);
//             ModelNode.AddChild(meshInstanceW);

//             // ---------------------------------------

//     }

//     public override void _Process(double delta)
//     {
//         UpdateModelPosition();
//     }

//     public void UpdateModelPosition()
//     {
//         // --- Define positions -----------------------

//         // Define the position and associated up direction for the label
//         GloLLAPoint posAbove = pos;
//         posAbove.AltMslM += 0.04f;

//         // Get the position 5 seconds ahead, or just north if stationary
//         GloLLAPoint posAhead = GloLLAPoint.Zero;

//             posAhead = pos;
//             posAhead.LatDegs += 0.001;

//         // --- Define vectors -----------------------

//         // Define the Vector3 Offsets
//         //Vector3 vecPos   = KoreGeoConvOperations.RealWorldToGodot(pos);
//         //Vector3 vecAbove = KoreGeoConvOperations.RealWorldToGodot(posAbove);
//         //Vector3 vecAhead = KoreGeoConvOperations.RealWorldToGodot(posAhead);
// //
//         //KoreEntityV3 platVecs = KoreGeoConvOperations.RealWorldToStruct(pos, GloCourse.Zero);
// //
//         //// Update node position and orientation
//         //ModelNode.Position = platVecs.Position;// vecPos;
//         //ModelNode.LookAt(platVecs.PosAhead, platVecs.PosAbove);

//         // Update camera position and orientation
//         // GloXYZPoint camOffsetXYZ = CameraOffset.ToXYZ();
//         // ModelCamera.Position = new Vector3((float)camOffsetXYZ.X, -(float)camOffsetXYZ.Y, -(float)camOffsetXYZ.Z);
//         // ModelCamera.LookAt(vecPos, vecAbove);
//     }
// }
