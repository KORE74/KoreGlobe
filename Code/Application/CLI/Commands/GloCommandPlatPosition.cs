using System.Collections.Generic;

// GloCommandPlatDelete

public class GloCommandPlatPosition : GloCommand
{
    public GloCommandPlatPosition()
    {
        Signature.Add("plat");
        Signature.Add("pos");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <LatDegs> <LonDegs> <AltMslM>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 4)
        {
            return "GloCommandPlatPosition.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        double latDegs = double.Parse(parameters[1]);
        double lonDegs = double.Parse(parameters[2]);
        double altMslM = double.Parse(parameters[3]);

        string retString = "";

        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            GloLLAPoint newLLA = new GloLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, AltMslM = altMslM };

            GloAppFactory.Instance.EventDriver.SetPlatformStartLLA(platName, newLLA);
            retString = $"Platform {platName} Updated: Position: {newLLA}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        GloCentralLog.AddEntry($"GloCommandPlatPosition.Execute -> {retString}");
        return retString;
    }
}
