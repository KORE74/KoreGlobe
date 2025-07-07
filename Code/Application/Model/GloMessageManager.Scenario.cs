using System;

using GloNetworking;
using GloJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class GloMessageManager
{
    private void ProcessMessage_ScenLoad(ScenLoad scenLoadMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScenLoad: Name:{scenLoadMsg.ScenName} ScenPos:{scenLoadMsg.ScenPos}");
        GloGodotFactory.Instance.UIState.ScenarioName = scenLoadMsg.ScenName;
        GloAppFactory.Instance.EventDriver.DeleteAllPlatforms();
    }

    private void ProcessMessage_ScenStart(ScenStart scenStartMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScenStart");
        GloAppFactory.Instance.EventDriver.SimClockReset();
        GloAppFactory.Instance.EventDriver.SimClockStart();
    }

    private void ProcessMessage_ScenStop(ScenStop scenStopMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScenStop");
        GloAppFactory.Instance.EventDriver.SimClockStop();
        GloAppFactory.Instance.EventDriver.DeleteElementAllBeams();
    }

    private void ProcessMessage_ScenPause(ScenPause scenPauseMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScenPause: ScenTime:{scenPauseMsg.ScenTimeHMS}");
        GloAppFactory.Instance.EventDriver.SimClockStop();
        GloAppFactory.Instance.EventDriver.SetSimTimeHMS(scenPauseMsg.ScenTimeHMS);
    }

    private void ProcessMessage_ScenCont(ScenCont scenContMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ScenCont: ScenTime:{scenContMsg.ScenTimeHMS}");
        GloAppFactory.Instance.EventDriver.SetSimTimeHMS(scenContMsg.ScenTimeHMS);
        GloAppFactory.Instance.EventDriver.SimClockResume();
    }

    private void ProcessMessage_ClockSync(ClockSync clockSyncMsg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_ClockSync: ScenTimeHMS:{clockSyncMsg.ScenTimeHMS}");
        GloAppFactory.Instance.EventDriver.SetSimTimeHMS(clockSyncMsg.ScenTimeHMS);
    }

}
