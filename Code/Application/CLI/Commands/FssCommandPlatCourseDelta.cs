using System.Collections.Generic;

// FssCommandPlatCourseDelta

public class FssCommandPlatCourseDelta : FssCommand
{
    public FssCommandPlatCourseDelta()
    {
        Signature.Add("plat");
        Signature.Add("coursedelta");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <speedChangeMpMps> <headingChangeClockwiseDegsSec>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "FssCommandPlatCourseDelta.Execute -> insufficient parameters";
        }

        string platName    = parameters[0];
        double speedChangeMpMps              = double.Parse(parameters[1]);
        double headingChangeClockwiseDegsSec = double.Parse(parameters[2]);

        string retString = "";

        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            FssCourseDelta newCourseDelta = new FssCourseDelta() {
                    SpeedChangeMpMps = speedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = headingChangeClockwiseDegsSec };

            FssAppFactory.Instance.EventDriver.SetPlatformCourseDelta(platName, newCourseDelta);
            retString = $"Platform {platName} Updated: Course: {newCourseDelta}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatCourseDelta.Execute -> {retString}");
        return retString;
    }
}
