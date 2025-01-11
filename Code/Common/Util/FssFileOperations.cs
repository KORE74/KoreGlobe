
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class FssFileOperations
{
    // --------------------------------------------------------------------------------------------

    // Function to standardize a path, changing any backslash characters to a "/".

    // Usage example: FssFileOperations.StandardizePath("C:\\Users\\User\\Documents\\file.txt");
    public static string StandardizePath(string inpath)
    {
        // First, replace all backslashes with forward slashes.
        string standardizedPath = inpath.Replace('\\', '/');

        // Then, ensure we do not turn protocol separators like "http://" into "http:/" by only replacing double slashes if they are not following ":".
        int protocolSeparatorIndex = standardizedPath.IndexOf("://");
        if (protocolSeparatorIndex != -1)
        {
            // Process the part before the protocol separator.
            string beforeProtocol = standardizedPath.Substring(0, protocolSeparatorIndex).Replace("//", "/");
            // Combine the processed part with the unmodified remainder of the path.
            standardizedPath = beforeProtocol + standardizedPath.Substring(protocolSeparatorIndex);
        }
        else
        {
            // If there's no protocol separator, just replace double slashes.
            standardizedPath = standardizedPath.Replace("//", "/");
        }

        return standardizedPath;
    }

    // --------------------------------------------------------------------------------------------

    // List all the files under a given top level directory.
    public static List<string> Filenames(string startPath)
    {
        List<string> filenames = new List<string>();
        string normalizedStartPath = StandardizePath(startPath);

        try
        {
            string[] files = Directory.GetFiles(normalizedStartPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                filenames.Add(StandardizePath(file));
            }
        }
        catch //(Exception ex)
        {
            // Console.WriteLine($"Error retrieving files: {ex.Message}"); // Can't write to console, this has to be usable in any framework (Unity, Godot, Command line etc).
        }

        return filenames;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Filtering
    // --------------------------------------------------------------------------------------------

    public static List<string> FilterFilenameList(List<string> filenames, string substring)
    {
        return filenames.Where(filename => filename.Contains(substring)).ToList();
    }

    // Filter where the string starts or ends with a specific string

    // Usage example: FssFileOperations.FilterFilenameSuffix(filenames, ".json");

    public static List<string> FilterFilenameSuffix(List<string> filenames, string suffixsubstring)
    {
        return filenames.Where(filename => filename.EndsWith(suffixsubstring)).ToList();
    }

    public static List<string> FilterFilenamePrefix(List<string> filenames, string prefixsubstring)
    {
        return filenames.Where(filename => filename.StartsWith(prefixsubstring)).ToList();
    }

    // --------------------------------------------------------------------------------------------

    public static List<string> RemovePrefix(List<string> filenames, string prefix)
    {
        return filenames.Select(filename => filename.StartsWith(prefix) ? filename.Substring(prefix.Length) : filename).ToList();
    }

    public static List<string> RemoveSuffix(List<string> filenames, string suffix)
    {
        return filenames.Select(filename => filename.EndsWith(suffix) ? filename.Substring(0, filename.Length - suffix.Length) : filename).ToList();
    }

    // --------------------------------------------------------------------------------------------

    public static List<string> OrderAlphabetically(List<string> filenames)
    {
        return filenames.OrderBy(filename => filename).ToList();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Directories
    // --------------------------------------------------------------------------------------------

    public static string DirectoryForPath(string path)
    {
        string normalizedPath = StandardizePath(path);
        return Path.GetDirectoryName(normalizedPath);
    }

    // Create a directory if it doesn't exist.

    // Usage example: FssFileOperations.CreateDirectoryForPath("C:/Users/User/Documents");

    public static void CreateDirectoryForPath(string path)
    {
        string normalizedPath = StandardizePath(path);
        string directory      = DirectoryForPath(normalizedPath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Joining Paths
    // --------------------------------------------------------------------------------------------

    // The standard Path.Combinbe can introduce a backslash "\" character.
    // This version with will:
    // - check for a backslah on the end of first or begoinning of the second path, removing it.
    // - Join the to paths toegether with a forward slash.
    //
    // Usage example: FssFileOperations.JoinPaths("C:/Users/User/Documents", "file.txt");
    //
    public static string JoinPaths(string path1, string path2)
    {
        string normalizedPath1 = StandardizePath(path1);
        string normalizedPath2 = StandardizePath(path2);

        if (normalizedPath1.EndsWith("/"))
            normalizedPath1 = normalizedPath1.Substring(0, normalizedPath1.Length - 1);

        if (normalizedPath2.StartsWith("/"))
            normalizedPath2 = normalizedPath2.Substring(1);

        return $"{normalizedPath1}/{normalizedPath2}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: File Extension
    // --------------------------------------------------------------------------------------------

    // Check the filename string has the right extension, or add it if it doesn't.

    // Usage example: FssFileOperations.EnsureExtension("file.txt", ".json");
    public static string EnsureExtension(string filename, string extension)
    {
        if (filename.EndsWith(extension))
            return filename;
        else
            return $"{filename}{extension}";
    }

}

