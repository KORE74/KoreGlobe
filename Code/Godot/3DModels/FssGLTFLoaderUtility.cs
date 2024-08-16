using Godot;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;


public class FssGLTFLoaderUtility
{
    // Dictionary<string, Node> ModelCache = new Dictionary<string, Node>();

    // public void LoadGLTF(string cacheName string filePath)
    // {
    //     // Ensure the file exists
    //     if (!File.Exists(filePath))
    //     {
    //         GD.PrintErr($"File not found: {filePath}");
    //         return;
    //     }

    //     // Load the GLTF file asynchronously
    //     {
    //         var gltfState  = new GLTFState();
    //         var gltfLoader = new GLTFLoader();

    //         var error = gltfLoader.LoadFromFile(filePath, gltfState);

    //         if (error != Error.Ok)
    //         {
    //             GD.PrintErr($"Failed to load GLTF file: {filePath}");
    //             return null;
    //         }

    //         // Create the scene tree from the loaded GLTF file
    //         var sceneRoot = gltfState.GenerateScene();
    //         return sceneRoot;
    //     });
    // }

    // public void AddGLTFToScene(Node? gltfNode, Node parentNode)
    // {
    //     if (gltfNode == null)
    //     {
    //         GD.PrintErr("GLTF node is null. Cannot add to scene.");
    //         return;
    //     }

    //     parentNode.AddChild(gltfNode);
    //     GD.Print("GLTF model added to the scene.");
    // }
}
