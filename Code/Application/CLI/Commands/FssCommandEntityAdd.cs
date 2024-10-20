using System.Collections.Generic;

// FssCommandPlatAdd

public class FssCommandEntityAdd : FssCommand
{
    public FssCommandEntityAdd()
    {
        Signature.Add("ent");
        Signature.Add("add");
    }

    public override string HelpString => $"{SignatureString} <entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandEntityAdd.Execute -> insufficient parameters";
        }

        string entName  = parameters[0];
        string retString = "";

        // Commands exist to perform their task, delete any pre-existing platform by the name
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(entName))
        {
            FssAppFactory.Instance.EventDriver.DeletePlatform(entName);
            retString += $"Platform {entName} deleted. ";
        }

        FssAppFactory.Instance.EventDriver.AddPlatform(entName);
        retString += $"Platform {entName} added.";

        // Set the default platform details - adding it with no location will create rendering div0's etc.
        FssAppFactory.Instance.EventDriver.DefaultPlatformDetails(entName);

        FssCentralLog.AddEntry($"FssCommandEntityAdd.Execute -> {retString}");
        return retString;
    }
}
