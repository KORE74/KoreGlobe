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
    private static string runtimeFilename { get; set; } = $"{FssCoreTime.TimestampLocal}.log";
    private static List<string> logEntries = new List<string>();
    private static readonly object lockObject = new object();
    public static bool LoggingActive { set; get; } = true;

    static FssCentralLog()
    {
    }

    public static void AddStartupEntry(string versionStr)
    {
        AddEntry($"Startup: Version {versionStr}");
    }

    public static void UpdatePath(string newPath)
    {
        runtimeFilename = FssFileOperations.JoinPaths(newPath, $"{FssCoreTime.TimestampLocal}.log");
    }

    public static void AddEntry(string entry)
    {
        if (!LoggingActive)
            return;

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

    // --------------------------------------------------------------------------------------------------------------------------
    // MARK: Internal Access of logged lines
    // --------------------------------------------------------------------------------------------------------------------------

    public static List<string> GetLatestLines()
    {
        List<string> result = new ();

        lock (lockObject)
        {
            foreach (string line in logEntries)
            {
                result.Add(line);
            }
            logEntries.Clear();

        }

        return result;
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // Private functions
    // --------------------------------------------------------------------------------------------------------------------------

    private static void AppendToFile(string fileName, string entry)
    {
        if (!LoggingActive)
            return;

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
