using System;
using System.Collections.Generic;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Waypoints
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatWayPoints(PlatWayPoints platWayPtsMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatWayPoints: Name:{platWayPtsMsg.PlatName}");

        string platName          = platWayPtsMsg.PlatName;
        List<FssLLAPoint> points = platWayPtsMsg.Points();

        FssAppFactory.Instance.EventDriver.PlatformSetRoute(platName, points);
    }

}
