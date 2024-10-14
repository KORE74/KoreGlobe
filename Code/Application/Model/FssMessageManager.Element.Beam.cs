using System;
using System.Collections.Generic;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Beam
    // --------------------------------------------------------------------------------------------

    // Beam load - setting up the "port", the mounting point for a scan pattern

    private void ProcessMessage_BeamLoad(BeamLoad beamLoadMsg)
    {
        // Extract everything from the message for convenience
        // string platName = beamLoadMsg.PlatName;
        // double detectionRangeRxMtrs = beamLoadMsg.DetectionRangeRxMtrs;
        // double detectionRangeMtrs = beamLoadMsg.DetectionRangeMtrs;

        // //FssAzElBox azElBox = beamLoadMsg.AzElBox();

        FssAzElBox azElBox = new FssAzElBox() { MinAzDegs = -10, MaxAzDegs = 10, MinElDegs = -10, MaxElDegs = 10 };

        string elemName = FssEventDriver.ElementNameForBeam(beamLoadMsg.PlatName, beamLoadMsg.EmitName, beamLoadMsg.BeamName);

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamLoad: Name:{elemName}");

        FssAppFactory.Instance.EventDriver.PlatformAddBeam(beamLoadMsg.PlatName, elemName);
        FssAppFactory.Instance.EventDriver.PlatformSetBeamTargeting(beamLoadMsg.PlatName, elemName, beamLoadMsg.Targeted, beamLoadMsg.TargetPlatName);
        FssAppFactory.Instance.EventDriver.PlatformSetBeamRanges(beamLoadMsg.PlatName, elemName, beamLoadMsg.DetectionRangeMtrs, beamLoadMsg.DetectionRangeRxMtrs);
    }

    private void ProcessMessage_BeamDelete(BeamDelete beamDelMsg)
    {
        string elemName = FssEventDriver.ElementNameForBeam(beamDelMsg.PlatName, beamDelMsg.EmitName, beamDelMsg.BeamName);

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDelete: Name:{elemName}");

        //FssAppFactory.Instance.EventDriver.DeleteElement(beamDelMsg.PlatName, elemName);

        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(beamDelMsg.PlatName);
        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0022: BeamDelete: Platform {beamDelMsg.PlatName} not found.");
            return;
        }

        // Get the element
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);
    }

    private void ProcessMessage_BeamEnable(BeamEnable beamEnMsg)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(beamEnMsg.PlatName);
        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0022: BeamEnable: Platform {beamEnMsg.PlatName} not found.");
            return;
        }

        // Get the element
        string elemName = FssEventDriver.ElementNameForBeam(beamEnMsg.PlatName, beamEnMsg.EmitName, beamEnMsg.BeamName);
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            element!.Enabled = true;

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamEnable: Name:{elemName}");
    }

    private void ProcessMessage_BeamDisable(BeamDisable beamDisMsg)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(beamDisMsg.PlatName);
        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0022: BeamDisable: Platform {beamDisMsg.PlatName} not found.");
            return;
        }

        // Get the element
        string elemName = FssEventDriver.ElementNameForBeam(beamDisMsg.PlatName, beamDisMsg.EmitName, beamDisMsg.BeamName);
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            element!.Enabled = true;

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDisable: Name:{elemName}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Scan Pattern
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_ScanPattern(ScanPattern scanPatMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScanPattern");

        string elemName = FssEventDriver.ElementNameForBeam(scanPatMsg.PlatName, scanPatMsg.EmitName, scanPatMsg.BeamName);

        FssAppFactory.Instance.EventDriver.PlatformSetBeamAngles(scanPatMsg.PlatName, elemName, scanPatMsg.GetTrackOffset(), scanPatMsg.GetAzElBox());
        FssAppFactory.Instance.EventDriver.PlatformSetBeamScanType(scanPatMsg.PlatName, elemName, scanPatMsg.ScanType, scanPatMsg.PeriodSecs);


    }

    private void ProcessMessage_PlatformElement_AddCircularScan(PlatformElement_AddCircularScan platElemAddCircScanMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_PlatformElement_AddCircularScan: Name:{platElemAddCircScanMsg.PlatName}");
    }

}
