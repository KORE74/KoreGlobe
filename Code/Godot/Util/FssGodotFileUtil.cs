
using Godot;
using System.IO;


public static class FssGodotFileUtil
{
    // Usage: string filePath = FssGodotFileUtil.GetActualPath("res://assets/earth/earth.jpg");

    public static string GetActualPath(string resourcePath)
    {
        // Convert the resource path to an absolute path
        string absolutePath = ProjectSettings.GlobalizePath(resourcePath);

        // Optionally check if the file exists at the absolute path
        if (File.Exists(absolutePath))
        {
            GD.Print("File exists at the absolute path.");
        }
        else
        {
            GD.Print("File does not exist at the absolute path.");
        }

        return absolutePath;
    }
}