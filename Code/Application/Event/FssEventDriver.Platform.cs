
using System;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Position
    // ---------------------------------------------------------------------------------------------

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        // if (FssAppFactory.Instance.PlatformManager == null)
        //     FssCentralLog.AddEntry("E00002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            FssCentralLog.AddEntry($"E00001: Platform {platName} not created, already exists.");
        }
        return;

        newPlat.Type = platType;
    }

    public void SetPlatformStartLLA(string platName, FssLLALocation loc)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: Platform {platName} not found.");
            return;
        }

        // Set the platform's start location
        platform.Kinetics.StartPosition = loc.ToLLA();
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
    // #MARK: Platform Management
    // ---------------------------------------------------------------------------------------------

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
        FssCentralLog.AddEntry("Creating FssAppFactory objects");

        FssLLALocation loc1 = new FssLLALocation() { LatDegs = 52.8, LonDegs = -4.2, HeightM = 1000 };
        FssLLALocation loc2 = new FssLLALocation() { LatDegs = 52.9, LonDegs = -4.3, HeightM = 2000 };
        FssLLALocation loc3 = new FssLLALocation() { LatDegs = 52.7, LonDegs = -4.1, HeightM = 3000 };

        FssCourse course1 = new FssCourse() { HeadingDegs = 90,  SpeedKph = 1000 };
        FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 2000 };
        FssCourse course3 = new FssCourse() { HeadingDegs = 270, SpeedKph = 3000 };

        AddPlatform("TEST-001", "F16");
        SetPlatformStartLLA("TEST-001", loc1);
        SetPlatformCourse("TEST-001", course1);

        AddPlatform("TEST-002", "F18");
        SetPlatformStartLLA("TEST-002", loc2);
        SetPlatformCourse("TEST-002", course2);

        AddPlatform("TEST-003", "Tornado");
        SetPlatformStartLLA("TEST-003", loc3);
        SetPlatformCourse("TEST-003", course3);
    }

    // ---------------------------------------------------------------------------------------------

}


