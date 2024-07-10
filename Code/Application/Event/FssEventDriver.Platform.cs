
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        // if (FssAppFactory.Instance.PlatformManager == null)
        //     FssCentralLog.AddEntry("E00002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        // FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        // if (newPlat == null)
        // {
        //     FssCentralLog.AddEntry($"E00001: Platform {platName} not created, already exists.");
        // }
        // return;

        // newPlat.Type = platType;
    }

    public void SetPlatformStartLLA(string platName, FssLLALocation loc)
    {
        // // Get the platform
        // FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        // if (platform == null)
        //     return;

        // // Set the platform's start location
        // platform.Kinetics.StartPosition = loc.ToLLA();
    }

    public FssLLAPoint PlatformCurrLLA(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return new FssLLAPoint();

        return platform.Kinetics.StartPosition;
    }




    public void DeletePlatform(string platName) => FssAppFactory.Instance.PlatformManager.Delete(platName);

    public void DeleteAllPlatforms() => FssAppFactory.Instance.PlatformManager.DeleteAllPlatforms();

    public int NumPlatforms() => FssAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // Platform Names
    // ---------------------------------------------------------------------------------------------

    public string PlatformNameForIndex(int index) => FssAppFactory.Instance.PlatformManager.PlatNameForIndex(index);
    public FssPlatform? PlatformForIndex(int index) => FssAppFactory.Instance.PlatformManager.PlatForIndex(index);

    // Id being the 1-based user presented index

    public string PlatformIdForName(string platname) => FssAppFactory.Instance.PlatformManager.PlatIdForName(platname);
    public string PlatformNameForId(int platId) => FssAppFactory.Instance.PlatformManager.PlatNameForId(platId);

    public int PlatformIdNext(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdNext(currPlatId);
    public int PlatformIdPrev(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdPrev(currPlatId);


    // ---------------------------------------------------------------------------------------------

    public void SetupTestPlatforms()
    {
        FssCentralLog.AddEntry("Creating FssAppFactory objects");

        AddPlatform("TEST-001", "F16");
        SetPlatformStartLLA("TEST-001", new FssLLALocation(52.8, -4.2, 1000));

        AddPlatform("TEST-002", "F18");
        SetPlatformStartLLA("TEST-002", new FssLLALocation(52.9, -4.3, 2000));

        AddPlatform("TEST-003", "Tornado");
        SetPlatformStartLLA("TEST-003", new FssLLALocation(52.7, -4.1, 3000));
    }

    // ---------------------------------------------------------------------------------------------

}


