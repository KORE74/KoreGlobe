using System.Collections.Generic;

// GloCommandPlatDelete

public class GloCommandPlatDeleteAll : GloCommand
{
    public GloCommandPlatDeleteAll()
    {
        Signature.Add("plat");
        Signature.Add("delall");
    }

    public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 0)
        {
            return "GloCommandPlatDeleteAll.Execute -> incorrect number of parameters";
        }

        GloAppFactory.Instance.EventDriver.DeleteAllPlatforms();

        GloCentralLog.AddEntry($"GloCommandPlatDeleteAll.Execute");

        string retString = "Done.";
        return retString;
    }
}
