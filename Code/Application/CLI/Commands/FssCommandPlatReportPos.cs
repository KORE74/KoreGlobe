using System.Collections.Generic;

// FssCommandVersion

public class FssCommandPlatReportPos : FssCommand
{
    public FssCommandPlatReportPos()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("pos");
    }

    public override string Execute(List<string> parameters)
    {
        int num = FssAppFactory.Instance.EventDriver.NumPlatforms();
        string rep = FssAppFactory.Instance.EventDriver.PlatformPositionsReport();

        FssCentralLog.AddEntry("FssCommandPlatReport.Execute: " + FssGlobals.VersionString);
        return $"Platform Positions Report:\n{rep}";
    }
}
