

// Class to server as the central repository for all 3D models.

using Godot;

using System;
using System.Collections.Generic;
using System.Text.Json;

// The FssDlcOperations class deals with the creation/loading and managing or DLC .PCK files.
// The Fss3DModelLibrary class deals with the supply of 3D model Nodes to the application, along with the 
// model informatino around scaling, bounding boxes, etc.

public class Fss3DModelLibrary
{
    // List of model information, and attributes under general C# control.
    Dictionary<string, Fss3DModelInfo> ModelInfoList = new Dictionary<string, Fss3DModelInfo>();

    // Named list of specific 3D model objects for Godot scene tree.
    Dictionary<string, Node> ModelCache = new Dictionary<string, Node>();

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config
    // ------------------------------------------------------------------------------------------------

    // usage: Fss3DModelLibrary.TestLoadModel();

    public static void TestLoadModel(Node parent)
    {
        string ModelPathG = "C:/Util/Godot/Globe4-3DModels/Prep/Ship/GenericSupportShip/GenericSupportShip.glb";
        string ModelPathF = "C:/Util/Godot/Globe4-3DModels/Prep/Ship/GenericSupportShip/GenericSupportShip.fbx";
        string ModelPathR = "res://Resources/Models/Plane/Plane_Paper/PaperPlanes_v002.glb";

        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPathR);

        if (importedModel != null)
        {
            // Instance the model. Add the model to the scene
            Node modelInstance = importedModel.Instantiate();
            parent.AddChild(modelInstance);
            GD.Print($"====== Loaded model: {ModelPathR}");
        }
        else
        {
            GD.PrintErr($"====== Failed to load model: {ModelPathR}");
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config
    // ------------------------------------------------------------------------------------------------

    // Load a file from the Godot virtual file system, adding its details to the model list.
    // See: FssDLCOperations.InventoryJsonForDLCTitle()

    public void LoadJSONConfigFile(string jsonString)
    {
        // string jsonString = System.IO.File.ReadAllText(fullFilepath);
        var modelList = JsonSerializer.Deserialize<List<Fss3DModelInfo>>(jsonString);

        if (modelList == null)
        {
            GD.PrintErr($"Failed to parse JSON file");
            return;
        }

        // Add the model information to the list
        foreach (var model in modelList)
        {
            ModelInfoList.Add(model.Name, model);
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config String
    // ------------------------------------------------------------------------------------------------

    // Usage: string JSONString = Fss3DModelLibrary.SerializeJSONConfig(ModelInfoList.Values.ToList());
    public static string SerializeJSONConfig(List<Fss3DModelInfo> modelList)
    {
        // Serialize the model list to JSON
        return JsonSerializer.Serialize(modelList);
    }

    // To allow us to read the JSON config from multiple sources, we pass the already read string to this
    // function to decode it and add it to the model list.
    // Use standard .Net JSON parser.
    public static void DeserializeJSONConfig(string JSONString)
    {
        GD.Print($"JSONString: {JSONString}");

        var modelList = JsonSerializer.Deserialize<List<Fss3DModelInfo>>(JSONString);

        if (modelList == null)
        {
            GD.PrintErr($"Failed to load JSON string.");
            return;
        }

        // Add the model information to the list
        foreach (var model in modelList)
        {
            //modelList.Add(model.Name, model);
            GD.Print($"Loaded model: {model.Name}");
        }
    }

    // ------------------------------------------------------------------------------------------------

    public void SaveJSONConfig(string fullFilepath)
    {
        // Serialize the model list to JSON
        string jsonString = JsonSerializer.Serialize(ModelInfoList.Values);

        // Write the JSON to the file
        System.IO.File.WriteAllText(fullFilepath, jsonString);
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Model Loading
    // ------------------------------------------------------------------------------------------------

    public void LoadModel(string modelName)
    {
        // if (!ModelInfoList.ContainsKey(modelName))
        // {
        //     GD.PrintErr($"Model does not exist: {modelName}");
        //     return;
        // }

        // // Ensure the model is not already loaded
        // if (ModelCache.ContainsKey(modelName))
        // {
        //     GD.PrintErr($"Model already loaded: {modelName}");
        //     return;
        // }

        // // Load the model asynchronously
        // {
        //     var modelInfo = ModelInfoList[modelName];
        //     var modelPath = modelInfo.FilePath;

        //     // Ensure the file exists
        //     if (!System.IO.File.Exists(modelPath))
        //     {
        //         GD.PrintErr($"File not found: {modelPath}");
        //         return;
        //     }

        //     // Load the GLTF file asynchronously
        //     {
        //         var gltfState  = new GLTFState();
        //         var gltfLoader = new GLTFLoader();

        //         var error = gltfLoader.LoadFromFile(modelPath, gltfState);

        //         if (error != Error.Ok)
        //         {
        //             GD.PrintErr($"Failed to load GLTF file: {modelPath}");
        //             return;
        //         }

        //         // Create the scene tree from the loaded GLTF file
        //         var sceneRoot = gltfState.GenerateScene();
        //         ModelCache.Add(modelName, sceneRoot);
        //     };
        // }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Edits to info list
    // ------------------------------------------------------------------------------------------------

    public void AddModelInfo(string modelName)
    {
        if (ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model already exists: {modelName}");
            return;
        }

        ModelInfoList.Add(modelName, new Fss3DModelInfo { Name = modelName });
    }

    // ------------------------------------------------------------------------------------------------

    public void RemoveModelInfo(string modelName)
    {
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return;
        }

        ModelInfoList.Remove(modelName);
    }

    // ------------------------------------------------------------------------------------------------

    public Fss3DModelInfo? GetModelInfo(string modelName)
    {
        return ModelInfoList.ContainsKey(modelName) ? ModelInfoList[modelName] : null;
    }

    // ------------------------------------------------------------------------------------------------

    public void SetModelPathForName(string modelName, string filePath)
    {
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return;
        }

        ModelInfoList[modelName].FilePath = filePath;
    }

    // ------------------------------------------------------------------------------------------------

    public void SetModelAABBForName(string modelName, FssXYZBox aabb)
    {
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return;
        }

        ModelInfoList[modelName].RwAABB = aabb;
    }

    // ------------------------------------------------------------------------------------------------

    public void DuplicateModelForName(string modelName, string newModelName)
    {
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return;
        }

        if (ModelInfoList.ContainsKey(newModelName))
        {
            GD.PrintErr($"Model already exists: {newModelName}");
            return;
        }

        ModelInfoList.Add(newModelName, ModelInfoList[modelName]);
    }
}








