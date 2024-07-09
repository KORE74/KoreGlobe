using System;
using System.Collections.Generic;

using FssJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public class FssMessageManagerPlatform
{
    // Pool of incoming messages to queue up for processing
    private JSONThreadsafeMessageFIFO PendingMessageList = new();

    // Platform manager, top level of the platform data structure to apply messages to.
    //public FssPlatformManager PlatformManager { get; set; }

    // --------------------------------------------------------------------------------------------

    public void QueueMessage(JSONMessage msg)
    {
        PendingMessageList.EnqueueMessage(msg);
    }

    public void ApplyMessages()
    {
        while (PendingMessageList.TryDequeueMessage(out JSONMessage currMsg))

        //foreach (JSONMessage currMsg in PendingMessageList)
        {
            if (currMsg != null)
            {
                // Get the message type and cast it
                if      (currMsg is PlatAdd addMsg)           { ProcessMessage_PlatAdd(addMsg); }
                else if (currMsg is PlatDelete deleteMsg)     { ProcessMessage_PlatDelete(deleteMsg); }
                else if (currMsg is PlatUpdate updateMsg)     { ProcessMessage_PlatUpdate(updateMsg); } // Update = runtime
                else if (currMsg is PlatPosition positionMsg) { ProcessMessage_PlatPosition(positionMsg); } // Position = static
            }
        }
    }

    // ========================================================================================
    // Platform Messages
    // ========================================================================================

    private void ProcessMessage_PlatAdd(PlatAdd msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Add the new platform. If the function returns a non-null value, assign the other message fields.
        FssPlatform? newPlat = platMgr.Add(msg.PlatName);

        if (newPlat != null)
        {
            newPlat.Type                   = msg.PlatClass;
            newPlat.Kinetics.StartPosition = msg.Pos;
            newPlat.Kinetics.StartAttitude = msg.Attitude;
            newPlat.Kinetics.CurrCourse    = msg.Course;
        }
    }

    private void ProcessMessage_PlatDelete(PlatDelete msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Debug.Log($"ProcessMessage_PlatDelete");
        platMgr.Delete(msg.PlatName);
    }

    private void ProcessMessage_PlatUpdate(PlatUpdate msg)
    {
        //Debug.Log($"Received PlatUpdate: PlatName = {msg.PlatName}, course = {msg.GetCourse().HeadingDegs}, speed = {msgUpdate.GetCourse().SpeedKph}kph, lat = {msgUpdate.LatDegs}, lon = {msgUpdate.LonDegs}, alt = {msgUpdate.AltMslKm}, roll = {msgUpdate.RollClockwiseDegs}, pitch = {msgUpdate.PitchUpDegs}, yaw = {msgUpdate.YawClockwiseDegs}");
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Get the new platform. If the function returns a non-null value, assign the other message fields.
        FssPlatform? currPlat = platMgr.PlatForName(msg.PlatName);
        if (currPlat != null)
        {
            currPlat.Kinetics.CurrPosition    = msg.Pos;
            currPlat.Kinetics.CurrAttitude    = msg.Attitude;
            currPlat.Kinetics.CurrCourse      = msg.Course;
            currPlat.Kinetics.CurrCourseDelta = new FssCourseDelta() { HeadingChangeClockwiseDegsSec = msg.TurnRateDegsSec * -1 };
        }
    }

    private void ProcessMessage_PlatPosition(PlatPosition msg)
    {
        // Debug.Log($"ProcessMessage_PlatPosition");
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Get the new platform. If the function returns a non-null value, assign the other message fields.
        FssPlatform? currPlat = platMgr.PlatForName(msg.PlatName);
        if (currPlat != null)
        {
            currPlat.Kinetics.CurrPosition    = msg.Pos;
            currPlat.Kinetics.CurrAttitude    = msg.Attitude;
            currPlat.Kinetics.CurrCourse      = msg.Course;
        }
    }

    // ========================================================================================
    // Utils
    // ========================================================================================

    // Isolate the external architectural access to the platform manager in a utility function.
    private FssPlatform? PlatformForName(string name)
    {
        return FssAppFactory.Instance.PlatformManager.PlatForName(name);
    }

    private FssPlatformManager? PlatMgr()
    {
        return FssAppFactory.Instance.PlatformManager;
    }
}


