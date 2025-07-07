using System;
using System.Collections.Generic;

using GloNetworking;
using GloJSON;
using Godot;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class GloMessageManager
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

        // //GloAzElBox azElBox = beamLoadMsg.AzElBox();

        GloAzElBox azElBox = new GloAzElBox() { MinAzDegs = -10, MaxAzDegs = 10, MinElDegs = -10, MaxElDegs = 10 };

        string elemName = GloEventDriver.ElementNameForBeam(beamLoadMsg.PlatName, beamLoadMsg.EmitName, beamLoadMsg.BeamName);

        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_BeamLoad: Name:{elemName}");

        GloAppFactory.Instance.EventDriver.PlatformAddBeam(beamLoadMsg.PlatName, elemName);
        GloAppFactory.Instance.EventDriver.PlatformSetBeamTargeting(beamLoadMsg.PlatName, elemName, beamLoadMsg.Targeted, beamLoadMsg.TargetPlatName);
        GloAppFactory.Instance.EventDriver.PlatformSetBeamRanges(beamLoadMsg.PlatName, elemName, beamLoadMsg.DetectionRangeMtrs, beamLoadMsg.DetectionRangeRxMtrs);
        GloAppFactory.Instance.EventDriver.PlatformSetBeamPortAngles(beamLoadMsg.PlatName, elemName, beamLoadMsg.PortAttitude);

        GD.Print($"GloMessageManager.ProcessMessage_BeamLoad: Name:{elemName} // {beamLoadMsg.PortAttitude}");
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_BeamDelete(BeamDelete beamDelMsg)
    {
        string elemName = GloEventDriver.ElementNameForBeam(beamDelMsg.PlatName, beamDelMsg.EmitName, beamDelMsg.BeamName);

        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_BeamDelete: Name:{elemName}");

        //GloAppFactory.Instance.EventDriver.DeleteElement(beamDelMsg.PlatName, elemName);

        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(beamDelMsg.PlatName);
        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0022: BeamDelete: Platform {beamDelMsg.PlatName} not found.");
            return;
        }

        // Get the element
        GloPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_BeamEnable(BeamEnable beamEnMsg)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(beamEnMsg.PlatName);
        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0022: BeamEnable: Platform {beamEnMsg.PlatName} not found.");
            return;
        }

        // Get the element
        string elemName = GloEventDriver.ElementNameForBeam(beamEnMsg.PlatName, beamEnMsg.EmitName, beamEnMsg.BeamName);
        GloPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            element!.Enabled = true;

        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_BeamEnable: Name:{elemName}");
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_BeamDisable(BeamDisable beamDisMsg)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(beamDisMsg.PlatName);
        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0022: BeamDisable: Platform {beamDisMsg.PlatName} not found.");
            return;
        }

        // Get the element
        string elemName = GloEventDriver.ElementNameForBeam(beamDisMsg.PlatName, beamDisMsg.EmitName, beamDisMsg.BeamName);
        GloPlatformElement? element = platform.ElementForName(elemName);

        if (element != null)
            element!.Enabled = false;

        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_BeamDisable: Name:{elemName}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elements / Scan Pattern
    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_ScanPattern(ScanPattern scanPatMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScanPattern");

        string elemName = GloEventDriver.ElementNameForBeam(scanPatMsg.PlatName, scanPatMsg.EmitName, scanPatMsg.BeamName);

        GloAppFactory.Instance.EventDriver.PlatformSetBeamAngles(scanPatMsg.PlatName, elemName, scanPatMsg.GetTrackOffset(), scanPatMsg.GetAzElBox());
        GloAppFactory.Instance.EventDriver.PlatformSetBeamScanType(scanPatMsg.PlatName, elemName, scanPatMsg.ScanType, scanPatMsg.PeriodSecs);
    }

    // --------------------------------------------------------------------------------------------

    // private void ProcessMessage_PlatformElement_AddCircularScan(PlatformElement_AddCircularScan platElemAddCircScanMsg)
    // {
    //     GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatformElement_AddCircularScan: Name:{platElemAddCircScanMsg.PlatName}");
    // }

}
