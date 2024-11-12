using System.Collections.Generic;

// FssCommandEntityCourse

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
            return "FssCommandEntityCourse.Execute -> insufficient parameters";
        }

        string entName                       = parameters[0];
        double speedChangeMpMps              = double.Parse(parameters[1]);
        double headingChangeClockwiseDegsSec = double.Parse(parameters[2]);

        string retString = "";

        if (FssEventDriver.DoesEntityExist(entName))
        {
            FssCourseDelta newCourseDelta = new FssCourseDelta() {
                    SpeedChangeMpMps = speedChangeMpMps,
                    HeadingChangeClockwiseDegsSec = headingChangeClockwiseDegsSec };

            FssEventDriver.SetEntityCourseDelta(entName, newCourseDelta);
            retString = $"Entity {entName} Updated: Course: {newCourseDelta}.";
        }
        else
        {
            retString = $"Entity {entName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandEntityCourse.Execute -> {retString}");
        return retString;
    }
}
