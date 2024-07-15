using System.Collections.Generic;

// FssCommandSimStop

public class FssCommandSimStart : FssCommand
{
    public FssCommandSimStart()
    {
        Signature.Add("sim");
        Signature.Add("start");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandSimStart.Execute");

        FssAppFactory.Instance.ModelRun.Start();

        return "Simulation started";
    }
}