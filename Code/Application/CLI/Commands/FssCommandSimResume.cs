using System.Collections.Generic;

// FssCommandSimStop

public class FssCommandSimResume : FssCommand
{
    public FssCommandSimResume()
    {
        Signature.Add("sim");
        Signature.Add("resume");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandSimResume.Execute");

        FssAppFactory.Instance.ModelRun.Resume();

        return "Simulation resumed";
    }
}