using System;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    private void ProcessMessage_BeamLoad(BeamLoad beamLoadMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamLoad: Name:{beamLoadMsg.BeamName}");
    }

    private void ProcessMessage_BeamDelete(BeamDelete beamDelMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDelete: Name:{beamDelMsg.BeamName}");
    }

    private void ProcessMessage_BeamEnable(BeamEnable beamEnMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamEnable: Name:{beamEnMsg.BeamName}");
    }

    private void ProcessMessage_BeamDisable(BeamDisable beamDisMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_BeamDisable: Name:{beamDisMsg.BeamName}");
    }

    private void ProcessMessage_RxAntenna(RxAntenna rxAntMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_RxAntenna");
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

