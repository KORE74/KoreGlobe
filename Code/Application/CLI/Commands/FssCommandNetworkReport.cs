using System.Collections.Generic;

// FssCommandNetworkReport

public class FssCommandNetworkReport : FssCommand
{
    public FssCommandNetworkReport()
    {
        Signature.Add("network");
        Signature.Add("report");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandNetworkReport.Execute");

        return FssAppFactory.Instance.EventDriver.NetworkReport();
    }
}