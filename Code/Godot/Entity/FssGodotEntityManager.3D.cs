using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{


    // --------------------------------------------------------------------------------------------
    // MARK: Update - 3D Model
    // --------------------------------------------------------------------------------------------

    private void MatchModelPlatform3DModel(string platName)
    {
        MatchBoundingBox(platName);

    //     // 1 - GetEntity the top level object of the model and presentation sides

    //     // Get the model
    //     FssPlatform? platform = FssAppFactory.Instance.EventDriver.GetPlatform(platName);
    //     if (platform == null)
    //     {
    //         FssCentralLog.AddEntry($"MatchModelPlatform3DModel: Platform {platName} not found.");
    //         return;
    //     }

    //     // Get the godot entity
    //     FssGodotEntity? ent = GetEntity(platName);
    //     if (ent == null)
    //     {
    //         FssCentralLog.AddEntry($"MatchModelPlatform3DModel: Entity {platName} not found.");
    //         return;
    //     }

    //     // 2 - Get the 3D model name we'll be dealing with

    //     // Get the model name
    //     string modelName = platform.ModelName;
    //     if (modelName == "")
    //     {
    //         FssCentralLog.AddEntry($"MatchModelPlatform3DModel: Platform {platName} has no model.");
    //         return;
    //     }
    //     string requiredModelNodeName = $"model_{modelName}";

    //     // 3 - Check if the model is already attached to the entity - Do we need to do anything?
    //     // - The model name is used in the name of the child node attached to the main entity node.
    //     // - We use a "model_" prefix so we can identify other 3D models tha may need removing.

    //     // Check if the model is already attached to the entity
    //     bool modelAlreadyAttached = false;
    //     foreach (Node3D currNode in ent.GetChildren())
    //     {
    //         // check for any "model_" prefixed nodes
    //         if (currNode.Name.StartsWith("model_"))
    //         {
    //             if (currNode.Name == requiredModelNodeName)
    //             {
    //                 modelAlreadyAttached = true;
    //                 break;
    //             }
    //             else
    //             {
    //                 // Remove any other model nodes
    //                 currNode.QueueFree();
    //             }
    //         }
    //     }

    //     // 4 - If the model is not attached, attach it
    //     if (!modelAlreadyAttached)
    //     {
    //         Node3D modelNode = FssGodotFactory.Instance.ModelLibrary.PrepModel(modelName);

    //         if (modelNode != null)
    //         {
    //             modelNode.Name = requiredModelNodeName;
    //             ent.AddChild(modelNode);
    //         }
    //     }
    }

    private void MatchBoundingBox(string platName)
    {
        // The name of the bounding box node
        string bbNodeName = "aabb";

        // Get the platform node
        FssGodotEntity? ent = GetEntity(platName);

        if (ent != null)
        {
            // Get the bounding box node name
            //Node3D? bbNode = ent.FindNode(bbNodeName) as Node3D;

            Node3D? bbNode = ent.GetNodeOrNull<Node3D>(bbNodeName);

            // If the bounding box node exists, remove it
            if (bbNode != null)
            {
                return;
                bbNode.QueueFree();
            }

            // Create the bounding box
            FssXYZBox aabb = new FssXYZBox() { Height = 2, Width = 4, Length = 6 };

            // Create a new bounding box node, using our LineMesh class
            FssLineMesh3D bbMesh = new FssLineMesh3D();
            bbMesh.Name = bbNodeName;
            bbMesh.AddBox(aabb, new Color(1, 1, 0, 1));

            // Add the bounding box to the entity
            ent.AddChild(bbMesh);
        }
    }

}


