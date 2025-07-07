using System;

using GloNetworking;
using GloJSON;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class GloMessageManager
{
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatFocus(PlatFocus msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatFocus: {msg.PlatName}");
        GloGodotFactory.Instance.UIMsgQueue.EnqueueMessage(msg);
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatAdd(PlatAdd msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatAdd: {msg.PlatName}");

        // Check if the platform already exists
        if (!GloAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            GloAppFactory.Instance.EventDriver.AddPlatform(msg.PlatName, msg.PlatClass);
        }

        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            // Name (like tail number), Class (like F-16), Category (like aircraft)
            GloAppFactory.Instance.EventDriver.SetPlatformType(
                msg.PlatName, msg.PlatClass , msg.PlatCategory);

            GloAppFactory.Instance.EventDriver.SetPlatformStartDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course);

            GloAppFactory.Instance.EventDriver.SetPlatformCurrDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course, GloCourseDelta.Zero);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatDelete(PlatDelete msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatDelete: {msg.PlatName}");

        // Check if the platform exists
        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            GloAppFactory.Instance.EventDriver.DeletePlatform(msg.PlatName);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatUpdate(PlatUpdate msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatUpdate: {msg.PlatName}");

        // Check if the platform exists
        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            GloAppFactory.Instance.EventDriver.SetPlatformCurrDetails(
                msg.PlatName, msg.Pos, msg.Attitude, msg.Course, msg.CourseDelta);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatPosition(PlatPosition msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatPosition: {msg.PlatName}");

        // Check if the platform exists
        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(msg.PlatName))
        {
            GloAppFactory.Instance.EventDriver.SetPlatformPosition(msg.PlatName, msg.Pos);
            GloAppFactory.Instance.EventDriver.SetPlatformCourse(msg.PlatName, msg.Course);
            GloAppFactory.Instance.EventDriver.SetPlatformAttitude(msg.PlatName, msg.Attitude);
        }
    }
}

