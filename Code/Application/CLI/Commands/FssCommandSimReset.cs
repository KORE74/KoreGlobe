using System.Collections.Generic;

// FssCommandSimReset

public class FssCommandSimReset : FssCommand
{
    public FssCommandSimReset()
    {
        Signature.Add("sim");
        Signature.Add("reset");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandSimReset.Execute");

        FssAppFactory.Instance.PlatformManager.Reset(); // EventDriver this
        FssAppFactory.Instance.EventDriver.SimClockReset();

        return "PlatformManager Reset (platform positions to start)";
    }
}
