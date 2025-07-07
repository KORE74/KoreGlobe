using System.Collections.Generic;

// GloCommandPlatCourseDelta

public class GloCommandPlatCourseDelta : GloCommand
{
    public GloCommandPlatCourseDelta()
    {
        Signature.Add("plat");
        Signature.Add("coursedelta");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <speedChangeMpMps> <headingChangeClockwiseDegsSec>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "GloCommandPlatCourseDelta.Execute -> insufficient parameters";
        }

        string platName    = parameters[0];
        double speedChangeMpMps              = double.Parse(parameters[1]);
        double headingChangeClockwiseDegsSec = double.Parse(parameters[2]);

        string retString = "";

        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            GloCourseDelta newCourseDelta = new GloCourseDelta() {
                    SpeedChangeMpMps = speedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = headingChangeClockwiseDegsSec };

            GloAppFactory.Instance.EventDriver.SetPlatformCourseDelta(platName, newCourseDelta);
            retString = $"Platform {platName} Updated: Course: {newCourseDelta}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        GloCentralLog.AddEntry($"GloCommandPlatCourseDelta.Execute -> {retString}");
        return retString;
    }
}
