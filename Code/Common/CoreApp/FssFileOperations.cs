
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class FssFileOperations
{
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

    public static List<string> FilterFilenameList(List<string> filenames, string substring)
    {
        return filenames.Where(filename => filename.Contains(substring)).ToList();
    }

    public static List<string> OrderAlphabetically(List<string> filenames)
    {
        return filenames.OrderBy(filename => filename).ToList();
    }
}

