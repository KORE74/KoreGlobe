using System.Collections.Generic;

// GloCommandVersion

public class GloCommandPlatReportPos : GloCommand
{
    public GloCommandPlatReportPos()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("pos");
    }

    public override string Execute(List<string> parameters)
    {
        int num = GloAppFactory.Instance.EventDriver.NumPlatforms();
        string rep = GloAppFactory.Instance.EventDriver.PlatformPositionsReport();

        GloCentralLog.AddEntry("GloCommandPlatReport.Execute: " + KoreGlobals.VersionString);
        return $"Platform Positions Report:\n{rep}";
    }
}
