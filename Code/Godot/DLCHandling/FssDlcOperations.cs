using System;
using System.IO;
using System.Collections.Generic;

using Godot;

// static class to handling generic functionality around finding and importing DLCs

public static class FssDlcOperations
{
    private static string dlcDirectory = "DLC";

    private static string dlcExportFolder = "C:/Util/Data/Godot/ExportDLC";
    //private static string dlcExportFolder = "c:/util/godot/dlc/";

    // --------------------------------------------------------------------------------------------

    public static void EnsureDlcFolderExists()
    {
        // Base path for the DLCs
        string execDir = FssFileOperations.StandardizePath(OS.GetExecutablePath().GetBaseDir());
        string fullDlcPath = FssFileOperations.JoinPaths(execDir, dlcDirectory);

        // Create the DLC directory if it doesn't exist
        if (!Directory.Exists(fullDlcPath))
            Directory.CreateDirectory(fullDlcPath);
    }

    // --------------------------------------------------------------------------------------------

    public static List<string> FindDLCs()
    {
        // Base path for the DLCs
        string execDir = FssFileOperations.StandardizePath(OS.GetExecutablePath().GetBaseDir());
        string fullDlcPath = FssFileOperations.JoinPaths(execDir, dlcDirectory);

        // Get the list of files in the DLC directory
        List<string> dlcFiles = FssFileOperations.Filenames(fullDlcPath);

        // Filter the list to only include files with the .pak extension
        List<string> dlcPaks = FssFileOperations.FilterFilenameSuffix(dlcFiles, ".pak");

        return dlcPaks;
    }

    // --------------------------------------------------------------------------------------------

    // A string of teh available DLCs (and directories) for reporting

    public static string DlcReport()
    {
        List<string> dlcPaks = FindDLCs();
        string dlcReport = "Available DLCs:\n";

        foreach (string dlc in dlcPaks)
        {
            dlcReport += $"- {Path.GetFileName(dlc)}\n";
        }

        return dlcReport;
    }

    // --------------------------------------------------------------------------------------------

    public static void LoadDlc(string dlcPath)
    {
        if (ProjectSettings.LoadResourcePack(dlcPath))
        {
            GD.Print("DLC pack loaded successfully!");
        }
        else
        {
            GD.Print("Failed to load DLC pack.");
        }
    }

    // --------------------------------------------------------------------------------------------

    public static void CreateDlc()
    {


        {
            List<string> DLCsToExport = FssGodotFileOperations.ListDLCPrepDirs();
            GD.Print($"DLCs to export: {DLCsToExport.Count}");
            foreach (string dlc in DLCsToExport)
            {
                GD.Print($"Exporting DLC: {dlc}");
            }
        }

        return;

        // List all potential DLC files, in the res://DLCPrep folder
        {
            List<string> fileList = FssGodotFileOperations.ListFiles(FssGodotFileOperations.DlcPrepDir);

            foreach (string file in fileList)
            {
                GD.Print($"DLC Prep: {file}");
            }

        }

        return;


        {
            // define the PCK Files
            string dlcName = "DLC_001.pck";
            string fullpath = FssFileOperations.JoinPaths(dlcExportFolder, dlcName);

            // Start the packing
            var packer = new PckPacker();
            packer.PckStart(fullpath);

            // Add files to the pack
            string currResPath = "icon2.svg";
            string DLCResPath = "res://DLC/001/icon2.svg";
            var x = packer.AddFile(DLCResPath, currResPath);
            GD.Print($"Added file:{currResPath}  // to {DLCResPath} // return: {x}");

            // Add files to the pack
            currResPath = "cmd.txt";
            DLCResPath = "res://DLC/001/cmd.txt";
            x = packer.AddFile(DLCResPath, currResPath);
            GD.Print($"Added file:{currResPath}  // to {DLCResPath} // return: {x}");

            // Add files to the pack
            currResPath = "inventory.json";
            DLCResPath = "res://DLC/001/inventory.json";
            x = packer.AddFile(DLCResPath, currResPath);
            GD.Print($"Added file:{currResPath}  // to {DLCResPath} // return: {x}");

            // Conclude the adding
            packer.Flush(true);
        }
    }

    public static void LoadDlc()
    {
        {
            string dlcName = "DLC_001.pck";
            string fullpath = FssFileOperations.JoinPaths(dlcExportFolder, dlcName);

            ProjectSettings.LoadResourcePack(fullpath);

            // // Open the text file from the resource pack
            // string filePath = "res://DLCs/cmd.txt";

            // if (Godot.FileAccess.FileExists(filePath))
            //     GD.Print("======= File exists!");
        }

    }

    // FssDlcOperations.DlcInv()
    public static void DlcInv()
    {
        List<Fss3DModelInfo> modelList = new List<Fss3DModelInfo>();

        Fss3DModelInfo info1 = new Fss3DModelInfo() { Name = "Model1", FilePath = "res://DLCs/001/model1.glb", RwAABB = new FssXYZBox() { Height = 10, Depth = 9, Width = 8}, Scale = 1.0f };
        Fss3DModelInfo info2 = new Fss3DModelInfo() { Name = "Model2", FilePath = "res://DLCs/001/model2.glb", RwAABB = new FssXYZBox(), Scale = 10.0f };
        Fss3DModelInfo info3 = new Fss3DModelInfo() { Name = "Model3", FilePath = "res://DLCs/001/model3.glb", RwAABB = new FssXYZBox(), Scale = 0.00001f };

        modelList.Add(info1);
        modelList.Add(info2);
        modelList.Add(info3);

        string JSONString = Fss3DModelLibrary.SerializeJSONConfig(modelList);
        GD.Print(JSONString);
    }

    // --------------------------------------------------------------------------------------------

    public static float VarToFloat(object input)
    {
        if (input is float)
        {
            return (float)input;
        }
        else if (input is string)
        {
            if (float.TryParse((string)input, out float result))
            {
                return result;
            }
        }
        else
        {
            try
            {
                return Convert.ToSingle(input);
            }
            catch (InvalidCastException)
            {
                // Handle the case where input cannot be converted to float
            }
            catch (FormatException)
            {
                // Handle the case where input is a string that doesn't represent a valid float
            }
        }

        // Return -1 if the conversion fails
        return -1;
    }

    // Function to list files in a resource folder (looking for files loaded from a PCK file)

    // --------------------------------------------------------------------------------------------

    private static void DirContents(string path)
    {
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    GD.Print($"Found directory: {fileName}");
                }
                else
                {
                    GD.Print($"Found file: {fileName}");
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print($"An error occurred when trying to access the path: {path}");
        }
    }

    // --------------------------------------------------------------------------------------------

    private static List<string> FindResourceFiles(string subdirPath)
    {
        // Determine the full path to the resource folder
        string resourcePath = FssFileOperations.JoinPaths("res://", subdirPath);

        resourcePath = "./DLCs/";
        GD.Print($"Looking for resources in: {resourcePath}");

        // Our return value
        List<string> resFiles = new List<string>();

        // Open the directory
        var dir = DirAccess.Open(resourcePath);

        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName;

            while ((fileName = dir.GetNext()) != "")
            {
                if (!dir.CurrentIsDir())
                {
                    // Create the path we would need to later access and open the file
                    string filePath = FssFileOperations.JoinPaths(subdirPath, fileName);
                    resFiles.Add(filePath);
                }
            }

            dir.ListDirEnd();
        }
        else
        {
            GD.PrintErr("Failed to open directory: ", subdirPath);
        }

        return resFiles;
    }

}