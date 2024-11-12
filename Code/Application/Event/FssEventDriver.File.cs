
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class FssEventDriver
{
    public static void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        //FssAppFactory.Instance.MapIOManager.SetRootDir(rootDir);

        FssCentralLog.AddEntry($"SetRootDir: {rootDir}");
    }

    public static string ReportRootDir()
    {
        return "undefined"; // FssAppFactory.Instance.MapIOManager.ReportRootDir();
    }

    // ---------------------------------------------------------------------------------------------

    public static void CreateBaseDirectories()
    {
        // string rootDir = "undefined"; // FssAppFactory.Instance.MapIOManager.ReportRootDir();

        // // Validity Checks - Map, RootDir, RootDir Exists
        // if (string.IsNullOrEmpty(rootDir)) return;
        // if (!System.IO.Directory.Exists(rootDir)) return;

        // FssMapOperations.CreateBaseDirectories(rootDir);
    }

}