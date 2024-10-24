// using Godot;
// using System;

// #nullable enable

// // FssAppNode: Singleton class to manage the application (namely a clean exit)
// // - FssAppNode.Instance.ExitApplication();

// public partial class FssAppNode : Node
// {
//     private static FssAppNode _instance;

//     public static FssAppNode Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 GD.PrintErr("FssAppNode instance is not initialized.");
//             }
//             return _instance;
//         }
//     }

//     public override void _Ready()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//             // Make sure the FssAppNode persists across scene changes
//             //SetProcess(true);
//             //SetPhysicsProcess(true);
//             //SetProcessInput(true);
//             //SetProcessUnhandledInput(true);
//             //SetProcessUnhandledKeyInput(true);
//             //CallDeferred(nameof(RegisterSingleton));

//             //CallDeferred( nameof(FssAppFactory.Instance.CallStart) );
//         }
//         else
//         {
//             QueueFree(); // There can only be one instance of FssAppNode
//         }
//     }

//     private void RegisterSingleton()
//     {
//         // Make the FssAppNode a singleton
//         //GetTree().Root.AddChild(this);
//         //GetTree().Root.SetMeta("FssAppNode", this);
//     }

//     public void ExitApplication()
//     {
//         GetTree().Quit();
//     }

//     // --------------------------------------------------------------------------------------------
//     // #MARK: FindNode
//     // --------------------------------------------------------------------------------------------

//     // Function to find a node by its name in the scene tree
//     public Node? FindNode(string name)
//     {
//         return FindNodeByNameRecursive(this, name);
//     }

//     // Recursive helper function to search for the node
//     private Node? FindNodeByNameRecursive(Node parent, string name)
//     {
//         if (parent == null)
//         {
//             return null;
//         }

//         if (parent.Name == name)
//         {
//             return parent;
//         }

//         foreach (Node child in parent.GetChildren())
//         {
//             Node result = FindNodeByNameRecursive(child, name);
//             if (result != null)
//             {
//                 return result;
//             }
//         }

//         return null;
//     }
// }
