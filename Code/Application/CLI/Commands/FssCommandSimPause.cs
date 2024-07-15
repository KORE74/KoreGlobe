using System.Collections.Generic;

// FssCommandSimPause

public class FssCommandSimPause : FssCommand
{
    public FssCommandSimPause()
    {
        Signature.Add("sim");
        Signature.Add("pause");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandSimPause.Execute");

        FssAppFactory.Instance.ModelRun.Pause();

        return "Simulation paused";
    }
}