using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;

// GloCentralLog:
// - A logfile capability available regardless of the Unity / Godot / Dotnet environment the code is ultimately used in.
// - A static class allows us to log messages from within internal functions.
// - Has some basic file and thread protection.

public static class GloCentralLog
{
    // Log storage and access
    private static List<string> LogEntries     = new List<string>();
    private static List<string> DisplayEntries = new List<string>();
    private static readonly object LogLock = new object();

    // Activity flags
    public  static bool LoggingActive { get; set; } = true;
    private static bool IsLogReady = false;

    // output file and control
    private static string runtimeFilename = "invalid.log";
    private static System.Threading.Timer writeTimer;

    static GloCentralLog()
    {
        // Create a timer that triggers write operations every second
        writeTimer = new System.Threading.Timer(WriteLogEntries, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public static void AddEntry(string entry)
    {
        string timestamp = GloCentralTime.TimestampLocal;
        string formattedEntry = $"{timestamp} : {entry}";

        lock (LogLock)
        {
            // Limit the number of entries in the display list
            DisplayEntries.Add(formattedEntry);
            while (DisplayEntries.Count > 200) // Maintain latest 200 entries
                DisplayEntries.RemoveAt(0);

            if (!LoggingActive)
                return;

            LogEntries.Add(formattedEntry);
        }
    }

    private static void WriteLogEntries(object state)
    {
        if (!IsLogReady || !LoggingActive)
            return;

        List<string> entriesToWrite;
        lock (LogLock)
        {
            if (LogEntries.Count == 0)
                return;

            entriesToWrite = new List<string>(LogEntries);
            LogEntries.Clear();
        }

        try
        {
            File.AppendAllLines(runtimeFilename, entriesToWrite);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            // Add entries back to the queue
            lock (LogLock)
            {
                LogEntries.InsertRange(0, entriesToWrite);
            }
        }
    }

    public static void UpdatePath(string newPath)
    {
        lock (LogLock)
        {
            runtimeFilename = Path.Combine(newPath, $"{GloCentralTime.TimestampLocal}.log");
            IsLogReady = true;
        }
    }

    public static void Shutdown()
    {
        writeTimer?.Dispose();
        WriteLogEntries(null); // Final write
    }

    public static List<string> GetLatestLines()
    {
        lock (LogLock)
        {
            return new List<string>(DisplayEntries);
        }
    }
}