using System.Collections.Generic;

// FssCommandPlatDelete

public class FssCommandPlatPosition : FssCommand
{
    public FssCommandPlatPosition()
    {
        Signature.Add("plat");
        Signature.Add("pos");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <LatDegs> <LonDegs> <AltMslM>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 4)
        {
            return "FssCommandPlatPosition.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        double latDegs = double.Parse(parameters[1]);
        double lonDegs = double.Parse(parameters[2]);
        double altMslM = double.Parse(parameters[3]);

        string retString = "";

        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            FssLLAPoint newLLA = new FssLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, AltMslM = altMslM };

            FssAppFactory.Instance.EventDriver.SetPlatformStartLLA(platName, newLLA);
            retString = $"Platform {platName} Updated: Position: {newLLA}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatPosition.Execute -> {retString}");
        return retString;
    }
}
