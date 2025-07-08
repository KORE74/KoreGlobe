// using System;
// using System.Collections.Generic;
// using System.Text;

// using Godot;

// #nullable enable

// public partial class KoreGodotEntityManager : Node3D
// {
//     // --------------------------------------------------------------------------------------------
//     // MARK: Update - 3D Model
//     // --------------------------------------------------------------------------------------------

//     private void MatchModelPlatform3DModel(string platName)
//     {
//         // Get the platform, and platform type, from the mode model side
//         string? platformType = GloAppFactory.Instance.EventDriver.PlatformType(platName);
//         if (string.IsNullOrEmpty(platformType))
//             platformType = "default";

//         // if (GloMapManager.ShowDebug)
//         //     MatchBoundingBox(platName, platformType);

//         // Get the entity
//         GloGodotEntity? ent = GetEntity(platName);
//         if (ent != null)
//         {
//             Node3D? attNode = ent.AttitudeNode;
//             if (attNode != null)
//             {
//                 // Get the model to match the modelname - add a new one if it doesn't already exist
//                 string modelNodeName = "model";
//                 Node3D? modelNode = ent.AttitudeNode.GetNodeOrNull<Node3D>(modelNodeName);
//                 if (modelNode == null)
//                 {
//                     modelNode = GloGodotFactory.Instance.ModelLibrary.PrepModel2(platformType);

//                     if (modelNode != null)
//                     {
//                         ent.AttitudeNode.AddChild(modelNode);
//                         GloCentralLog.AddEntry($"MatchModelPlatform3DModel // {platName} -> {platformType}");
//                     }
//                 }
//             }
//         }
//     }

//     private void MatchBoundingBox(string platName, string platformType)
//     {
//         // The name of the bounding box node
//         string bbNodeName = "aabb";

//         // Get the platform node
//         GloGodotEntity? ent = GetEntity(platName);

//         // Get the AABB for the model
//         GloXYZBox? rwAABB = GloGodotFactory.Instance.ModelLibrary.AABBForModel(platformType);


//         if (rwAABB == null)
//         {
//             GloCentralLog.AddEntry($"MatchBoundingBox: {platformType} => rwAABB is null");

//             List<string> modelNames = GloGodotFactory.Instance.ModelLibrary.ModelNamesList();
//             string modelNamesStr = string.Join(", ", modelNames);

//             GloCentralLog.AddEntry($"Known models: {modelNamesStr}");
//         }
//         else
//             GloCentralLog.AddEntry($"MatchBoundingBox: {platformType} => rwAABB:{rwAABB}");

//         if ((ent != null) && (rwAABB != null))
//         {
//             // Get the bounding box node name
//             //Node3D? bbNode = ent.FindNode(bbNodeName) as Node3D;

//             Node3D? bbNode = ent.AttitudeNode.GetNodeOrNull<Node3D>(bbNodeName);

//             // If the bounding box node exists, remove it
//             if (bbNode != null)
//             {
//                 //bbNode.QueueFree();
//                 return;
//             }

//             // Create the bounding box
//             //GloXYZBox rwAABB = new GloXYZBox() { Height = 200, Width = 400, Length = 600 };
//             //rwAABB = ent.ModelInfo.RwAABB;
//             GloXYZBox geAABB = rwAABB.Scale(GloZeroOffset.RwToGeDistanceMultiplier);

//             GD.Print($"MatchBoundingBox: {platformType} => geAABB:{geAABB}");

//             // Create a new bounding box node, using our LineMesh class
//             GloLineMesh3D bbMesh = new GloLineMesh3D();
//             bbMesh.Name = bbNodeName;
//             Color lineColor = GloColorUtil.LookupColor("Yellow", 0.5f);

//             bbMesh.AddBoxWithLeadingEdge(geAABB, lineColor);

//             // Add the bounding box to the entity
//             ent.AttitudeNode.AddChild(bbMesh);
//         }
//     }

//     // --------------------------------------------------------------------------------------------

//     // Set the model scale.
//     // 1 - Get the "model" node. Then get its only child, which is the model itself.
//     // 2 - Get the scale modifier, multiply it by the 3D Library scale factor, and apply it to the node.
//     //      - We have the platform name and type to assist in lookup up elements.

//     void SetModelScale(string platName, string platformType, bool applyRwScaling, float scaleModifier)
//     {
//         // Adjust the scale of the model
//         //scaleModifier = GloValueUtils.ScaleVal(scaleModifier,  1f, 10f,  1f, 400f);
//         scaleModifier = (float)InfographicScaleRange.GetValue(scaleModifier);

//         // Get the entity
//         GloGodotEntity? ent = GetEntity(platName);
//         if (ent == null)
//         {
//             GloCentralLog.AddEntry($"SetModelScale: Entity {platName} not found.");
//             return;
//         }
//         // Get the model node
//         Node3D? modelNode = ent.AttitudeNode.GetNodeOrNull<Node3D>("model");
//         if (modelNode == null)
//         {
//             GloCentralLog.AddEntry($"SetModelScale: Model node not found for {platName}.");
//             return;
//         }

//         // Get the 3D Library entry for the type
//         Glo3DModelInfo? modelInfo = GloGodotFactory.Instance.ModelLibrary.GetModelInfo(platformType);
//         if (modelInfo == null)
//         {
//             GloCentralLog.AddEntry($"SetModelScale: Model info not found for {platformType}.");
//             return;
//         }

//         // If we are to apply the natural platform scaling
//         if (applyRwScaling)
//         {
//             // Apply the scale to the model node
//             modelNode.Scale = new Vector3(modelInfo.Scale, modelInfo.Scale, modelInfo.Scale);
//         }
//         else
//         {
//             // Apply a scaling such that a "scale of 1" makes any platform's longest axis 100m.
//             float basicInfographicScale = 100f / (float)modelInfo.RwAABB.LongestDimension;

//             float adjscale = modelInfo.Scale * scaleModifier * basicInfographicScale;
//             modelNode.Scale = new Vector3(adjscale, adjscale, adjscale);
//         }
//     }

//     // Using Godot's AABB handling is parked until we can get the models loaded as a MeshInstance3D object rather than a Node3D.
//     // - Then we can use the AABB size to start considering LODs.

//     // private Node3D? CheckAABB(string platName)
//     // {
//     //     // Get the entity
//     //     GloGodotEntity? ent = GetEntity(platName);
//     //     if (ent != null)
//     //     {
//     //         Node3D? attNode = ent.AttitudeNode;
//     //         if (attNode != null)
//     //         {
//     //             // Get the model to match the modelname - add a new one if it doesn't already exist
//     //             string modelNodeName = "model";
//     //             Node3D? modelNode = ent.AttitudeNode.GetNodeOrNull<Node3D>(modelNodeName);
//     //             if (modelNode != null)
//     //             {
//     //                 // if the node is a MeshInstance3D, we can get the AABB
//     //                 if (modelNode is MeshInstance3D)
//     //                 {
//     //                     MeshInstance3D modelNodeMI = (MeshInstance3D)modelNode;
//     //                     Aabb godotBBox = modelNodeMI.GetAabb();

//     //                     float longestEdge = godotBBox.GetLongestAxisSize();

//     //                     GD.Print($"GetModelNode: {platName} => AabbLongestEdge:{longestEdge}");
//     //                 }
//     //                 else
//     //                 {
//     //                     GD.Print($"GetModelNode: {platName} => modelNode is not a MeshInstance3D");
//     //                 }
//     //             }
//     //         }

//     //     }

//     //     return null;
//     // }

// }


