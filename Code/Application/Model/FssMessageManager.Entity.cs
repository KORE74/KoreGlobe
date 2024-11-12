using System;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    private void ProcessMessage_EntityAdd(EntityAdd msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_EntityAdd: {msg.Name}");

        // Check if the platform already exists
        if (!FssEventDriver.DoesEntityExist(msg.Name))
        {
            FssEventDriver.AddEntity(msg.Name);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityDelete(EntityDelete msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_EntityDelete: {msg.Name}");

        // Check if the platform exists
        if (FssEventDriver.DoesEntityExist(msg.Name))
        {
            FssEventDriver.DeleteEntity(msg.Name);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityPosition(EntityPosition msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_EntityPosition: {msg.Name}");

        // Check if the platform exists
        if (FssEventDriver.DoesEntityExist(msg.Name))
        {
            FssEventDriver.SetEntityCurrLLA(msg.Name, msg.Pos);
        }
    }

    // private void ProcessMessage_PlatUpdate(EntityPosition msg)
    // {
    //     FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatUpdate: {msg.Name}");

    //     // Check if the platform exists
    //     if (FssEventDriver.DoesEntityExist(msg.Name))
    //     {
    //         FssEventDriver.SetPlatformCurrDetails(
    //             msg.Name, msg.Pos, msg.Attitude, msg.Course, msg.CourseDelta);
    //     }
    // }

    // // --------------------------------------------------------------------------------------------

    // private void ProcessMessage_PlatPosition(PlatPosition msg)
    // {
    //     FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatPosition: {msg.PlatName}");

    //     // Check if the platform exists
    //     if (FssEventDriver.DoesEntityExist(msg.PlatName))
    //     {
    //         FssEventDriver.SetPlatformPosition(msg.PlatName, msg.Pos);
    //         FssEventDriver.SetPlatformCourse(msg.PlatName, msg.Course);
    //         FssEventDriver.SetPlatformAttitude(msg.PlatName, msg.Attitude);
    //     }
    // }
}

