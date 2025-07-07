using System.Collections.Generic;

// GloCommandSimStop

public class GloCommandSimStart : GloCommand
{
    public GloCommandSimStart()
    {
        Signature.Add("sim");
        Signature.Add("start");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandSimStart.Execute");

        GloAppFactory.Instance.ModelRun.Start();

        return "Simulation started";
    }
}