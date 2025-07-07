using System.Collections.Generic;

// GloCommandSimStop

public class GloCommandSimStop : GloCommand
{
    public GloCommandSimStop()
    {
        Signature.Add("sim");
        Signature.Add("stop");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandSimStop.Execute");

        GloAppFactory.Instance.ModelRun.Stop();

        return "Simulation stopped";
    }
}
