using System.Collections.Generic;

// FssCommandPlatAdd

public class FssCommandPlatAdd : FssCommand
{
    public FssCommandPlatAdd()
    {
        Signature.Add("plat");
        Signature.Add("add");
    }

    public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandPlatAdd.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        string retString = "";

        // Commands exist to perform their task, delete any pre-existing platform by the name
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            FssAppFactory.Instance.EventDriver.DeletePlatform(platName);
            retString += $"Platform {platName} deleted. ";
        }

        FssAppFactory.Instance.EventDriver.AddPlatform(platName);
        retString += $"Platform {platName} added.";

        // Set the default platform details - adding it with no location will create rendering div0's etc.
        FssAppFactory.Instance.EventDriver.DefaultPlatformDetails(platName);

        FssCentralLog.AddEntry($"FssCommandPlatAdd.Execute -> {retString}");
        return retString;
    }
}
