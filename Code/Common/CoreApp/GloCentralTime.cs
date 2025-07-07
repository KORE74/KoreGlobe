using System;
using System.Diagnostics;

// GloCentralTime - A standard C# class to provide a runtime timer for use throughout the class framework.

public static class GloCentralTime
{
    private static readonly Stopwatch stopwatch = new Stopwatch();

    static GloCentralTime()
    {
        // Start the stopwatch when the class is first loaded
        stopwatch.Start();
    }

    // Property to get the elapsed time in seconds since the application started
    public static float RuntimeSecs           => (float)stopwatch.Elapsed.TotalSeconds;
    public static int   RuntimeIntSecs        => (int)stopwatch.Elapsed.TotalSeconds; // Usage: GloCentralTime.RuntimeIntSecs

    // UTC Time and Data
    public static string TimeOfDayHHMMSSUTC   => DateTime.UtcNow.ToString("HHmmss");
    public static string DateYYYYMMDDUTC      => DateTime.UtcNow.ToString("yyyyMMdd");
    public static string TimestampUTC         => $"{DateYYYYMMDDUTC}-{TimeOfDayHHMMSSUTC}";

    // Local Time and Date
    public static string TimeOfDayHHMMSSLocal => DateTime.Now.ToString("HHmmss");
    public static string DateYYYYMMDDLocal    => DateTime.Now.ToString("yyyyMMdd");
    public static string TimestampLocal       => $"{DateYYYYMMDDLocal}-{TimeOfDayHHMMSSLocal}";
}
