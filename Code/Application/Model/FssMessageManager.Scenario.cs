using System;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    private void ProcessMessage_ScenLoad(ScenLoad scenLoadMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenLoad: Name:{scenLoadMsg.ScenName} ScenPos:{scenLoadMsg.ScenPos}");
    }

    private void ProcessMessage_ScenStart(ScenStart scenStartMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenStart");
        FssAppFactory.Instance.EventDriver.ClockReset();
        FssAppFactory.Instance.EventDriver.ClockStart();
    }

    private void ProcessMessage_ScenStop(ScenStop scenStopMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenStop");
        FssAppFactory.Instance.EventDriver.ClockStop();
    }

    private void ProcessMessage_ScenPause(ScenPause scenPauseMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenPause: ScenTime:{scenPauseMsg.ScenTime}");
        FssAppFactory.Instance.EventDriver.ClockStop();
    }

    private void ProcessMessage_ScenCont(ScenCont scenContMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenCont: ScenTime:{scenContMsg.ScenTime}");
        FssAppFactory.Instance.EventDriver.ClockStart();
    }

    private void ProcessMessage_ClockSync(ClockSync clockSyncMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ClockSync: ScenTimeHMS:{clockSyncMsg.ScenTimeHMS}");
    }

}

