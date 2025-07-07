using System.Collections.Generic;

// GloCommandSimStop

public class GloCommandSimResume : GloCommand
{
    public GloCommandSimResume()
    {
        Signature.Add("sim");
        Signature.Add("resume");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandSimResume.Execute");

        GloAppFactory.Instance.ModelRun.Resume();

        return "Simulation resumed";
    }
}