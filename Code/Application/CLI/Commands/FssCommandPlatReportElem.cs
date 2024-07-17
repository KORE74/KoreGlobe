using System.Collections.Generic;

// FssCommandVersion

public class FssCommandPlatReportElem : FssCommand
{
    public FssCommandPlatReportElem()
    {
        Signature.Add("plat");
        Signature.Add("report");
        Signature.Add("elem");
    }

    public override string Execute(List<string> parameters)
    {
        int num = FssAppFactory.Instance.EventDriver.NumPlatforms();
        string rep = FssAppFactory.Instance.EventDriver.PlatformElementsReport();

        FssCentralLog.AddEntry("PlatformElementsReport.Execute: " + FssGlobals.VersionString);
        return $"Platform Elements Report:\n{rep}";
    }
}
