// #nullable enable

// using System;

// using Godot;

// // Class to add an entity from the PlatformModel.

// public partial class GloTestSim : Node3D
// {
//     public  string ModelName = "TEST-003";
//     private bool   IsCreated = false;
//     private float lerpSpeed = 5.0f; // Adjust this value to control the lerp speed

//     private Node3D ModelNode;

//     public override void _Ready()
//     {
//         GD.Print("GloTestSim._Ready");
//     }

//     public void SetModelName(string modelName)
//     {
//         ModelName = modelName;
//         GloCentralLog.AddEntry($"GloTestSim: ModelName set to {ModelName}");
//     }

//     public override void _Process(double delta)
//     {
//         // Find the model
//         GloPlatform? model = GloAppFactory.Instance.PlatformManager.PlatForName(ModelName);

//         // If the model is not null, and we're not yet setup to look at it
//         if ((model != null) && (!IsCreated))
//         {
//             // Create the nodes
//             ModelNode = new Node3D() { Name = "ModelNode" };
//             AddChild(ModelNode);
//             GloPrimitiveFactory.AddAxisMarkers(ModelNode, 0.02f, 0.005f);
//             IsCreated = true;

//             GloCentralLog.AddEntry($"GloTestSim: Node3D {ModelName} created.");
//         }

//         if ((model != null) && (IsCreated))
//         {
//             // Get the model's start position
//             GloLLAPoint pos    = model.Kinetics.CurrPosition;
//             GloCourse   course = model.Kinetics.CurrCourse;

//             // Adjust for current test situation
//             pos.AltMslM = 1.25f;

//             // Create the Vector3 positions
//             GloEntityV3 platformV3 = GloGeoConvOperations.RwToGeStruct(pos, course);
//             ModelNode.Position     = platformV3.Pos; //ModelNode.Position.Lerp(platformV3.Position, (float)(lerpSpeed * delta));

//             // Find where we are currently looking, determine where we need to be looking, and lerp to that orientation
//             Transform3D currentTransform = ModelNode.Transform;
//             Transform3D targetTransform  = currentTransform.LookingAt(platformV3.PosAhead, platformV3.PosAbove);
//             ModelNode.Transform = currentTransform.InterpolateWith(targetTransform, (float)(lerpSpeed * delta));

//         }

//         // If the model is null, delete ourselves
//         if ((model == null) && (IsCreated))
//         {
//             ModelNode.QueueFree();
//             IsCreated = false;
//             GloCentralLog.AddEntry($"GloTestSim: Node3D {ModelName} deleted.");
//         }


//         // GD.Print("GloTestSim._Process");
//     }

// }
