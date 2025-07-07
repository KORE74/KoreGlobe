// using System;
// using System.IO;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.Json;

// // A JSON wrapper class for a quick/dirty way to write parameters out to a file.

// public class GloConfig
// {
//     private string jsonFilePath;

//     private Dictionary<string, string> configData;

//     private JsonSerializerOptions jsonOptions;

//     // --------------------------------------------------------------------------------------------

//     public GloConfig()
//     {
//         jsonFilePath = "";
//         configData = new Dictionary<string, string>();
//         jsonOptions = new JsonSerializerOptions
//         {
//             WriteIndented = true // Equivalent to Formatting.Indented in Newtonsoft.Json
//         };
//     }

//     // ---------------------------------------------------------------------------------------------------------------

//     public bool LoadOrCreateJSONConfig(string newJsonFileName)
//     {
//         jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), newJsonFileName);

//         if (File.Exists(jsonFilePath))
//         {
//             try
//             {
//                 string jsonString = File.ReadAllText(jsonFilePath);
//                 configData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString, jsonOptions);
//             }
//             catch (System.Exception)
//             {
//                 return false;
//             }
//             return true;
//         }
//         else
//         {
//             configData = new Dictionary<string, string>();
//             return SaveJSONConfig();
//         }
//     }

//     // --------------------------------------------------------------------------------------------

//     public bool SaveJSONConfig()
//     {
//         try
//         {
//             string jsonString = JsonSerializer.Serialize(configData, jsonOptions);
//             File.WriteAllText(jsonFilePath, jsonString);
//         }
//         catch (System.Exception)
//         {
//             return false;
//         }
//         return true;
//     }

//     public bool SaveSortedJSONConfig()
//     {
//         try
//         {
//             // Create a sorted version of the dictionary
//             var sortedConfigData = configData.OrderBy(pair => pair.Key)
//                                             .ToDictionary(pair => pair.Key, pair => pair.Value);

//             string jsonString = JsonSerializer.Serialize(sortedConfigData, jsonOptions);
//             File.WriteAllText(jsonFilePath, jsonString);
//         }
//         catch (System.Exception)
//         {
//             return false;
//         }
//         return true;
//     }

//     // --------------------------------------------------------------------------------------------

//     public bool SetParam(string name, string value, bool WriteOnAssign = true)
//     {
//         bool retFlag = true;

//         // Sanity checking. This class is for small config items.
//         if (name.Length > 256)   return false;
//         if (value.Length > 1024) return false;

//         configData[name] = value;

//         if (WriteOnAssign)
//             retFlag = SaveSortedJSONConfig();

//         return retFlag;
//     }

//     public bool HasParam(string name)
//     {
//         return configData.ContainsKey(name);
//     }

//     public string GetParam(string name)
//     {
//         if (!configData.ContainsKey(name))
//             return string.Empty;

//         return configData[name];
//     }
// }
