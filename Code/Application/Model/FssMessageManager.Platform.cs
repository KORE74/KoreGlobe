using System;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatAdd(PlatAdd msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatAdd: {msg.PlatName}");

        // Check if the platform already exists
        if (!FssAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            FssAppFactory.Instance.EventDriver.AddPlatform(msg.PlatName, msg.PlatClass);
        }

        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            // Name (like tail number), Class (like F-16), Category (like aircraft)
            FssAppFactory.Instance.EventDriver.SetPlatformType(
                msg.PlatName, msg.PlatClass , msg.PlatCategory);

            FssAppFactory.Instance.EventDriver.SetPlatformStartDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course);

            FssAppFactory.Instance.EventDriver.SetPlatformCurrDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course, FssCourseDelta.Zero);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatDelete(PlatDelete msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatDelete: {msg.PlatName}");

        // Check if the platform exists
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            FssAppFactory.Instance.EventDriver.DeletePlatform(msg.PlatName);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatUpdate(PlatUpdate msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatUpdate: {msg.PlatName}");

        // Check if the platform exists
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            FssAppFactory.Instance.EventDriver.SetPlatformCurrDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course, msg.CourseDelta);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatPosition(PlatPosition msg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatPosition: {msg.PlatName}");

        // Check if the platform exists
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            FssAppFactory.Instance.EventDriver.SetPlatformPosition(msg.PlatName, msg.Pos);
            FssAppFactory.Instance.EventDriver.SetPlatformCourse(msg.PlatName, msg.Course);
            FssAppFactory.Instance.EventDriver.SetPlatformAttitude(msg.PlatName, msg.Attitude);
        }
    }
}

