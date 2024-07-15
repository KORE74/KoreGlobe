
using System;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Position
    // ---------------------------------------------------------------------------------------------

    public void AddPlatform(string platName)
    {
        // Create a new platform
        // if (FssAppFactory.Instance.PlatformManager == null)
        //     FssCentralLog.AddEntry("E00002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            FssCentralLog.AddEntry($"E00001: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = "Unknown";
    }

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        // if (FssAppFactory.Instance.PlatformManager == null)
        //     FssCentralLog.AddEntry("E00002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            FssCentralLog.AddEntry($"E00001: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = platType;
    }

    public void SetPlatformStartLLA(string platName, FssLLAPoint newpos)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: Platform {platName} not found.");
            return;
        }

        // Set the platform's start location
        platform.Kinetics.StartPosition = newpos;
    }

    public FssLLAPoint? PlatformCurrLLA(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.StartPosition;
    }


    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Course
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformCourse(string platName, FssCourse course)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00004: Platform {platName} not found.");
            return;
        }

        // Set the platform's course
        platform.Kinetics.CurrCourse = course;
    }

    public FssCourse? PlatformCurrCourse(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrCourse;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Course Delta
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformCourseDelta(string platName, FssCourseDelta courseDelta)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00005: Platform {platName} not found.");
            return;
        }

        // Set the platform's course delta
        platform.Kinetics.CurrCourseDelta = courseDelta;
    }

    public FssCourseDelta? PlatformCurrCourseDelta(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrCourseDelta;
    }


    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Management
    // ---------------------------------------------------------------------------------------------

    public bool DoesPlatformExist(string platName) => FssAppFactory.Instance.PlatformManager.DoesPlatformExist(platName);

    public void DeletePlatform(string platName) => FssAppFactory.Instance.PlatformManager.Delete(platName);

    public void DeleteAllPlatforms() => FssAppFactory.Instance.PlatformManager.DeleteAllPlatforms();

    public int NumPlatforms() => FssAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Report
    // ---------------------------------------------------------------------------------------------

    public string PlatformReport()
    {
        string report = "Platform Report\n";

        // Loop through teh platforms by index
        int totalPlatforms = NumPlatforms();
        for (int i = 0; i < totalPlatforms; i++)
        {
            FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForIndex(i);
            if (platform == null)
                continue;

            report += $"Platform{i}: {platform.Name}\n";
            report += $"  Type: {platform.Type}\n";
            report += $"  Start Position: {platform.Kinetics.StartPosition}\n";
            report += $"  Course: {platform.Kinetics.CurrCourse}\n";
        }

        return report;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Names
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
        FssCentralLog.AddEntry("Creating Test Platform Entities");

        FssLLAPoint loc1 = new FssLLAPoint() { LatDegs = 52.8, LonDegs = -4.2, AltMslM = 1000 };
        FssLLAPoint loc2 = new FssLLAPoint() { LatDegs = 52.9, LonDegs = -4.3, AltMslM = 2000 };
        FssLLAPoint loc3 = new FssLLAPoint() { LatDegs = 52.7, LonDegs = -4.1, AltMslM = 3000 };

        FssCourse course1 = new FssCourse() { HeadingDegs = 90,  SpeedKph = 1000 };
        FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 2000 };
        FssCourse course3 = new FssCourse() { HeadingDegs = 270, SpeedKph = 30000 };

        AddPlatform("TEST-001", "F16");
        SetPlatformStartLLA("TEST-001", loc1);
        SetPlatformCourse("TEST-001", course1);

        AddPlatform("TEST-002", "F18");
        SetPlatformStartLLA("TEST-002", loc2);
        SetPlatformCourse("TEST-002", course2);

        AddPlatform("TEST-003", "Tornado");
        SetPlatformStartLLA("TEST-003", loc3);
        SetPlatformCourse("TEST-003", course3);
        SetPlatformCourseDelta("TEST-003", new FssCourseDelta() { HeadingChangeClockwiseDegsSec = 10, SpeedChangeMpMps = 0 });
    }

    // ---------------------------------------------------------------------------------------------

}


