using System.Collections.Generic;

// FssCommandPlatCourseDelta

public class FssCommandEntityCourse : FssCommand
{
    public FssCommandEntityCourse()
    {
        Signature.Add("ent");
        Signature.Add("course");
    }

    public override string HelpString => $"{SignatureString} <entity_name> ";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 1)
        {
            return "FssCommandPlatCourseDelta.Execute -> insufficient parameters";
        }

        string entName                       = parameters[0];
        double speedChangeMpMps              = double.Parse(parameters[1]);
        double headingChangeClockwiseDegsSec = double.Parse(parameters[2]);

        string retString = "";

        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(entName))
        {
            FssCourseDelta newCourseDelta = new FssCourseDelta() {
                    SpeedChangeMpMps = speedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = headingChangeClockwiseDegsSec };

            FssAppFactory.Instance.EventDriver.SetPlatformCourseDelta(entName, newCourseDelta);
            retString = $"Platform {entName} Updated: Course: {newCourseDelta}.";
        }
        else
        {
            retString = $"Platform {entName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatCourse.Execute -> {retString}");
        return retString;
    }
}
