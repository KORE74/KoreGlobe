using System.Collections.Generic;

// GloCommandSimPause

public class GloCommandSimPause : GloCommand
{
    public GloCommandSimPause()
    {
        Signature.Add("sim");
        Signature.Add("pause");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandSimPause.Execute");

        GloAppFactory.Instance.ModelRun.Pause();

        return "Simulation paused";
    }
}