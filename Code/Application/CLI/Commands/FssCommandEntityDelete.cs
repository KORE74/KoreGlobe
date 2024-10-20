using System.Collections.Generic;

// FssCommandPlatDelete

public class FssCommandEntityDelete : FssCommand
{

    public FssCommandEntityDelete()
    {
        Signature.Add("ent");
        Signature.Add("del");
    }

    public override string HelpString => $"{SignatureString} <entity_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandEntityDelete.Execute -> insufficient parameters";
        }

        string entName  = parameters[0];
        string retString = "";

        // Delete the platform
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(entName))
        {
            FssAppFactory.Instance.EventDriver.DeletePlatform(entName);
            retString = $"Platform {entName} deleted.";
        }
        else
        {
            retString = $"Platform {entName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatDelete.Execute -> {retString}");
        return retString;
    }
}
