// using System;
// using System.Collections;
// using System.Collections.Generic;

// // Static class wrapper for a single/central instance of a FssConfig.

// public static class FssCentralConfig
// {
//     private static FssConfig configData;

//     static FssCentralConfig()
//     {
//         configData = new FssConfig();
//         configData.LoadOrCreateJSONConfig("CentralConfig.json");

//         // Setup default configs if not present
//         if (!FssCentralConfig.HasParam("MapRootDir")) FssCentralConfig.SetParam("MapRootDir", "C:/Util/FssLibrary/Maps/", false);
//         if (!FssCentralConfig.HasParam("CaptureDir")) FssCentralConfig.SetParam("CaptureDir", "C:/Util/FssCapture/", false);
//     }

//     public static void SetParam(string name, string value, bool WriteOnAssign = true)
//     {
//         configData.SetParam(name, value, WriteOnAssign);
//     }

//     public static bool HasParam(string name)
//     {
//         return configData.HasParam(name);
//     }

//     public static string GetParam(string name)
//     {
//         return configData.GetParam(name);
//     }

//     public static int GetParamAsInt(string name)
//     {
//         string value = GetParam(name);
//         if (value != null && int.TryParse(value, out int result))
//         {
//             return result;
//         }
//         else
//         {
//             return 0;
//         }
//     }

//     // --------------------------------------------------------------------------------------

//     public static string BoolToString(bool value)
//     {
//         return value ? "True" : "False";
//     }

//     public static bool StringToBool(string value)
//     {
//         return string.Equals(value.Trim(), "True", StringComparison.OrdinalIgnoreCase);
//     }

// }
