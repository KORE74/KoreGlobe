using System.Collections.Generic;

// GloCommandPlatDelete

public class GloCommandPlatDeleteAllEmitters : GloCommand
{
    public GloCommandPlatDeleteAllEmitters()
    {
        Signature.Add("plat");
        Signature.Add("delallemitters");
    }

    // public override string HelpString => $"{SignatureString} <platform_name>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 0)
        {
            return "GloCommandPlatDeleteAllEmitters.Execute -> incorrect number of parameters";
        }

        GloAppFactory.Instance.EventDriver.DeleteElementAllBeams();

        GloCentralLog.AddEntry($"GloCommandPlatDeleteAllEmitters.Execute");

        string retString = "Done.";
        return retString;
    }
}
