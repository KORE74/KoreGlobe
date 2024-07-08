using System;
using System.Diagnostics;

// FssCoreTime - A standard C# class to provide a runtime timer for use throughout the class framework.

public static class FssCoreTime
{
    private static readonly Stopwatch stopwatch = new Stopwatch();

    static FssCoreTime()
    {
        // Start the stopwatch when the class is first loaded
        stopwatch.Start();
    }

    // Property to get the elapsed time in seconds since the application started
    public static float RuntimeSecs           => (float)stopwatch.Elapsed.TotalSeconds;

    // UTC Time and Data
    public static string TimeOfDayHHMMSSUTC   => DateTime.UtcNow.ToString("HHmmss");
    public static string DateYYYYMMDDUTC      => DateTime.UtcNow.ToString("yyyyMMdd");
    public static string TimestampUTC         => $"{DateYYYYMMDDUTC}-{TimeOfDayHHMMSSUTC}";

    // Local Time and Date
    public static string TimeOfDayHHMMSSLocal => DateTime.Now.ToString("HHmmss");
    public static string DateYYYYMMDDLocal    => DateTime.Now.ToString("yyyyMMdd");
    public static string TimestampLocal       => $"{DateYYYYMMDDLocal}-{TimeOfDayHHMMSSLocal}";
}
