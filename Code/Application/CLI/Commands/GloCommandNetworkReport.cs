using System.Collections.Generic;

// GloCommandNetworkReport

public class GloCommandNetworkReport : GloCommand
{
    public GloCommandNetworkReport()
    {
        Signature.Add("network");
        Signature.Add("report");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandNetworkReport.Execute");

        return GloAppFactory.Instance.EventDriver.NetworkReport();
    }
}