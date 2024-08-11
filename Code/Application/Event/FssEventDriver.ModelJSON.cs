
// using System;
// using System.IO;

// using System.Text.Json;
// using System.Text.Json.Serialization;

// #nullable enable

// // Design Decisions:
// // - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

// public partial class FssEventDriver
// {
//     public void ModelToJsonFile(string filename)
//     {
//         // Check the input name is valid
//         if (string.IsNullOrEmpty(filename)) return;

//         // Check the file extension
//         filename = FssFileOperations.EnsureExtension(filename, ".json");

//         var options = new JsonSerializerOptions
//         {
//             WriteIndented    = true, // Makes the JSON output human-readable
//             ReferenceHandler = ReferenceHandler.Preserve // Handles circular references, if any
//         };

//         // Assuming `simulationState` is an instance of `SimulationState`
//         string jsonString = JsonSerializer.Serialize(FssAppFactory.Instance.PlatformManager, options);

//         // Write the JSON string to a file
//         File.WriteAllText(filename, jsonString);
        
//         FssCentralLog.AddEntry($"ModelToJsonFile: {filename}");
//     }
// }