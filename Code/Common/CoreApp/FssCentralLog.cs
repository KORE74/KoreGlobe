using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

// FssCentralLog:
// - A logfile capability available regardless of the Unity / Godot / Dotnet environment the code is ultimately used in.
// - A static class allows us to log messages from within internal functions.
// - Has some basic file and thread protection.

public static class FssCentralLog
{
    private static string runtimeFilename { get; } = $"{FssCoreTime.TimestampLocal}.log";
    private static List<string> logEntries = new List<string>();
    private static readonly object lockObject = new object();

    static FssCentralLog()
    {
    }

    public static void AddStartupEntry(string versionStr)
    {
        AddEntry($"Startup: Version {versionStr}");
    }

    public static void AddEntry(string entry)
    {
        // Try to lock the exclusive access to write our log, wait a maximum of 100ms before abandoning the log.
        if (Monitor.TryEnter(lockObject, TimeSpan.FromMilliseconds(100)))
        {
            try
            {
                string timestamp = FssCoreTime.TimestampLocal;
                logEntries.Add($"{timestamp} : {entry}");

                if (logEntries.Count > 100)
                    logEntries.RemoveAt(0);

                AppendToFile(runtimeFilename, $"{timestamp} : {entry}");
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
        }
    }

    public static string GetLatestLines(int numLines)
    {
        int startIndex = Math.Max(0, logEntries.Count - numLines);
        return string.Join("\n", logEntries.GetRange(startIndex, logEntries.Count - startIndex));
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // Private functions
    // --------------------------------------------------------------------------------------------------------------------------

    private static void AppendToFile(string fileName, string entry)
    {
        try
        {
            File.AppendAllText(fileName, entry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // Alternative logging, e.g., to console or a different file
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            // Optionally, write to a different file or system log
        }
    }
}
