
using System;

using FssNetworking;

// Design Decisions:
// - The GlobeEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GlobeEventDriver
{
    public void SetRootDir(string rootDir)
    {
        // Check the root dir is valid
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        GlobeAppFactory.Instance.MapIOManager.SetRootDir(rootDir);

        GlobeCentralLog.AddEntry($"SetRootDir: {rootDir}");
    }

    public string ReportRootDir()
    {
        return GlobeAppFactory.Instance.MapIOManager.ReportRootDir();
    }

    // ---------------------------------------------------------------------------------------------

    public void CreateBaseDirectories()
    {
        string rootDir = GlobeAppFactory.Instance.MapIOManager.ReportRootDir();

        // Validity Checks - Map, RootDir, RootDir Exists
        if (string.IsNullOrEmpty(rootDir)) return;
        if (!System.IO.Directory.Exists(rootDir)) return;

        GlobeMapOperations.CreateBaseDirectories(rootDir);
    }

}