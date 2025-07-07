using System.Collections.Generic;

// GloCommandVersion

public class GloCommandVersion : GloCommand
{
    public GloCommandVersion()
    {
        Signature.Add("version");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandVersion.Execute: " + KoreGlobals.VersionString);
        return KoreGlobals.VersionString;
    }

}
