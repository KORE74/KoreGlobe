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
        FssGodotFactory.Instance.UIState.ScenarioName = scenLoadMsg.ScenName;
    }

    private void ProcessMessage_ScenStart(ScenStart scenStartMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenStart");
        FssEventDriver.SimClockReset();
        FssEventDriver.SimClockStart();
    }

    private void ProcessMessage_ScenStop(ScenStop scenStopMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenStop");
        FssEventDriver.SimClockStop();
    }

    private void ProcessMessage_ScenPause(ScenPause scenPauseMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenPause: ScenTime:{scenPauseMsg.ScenTimeHMS}");
        FssEventDriver.SimClockStop();
        FssEventDriver.SetSimTimeHMS(scenPauseMsg.ScenTimeHMS);
    }

    private void ProcessMessage_ScenCont(ScenCont scenContMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ScenCont: ScenTime:{scenContMsg.ScenTimeHMS}");
        FssEventDriver.SimClockStart();
        FssEventDriver.SetSimTimeHMS(scenContMsg.ScenTimeHMS);
    }

    private void ProcessMessage_ClockSync(ClockSync clockSyncMsg)
    {
        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage_ClockSync: ScenTimeHMS:{clockSyncMsg.ScenTimeHMS}");
        FssEventDriver.SetSimTimeHMS(clockSyncMsg.ScenTimeHMS);
    }

}
