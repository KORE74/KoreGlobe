using System.Collections.Generic;

// GloCommandVersion

public class GloCommandPlatReportElem : GloCommand
{
    public GloCommandPlatReportElem()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("elem");
    }

    public override string Execute(List<string> parameters)
    {
        int num = GloAppFactory.Instance.EventDriver.NumPlatforms();
        string rep = GloAppFactory.Instance.EventDriver.PlatformElementsReport();

        GloCentralLog.AddEntry("PlatformElementsReport.Execute: " + KoreGlobals.VersionString);
        return $"Platform Elements Report:\n{rep}";
    }
}
