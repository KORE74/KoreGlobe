
using System;
using System.Collections.Generic;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add/Del
    // ---------------------------------------------------------------------------------------------
    public void AddPlatform(string platName)
    {
        // Create a new platform
        // if (GloAppFactory.Instance.PlatformManager == null)
        //     GloCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Platform Manager not found in GloAppFactory.Instance");

        GloPlatform? newPlat = GloAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            GloCentralLog.AddEntry($"EC0-0026: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = "Unknown";
    }

    public void AddPlatform(string platName, string platType)
    {
        // Create a new platform
        // if (GloAppFactory.Instance.PlatformManager == null)
        //     GloCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Platform Manager not found in GloAppFactory.Instance");

        GloPlatform? newPlat = GloAppFactory.Instance.PlatformManager.Add(platName);
        if (newPlat == null)
        {
            GloCentralLog.AddEntry($"EC0-0001: Platform {platName} not created, already exists.");
            return;
        }
        newPlat.Type = platType;

        DefaultPlatformDetails(platName);
    }

    public bool DoesPlatformExist(string platName) => GloAppFactory.Instance.PlatformManager.DoesPlatExist(platName);
    public void DeletePlatform(string platName)    => GloAppFactory.Instance.PlatformManager.Delete(platName);
    public void DeleteAllPlatforms()               => GloAppFactory.Instance.PlatformManager.DeleteAllPlatforms();
    public int  NumPlatforms()                     => GloAppFactory.Instance.PlatformManager.NumPlatforms();

    // ---------------------------------------------------------------------------------------------
    // MARK: Details
    // ---------------------------------------------------------------------------------------------

    public void DefaultPlatformDetails(string platName)
    {
        GloLLAPoint    startPos    = new GloLLAPoint()    { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        GloLLAPoint    currPos     = new GloLLAPoint()    { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        GloAttitude    att         = new GloAttitude()    { PitchUpDegs = 0.0, RollClockwiseDegs = 0.0, YawClockwiseDegs = 0.0 };
        GloCourse      course      = new GloCourse()      { SpeedKph = 0.0, HeadingDegs = 0.0, ClimbRateMps = 0.0 };
        GloCourseDelta courseDelta = new GloCourseDelta() { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 };

        SetPlatformStartDetails(platName, startPos, att, course);
        SetPlatformCurrDetails(platName, currPos, att, course, courseDelta);
    }

    public void SetPlatformStartDetails(string platName, GloLLAPoint startPos, GloAttitude startAtt, GloCourse startCourse)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0002: Platform {platName} not found.");
            return;
        }

        // Set the platform's details
        platform.Kinetics.StartPosition = startPos;
        platform.Kinetics.StartAttitude = startAtt;
        platform.Kinetics.StartCourse   = startCourse;
    }

    public void SetPlatformCurrDetails(string platName, GloLLAPoint currPos, GloAttitude currAtt, GloCourse course, GloCourseDelta courseDelta)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0002: Platform {platName} not found.");
            return;
        }

        // Set the platform's details
        platform.Kinetics.CurrPosition    = currPos;
        platform.Kinetics.CurrAttitude    = currAtt;
        platform.Kinetics.CurrCourse      = course;
        platform.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Start
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformStart(string platName, GloLLAPoint newpos, GloCourse newcourse, GloAttitude newAtt)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0005: Platform {platName} not found.");
            return;
        }

        // Set the platform's start location
        platform.Kinetics.StartPosition = newpos;
        platform.Kinetics.StartCourse   = newcourse;
        platform.Kinetics.StartAttitude = newAtt;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK:  Type
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformType(string platName, string platType, string platCategory)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0031: Platform {platName} not found.");
            return;
        }

        // Set the platform's type
        platform.Type     = platType;
        platform.Category = platCategory;
    }

    public string? PlatformType(string platName) =>
        GloAppFactory.Instance.PlatformManager.PlatForName(platName)?.Type;

    public string? PlatformCategory(string platName) =>
        GloAppFactory.Instance.PlatformManager.PlatForName(platName)?.Category;

    // ---------------------------------------------------------------------------------------------
    // MARK: Position
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformStartLLA(string platName, GloLLAPoint newpos)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0005: Platform {platName} not found.");
            return;
        }

        // Set the platform's start location
        platform.Kinetics.StartPosition = newpos;
    }

    public GloLLAPoint? PlatformStartLLA(string platName)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.StartPosition;
    }

    // ---------------------------------------------------------------------------------------------

    public void SetPlatformPosition(string platName, GloLLAPoint newpos)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0006: Platform {platName} not found.");
            return;
        }

        // Set the platform's position
        platform.Kinetics.CurrPosition = newpos;
    }

    public GloLLAPoint? GetPlatformPosition(string platName)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrPosition;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Attitude
    // ---------------------------------------------------------------------------------------------

    public GloAttitude? GetPlatformAttitude(string platName)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrAttitude;
    }

    public void SetPlatformAttitude(string platName, GloAttitude newatt)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0008: Platform {platName} not found.");
            return;
        }

        // Set the platform's attitude
        platform.Kinetics.CurrAttitude = newatt;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Course
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformCourse(string platName, GloCourse course)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0009: Platform {platName} not found.");
            return;
        }

        // Set the platform's course
        platform.Kinetics.CurrCourse = course;
    }

    public GloCourse? PlatformCurrCourse(string platName) =>
        GloAppFactory.Instance.PlatformManager.PlatForName(platName)?.Kinetics.CurrCourse;

    // ---------------------------------------------------------------------------------------------
    // MARK: Course Delta
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformCourseDelta(string platName, GloCourseDelta courseDelta)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0010: Platform {platName} not found.");
            return;
        }

        // Set the platform's course delta
        platform.Kinetics.CurrCourseDelta = courseDelta;
    }

    public GloCourseDelta? PlatformCurrCourseDelta(string platName)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.Kinetics.CurrCourseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Focus
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformFocus(string platName)
    {
        // Check the platform exists
        if (!DoesPlatformExist(platName))
        {
            GloCentralLog.AddEntry($"Attempt to select non-existent platform {platName}");
            return;
        }

        // check the name matches an entity
        if (!GloGodotFactory.Instance.GodotEntityManager.EntityExists(platName))
        {
            GloCentralLog.AddEntry($"Attempt to select non-existent entity {platName}");
            return;
        }

        // Select the chase cam mode
        // move the chase cam if in the right mode
        // if (GloGodotFactory.Instance.UIState.IsCamModeChaseCam())
        // {

            int platFindCountdown = NumPlatforms();
            while (NearPlatformName() != platName)
            {
                NearPlatformNext();
                platFindCountdown--;
                if (platFindCountdown <= 0)
                {
                    GloCentralLog.AddEntry($"Platform {platName} not found in list.");
                    break;
                }
            }
            GloCentralLog.AddEntry($"SetPlatformFocus: near platform [{platName}] selected");

            SetCameraModeChaseCam();

            //GloGodotFactory.Instance.GodotEntityManager.EnableChaseCam(platName);
            GloGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();

        // }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Report
    // ---------------------------------------------------------------------------------------------

    public string PlatformPositionsReport() => GloAppFactory.Instance.PlatformManager.PlatformPositionsReport();
    public string PlatformElementsReport()  => GloAppFactory.Instance.PlatformManager.PlatformElementsReport();

    // ---------------------------------------------------------------------------------------------
    // MARK: Names
    // ---------------------------------------------------------------------------------------------

    public string PlatformNameForIndex(int index)        => GloAppFactory.Instance.PlatformManager.PlatNameForIndex(index);
    public GloPlatform? PlatformForIndex(int index)      => GloAppFactory.Instance.PlatformManager.PlatForIndex(index);
    public GloPlatform? PlatformForName(string platname) => GloAppFactory.Instance.PlatformManager.PlatForName(platname);

    // Id being the 1-based user presented index

    public string PlatformIdForName(string platname) => GloAppFactory.Instance.PlatformManager.PlatIdForName(platname);
    public string PlatformNameForId(int platId)      => GloAppFactory.Instance.PlatformManager.PlatNameForId(platId);

    public int PlatformIdNext(int currPlatId) =>  GloAppFactory.Instance.PlatformManager.PlatIdNext(currPlatId);
    public int PlatformIdPrev(int currPlatId) =>  GloAppFactory.Instance.PlatformManager.PlatIdPrev(currPlatId);

    public List<string> PlatformNames() => GloAppFactory.Instance.PlatformManager.PlatNameList();

    // ---------------------------------------------------------------------------------------------
    // MARK: Near/Far UI
    // ---------------------------------------------------------------------------------------------

    // Usage:
    // - GloAppFactory.Instance.EventDriver.NearPlatformValid();
    // - GloAppFactory.Instance.EventDriver.NearPlatformName();

    public bool   NearPlatformValid()  => GloAppFactory.Instance.PlatformManager.NearPlatformValid();
    public bool   FarPlatformValid()   => GloAppFactory.Instance.PlatformManager.FarPlatformValid();
    public string NearPlatformName()   => GloAppFactory.Instance.PlatformManager.NearPlatformName();
    public string FarPlatformName()    => GloAppFactory.Instance.PlatformManager.FarPlatformName();

    public void NearPlatformNext()
    {
        GloAppFactory.Instance.PlatformManager.NearPlatformNext();

        // move the chase cam if in the right mode
        if (GloGodotFactory.Instance.UIState.IsCamModeChaseCam())
        {
            GloGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearPlatformName());
        }
    }

    public void NearPlatformPrev()
    {
        GloAppFactory.Instance.PlatformManager.NearPlatformPrev();

        // move the chase cam if in the right mode
        if (GloGodotFactory.Instance.UIState.IsCamModeChaseCam())
        {
            GloGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearPlatformName());
        }
    }

    public void FarPlatformNext()    => GloAppFactory.Instance.PlatformManager.FarPlatformNext();
    public void FarPlatformPrev()    => GloAppFactory.Instance.PlatformManager.FarPlatformPrev();

    // ---------------------------------------------------------------------------------------------

    public void SetupTestPlatforms()
    {
        GloCentralLog.AddEntry("Creating Test Platform Entities");

        GloLLAPoint loc1 = new GloLLAPoint() { LatDegs = 52.1, LonDegs =    -4.2, AltMslM = 5000 };
        GloLLAPoint loc2 = new GloLLAPoint() { LatDegs = 52.5, LonDegs =     0.3, AltMslM = 2000 };
        GloLLAPoint loc3 = new GloLLAPoint() { LatDegs = 52.9, LonDegs =     8.1, AltMslM = 1000 };
        GloLLAPoint loc4 = new GloLLAPoint() { LatDegs = 32.5, LonDegs =  -117.2, AltMslM = 8000 };

        GloCourse course1 = new GloCourse() { HeadingDegs = 270, SpeedKph = 800.08 };
        GloCourse course2 = new GloCourse() { HeadingDegs = 180, SpeedKph = 800.08 };
        GloCourse course3 = new GloCourse() { HeadingDegs =  90, SpeedKph = 800.08 };
        GloCourse course4 = new GloCourse() { HeadingDegs =  30, SpeedKph = 200.00 };

        GloCourseDelta courseDelta1 = new GloCourseDelta() { HeadingChangeClockwiseDegsSec =  2,   SpeedChangeMpMps = 0 };
        GloCourseDelta courseDelta2 = new GloCourseDelta() { HeadingChangeClockwiseDegsSec = -3,   SpeedChangeMpMps = 0 };
        GloCourseDelta courseDelta3 = new GloCourseDelta() { HeadingChangeClockwiseDegsSec =  1.2, SpeedChangeMpMps = 0 };
        GloCourseDelta courseDelta4 = new GloCourseDelta() { HeadingChangeClockwiseDegsSec =  0.1, SpeedChangeMpMps = 0 };

        GloAttitude att1 = new GloAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 20, YawClockwiseDegs = 0 };
        GloAttitude att3 = new GloAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 12, YawClockwiseDegs = 0 };

        AddPlatform("TEST-F16", "F16");
        SetPlatformStart("TEST-F16", loc1, course1, att1);
        SetPlatformCourseDelta("TEST-F16", courseDelta1);

        // AddPlatform("TEST-F18", "F18");
        // SetPlatformStartLLA("TEST-F18", loc2);
        // SetPlatformCourse("TEST-F18", course2);
        // SetPlatformCourseDelta("TEST-F18", courseDelta2);

        AddPlatform("TEST-Torn", "Tornado");
        SetPlatformStart("TEST-Torn", loc3, course3, att3);
        SetPlatformCourseDelta("TEST-Torn", courseDelta3);

        // AddPlatform("TEST-MQ9", "MQ9Reaper");
        // SetPlatformStartLLA("TEST-MQ9", loc4);
        // SetPlatformCourse("TEST-MQ9", course4);
        // SetPlatformCourseDelta("TEST-MQ9", courseDelta4);

        // {
        //     GloLLAPoint    loc         = new GloLLAPoint()    { LatDegs = 52.8, LonDegs =    -4.28, AltMslM = 0 };
        //     GloCourse      course      = new GloCourse()      { HeadingDegs = 1850, SpeedKph = 10 };
        //     GloCourseDelta courseDelta = new GloCourseDelta() { HeadingChangeClockwiseDegsSec =  0, SpeedChangeMpMps = 0 };
        //     AddPlatform("TEST-Ship1", "SupportShip");
        //     SetPlatformStartLLA("TEST-Ship1", loc);
        //     SetPlatformCourse("TEST-Ship1", course);
        //     SetPlatformCourseDelta("TEST-Ship1", courseDelta);
        // }

        {
            GloLLAPoint locIOW1    = new GloLLAPoint() { LatDegs = 50.580463, LonDegs =  -1.300776, AltMslM = 104.9 };
            GloCourse   courseIOW1 = new GloCourse()   { HeadingDegs = 90, SpeedKph = 0.0 };
            GloAttitude attIOW1    = new GloAttitude() { PitchUpDegs = 0, RollClockwiseDegs = 0, YawClockwiseDegs = 0 };

            AddPlatform("TEST-SAM", "S400Radar");
            SetPlatformStart("TEST-SAM", locIOW1, courseIOW1, attIOW1);
            SetPlatformType("TEST-SAM", "S400Radar", "GroundClamped");
        }
    }

    // ---------------------------------------------------------------------------------------------

}


