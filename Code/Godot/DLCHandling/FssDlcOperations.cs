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

    public static void CreateDlc(string dlcPath)
    {
        // if (ProjectSettings.SaveResourcePack(dlcPath))
        // {
        //     GD.Print("DLC pack created successfully!");
        // }
        // else
        // {
        //     GD.Print("Failed to create DLC pack.");
        // }
    }

}