
using Godot;
using System.Collections.Generic;
using System.IO;


// Class to perform file operations on the Godot virtual file system under "res://".
// - List files in directories, supplying lists of paths that can then be directly accessed in other calls.
// - One intended use is finding files in DLCs after a PCK file has been mounted.

public static class FssGodotFileOperations
{
    public static string RootDir    = "res://";
    public static string DlcLoadDir = "res://Resources/DLC/"; // FssGodotFileOperations.DlcLoadDir
    public static string DlcPrepDir = "res://Resources/DLCPrep/"; // FssGodotFileOperations.DlcPrepDir

    public enum ListContent { Files, Directories, Both };

    // Usage: string filePath = FssGodotFileUtil.GetActualPath("res://assets/earth/earth.jpg");

    public static string GetActualPath(string resourcePath)
    {
        // Convert the resource path to an absolute path
        string absolutePath = ProjectSettings.GlobalizePath(resourcePath);

        return absolutePath;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File & Dir Lists
    // --------------------------------------------------------------------------------------------

    // List all the files, in the godot virtual file system, under a given top level directory.

    // Usage: List<string> fileList = FssGodotFileUtil.ListFiles("res://assets/earth");
    public static List<string> ListFiles(string topLevel, ListContent content = ListContent.Files)
    {
        List<string> fileList = new List<string>();

        GD.Print($"Listing contents of directory: {topLevel}");

        using var dir = DirAccess.Open(topLevel);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    if (content == ListContent.Directories || content == ListContent.Both)
                        fileList.Add(JoinPaths(topLevel, fileName));

                    List<string> subList = ListFiles(JoinPaths(topLevel, fileName), content);
                    fileList.AddRange(subList);
                }
                else
                {
                    if (content == ListContent.Files || content == ListContent.Both)
                        fileList.Add(JoinPaths(topLevel, fileName));
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.Print("An error occurred when trying to access the path.");
        }

        return fileList;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: DLC Specific
    // --------------------------------------------------------------------------------------------

    public static List<string> ListLoadedDLCs()
    {
        return ListFiles(DlcLoadDir, ListContent.Directories);
    }

    public static List<string> ListDLCFiles(string dlcRootPath)
    {
        return ListFiles(dlcRootPath);
    }

    // --------------------------------------------------------------------------------------------

    public static List<string> ListDLCPrepDirs()
    {
        return ListFiles(DlcPrepDir, ListContent.Directories);
    }

    public static List<string> ListDLCPrepFiles()
    {
        return ListFiles(DlcPrepDir, ListContent.Files);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Utilities
    // --------------------------------------------------------------------------------------------

    public static string JoinPaths(string path1, string path2)
    {
        if (path1.EndsWith("/"))
            path1 = path1.Substring(0, path1.Length - 1);

        if (path2.StartsWith("/"))
            path2 = path2.Substring(1);

        return $"{path1}/{path2}";
    }

    // Usage: string fileName = FssGodotFileOperations.LastPathElement("res://assets/earth/earth.jpg");
    public static string LastPathElement(string path)
    {
        string[] parts = path.Split("/");
        return parts[parts.Length - 1];
    }
}