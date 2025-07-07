using System.Collections.Generic;

// GloCommandPlatAdd

public class GloCommandPlatAdd : GloCommand
{
    public GloCommandPlatAdd()
    {
        Signature.Add("plat");
        Signature.Add("add");
    }

    public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "GloCommandPlatAdd.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        string retString = "";

        // Commands exist to perform their task, delete any pre-existing platform by the name
        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            GloAppFactory.Instance.EventDriver.DeletePlatform(platName);
            retString += $"Platform {platName} deleted. ";
        }

        GloAppFactory.Instance.EventDriver.AddPlatform(platName);
        retString += $"Platform {platName} added.";

        // Set the default platform details - adding it with no location will create rendering div0's etc.
        GloAppFactory.Instance.EventDriver.DefaultPlatformDetails(platName);

        GloCentralLog.AddEntry($"GloCommandPlatAdd.Execute -> {retString}");
        return retString;
    }
}
