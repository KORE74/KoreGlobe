

// Class to server as the central repository for all 3D models.

using Godot;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
// using System.Text.Json.Serialization;
// using System.Linq;

// The FssDlcOperations class deals with the creation/loading and managing or DLC .PCK files.
// The Fss3DModelLibrary class deals with the supply of 3D model Nodes to the application, along with the
// model information around scaling, bounding boxes, etc.

#nullable enable

public class Fss3DModelLibrary
{
    // List of model information, and attributes under general C# control.
    private Dictionary<string, Fss3DModelInfo> ModelInfoList = new Dictionary<string, Fss3DModelInfo>();

    // Named list of specific 3D model objects for Godot scene tree.
    private Dictionary<string, Node3D> ModelCache = new Dictionary<string, Node3D>();

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config
    // ------------------------------------------------------------------------------------------------

    // usage: Fss3DModelLibrary.TestLoadModel();

    public void TestLoadModel(Node parent)
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
    // MARK: Supply Model
    // ------------------------------------------------------------------------------------------------

    // Usage: Node modelNode = Fss3DModelLibrary.PrepModel("GenericSupportShip");
    public Node3D PrepModel(string modelName)
    {
        // // If the model is already loaded, return it
        // if (ModelCache.ContainsKey(modelName))
        // {
        //     return ModelCache[modelName];
        // }

        GD.Print("======> 0");


        // If the name is not in the ModelInfoList, we don't know about it, so return null
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return null;
        }

        // Load the model
        string modelResPath = ModelInfoList[modelName].FilePath;
        float modelScale    = ModelInfoList[modelName].Scale;
        Vector3 modelOffset = ModelInfoList[modelName].CenterOffset;

        modelResPath = "res://Resources/DLC/MilitaryVehicles/Plane/C-130/C-130_2024-09-02_005.glb";
        modelResPath = "res://Resources/TestRes/C-130_2024-09-02_005.glb";
        modelResPath = "res://Resources/DLC/MilitaryVehicles/Plane/Yak130/Yak130.glb";

        modelScale = 0.0001f;


        // Access the model resource
        PackedScene importedModel = (PackedScene)ResourceLoader.Load(modelResPath);
        //var importedModel = (Node)ResourceLoader.Load(modelResPath);


        // Load the .glb model as a resource
        var glbResource = ResourceLoader.Load(modelResPath);

        if (glbResource != null)
        {
            GD.Print($"======> 1.5 // LOADED modelResPath:{modelResPath}");
        }

        GD.Print($"======> 1 // modelResPath:{modelResPath}");

        // If the model resource is not null, instantiate it
        if (importedModel != null)
        {
            GD.Print("======> 2 - ");

            // Instance the model
            Node3D modelInstance     = (Node3D)importedModel.Instantiate();
            Node3D ModelResourceNode = modelInstance as Node3D;
            ModelResourceNode.Name   = modelName;

            ModelResourceNode.Scale    = new Vector3(modelScale, modelScale, modelScale); // Set the model scale
            ModelResourceNode.Position = modelOffset; // Set the model position

            ModelResourceNode.RotationDegrees = new Vector3(0, (float)(180 * FssConsts.DegsToRadsMultiplier), 0); // Set the model rotation
            //ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up); // Can't look at something until paced in the tree

            ModelCache.Add(modelName, ModelResourceNode);

            GD.Print("======> 3 - ");

            return ModelResourceNode;

        }
        else
        {
            FssCentralLog.AddEntry($"Failed to load model. name:{modelName} // Path:{modelResPath}");
        }
        return ModelCache[modelName];
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config
    // ------------------------------------------------------------------------------------------------

    // Load a file from the Godot virtual file system, adding its details to the model list.
    // See: FssDLCOperations.InventoryJsonForDLCTitle()

    // Usage: Fss3DModelLibrary.LoadJSONConfigFile(jsonString);
    public void LoadJSONConfigFile(string jsonString)
    {
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
            FssCentralLog.AddEntry($"Loaded JSON: {model.Name}");
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Load / Save JSON Config String
    // ------------------------------------------------------------------------------------------------

    // Usage: string JSONString = Fss3DModelLibrary.SerializeJSONConfig(ModelInfoList.Values.ToList());
    public string SerializeJSONConfig(List<Fss3DModelInfo> modelList)
    {
        // Serialize the model list to JSON
        return JsonSerializer.Serialize(modelList);
    }

    // To allow us to read the JSON config from multiple sources, we pass the already read string to this
    // function to decode it and add it to the model list.
    // Use standard .Net JSON parser.
    public void DeserializeJSONConfig(string jsonString)
    {
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
            FssCentralLog.AddEntry($"Loaded JSON: {model.Name}");
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
    // MARK: Content Report
    // ------------------------------------------------------------------------------------------------

    public string ReportContent()
    {
        StringBuilder report = new StringBuilder();

        report.AppendLine("Model Library Content Report");
        report.AppendLine("=============================");
        report.AppendLine();

        foreach (var model in ModelInfoList.Values)
        {
            report.AppendLine($"Model: {model.Name}");
            report.AppendLine($"  Path: {model.FilePath}");
            report.AppendLine($"  AABB: {model.RwAABB}");
            report.AppendLine();
        }

        return report.ToString();
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Names
    // ------------------------------------------------------------------------------------------------

    public List<string> ModelNamesList()
    {
        return new List<string>(ModelInfoList.Keys);
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: AABB
    // ------------------------------------------------------------------------------------------------

    public FssXYZBox? AABBForModel(string modelName)
    {
        if (!ModelInfoList.ContainsKey(modelName))
        {
            GD.PrintErr($"Model does not exist: {modelName}");
            return null;
        }

        return ModelInfoList[modelName].RwAABB;
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
