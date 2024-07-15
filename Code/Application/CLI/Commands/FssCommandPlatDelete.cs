using System.Collections.Generic;

// FssCommandPlatDelete

public class FssCommandPlatDelete : FssCommand
{

    public FssCommandPlatDelete()
    {
        Signature.Add("plat");
        Signature.Add("del");
    }

    public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandPlatDelete.Execute -> insufficient parameters";
        }

        string platName = parameters[0];
        string retString = "";

        // Delete the platform
        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            FssAppFactory.Instance.EventDriver.DeletePlatform(platName);
            retString = $"Platform {platName} deleted.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatDelete.Execute -> {retString}");
        return retString;
    }
}
