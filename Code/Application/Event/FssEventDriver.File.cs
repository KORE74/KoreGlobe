
using System;

using FssNetworking;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    public void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        FssAppFactory.Instance.MapIOManager.SetRootDir(rootDir);

        FssCentralLog.AddEntry($"SetRootDir: {rootDir}");
    }

    public string ReportRootDir()
    {
        return FssAppFactory.Instance.MapIOManager.ReportRootDir();
    }

    // ---------------------------------------------------------------------------------------------

    public void CreateBaseDirectories()
    {
        string rootDir = FssAppFactory.Instance.MapIOManager.ReportRootDir();

        // Validity Checks - Map, RootDir, RootDir Exists
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        FssMapOperations.CreateBaseDirectories(rootDir);
    }

}