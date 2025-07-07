
using System;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    public void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        //GloAppFactory.Instance.MapIOManager.SetRootDir(rootDir);

        GloCentralLog.AddEntry($"SetRootDir: {rootDir}");
    }

    public string ReportRootDir()
    {
        return "undefined"; // GloAppFactory.Instance.MapIOManager.ReportRootDir();
    }

    // ---------------------------------------------------------------------------------------------

    public void CreateBaseDirectories()
    {
        // string rootDir = "undefined"; // GloAppFactory.Instance.MapIOManager.ReportRootDir();

        // // Validity Checks - Map, RootDir, RootDir Exists
        // if (string.IsNullOrEmpty(rootDir)) return;
        // if (!System.IO.Directory.Exists(rootDir)) return;

        // GloMapOperations.CreateBaseDirectories(rootDir);
    }

}