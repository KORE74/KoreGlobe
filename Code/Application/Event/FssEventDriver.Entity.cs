
using System;
using System.Collections.Generic;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Management
    // ---------------------------------------------------------------------------------------------

    public void AddEntity(string platName)
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

    // public void AddPlatform(string platName, string platType)
    // {
    //     // Create a new platform
    //     // if (FssAppFactory.Instance.PlatformManager == null)
    //     //     FssCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Platform Manager not found in FssAppFactory.Instance");

    //     FssPlatform? newPlat = FssAppFactory.Instance.PlatformManager.Add(platName);
    //     if (newPlat == null)
    //     {
    //         FssCentralLog.AddEntry($"EC0-0001: Platform {platName} not created, already exists.");
    //         return;
    //     }
    //     newPlat.Type = platType;

    //     DefaultPlatformDetails(platName);
    // }

    public bool DoesEntityExist(string name) => FssAppFactory.Instance.EntityManager.DoesEntityExist(name);
    public void DeleteEntity(string name)    => FssAppFactory.Instance.EntityManager.Delete(name);
    public void DeleteAllEntities()          => FssAppFactory.Instance.EntityManager.DeleteAllEntities();
    public int  NumEntities()                => FssAppFactory.Instance.EntityManager.NumEntities();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Full Details
    // ---------------------------------------------------------------------------------------------

    public void DefaultEntityDetails(string entName)
    {
        FssLLAPoint    startPos    = new FssLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssLLAPoint    currPos     = new FssLLAPoint() { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssAttitude    att         = new FssAttitude() { PitchUpDegs = 0.0, RollClockwiseDegs = 0.0, YawClockwiseDegs = 0.0 };
        FssCourse      course      = new FssCourse()   { SpeedKph = 0.0, HeadingDegs = 0.0, ClimbRateMps = 0.0 };
        FssCourseDelta courseDelta = new FssCourseDelta() { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 };

        SetEntityStartDetails(entName, startPos, att, course);
        SetEntityCurrDetails(entName, currPos, att, course, courseDelta);
    }

    public void SetEntityStartDetails(string entName, FssLLAPoint startPos, FssAttitude startAtt, FssCourse startCourse)
    {
        // Get the entity
        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entName);

        if (ent == null)
        {
            FssCentralLog.AddEntry($"EC0-0002: Platform {entName} not found.");
            return;
        }

        // Set the platform's details
        ent.Kinetics.StartPosition = startPos;
        ent.Kinetics.StartAttitude = startAtt;
        ent.Kinetics.StartCourse   = startCourse;
    }

    public void SetEntityCurrDetails(string entName, FssLLAPoint currPos, FssAttitude currAtt, FssCourse course, FssCourseDelta courseDelta)
    {
        // Get the entity
        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entName);

        if (ent == null)
        {
            FssCentralLog.AddEntry($"EC0-0002: Platform {entName} not found.");
            return;
        }

        // Set the platform's details
        ent.Kinetics.CurrPosition    = currPos;
        ent.Kinetics.CurrAttitude    = currAtt;
        ent.Kinetics.CurrCourse      = course;
        ent.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Position
    // ---------------------------------------------------------------------------------------------

    public void SetEntityStartLLA(string entName, FssLLAPoint newpos)
    {
        var entity = FssAppFactory.Instance.EntityManager.EntityForName(entName);
        if (entity != null)
            entity.Kinetics.StartPosition = newpos;
    }

    public void SetEntityCurrLLA(string entName, FssLLAPoint newpos)
    {
        var entity = FssAppFactory.Instance.EntityManager.EntityForName(entName);
        if (entity != null)
            entity.Kinetics.CurrPosition = newpos;
    }

    public FssLLAPoint? EntityStartLLA(string entName) => FssAppFactory.Instance.EntityManager.EntityForName(entName)?.Kinetics.StartPosition;
    public FssLLAPoint? EntityCurrLLA(string entName)  => FssAppFactory.Instance.EntityManager.EntityForName(entName)?.Kinetics.CurrPosition;

    // ---------------------------------------------------------------------------------------------
    // #MARK: Platform Attitude
    // ---------------------------------------------------------------------------------------------

    public void SetPlatformAttitude(string entName, FssAttitude newatt)
    {
        var entity = FssAppFactory.Instance.EntityManager.EntityForName(entName);
        if (entity != null)
            entity.Kinetics.CurrAttitude = newatt;
    }

    public FssAttitude? PlatformCurrAttitude(string entName) => FssAppFactory.Instance.EntityManager.EntityForName(entName)?.Kinetics.CurrAttitude;

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


