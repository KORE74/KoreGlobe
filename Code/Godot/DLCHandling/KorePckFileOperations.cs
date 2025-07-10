// using System;
// using System.IO;
// using System.Text;
// using System.Collections.Generic;

// using Godot;

// // static class to handling generic functionality around finding and importing DLCs

// // We have a few areas where DLCs are considered:
// // - Prep: The folders of files in the res:// area that are prepared for export
// // - Files: The exported .pck files that could be loaded.
// // - Loaded: The DLCs that are loaded into the game, with accessibly JSON and glTF files.

// // This class builds ontop of:
// // - GloFileOperations: General file operations
// // - GloGodotFileOperations: Godot specific file operations

// public static class KorePckFileOperations
// {
//     private static string dlcDirectory = "DLC";

//     public static string RootDir    = "res://";
//     public static string DlcLoadDir = "res://Resources/DLC/"; // GloGodotFileOperations.DlcLoadDir
//     public static string DlcPrepDir = "res://Resources/DLCPrep/"; // GloGodotFileOperations.DlcPrepDir


//     // private static string dlcExportFolder = "C:/Util/Data/Godot/ExportDLC";
//     // private static string dlcExportFolder = "c:/util/godot/dlc/";


//     // --------------------------------------------------------------------------------------------
//     // MARK: DLC Pre-export listing
//     // --------------------------------------------------------------------------------------------

//     public static void EnsureDlcFolderExists()
//     {
//         // Base path for the DLCs
//         string execDir     = GloFileOperations.StandardizePath(OS.GetExecutablePath().GetBaseDir());
//         string fullDlcPath = GloFileOperations.JoinPaths(execDir, dlcDirectory);

//         // Create the DLC directory if it doesn't exist
//         if (!Directory.Exists(fullDlcPath))
//             Directory.CreateDirectory(fullDlcPath);
//     }

//     public static List<string> AllDlcExportableContent()
//     {
//         List<string> dlcListing = GloGodotFileOperations.ListFiles(DlcPrepDir, GloGodotFileOperations.EFileType.Both);
//         return dlcListing;
//     }

//     public static List<string> DlcExportTitles()
//     {
//         List<string> dlcListing = GloGodotFileOperations.ListFiles(DlcPrepDir, GloGodotFileOperations.EFileType.Directories, false);
//         List<string> dlcTitles = new List<string>();

//         foreach (string dlc in dlcListing)
//         {
//             string dlcName = GloGodotFileOperations.LastPathElement(dlc);
//             dlcTitles.Add(dlcName);
//         }
//         return dlcTitles;
//     }

//     public static List<string> DlcExportContentForTitle(string DlcTitle)
//     {
//         // Determine the prefix we need
//         string dlcPrefix = GloGodotFileOperations.JoinPaths(DlcPrepDir, DlcTitle);

//         // Get the listing of all the files in the DLC Prep area. Any with a matching prefix
//         // have the prefix removed and added to the output list.

//         List<string> dlcFullListing = GloGodotFileOperations.ListFiles(DlcPrepDir, GloGodotFileOperations.EFileType.Both);
//         List<string> filteredList   = GloFileOperations.FilterFilenamePrefix(dlcFullListing, dlcPrefix);
//         List<string> OutputList     = GloFileOperations.RemovePrefix(filteredList, dlcPrefix);

//         return OutputList;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Create DLC
//     // --------------------------------------------------------------------------------------------

//     public static void CreateDlc()
//     {
//         string dlcExportFolder = GloCentralConfig.Instance.GetParam<string>("DlcPath");

//         // List the potential DLCs to export
//         List<string> DLCsToExport = DlcExportTitles();

//         foreach (string dlcName in DLCsToExport)
//         {
//             string dlcPckName = $"{dlcName}.pck";

//             // PCK File
//             string dlcPckFilename = GloGodotFileOperations.JoinPaths(dlcExportFolder, dlcPckName);
//             if (File.Exists(dlcPckFilename))
//                 File.Delete(dlcPckFilename);

//             GD.Print($"\n\n\nExporting DLC: {dlcName} to {dlcPckFilename}");

//             // Start the packing
//             var packer = new PckPacker();
//             packer.PckStart(dlcPckFilename);

//             // list all the files in the DLCs folder
//             List<string> fileList = DlcExportContentForTitle(dlcName);

//             foreach (string file in fileList)
//             {
//                 string localFileLocation = GloGodotFileOperations.JoinPaths(DlcPrepDir, dlcName, file);
//                 string loadDesination    = GloGodotFileOperations.JoinPaths(DlcLoadDir, dlcName, file);

//                 // Add files to the pack
//                 var x = packer.AddFile(loadDesination, localFileLocation);
//                 GD.Print($"Added file:{file}  // to {loadDesination} // return: {x}");
//             }

//             // Conclude the adding for this .PCK file
//             packer.Flush(true);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Load DLC
//     // --------------------------------------------------------------------------------------------

//     // Loadable = on the disk, not inside the godot virtual filesystem

//     public static List<string> ListLoadableDlcPaths()
//     {
//         string dlcExportFolder = GloCentralConfig.Instance.GetParam<string>("DlcPath");

//         List<string> dlcFolderFiles = GloFileOperations.Filenames(dlcExportFolder);
//         List<string> dlcPCKFiles    = GloFileOperations.FilterFilenameSuffix(dlcFolderFiles, ".pck");

//         return dlcPCKFiles;
//     }

//     public static void LoadDlc(string dlcPath)
//     {
//         // string dlcExportFolder = GloCentralConfig.Instance.GetParam<string>("DlcPath");
//         // string dlcPckName = $"{dlcName}.pck";
//         // string dlcPath = GloFileOperations.JoinPaths(dlcExportFolder, dlcPckName);

//         if (ProjectSettings.LoadResourcePack(dlcPath))
//         {
//             GD.Print($"DLC pack loaded successfully! ({dlcPath})");
//         }
//         else
//         {
//             GD.Print($"Failed to load DLC pack: {dlcPath}");
//         }
//     }

//     // Handle the "Inventory.json" file in the DLC, adding the items to the modellibrary

//     public static string InventoryJsonForDLCTitle(string dlcTitle)
//     {
//         string dlcPath       = GloGodotFileOperations.JoinPaths(DlcLoadDir, dlcTitle);
//         string inventoryPath = GloGodotFileOperations.JoinPaths(dlcPath, "Inventory.json");

//         if (!GloGodotFileOperations.Exists(inventoryPath))
//         {
//             GloCentralLog.AddEntry($"Did not find expected file: {inventoryPath}");
//             return "";
//         }

//         string strFileContent = GloGodotFileOperations.ReadStringFromFile(inventoryPath);

//         //string inventoryJson = GloGodotFileOperations.LoadText(inventoryPath);
//         return strFileContent;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Loaded DLC
//     // --------------------------------------------------------------------------------------------

//     // Loaded = inside the godot virtual filesystem

//     public static List<string> ListLoadedDlcPaths()
//     {
//         List<string> loadedDlcPathsList = GloGodotFileOperations.ListFiles(DlcLoadDir, GloGodotFileOperations.EFileType.Directories, false);
//         return loadedDlcPathsList;
//     }

//     public static List<string> ListLoadedDlcTitles()
//     {
//         List<string> loadedDlcPathsList  = GloGodotFileOperations.ListFiles(DlcLoadDir, GloGodotFileOperations.EFileType.Directories, false);
//         List<string> loadedDlcTitlesList = GloFileOperations.RemovePrefix(loadedDlcPathsList, DlcLoadDir);
//         return loadedDlcTitlesList;
//     }

//     public static List<string> ListLoadedDlcContentForPath(string LoadedDlcPath)
//     {
//         List<string> dlcFullListing = GloGodotFileOperations.ListFiles(LoadedDlcPath, GloGodotFileOperations.EFileType.Both);
//         //List<string> filteredList   = GloFileOperations.FilterFilenamePrefix(dlcFullListing, dlcPrefix);
//         //List<string> OutputList     = GloFileOperations.RemovePrefix(filteredList, dlcPrefix);

//         return dlcFullListing;
//     }

//     public static List<string> ListLoadedDlcContentForTitle(string dlcTitle)
//     {
//         // Determine the loaded path from the title
//         string loadedDlcPath = GloGodotFileOperations.JoinPaths(DlcLoadDir, dlcTitle);

//         List<string> dlcFullListing = GloGodotFileOperations.ListFiles(loadedDlcPath, GloGodotFileOperations.EFileType.Both);
//         //List<string> filteredList   = GloFileOperations.FilterFilenamePrefix(dlcFullListing, dlcPrefix);
//         //List<string> OutputList     = GloFileOperations.RemovePrefix(filteredList, dlcPrefix);

//         return dlcFullListing;
//     }

//     public static List<string> ListLoadedDlcContent()
//     {
//         List<string> loadedDlc = GloGodotFileOperations.ListFiles(DlcLoadDir, GloGodotFileOperations.EFileType.Both);
//         return loadedDlc;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: DLC Report
//     // --------------------------------------------------------------------------------------------

//     // A list of all prep, pck and loaded DLCs in one string report for debugging analysis

//     public static string DlcReport()
//     {
//         StringBuilder sb = new StringBuilder();

//         sb.AppendLine("DLC Report");

//         sb.AppendLine("Prep DLCs");
//         List<string> prepDlcs = DlcExportTitles();
//         foreach (string dlc in prepDlcs)
//         {
//             sb.AppendLine(dlc);
//             List<string> content = DlcExportContentForTitle(dlc);
//             foreach (string file in content)
//             {
//                 sb.AppendLine($"- {file}");
//             }
//         }

//         sb.AppendLine("\nLoadable DLCs");
//         sb.AppendLine($"- DLC Path: {GloCentralConfig.Instance.GetParam<string>("DlcPath")}");
//         List<string> loadableDlcs = ListLoadableDlcPaths();
//         foreach (string dlc in loadableDlcs)
//         {
//             sb.AppendLine($"- {dlc}");
//         }

//         sb.AppendLine("\nLoaded DLCs");
//         List<string> loadedDlcs = ListLoadedDlcTitles();
//         foreach (string dlc in loadedDlcs)
//         {
//             sb.AppendLine($"- {dlc}");
//             List<string> content = ListLoadedDlcContentForTitle(dlc);
//             foreach (string file in content)
//             {
//                 sb.AppendLine($"- {file}");
//             }
//         }

//         sb.AppendLine("\nLoaded DLC Content");
//         List<string> loadedContent = ListLoadedDlcContent();
//         foreach (string file in loadedContent)
//         {
//             sb.AppendLine($"- {file}");
//         }

//         return sb.ToString();
//     }
// }