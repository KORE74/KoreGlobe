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
        if (!FssAppFactory.Instance.EventDriver.DoesEntityExist(msg.Name))
        {
            FssAppFactory.Instance.EventDriver.AddEntity(msg.Name);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityDelete(EntityDelete msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_EntityDelete: {msg.Name}");

        // Check if the platform exists
        if (FssAppFactory.Instance.EventDriver.DoesEntityExist(msg.Name))
        {
            FssAppFactory.Instance.EventDriver.DeleteEntity(msg.Name);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_EntityPosition(EntityPosition msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_EntityPosition: {msg.Name}");

        // Check if the platform exists
        if (FssAppFactory.Instance.EventDriver.DoesEntityExist(msg.Name))
        {
            FssAppFactory.Instance.EventDriver.SetEntityCurrLLA(msg.Name, msg.Pos);
        }
    }

    // private void ProcessMessage_PlatUpdate(EntityPosition msg)
    // {
    //     FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatUpdate: {msg.Name}");

    //     // Check if the platform exists
    //     if (FssAppFactory.Instance.EventDriver.DoesEntityExist(msg.Name))
    //     {
    //         FssAppFactory.Instance.EventDriver.SetPlatformCurrDetails(
    //             msg.Name, msg.Pos, msg.Attitude, msg.Course, msg.CourseDelta);
    //     }
    // }

    // // --------------------------------------------------------------------------------------------

    // private void ProcessMessage_PlatPosition(PlatPosition msg)
    // {
    //     FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatPosition: {msg.PlatName}");

    //     // Check if the platform exists
    //     if (FssAppFactory.Instance.EventDriver.DoesEntityExist(msg.PlatName))
    //     {
    //         FssAppFactory.Instance.EventDriver.SetPlatformPosition(msg.PlatName, msg.Pos);
    //         FssAppFactory.Instance.EventDriver.SetPlatformCourse(msg.PlatName, msg.Course);
    //         FssAppFactory.Instance.EventDriver.SetPlatformAttitude(msg.PlatName, msg.Attitude);
    //     }
    // }
}

