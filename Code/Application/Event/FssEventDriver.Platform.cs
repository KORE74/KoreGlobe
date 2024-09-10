
using System;
using System.Collections.Generic;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Management
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

    public bool DoesPlatformExist(string platName) => FssAppFactory.Instance.PlatformManager.DoesPlatExist(platName);

    public void DeletePlatform(string platName) => FssAppFactory.Instance.PlatformManager.Delete(platName);

    public void DeleteAllPlatforms() => FssAppFactory.Instance.PlatformManager.DeleteAllPlatforms();

    public int NumPlatforms() => FssAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Full Details
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformDetails(string platName, FssLLAPoint startPos, FssLLAPoint currPos, FssAttitude att, FssCourse course, FssCourseDelta courseDelta)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: Platform {platName} not found.");
            return;
        }

        // Set the platform's details
        platform.Kinetics.StartPosition   = startPos;
        platform.Kinetics.CurrPosition    = currPos;
        platform.Kinetics.CurrAttitude    = att;
        platform.Kinetics.CurrCourse      = course;
        platform.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Position
    // ---------------------------------------------------------------------------------------------

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

    public FssLLAPoint? PlatformStartLLA(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.StartPosition;
    }

    // ---------------------------------------------------------------------------------------------

    public void SetPlatformPosition(string platName, FssLLAPoint newpos)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: Platform {platName} not found.");
            return;
        }

        // Set the platform's position
        platform.Kinetics.CurrPosition = newpos;
    }

    public FssLLAPoint? GetPlatformPosition(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrPosition;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Type
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformType(string platName, string platType)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00004: Platform {platName} not found.");
            return;
        }

        // Set the platform's type
        platform.Type = platType;
    }

    public string? PlatformType(string platName) =>
        FssAppFactory.Instance.PlatformManager.PlatForName(platName)?.Type;

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Attitude
    // ---------------------------------------------------------------------------------------------

    public FssAttitude? GetPlatformAttitude(string platName)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrAttitude;
    }

    public void SetPlatformAttitude(string platName, FssAttitude newatt)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: Platform {platName} not found.");
            return;
        }

        // Set the platform's attitude
        platform.Kinetics.CurrAttitude = newatt;
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

    public FssCourse? PlatformCurrCourse(string platName) =>
        FssAppFactory.Instance.PlatformManager.PlatForName(platName)?.Kinetics.CurrCourse;

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



    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Report
    // ---------------------------------------------------------------------------------------------

    public string PlatformPositionsReport()
    {
        return FssAppFactory.Instance.PlatformManager.PlatformPositionsReport();
    }

    public string PlatformElementsReport()
    {
        return FssAppFactory.Instance.PlatformManager.PlatformElementsReport();
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Names
    // ---------------------------------------------------------------------------------------------

    public string PlatformNameForIndex(int index) => FssAppFactory.Instance.PlatformManager.PlatNameForIndex(index);
    public FssPlatform? PlatformForIndex(int index) => FssAppFactory.Instance.PlatformManager.PlatForIndex(index);
    public FssPlatform? PlatformForName(string platname) => FssAppFactory.Instance.PlatformManager.PlatForName(platname);

    // Id being the 1-based user presented index

    public string PlatformIdForName(string platname) => FssAppFactory.Instance.PlatformManager.PlatIdForName(platname);
    public string PlatformNameForId(int platId) => FssAppFactory.Instance.PlatformManager.PlatNameForId(platId);

    public int PlatformIdNext(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdNext(currPlatId);
    public int PlatformIdPrev(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdPrev(currPlatId);

    public List<string> PlatformNames() => FssAppFactory.Instance.PlatformManager.PlatNameList();

    // ---------------------------------------------------------------------------------------------

    public void SetupTestPlatforms()
    {
        FssCentralLog.AddEntry("Creating Test Platform Entities");

        // FssLLAPoint loc1 = new FssLLAPoint() { LatDegs = 52.8, LonDegs = -4.2, AltMslM = 1000 };
        // FssLLAPoint loc2 = new FssLLAPoint() { LatDegs = 52.9, LonDegs =  0.3, AltMslM = 2000 };
        // FssLLAPoint loc3 = new FssLLAPoint() { LatDegs = 52.7, LonDegs =  8.1, AltMslM = 3000 };

        FssLLAPoint loc1 = new FssLLAPoint() { LatDegs = 52.8, LonDegs = -4.2, RadiusM = 10.1 };
        FssLLAPoint loc2 = new FssLLAPoint() { LatDegs = 52.9, LonDegs =  0.3, RadiusM = 10.2 };
        FssLLAPoint loc3 = new FssLLAPoint() { LatDegs = 52.7, LonDegs =  8.1, RadiusM = 10.15 };

        // FssCourse course1 = new FssCourse() { HeadingDegs = 270, SpeedKph = 15000 };
        // FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 20000 };
        // FssCourse course3 = new FssCourse() { HeadingDegs =  90, SpeedKph = 30000 };

        FssCourse course1 = new FssCourse() { HeadingDegs = 270, SpeedKph = 0.08 };
        FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 0.08 };
        FssCourse course3 = new FssCourse() { HeadingDegs =  90, SpeedKph = 0.08 };


        AddPlatform("TEST-001", "F16");
        SetPlatformStartLLA("TEST-001", loc1);
        SetPlatformCourse("TEST-001", course1);
        SetPlatformCourseDelta("TEST-001", new FssCourseDelta() { HeadingChangeClockwiseDegsSec = 5, SpeedChangeMpMps = 0 });

        AddPlatform("TEST-002", "F18");
        SetPlatformStartLLA("TEST-002", loc2);
        SetPlatformCourse("TEST-002", course2);
        SetPlatformCourseDelta("TEST-002", new FssCourseDelta() { HeadingChangeClockwiseDegsSec = -8, SpeedChangeMpMps = 0 });

        AddPlatform("TEST-003", "Tornado");
        SetPlatformStartLLA("TEST-003", loc3);
        SetPlatformCourse("TEST-003", course3);
        SetPlatformCourseDelta("TEST-003", new FssCourseDelta() { HeadingChangeClockwiseDegsSec = 10, SpeedChangeMpMps = 0 });
    }

    // ---------------------------------------------------------------------------------------------

}


