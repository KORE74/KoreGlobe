
// // Class to server as the central repository for all 3D models.

// using Godot;

// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Text.Json;
// using System.Threading.Tasks;

// // using System.Text.Json.Serialization;
// // using System.Linq;

// // The GloDlcOperations class deals with the creation/loading and managing or DLC .PCK files.
// // The Glo3DModelLibrary class deals with the supply of 3D model Nodes to the application, along with the
// // model information around scaling, bounding boxes, etc.

// #nullable enable

// public class Glo3DModelLibrary
// {
//     //private ResourceInteractiveLoader _loader;

//     // List of model information, and attributes under general C# control.
//     private Dictionary<string, Glo3DModelInfo> ModelInfoList = new Dictionary<string, Glo3DModelInfo>();

//     // Named list of specific 3D model objects for Godot scene tree.
//     private Dictionary<string, Node3D> ModelCache = new Dictionary<string, Node3D>();


//     private Dictionary<string, PackedScene> PSCache = new Dictionary<string, PackedScene>();


//     // ------------------------------------------------------------------------------------------------
//     // MARK: Model Cache
//     // ------------------------------------------------------------------------------------------------

//     public async void LoadModelToCache()
//     {
//         // Run the process as an asynchronous task
//         await Task.Run(async () =>
//         {
//             // Loop through each of the ModelInfoList
//             foreach (KeyValuePair<string, Glo3DModelInfo> currModel in ModelInfoList)
//             {
//                 try
//                 {
//                     string         keyName   = currModel.Key;
//                     Glo3DModelInfo valueInfo = currModel.Value;

//                     GloCentralLog.AddEntry($"ModelImport // Processing Model: {keyName}");

//                     string modelResPath = valueInfo.FilePath;

//                     //Error load_threaded_request(modelResPath, type_hint: String = "", use_sub_threads: bool = false, cache_mode: CacheMode = 1)

//                     PackedScene model = ResourceLoader.Load<PackedScene>(modelResPath);

//                     if (model == null)
//                     {
//                         GloCentralLog.AddEntry($"ModelImport // LoadModelToCache: Failed to load model: {keyName} => {modelResPath}");
//                     }
//                     else
//                     {
//                         PSCache.Add(keyName, model);
//                         GloCentralLog.AddEntry($"ModelImport // LoadModelToCache: Loaded model: {keyName} => {modelResPath}");
//                     }

//                     // Be a good citizen, add small yield/delay between each model load
//                     await Task.Delay(100);
//                     await Task.Yield();

//                 }
//                 catch (Exception ex)
//                 {
//                     GloCentralLog.AddEntry($"ModelImport // EXCPETION loading model {currModel.Key}: {ex.Message}");
//                 }
//             }
//         });
//     }

//     // public void CheckModelLoad()
//     // {
//     //     // Loop through each of the ModelInfoList
//     //     foreach (KeyValuePair<string, Glo3DModelInfo> currModel in ModelInfoList)
//     //     {
//     //         string         keyName   = currModel.Key;
//     //         Glo3DModelInfo valueInfo = currModel.Value;

//     //         string modelResPath = valueInfo.FilePath;

//     //         // Poll the status of the threaded load
//     //         var status = ResourceLoader.LoadThreadedGetStatus(modelResPath);

//     //         GloCentralLog.AddEntry($"{keyName} // {status} // {modelResPath}");
//     //     }
//     // }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Supply Model
//     // ------------------------------------------------------------------------------------------------

//     // Usage: Node modelNode = Glo3DModelLibrary.PrepModel("GenericSupportShip");
//     // public Node3D? PrepModel(string modelName)
//     // {
//     //     // // If the model is already loaded, return it
//     //     // if (ModelCache.ContainsKey(modelName))
//     //     // {
//     //     //     return ModelCache[modelName];
//     //     // }

//     //     GloCentralLog.AddEntry("======> 0");


//     //     // If the name is not in the ModelInfoList, we don't know about it, so return null
//     //     if (!ModelInfoList.ContainsKey(modelName))
//     //     {
//     //         GloCentralLog.AddEntry($"Model does not exist: {modelName}");
//     //         return null;
//     //     }

//     //     // Load the model
//     //     string  modelResPath = ModelInfoList[modelName].FilePath;
//     //     float   modelScale   = ModelInfoList[modelName].Scale;
//     //     Vector3 modelOffset  = ModelInfoList[modelName].CenterOffset;
//     //     Vector3 modelRotate  = ModelInfoList[modelName].RotateDegs;

//     //     var glbResource = ResourceLoader.LoadThreadedGet(modelResPath);

//     //     if (glbResource == null)
//     //     {
//     //         GloCentralLog.AddEntry($"LoadThreadedGet FAIL: {modelName} => {modelResPath}");
//     //         return null;
//     //     }

//     //     GloCentralLog.AddEntry($"======> 0.5 // modelResPath:{modelResPath}");

//     //     // Flip for the Godot negative z-axis
//     //     modelScale *= (float)KoreZeroOffset.RwToGeDistanceMultiplier;
//     //     modelRotate.Y += 180;

//     //     // Access the model resource
//     //     //PackedScene importedModel = (PackedScene)ResourceLoader.Load(modelResPath);
//     //     //var importedModel = (Node)ResourceLoader.Load(modelResPath);

//     //     // Load the .glb model as a resource
//     //     // var glbResource = ResourceLoader.Load(modelResPath);

//     //     // if (glbResource != null)
//     //     // {
//     //     //     GloCentralLog.AddEntry($"======> 1.5 // LOADED modelResPath:{modelResPath}");
//     //     // }

//     //     GloCentralLog.AddEntry($"======> 1 // modelResPath:{modelResPath}");

//     //     if (glbResource is PackedScene loadedScene)
//     //     {
//     //         Node3D modelInstance = loadedScene.Instantiate<Node3D>();


//     //     // // If the model resource is not null, instantiate it
//     //     // if (importedModel != null)
//     //     // {
//     //     //     GloCentralLog.AddEntry("======> 2 - ");

//     //         // Create a "container" node, allowing the model to be scaled and rotated as a node attached to this one.
//     //         // Allows this node to be re-parented without losing those modifications.
//     //         Node3D modelContainerNode = new Node3D() { Name = "model" };

//     //         // Instance the model
//     //         // Node3D modelInstance     = (Node3D)importedModel.Instantiate();
//     //         Node3D ModelResourceNode = modelInstance as Node3D;
//     //         ModelResourceNode.Name   = modelName;

//     //         // Add the model to the container
//     //         modelContainerNode.AddChild(ModelResourceNode);

//     //         // Apply the adjustments to the model
//     //         ModelResourceNode.Scale    = new Vector3(modelScale, modelScale, modelScale); // Set the model scale
//     //         ModelResourceNode.Position = modelOffset; // Set the model position
//     //         ModelResourceNode.RotationDegrees = modelRotate;


//     //         modelContainerNode.Scale    = new Vector3(1f, 1f, 1f);
//     //         modelContainerNode.Position = new Vector3(0f, 0f, 0f);

//     //         //ModelCache.Add(modelName, modelContainerNode);

//     //         GloCentralLog.AddEntry($"======> 3 - scale:{modelScale} // offset:{modelOffset} // rotate:{modelRotate}");

//     //         return modelContainerNode;
//     //     }
//     //     else
//     //     {
//     //         GloCentralLog.AddEntry($"Failed to load model. name:{modelName} // Path:{modelResPath}");
//     //     }
//     //     return null; //ModelCache[modelName];
//     // }

//     public Node3D? PrepModel2(string modelName)
//     {
//         if (string.IsNullOrEmpty(modelName))
//         {
//             GloCentralLog.AddEntry("Model name is empty");
//             return null;
//         }

//         if (!ModelInfoList.ContainsKey(modelName) || !PSCache.ContainsKey(modelName))
//         {
//             GloCentralLog.AddEntry($"Model does not exist in cache: {modelName}");
//             return null;
//         }

//         PackedScene packedScene = PSCache[modelName];
//         if (packedScene == null)
//         {
//             GloCentralLog.AddEntry($"Failed to access model: {modelName}");
//             return null;
//         }

//         Node3D? instance = packedScene.Instantiate() as Node3D;
//         if (instance == null)
//         {
//             GloCentralLog.AddEntry($"Failed to instantiate model: {modelName}");
//             return null;
//         }

//         Node3D modelContainerNode = new Node3D() { Name = "model" };
//         modelContainerNode.AddChild(instance);

//         // Load the model
//         float   modelScale   = ModelInfoList[modelName].Scale;
//         float   modelScale2  = ModelInfoList[modelName].ModelScale;
//         Vector3 modelOffset  = ModelInfoList[modelName].CenterOffset;
//         Vector3 modelRotate  = ModelInfoList[modelName].RotateDegs;

//         modelRotate.Y += 180;

//         modelContainerNode.Scale = new Vector3(modelScale, modelScale, modelScale);

//         // Apply the adjustments to the model
//         instance.Scale           = new Vector3(modelScale2, modelScale2, modelScale2); // Set the model scale (inner non-changing number)
//         instance.Position        = modelOffset; // Set the model position
//         instance.RotationDegrees = modelRotate;


//         return modelContainerNode;
//     }


//     // ------------------------------------------------------------------------------------------------
//     // MARK: Load / Save JSON Config
//     // ------------------------------------------------------------------------------------------------

//     // Load a file from the Godot virtual file system, adding its details to the model list.
//     // See: GloDLCOperations.InventoryJsonForDLCTitle()

//     // Usage: Glo3DModelLibrary.LoadJSONConfigFile(jsonString);
//     public void LoadJSONConfigFile(string jsonString)
//     {
//         var modelList = JsonSerializer.Deserialize<List<Glo3DModelInfo>>(jsonString);

//         if (modelList == null)
//         {
//             GloCentralLog.AddEntry($"ModelImport // Failed to parse JSON file");
//             return;
//         }

//         // Add the model information to the list
//         foreach (var model in modelList)
//         {
//             ModelInfoList.Add(model.Name, model);
//             GloCentralLog.AddEntry($"ModelImport // Loaded JSON: {model.Name}");
//         }
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Load / Save JSON Config String
//     // ------------------------------------------------------------------------------------------------

//     // Usage: string JSONString = Glo3DModelLibrary.SerializeJSONConfig(ModelInfoList.Values.ToList());
//     public string SerializeJSONConfig(List<Glo3DModelInfo> modelList)
//     {
//         // Serialize the model list to JSON
//         return JsonSerializer.Serialize(modelList);
//     }

//     // To allow us to read the JSON config from multiple sources, we pass the already read string to this
//     // function to decode it and add it to the model list.
//     // Use standard .Net JSON parser.
//     public void DeserializeJSONConfig(string jsonString)
//     {
//         var modelList = JsonSerializer.Deserialize<List<Glo3DModelInfo>>(jsonString);

//         if (modelList == null)
//         {
//             GD.PrintErr($"Failed to parse JSON file");
//             return;
//         }

//         // Add the model information to the list
//         foreach (var model in modelList)
//         {
//             ModelInfoList.Add(model.Name, model);
//             GloCentralLog.AddEntry($"Loaded JSON: {model.Name}");
//         }
//     }

//     // ------------------------------------------------------------------------------------------------

//     public void SaveJSONConfig(string fullFilepath)
//     {
//         // Serialize the model list to JSON
//         string jsonString = JsonSerializer.Serialize(ModelInfoList.Values);

//         // Write the JSON to the file
//         System.IO.File.WriteAllText(fullFilepath, jsonString);
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Content Report
//     // ------------------------------------------------------------------------------------------------

//     public string ReportContent()
//     {
//         StringBuilder report = new StringBuilder();

//         report.AppendLine("Model Library Content Report");
//         report.AppendLine("=============================");
//         report.AppendLine();

//         foreach (var model in ModelInfoList.Values)
//         {
//             report.AppendLine($"Model: {model.Name}");
//             report.AppendLine($"  Path: {model.FilePath}");
//             report.AppendLine($"  AABB: {model.RwAABB}");
//             report.AppendLine();
//         }

//         return report.ToString();
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Names
//     // ------------------------------------------------------------------------------------------------

//     public List<string> ModelNamesList()
//     {
//         return new List<string>(ModelInfoList.Keys);
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: AABB
//     // ------------------------------------------------------------------------------------------------

//     public GloXYZBox? AABBForModel(string modelName)
//     {
//         if (!ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model does not exist: {modelName}");
//             return null;
//         }

//         return ModelInfoList[modelName].RwAABB;
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Model Loading
//     // ------------------------------------------------------------------------------------------------

//     public void LoadModel(string modelName)
//     {
//         // if (!ModelInfoList.ContainsKey(modelName))
//         // {
//         //     GD.PrintErr($"Model does not exist: {modelName}");
//         //     return;
//         // }

//         // // Ensure the model is not already loaded
//         // if (ModelCache.ContainsKey(modelName))
//         // {
//         //     GD.PrintErr($"Model already loaded: {modelName}");
//         //     return;
//         // }

//         // // Load the model asynchronously
//         // {
//         //     var modelInfo = ModelInfoList[modelName];
//         //     var modelPath = modelInfo.FilePath;

//         //     // Ensure the file exists
//         //     if (!System.IO.File.Exists(modelPath))
//         //     {
//         //         GD.PrintErr($"File not found: {modelPath}");
//         //         return;
//         //     }

//         //     // Load the GLTF file asynchronously
//         //     {
//         //         var gltfState  = new GLTFState();
//         //         var gltfLoader = new GLTFLoader();

//         //         var error = gltfLoader.LoadFromFile(modelPath, gltfState);

//         //         if (error != Error.Ok)
//         //         {
//         //             GD.PrintErr($"Failed to load GLTF file: {modelPath}");
//         //             return;
//         //         }

//         //         // Create the scene tree from the loaded GLTF file
//         //         var sceneRoot = gltfState.GenerateScene();
//         //         ModelCache.Add(modelName, sceneRoot);
//         //     };
//         // }
//     }

//     // ------------------------------------------------------------------------------------------------
//     // MARK: Edits to info list
//     // ------------------------------------------------------------------------------------------------

//     public void AddModelInfo(string modelName)
//     {
//         if (ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model already exists: {modelName}");
//             return;
//         }

//         ModelInfoList.Add(modelName, new Glo3DModelInfo { Name = modelName });
//     }

//     // ------------------------------------------------------------------------------------------------

//     public void RemoveModelInfo(string modelName)
//     {
//         if (!ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model does not exist: {modelName}");
//             return;
//         }

//         ModelInfoList.Remove(modelName);
//     }

//     // ------------------------------------------------------------------------------------------------

//     public Glo3DModelInfo? GetModelInfo(string modelName)
//     {
//         return ModelInfoList.ContainsKey(modelName) ? ModelInfoList[modelName] : null;
//     }

//     // ------------------------------------------------------------------------------------------------

//     public void SetModelPathForName(string modelName, string filePath)
//     {
//         if (!ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model does not exist: {modelName}");
//             return;
//         }

//         ModelInfoList[modelName].FilePath = filePath;
//     }

//     // ------------------------------------------------------------------------------------------------

//     public void SetModelAABBForName(string modelName, GloXYZBox aabb)
//     {
//         if (!ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model does not exist: {modelName}");
//             return;
//         }

//         ModelInfoList[modelName].RwAABB = aabb;
//     }

//     // ------------------------------------------------------------------------------------------------

//     public void DuplicateModelForName(string modelName, string newModelName)
//     {
//         if (!ModelInfoList.ContainsKey(modelName))
//         {
//             GD.PrintErr($"Model does not exist: {modelName}");
//             return;
//         }

//         if (ModelInfoList.ContainsKey(newModelName))
//         {
//             GD.PrintErr($"Model already exists: {newModelName}");
//             return;
//         }

//         ModelInfoList.Add(newModelName, ModelInfoList[modelName]);
//     }
// }
