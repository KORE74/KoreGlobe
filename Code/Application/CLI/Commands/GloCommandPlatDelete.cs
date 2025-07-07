using System.Collections.Generic;

// GloCommandPlatDelete

public class GloCommandPlatDelete : GloCommand
{

    public GloCommandPlatDelete()
    {
        Signature.Add("plat");
        Signature.Add("del");
    }

    public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "GloCommandPlatDelete.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        string retString = "";

        // Delete the platform
        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            GloAppFactory.Instance.EventDriver.DeletePlatform(platName);
            retString = $"Platform {platName} deleted.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        GloCentralLog.AddEntry($"GloCommandPlatDelete.Execute -> {retString}");
        return retString;
    }
}
