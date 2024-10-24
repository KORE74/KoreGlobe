
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

    public void AddEntity(string entityName)
    {
        // Create a new Entity
        // if (FssAppFactory.Instance.EntityManager == null)
        //     FssCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Entity Manager not found in FssAppFactory.Instance");

        FssEntity? newEntity = FssAppFactory.Instance.EntityManager.Add(entityName);
        if (newEntity == null)
        {
            FssCentralLog.AddEntry($"EC0-0026: Entity {entityName} not created, already exists.");
            return;
        }
        //newEntity.Type = "Unknown";
    }

    // public void AddEntity(string entityName, string EntityType)
    // {
    //     // Create a new Entity
    //     // if (FssAppFactory.Instance.EntityManager == null)
    //     //     FssCentralLog.AddEntry("EC0-0002: ERROR ERROR ERROR: Entity Manager not found in FssAppFactory.Instance");

    //     FssEntity? newEntity = FssAppFactory.Instance.EntityManager.Add(entityName);
    //     if (newEntity == null)
    //     {
    //         FssCentralLog.AddEntry($"EC0-0001: Entity {entityName} not created, already exists.");
    //         return;
    //     }
    //     newEntity.Type = EntityType;

    //     DefaultEntityDetails(entityName);
    // }

    public bool DoesEntityExist(string name) => FssAppFactory.Instance.EntityManager.DoesEntityExist(name);
    public void DeleteEntity(string name)    => FssAppFactory.Instance.EntityManager.Delete(name);
    public void DeleteAllEntities()          => FssAppFactory.Instance.EntityManager.DeleteAllEntities();
    public int  NumEntities()                => FssAppFactory.Instance.EntityManager.NumEntities();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Full Details
    // ---------------------------------------------------------------------------------------------

    public void DefaultEntityDetails(string entName)
    {
        FssLLAPoint    startPos    = new FssLLAPoint()    { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssLLAPoint    currPos     = new FssLLAPoint()    { LatDegs = 0.0, LonDegs = 0.0, AltMslM = 100.0 };
        FssAttitude    att         = new FssAttitude()    { PitchUpDegs = 0.0, RollClockwiseDegs = 0.0, YawClockwiseDegs = 0.0 };
        FssCourse      course      = new FssCourse()      { GroundSpeedKph = 0.0, HeadingDegs = 0.0, ClimbRateMps = 0.0 };
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
            FssCentralLog.AddEntry($"EC0-0002: Entity {entName} not found.");
            return;
        }

        // Set the Entity's details
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
            FssCentralLog.AddEntry($"EC0-0002: Entity {entName} not found.");
            return;
        }

        // Set the Entity's details
        ent.Kinetics.CurrPosition    = currPos;
        ent.Kinetics.CurrAttitude    = currAtt;
        ent.Kinetics.CurrCourse      = course;
        ent.Kinetics.CurrCourseDelta = courseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Position
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
    // #MARK: Entity Attitude
    // ---------------------------------------------------------------------------------------------

    public void SetEntityAttitude(string entName, FssAttitude newatt)
    {
        var entity = FssAppFactory.Instance.EntityManager.EntityForName(entName);
        if (entity != null)
            entity.Kinetics.CurrAttitude = newatt;
    }

    public FssAttitude? EntityCurrAttitude(string entName) => FssAppFactory.Instance.EntityManager.EntityForName(entName)?.Kinetics.CurrAttitude;

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Course
    // ---------------------------------------------------------------------------------------------

    public void SetEntityCourse(string entityName, FssCourse course)
    {
        // Get the Entity
        FssEntity? Entity = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            FssCentralLog.AddEntry($"EC0-0009: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's course
        Entity.Kinetics.CurrCourse = course;
    }

    public FssCourse? EntityCurrCourse(string entityName) =>
        FssAppFactory.Instance.EntityManager.EntityForName(entityName)?.Kinetics.CurrCourse;

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Course Delta
    // ---------------------------------------------------------------------------------------------

    public void SetEntityCourseDelta(string entityName, FssCourseDelta courseDelta)
    {
        // Get the Entity
        FssEntity? Entity = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
        {
            FssCentralLog.AddEntry($"EC0-0010: Entity {entityName} not found.");
            return;
        }

        // Set the Entity's course delta
        Entity.Kinetics.CurrCourseDelta = courseDelta;
    }

    public FssCourseDelta? EntityCurrCourseDelta(string entityName)
    {
        // Get the Entity
        FssEntity? Entity = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (Entity == null)
            return null;

        return Entity.Kinetics.CurrCourseDelta;
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Report
    // ---------------------------------------------------------------------------------------------

    // public string EntityPositionsReport() => FssAppFactory.Instance.EntityManager.EntityPositionsReport();
    // public string EntityElementsReport()  => FssAppFactory.Instance.EntityManager.EntityElementsReport();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Names
    // ---------------------------------------------------------------------------------------------

    public string EntityNameForIndex(int index)        => FssAppFactory.Instance.EntityManager.EntityNameForIndex(index);
    public FssEntity? EntityForIndex(int index)      => FssAppFactory.Instance.EntityManager.EntityForIndex(index);
    public FssEntity? EntityForName(string Entityname) => FssAppFactory.Instance.EntityManager.EntityForName(Entityname);

    // Id being the 1-based user presented index

    public string EntityIdForName(string Entityname) => FssAppFactory.Instance.EntityManager.EntityIdForName(Entityname);
    public string EntityNameForId(int EntityId)      => FssAppFactory.Instance.EntityManager.EntityNameForId(EntityId);

    public int EntityIdNext(int currEntityId) =>  FssAppFactory.Instance.EntityManager.EntityIdNext(currEntityId);
    public int EntityIdPrev(int currEntityId) =>  FssAppFactory.Instance.EntityManager.EntityIdPrev(currEntityId);

    public List<string> EntityNames() => FssAppFactory.Instance.EntityManager.EntityNameList();

    // ---------------------------------------------------------------------------------------------
    // #MARK: Entity Near/Far UI
    // ---------------------------------------------------------------------------------------------

    // Usage:
    // - FssAppFactory.Instance.EventDriver.NearEntityValid();
    // - FssAppFactory.Instance.EventDriver.NearEntityName();

    // public bool NearEntityValid()  => FssAppFactory.Instance.EntityManager.NearEntityValid();
    // public bool FarEntityValid()   => FssAppFactory.Instance.EntityManager.FarEntityValid();

    // public string NearEntityName() => FssAppFactory.Instance.EntityManager.NearEntityName();
    // public string FarEntityName()  => FssAppFactory.Instance.EntityManager.FarEntityName();

    // public void NearEntityNext()
    // {
    //     FssAppFactory.Instance.EntityManager.NearEntityNext();

    //     // move the chase cam if in the right mode
    //     if (FssGodotFactory.Instance.UIState.IsCamModeChaseCam())
    //     {
    //         FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearEntityName());
    //     }
    // }

    // public void NearEntityPrev()
    // {
    //     FssAppFactory.Instance.EntityManager.NearEntityPrev();

    //     // move the chase cam if in the right mode
    //     if (FssGodotFactory.Instance.UIState.IsCamModeChaseCam())
    //     {
    //         FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(NearEntityName());
    //     }
    // }

    // public void FarEntityNext()    => FssAppFactory.Instance.EntityManager.FarEntityNext();
    // public void FarEntityPrev()    => FssAppFactory.Instance.EntityManager.FarEntityPrev();

    // ---------------------------------------------------------------------------------------------

    // public void SetupTestEntitys()
    // {
    //     FssCentralLog.AddEntry("Creating Test Entity Entities");

    //     FssLLAPoint loc1 = new FssLLAPoint() { LatDegs = 52.1, LonDegs =    -4.2, AltMslM = 5000 };
    //     FssLLAPoint loc2 = new FssLLAPoint() { LatDegs = 52.5, LonDegs =     0.3, AltMslM = 2000 };
    //     FssLLAPoint loc3 = new FssLLAPoint() { LatDegs = 52.9, LonDegs =     8.1, AltMslM = 1000 };
    //     FssLLAPoint loc4 = new FssLLAPoint() { LatDegs = 32.5, LonDegs =  -117.2, AltMslM = 8000 };

    //     FssCourse course1 = new FssCourse() { HeadingDegs = 270, SpeedKph = 800.08 };
    //     FssCourse course2 = new FssCourse() { HeadingDegs = 180, SpeedKph = 800.08 };
    //     FssCourse course3 = new FssCourse() { HeadingDegs =  90, SpeedKph = 800.08 };
    //     FssCourse course4 = new FssCourse() { HeadingDegs =  30, SpeedKph = 200.00 };

    //     FssCourseDelta courseDelta1 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  2, SpeedChangeMpMps = 0 };
    //     FssCourseDelta courseDelta2 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec = -3, SpeedChangeMpMps = 0 };
    //     FssCourseDelta courseDelta3 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  4, SpeedChangeMpMps = 0 };
    //     FssCourseDelta courseDelta4 = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  0.1, SpeedChangeMpMps = 0 };

    //     AddEntity("TEST-F16", "F16");
    //     SetEntityStartLLA("TEST-F16", loc1);
    //     SetEntityCourse("TEST-F16", course1);
    //     SetEntityCourseDelta("TEST-F16", courseDelta1);

    //     // AddEntity("TEST-F18", "F18");
    //     // SetEntityStartLLA("TEST-F18", loc2);
    //     // SetEntityCourse("TEST-F18", course2);
    //     // SetEntityCourseDelta("TEST-F18", courseDelta2);

    //     AddEntity("TEST-Torn", "Su34");
    //     SetEntityStartLLA("TEST-Torn", loc3);
    //     SetEntityCourse("TEST-Torn", course3);
    //     SetEntityCourseDelta("TEST-Torn", courseDelta3);

    //     // AddEntity("TEST-MQ9", "MQ9Reaper");
    //     // SetEntityStartLLA("TEST-MQ9", loc4);
    //     // SetEntityCourse("TEST-MQ9", course4);
    //     // SetEntityCourseDelta("TEST-MQ9", courseDelta4);

    //     // {
    //     //     FssLLAPoint    loc         = new FssLLAPoint()    { LatDegs = 52.8, LonDegs =    -4.28, AltMslM = 0 };
    //     //     FssCourse      course      = new FssCourse()      { HeadingDegs = 1850, SpeedKph = 10 };
    //     //     FssCourseDelta courseDelta = new FssCourseDelta() { HeadingChangeClockwiseDegsSec =  0, SpeedChangeMpMps = 0 };

    //     //     AddEntity("TEST-Ship1", "SupportShip");
    //     //     SetEntityStartLLA("TEST-Ship1", loc);
    //     //     SetEntityCourse("TEST-Ship1", course);
    //     //     SetEntityCourseDelta("TEST-Ship1", courseDelta);
    //     // }

    // }

    // ---------------------------------------------------------------------------------------------

}


