using System;
using System.IO;
using System.Collections.Generic;

using Godot;

// static class to handling generic functionality around finding and importing DLCs

public static class FssDlcOperations
{
    private static string dlcDirectory = "DLC";

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
        // var width  = ProjectSettings.GetSetting("display/window/size/width");
        // var height = ProjectSettings.GetSetting("display/window/size/height");

        // GD.Print($"Width: {width} // Height: {height}");

        // // convert (or check) the var to a float and then vector2
        // float viewport_width  = VarToFloat(width);
        // float viewport_height = VarToFloat(height);

        // Vector2 viewport_start_size = new Vector2(viewport_width, viewport_height);

        // GD.Print($"Viewport start size: {viewport_start_size}");


        {
            var packer = new PckPacker();
            packer.PckStart("test.pck");
            packer.AddFile("cmd.txt", "DLCs/cmd2.txt");
            packer.AddFile("icon2.svg", "DLCs/icon2.svg");
            packer.Flush(true);
        }


        {
            ProjectSettings.LoadResourcePack("res://PCK002.zip");

        }

        {

            DirContents("res://");



            // List<string> files = FindResourceFiles("DLCs");

            // foreach (string file in files)
            // {
            //     GD.Print($"Found file: {file}");
            // }



            // string[] loadedPacks = ProjectSettings.GetResourcePackList();
            // foreach (string pack in loadedPacks)
            // {
            //     GD.Print($"Loaded pack: {pack}");
            // }
        }

        // var viewport_start_size = new Vector2(
        //     ProjectSettings.GetSetting("display/window/size/viewport_width"),
        //     ProjectSettings.GetSetting("display/window/size/viewport_height")
        // );


        // ProjectSettings
        // if (ProjectSettings.SaveResourcePack(dlcPath))
        // {
        //     GD.Print("DLC pack created successfully!");
        // }
        // else
        // {
        //     GD.Print("Failed to create DLC pack.");
        // }
    }


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
            GD.Print("An error occurred when trying to access the path.");
        }
    }

    private static List<string> FindResourceFiles(string subdirPath)
    {
        // Determine the full path to the resource folder
        string resourcePath = FssFileOperations.JoinPaths("res://", subdirPath);

        resourcePath = "res://DLCs/";
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