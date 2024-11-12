// using System;
// using System.Collections.Generic;

// using Godot;

// // Class to examine the model and maintain a list of platforms to match

// public partial class FssTestModelChildren : Node3D
// {
//     List<Node3D> platforms = new List<Node3D>();
//     List<string> childNodeNames = new List<string>();

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         // Create a background thread to examine the model, and then defer a call to add/remove any platforms
//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//         // Get the list of child node names
//         childNodeNames.Clear();
//         foreach (Node3D child in GetChildren())
//         {
//             childNodeNames.Add(child.Name);
//         }

//         // Get the list of platforms from the model
//         List<string> platformNames = FssEventDriver.PlatformNames();

//         // Nested loop, adding any platforms that don't exist
//         foreach (string platformName in platformNames)
//         {
//             bool found = false;
//             if (!childNodeNames.Contains(platformName))
//             {
//                 AddPlatform(platformName);
//                 found = true;
//             }
//             if (found)
//             {
//                 //
//                 Node child = FindChild(platformName);
//                 if (child != null)
//                 {
//                     RemoveChild(child);
//                     child.QueueFree();
//                 }
//             }
//         }
//     }

//     public void AddPlatform(string platformName)
//     {
//         FssCentralLog.AddEntry($"AddPlatform {platformName}");

//         if (string.IsNullOrEmpty(platformName))
//         {
//             FssCentralLog.AddEntry("EC0-0023: Platform name is null or empty.");
//             return;
//         }

//         if (!FssEventDriver.DoesEntityExist(platformName))
//         {
//             FssCentralLog.AddEntry($"EC0-0024: Platform {platformName} does not exist.");
//             return;
//         }

//         try
//         {
//             Node3D platNode = new FssTestSim() { Name = platformName };
//             platforms.Add(platNode);
//             AddChild(platNode);

//             var script = GD.Load<CSharpScript>("res://Code/Godot/FssTestSim.cs");

//             if (script == null)
//             {
//                 GD.Print("FssTestModelChildren.AddPlatform: script is null");
//                 return;
//             }

//             platNode.SetScript(script);
//             FssCentralLog.AddEntry($"FssTestModelChildren.AddPlatform: script set for {platformName}");

//             // Cast and set the property before adding to the scene tree
//             if (platNode is FssTestSim myNode3D)
//             {
//                 myNode3D.SetModelName(platformName);
//                 FssCentralLog.AddEntry($"FssTestModelChildren.AddPlatform: ModelName set for {platformName}");
//             }

//             // Add the node to the scene tree - calls _Ready()
//         }
//         catch (Exception ex)
//         {
//             GD.PrintErr($"Error in AddPlatform: {ex.Message}");
//             GD.PrintErr($"Stack Trace: {ex.StackTrace}");
//             FssCentralLog.AddEntry($"Error in AddPlatform: {ex.Message}");
//             FssCentralLog.AddEntry($"Stack Trace: {ex.StackTrace}");
//         }
//     }
// }
