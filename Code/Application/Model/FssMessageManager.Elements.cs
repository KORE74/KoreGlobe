using System;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    private void ProcessMessage_BeamLoad(BeamLoad beamLoadMsg)
    {
        // Extract everything from the message for convenience
        string platName = beamLoadMsg.PlatName;
        double detectionRangeRxMtrs = beamLoadMsg.DetectionRangeRxMtrs;
        double detectionRangeMtrs = beamLoadMsg.DetectionRangeMtrs;
        FssAzElBox azElBox = beamLoadMsg.AzElBox();

        string elemName = FssEventDriver.ElementNameForBeam(
            beamLoadMsg.PlatName,
            beamLoadMsg.EmitName,
            beamLoadMsg.BeamName
        );

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamLoad: Name:{elemName}");

        FssAppFactory.Instance.EventDriver.PlatformAddScanWedge(platName, elemName, detectionRangeMtrs, detectionRangeRxMtrs, azElBox);
    }

    private void ProcessMessage_BeamDelete(BeamDelete beamDelMsg)
    {
        string elemName = FssEventDriver.ElementNameForBeam(
            beamDelMsg.PlatName,
            beamDelMsg.EmitName,
            beamDelMsg.BeamName
        );

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDelete: Name:{elemName}");

        //FssAppFactory.Instance.EventDriver.DeleteElement(beamDelMsg.PlatName, elemName);

        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(beamDelMsg.PlatName);
        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: BeamDelete: Platform {beamDelMsg.PlatName} not found.");
            return;
        }

        // Get the element
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);
    }

    private void ProcessMessage_BeamEnable(BeamEnable beamEnMsg)
    {
        string elemName = FssEventDriver.ElementNameForBeam(
            beamEnMsg.PlatName,
            beamEnMsg.EmitName,
            beamEnMsg.BeamName
        );

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamEnable: Name:{elemName}");

    }

    private void ProcessMessage_BeamDisable(BeamDisable beamDisMsg)
    {
        string elemName = FssEventDriver.ElementNameForBeam(
            beamDisMsg.PlatName,
            beamDisMsg.EmitName,
            beamDisMsg.BeamName
        );

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDisable: Name:{elemName}");

    }

    private void ProcessMessage_RxAntenna(RxAntenna rxAntMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_RxAntenna");

        string         platName           = rxAntMsg.PlatName;
        string         patternName        = rxAntMsg.PortName;
        FssAzElBox     patternSize        = rxAntMsg.AzElBox;
        FssPolarOffset patternPolarOffset = rxAntMsg.PolarOffset;

        FssAppFactory.Instance.EventDriver.PlatformAddAntennaPattern(platName, patternName, patternPolarOffset, patternSize);
    }

    private void ProcessMessage_ScanPattern(ScanPattern scanPatMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScanPattern");
    }

    private void ProcessMessage_PlatWayPoints(PlatWayPoints platWayPtsMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatWayPoints: Name:{platWayPtsMsg.PlatName}");
    }

    private void ProcessMessage_PlatformElement_AddCircularScan(PlatformElement_AddCircularScan platElemAddCircScanMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatformElement_AddCircularScan: Name:{platElemAddCircScanMsg.PlatName}");
    }
}
