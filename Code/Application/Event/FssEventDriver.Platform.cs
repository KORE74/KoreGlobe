
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
        //     FssCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            FssCentralLog.AddEntry($"EC0-0026: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = "Unknown";
    }

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        // if (FssAppFactory.Instance.PlatformManager == null)
        //     FssCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

        FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            FssCentralLog.AddEntry($"EC0-0001: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = platType;

        DefaultPlatformDetails(platName);
    }

    public bool DoesPlatformExist(string platName) => FssAppFactory.Instance.PlatformManager.DoesPlatExist(platName);
    public void DeletePlatform(string platName)    => FssAppFactory.Instance.PlatformManager.Delete(platName);
    public void DeleteAllPlatforms()               => FssAppFactory.Instance.PlatformManager.DeleteAllPlatforms();
    public int  NumPlatforms()                     => FssAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Full Details
    // ---------------------------------------------------------------------------------------------

    public void DefaultPlatformDetails(string platName)
    {
        FssLLAPoint    startPos    = new FssLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssLLAPoint    currPos     = new FssLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssAttitude    att         = new FssAttitude() { PitchUpDegs = 0.0, RollClockwiseDegs = 0.0, YawClockwiseDegs = 0.0 };
        FssCourse      course      = new FssCourse() { SpeedKph = 0.0, HeadingDegs = 0.0 };
        FssCourseDelta courseDelta = new FssCourseDelta() { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 };

        SetPlatformStartDetails(platName, startPos, att, course);
        SetPlatformCurrDetails(platName, currPos, att, course, courseDelta);
    }

    public void SetPlatformStartDetails(string platName, FssLLAPoint startPos, FssAttitude startAtt, FssCourse startCourse)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0002: Platform {platName} not found.");
            return;
        }

        // Set the platform's details
        platform.Kinetics.StartPosition = startPos;
        platform.Kinetics.StartAttitude = startAtt;
        platform.Kinetics.StartCourse   = startCourse;
    }

    public void SetPlatformCurrDetails(string platName, FssLLAPoint currPos, FssAttitude currAtt, FssCourse course, FssCourseDelta courseDelta)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0002: Platform {platName} not found.");
            return;
        }

        // Set the platform's details
        platform.Kinetics.CurrPosition    = currPos;
        platform.Kinetics.CurrAttitude    = currAtt;
        platform.Kinetics.CurrCourse      = course;
        platform.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Type
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformType(string platName, string platType, string platCategory)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0031: Platform {platName} not found.");
            return;
        }

        // Set the platform's type
        platform.Type     = platType;
        platform.Category = platCategory;
    }

    public string? PlatformType(string platName) =>
        FssAppFactory.Instance.PlatformManager.PlatForName(platName)?.Type;

    public string? PlatformCategory(string platName) =>
        FssAppFactory.Instance.PlatformManager.PlatForName(platName)?.Category;

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Position
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformStartLLA(string platName, FssLLAPoint newpos)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0005: Platform {platName} not found.");
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
            FssCentralLog.AddEntry($"EC0-0006: Platform {platName} not found.");
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
            FssCentralLog.AddEntry($"EC0-0008: Platform {platName} not found.");
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
            FssCentralLog.AddEntry($"EC0-0009: Platform {platName} not found.");
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
            FssCentralLog.AddEntry($"EC0-0010: Platform {platName} not found.");
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
    // #MARK: Platform Report
    // ---------------------------------------------------------------------------------------------

    public string PlatformPositionsReport() => FssAppFactory.Instance.PlatformManager.PlatformPositionsReport();
    public string PlatformElementsReport()  => FssAppFactory.Instance.PlatformManager.PlatformElementsReport();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Names
    // ---------------------------------------------------------------------------------------------

    public string PlatformNameForIndex(int index)        => FssAppFactory.Instance.PlatformManager.PlatNameForIndex(index);
    public FssPlatform? PlatformForIndex(int index)      => FssAppFactory.Instance.PlatformManager.PlatForIndex(index);
    public FssPlatform? PlatformForName(string platname) => FssAppFactory.Instance.PlatformManager.PlatForName(platname);

    // Id being the 1-based user presented index

    public string PlatformIdForName(string platname) => FssAppFactory.Instance.PlatformManager.PlatIdForName(platname);
    public string PlatformNameForId(int platId)      => FssAppFactory.Instance.PlatformManager.PlatNameForId(platId);

    public int PlatformIdNext(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdNext(currPlatId);
    public int PlatformIdPrev(int currPlatId) =>  FssAppFactory.Instance.PlatformManager.PlatIdPrev(currPlatId);

    public List<string> PlatformNames() => FssAppFactory.Instance.PlatformManager.PlatNameList();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Near/Far UI
    // ---------------------------------------------------------------------------------------------

    // Usage:
    // - FssAppFactory.Instance.EventDriver.NearPlatformValid();
    // - FssAppFactory.Instance.EventDriver.NearPlatformName();

    public bool NearPlatformValid()  => FssAppFactory.Instance.PlatformManager.NearPlatformValid();
    public bool FarPlatformValid()   => FssAppFactory.Instance.PlatformManager.FarPlatformValid();

    public string NearPlatformName() => FssAppFactory.Instance.PlatformManager.NearPlatformName();
    public string FarPlatformName()  => FssAppFactory.Instance.PlatformManager.FarPlatformName();

    public void NearPlatformNext()
    {
        FssAppFactory.Instance.PlatformManager.NearPlatformNext();

        // move the chase cam if in the right mode
        if (FssGodotFactory.Instance.UIState.IsCamModeChaseCam())
        {
            FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearPlatformName());
        }
    }

    public void NearPlatformPrev()
    {
        FssAppFactory.Instance.PlatformManager.NearPlatformPrev();

        // move the chase cam if in the right mode
        if (FssGodotFactory.Instance.UIState.IsCamModeChaseCam())
        {
            FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearPlatformName());
        }
    }

    public void FarPlatformNext()    => FssAppFactory.Instance.PlatformManager.FarPlatformNext();
    public void FarPlatformPrev()    => FssAppFactory.Instance.PlatformManager.FarPlatformPrev();

    // ---------------------------------------------------------------------------------------------

    public void SetupTestPlatforms()
    {
        FssCentralLog.AddEntry("Creating Test Platform Entities");

        FssLLAPoint loc1 = new FssLLAPoint() { LatDegs = 52.1, LonDegs =    -4.2, AltMslM = 5000 };
        FssLLAPoint loc2 = new FssLLAPoint() { LatDegs = 52.5, LonDegs =     0.3, AltMslM = 2000 };
        FssLLAPoint loc3 = new FssLLAPoint() { LatDegs = 52.9, LonDegs =     8.1, AltMslM = 1000 };
        FssLLAPoint loc4 = new FssLLAPoint() { LatDegs = 32.5, LonDegs =  -117.2, AltMslM = 8000 };

        FssCourse course1 = new FssCourse() { HeadingDegs = 270, SpeedKph = 800.08 };
        FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 800.08 };
        FssCourse course3 = new FssCourse() { HeadingDegs =  90, SpeedKph = 800.08 };
        FssCourse course4 = new FssCourse() { HeadingDegs =  30, SpeedKph = 200.00 };

        FssCourseDelta courseDelta1 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  2, SpeedChangeMpMps = 0 };
        FssCourseDelta courseDelta2 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec = -3, SpeedChangeMpMps = 0 };
        FssCourseDelta courseDelta3 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  4, SpeedChangeMpMps = 0 };
        FssCourseDelta courseDelta4 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  0.1, SpeedChangeMpMps = 0 };

        AddPlatform("TEST-F16", "F16");
        SetPlatformStartLLA("TEST-F16", loc1);
        SetPlatformCourse("TEST-F16", course1);
        SetPlatformCourseDelta("TEST-F16", courseDelta1);

        // AddPlatform("TEST-F18", "F18");
        // SetPlatformStartLLA("TEST-F18", loc2);
        // SetPlatformCourse("TEST-F18", course2);
        // SetPlatformCourseDelta("TEST-F18", courseDelta2);

        AddPlatform("TEST-Torn", "Su34");
        SetPlatformStartLLA("TEST-Torn", loc3);
        SetPlatformCourse("TEST-Torn", course3);
        SetPlatformCourseDelta("TEST-Torn", courseDelta3);

        // AddPlatform("TEST-MQ9", "MQ9Reaper");
        // SetPlatformStartLLA("TEST-MQ9", loc4);
        // SetPlatformCourse("TEST-MQ9", course4);
        // SetPlatformCourseDelta("TEST-MQ9", courseDelta4);

        // {
        //     FssLLAPoint    loc         = new FssLLAPoint()    { LatDegs = 52.8, LonDegs =    -4.28, AltMslM = 0 };
        //     FssCourse      course      = new FssCourse()      { HeadingDegs = 1850, SpeedKph = 10 };
        //     FssCourseDelta courseDelta = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  0, SpeedChangeMpMps = 0 };

        //     AddPlatform("TEST-Ship1", "SupportShip");
        //     SetPlatformStartLLA("TEST-Ship1", loc);
        //     SetPlatformCourse("TEST-Ship1", course);
        //     SetPlatformCourseDelta("TEST-Ship1", courseDelta);
        // }

    }

    // ---------------------------------------------------------------------------------------------

}


