using System;
using System.Collections.Generic;

using GloNetworking;
using GloJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class GloMessageManager
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Waypoints
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatWayPoints(PlatWayPoints platWayPtsMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatWayPoints: Name:{platWayPtsMsg.PlatName}");

        string platName          = platWayPtsMsg.PlatName;
        List<GloLLAPoint> points = platWayPtsMsg.Points();



        GloAppFactory.Instance.EventDriver.PlatformSetRoute(platName, points);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Antenna Patterns
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_AntennaPattern(AntennaPattern antPatternMsg)
    {
        string sizeStr = $"Size:{antPatternMsg.AzPointCount}x{antPatternMsg.ElPointCount} ArrayCount:{antPatternMsg.Pattern.Count}";
        //GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_AntennaPattern: PlatName:{antPatternMsg.PlatName} // {sizeStr}");

        string platName = antPatternMsg.PlatName;
        string portName = antPatternMsg.PortName;

        GloAppFactory.Instance.EventDriver.PlatformSetAntennaPatternMetadata(platName, portName, antPatternMsg.AzElBox, antPatternMsg.PolarOffset);

        int azPointCount   = antPatternMsg.AzPointCount;
        int elPointCount   = antPatternMsg.ElPointCount;
        int dataPointCount = antPatternMsg.Pattern.Count;

        // check AP and assign unaffected.
        if (azPointCount * elPointCount == dataPointCount)
        {
            GloAppFactory.Instance.EventDriver.PlatformSetAntennaPatternData(platName, portName, antPatternMsg.AzPointCount, antPatternMsg.ElPointCount, antPatternMsg.Pattern);
            GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_AntennaPattern: PlatName:{antPatternMsg.PlatName} // {sizeStr} // Adding a +1 array");
        }
        else if ( (azPointCount+1) * (elPointCount+1) == dataPointCount)
        {
            GloAppFactory.Instance.EventDriver.PlatformSetAntennaPatternData(
                platName, portName,
                antPatternMsg.AzPointCount + 1,
                antPatternMsg.ElPointCount + 1,
                antPatternMsg.Pattern);
            GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_AntennaPattern: PlatName:{antPatternMsg.PlatName} // {sizeStr} // Adding a +1 array");

        }
        else
        {
            GloCentralLog.AddEntry("ProcessMessage_AntennaPattern: Size Issue");
        }
    }

}
