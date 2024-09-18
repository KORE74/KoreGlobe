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

    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Antenna Patterns
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_AntennaPattern(AntennaPattern antPatternMsg)
    {
        string sizeStr = $"Size:{antPatternMsg.AzPointCount}x{antPatternMsg.ElPointCount} ArrayCount:{antPatternMsg.Pattern.Count}";
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_AntennaPattern: PlatName:{antPatternMsg.PlatName} // {sizeStr}");

        string platName = antPatternMsg.PlatName;
        string portName = antPatternMsg.PortName;

        FssAppFactory.Instance.EventDriver.PlatformSetAntennaPatternMetadata(platName, portName, antPatternMsg.AzElBox, antPatternMsg.PolarOffset);
        FssAppFactory.Instance.EventDriver.PlatformSetAntennaPatternData(platName, portName, antPatternMsg.AzPointCount, antPatternMsg.ElPointCount, antPatternMsg.Pattern);
    }


}
