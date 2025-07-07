using System.Collections.Generic;

// GloCommandSimReset

public class GloCommandSimReset : GloCommand
{
    public GloCommandSimReset()
    {
        Signature.Add("sim");
        Signature.Add("reset");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandSimReset.Execute");

        GloAppFactory.Instance.PlatformManager.Reset(); // EventDriver this
        GloAppFactory.Instance.EventDriver.SimClockReset();

        return "PlatformManager Reset (platform positions to start)";
    }
}
