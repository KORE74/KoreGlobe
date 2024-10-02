using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class FssGodotEntityManager : Node3D
{


    // --------------------------------------------------------------------------------------------
    // MARK: Update - 3D Model
    // --------------------------------------------------------------------------------------------

    private void MatchModelPlatform3DModel(string platName)
    {
        // Get the platform, and platform type, from the mode model side
        string? platformType = FssAppFactory.Instance.EventDriver.PlatformType(platName);
        if (platformType == null)
            platformType = "default";




        MatchBoundingBox(platName, platformType);



        string modelNodeName = "model";

        FssGodotEntity? ent = GetEntity(platName);
        Node3D? modelNode = ent.GetNodeOrNull<Node3D>(modelNodeName);

        if (modelNode == null)
        {
            modelNode = FssGodotFactory.Instance.ModelLibrary.PrepModel(platformType);

            modelNode.Name = modelNodeName;
            ent.AddChild(modelNode);
        }





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




    private void MatchBoundingBox(string platName, string platformType)
    {
        // The name of the bounding box node
        string bbNodeName = "aabb";

        // Get the platform node
        FssGodotEntity? ent = GetEntity(platName);

        // Get the AABB for the model
        FssXYZBox? rwAABB = FssGodotFactory.Instance.ModelLibrary.AABBForModel(platformType);


        if (rwAABB == null)
        {
            FssCentralLog.AddEntry($"MatchBoundingBox: {platformType} => rwAABB is null");

            List<string> modelNames = FssGodotFactory.Instance.ModelLibrary.ModelNamesList();
            string modelNamesStr = string.Join(", ", modelNames);

            FssCentralLog.AddEntry($"Known models: {modelNamesStr}");
        }
        else
            FssCentralLog.AddEntry($"MatchBoundingBox: {platformType} => rwAABB:{rwAABB}");

        if ((ent != null) && (rwAABB != null))
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
            //FssXYZBox rwAABB = new FssXYZBox() { Height = 200, Width = 400, Length = 600 };
            //rwAABB = ent.ModelInfo.RwAABB;
            FssXYZBox geAABB = rwAABB.Scale(FssZeroOffset.RwToGeDistanceMultiplierM);

            GD.Print($"MatchBoundingBox: {platformType} => geAABB:{geAABB}");

            // Create a new bounding box node, using our LineMesh class
            FssLineMesh3D bbMesh = new FssLineMesh3D();
            bbMesh.Name = bbNodeName;
            bbMesh.AddBox(geAABB, new Color(1, 1, 0, 1));

            // Add the bounding box to the entity
            ent.AddChild(bbMesh);
        }
    }

    // --------------------------------------------------------------------------------------------

    // Set the model scale.
    // 1 - get the "model" node. Then get its only child, which is the model itself.
    // 2 - Get the scale modifier, multiply it by the 3D Library scale factor, and apply it to the node.
    //      - We have the platform name and type to assist in lookup up elements.

    void SetModelScale(string platName, string platformType, bool applyRwScaling, float scaleModifier)
    {
        // Adjust the scale of the model
        scaleModifier = FssValueUtils.ScaleVal(scaleModifier,  1f, 10f,  1f, 500f);

        // Get the entity
        FssGodotEntity? ent = GetEntity(platName);
        if (ent == null)
        {
            FssCentralLog.AddEntry($"SetModelScale: Entity {platName} not found.");
            return;
        }
        // Get the model node
        Node3D? modelNode = ent.GetNodeOrNull<Node3D>("model");
        if (modelNode == null)
        {
            FssCentralLog.AddEntry($"SetModelScale: Model node not found for {platName}.");
            return;
        }

        // Get the 3D Library entry for the type
        Fss3DModelInfo? modelInfo = FssGodotFactory.Instance.ModelLibrary.GetModelInfo(platformType);
        if (modelInfo == null)
        {
            FssCentralLog.AddEntry($"SetModelScale: Model info not found for {platformType}.");
            return;
        }

        // If we are to apply the natural platform scaling
        if (applyRwScaling)
        {
            // Apply the scale to the model node
            modelNode.Scale = new Vector3(modelInfo.Scale, modelInfo.Scale, modelInfo.Scale);
        }
        else
        {
            // Apply a scaling such that a "scale of 1" makes any platform's longest axis 100m.
            float basicInfographicScale = 100f / (float)modelInfo.RwAABB.LongestDimension;

            float adjscale = modelInfo.Scale * scaleModifier * basicInfographicScale;
            modelNode.Scale = new Vector3(adjscale, adjscale, adjscale);
        }
    }

}


