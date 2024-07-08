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
    public static float RuntimeSecs
    {
        get
        {
            // Return the elapsed time in seconds
            return (float)stopwatch.Elapsed.TotalSeconds;
        }
    }
}

