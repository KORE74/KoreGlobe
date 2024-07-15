using System.Collections.Generic;

// FssCommandSimStop

public class FssCommandSimStop : FssCommand
{
    public FssCommandSimStop()
    {
        Signature.Add("sim");
        Signature.Add("stop");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandSimStop.Execute");

        FssAppFactory.Instance.ModelRun.Stop();

        return "Simulation stopped";
    }
}
