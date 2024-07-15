using System.Collections.Generic;

// FssCommandVersion

public class FssCommandPlatReport : FssCommand
{
    public FssCommandPlatReport()
    {
        Signature.Add("plat");
        Signature.Add("report");
    }

    public override string Execute(List<string> parameters)
    {
        int num = FssAppFactory.Instance.EventDriver.NumPlatforms();
        string rep = FssAppFactory.Instance.EventDriver.PlatformReport();

        FssCentralLog.AddEntry("FssCommandPlatReport.Execute: " + FssGlobals.VersionString);
        return $"Platform Report:\n Number of Platforms: {num}\n{rep}";
    }
}
