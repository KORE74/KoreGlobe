using System.Collections.Generic;

// FssCommandVersion

public class FssCommandPlatReport : FssCommand
{
    public FssCommandPlatReport()
    {
        Signature.Add("platreport");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandPlatReport.Execute: " + FssGlobals.VersionString);
        return "FssCommandPlatReport";
    }

}
