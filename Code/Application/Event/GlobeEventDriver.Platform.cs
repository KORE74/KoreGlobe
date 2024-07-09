
using System;

using FssNetworking;

using UnityEngine;

#nullable enable

// Design Decisions:
// - The GlobeEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GlobeEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        if (GlobeAppFactory.Instance.PlatformManager == null)
            Debug.LogError("E00002: ERROR ERROR ERROR: Platform Manager not found in GlobeAppFactory.Instance");

        GlobePlatform? newPlat = GlobeAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            GlobeCentralLog.AddEntry($"E00001: Platform {platName} not created, already exists.");
        }
            return;

        newPlat.Type = platType;
    }

    public void SetPlatformStartLLA(string platName, GlobeLLALocation loc)
    {
        // Get the platform
        GlobePlatform? platform = GlobeAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        // Set the platform's start location
        platform.Kinetics.StartPosition = loc.ToLLA();
    }

    public GlobeLLAPoint PlatformCurrLLA(string platName)
    {
        // Get the platform
        GlobePlatform? platform = GlobeAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return new GlobeLLAPoint();

        return platform.Kinetics.StartPosition;
    }




    public void DeletePlatform(string platName) => GlobeAppFactory.Instance.PlatformManager.Delete(platName);

    public void DeleteAllPlatforms() => GlobeAppFactory.Instance.PlatformManager.DeleteAllPlatforms();

    public int NumPlatforms() => GlobeAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // Platform Names
    // ---------------------------------------------------------------------------------------------

    public string PlatformNameForIndex(int index) => GlobeAppFactory.Instance.PlatformManager.PlatNameForIndex(index);
    public GlobePlatform? PlatformForIndex(int index) => GlobeAppFactory.Instance.PlatformManager.PlatForIndex(index);

    // Id being the 1-based user presented index

    public string PlatformIdForName(string platname) => GlobeAppFactory.Instance.PlatformManager.PlatIdForName(platname);
    public string PlatformNameForId(int platId) => GlobeAppFactory.Instance.PlatformManager.PlatNameForId(platId);

    public int PlatformIdNext(int currPlatId) =>  GlobeAppFactory.Instance.PlatformManager.PlatIdNext(currPlatId);
    public int PlatformIdPrev(int currPlatId) =>  GlobeAppFactory.Instance.PlatformManager.PlatIdPrev(currPlatId);


    // ---------------------------------------------------------------------------------------------

    public void SetupTestPlatforms()
    {
        GlobeCentralLog.AddEntry("Creating GlobeAppFactory objects");

        AddPlatform("TEST-001", "F16");
        SetPlatformStartLLA("TEST-001", new GlobeLLALocation(52.8, -4.2, 1000));

        AddPlatform("TEST-002", "F18");
        SetPlatformStartLLA("TEST-002", new GlobeLLALocation(52.9, -4.3, 2000));

        AddPlatform("TEST-003", "Tornado");
        SetPlatformStartLLA("TEST-003", new GlobeLLALocation(52.7, -4.1, 3000));
    }

    // ---------------------------------------------------------------------------------------------

}


