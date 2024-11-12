using System.Collections.Generic;

// FssCommandEntityCourseDelta

public class FssCommandEntityCourseDelta : FssCommand
{
    public FssCommandEntityCourseDelta()
    {
        Signature.Add("ent");
        Signature.Add("coursedelta");
    }

    public override string HelpString => $"{SignatureString} <entity_name> <speedChangeMpMps> <headingChangeClockwiseDegsSec>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "FssCommandEntityCourseDelta.Execute -> insufficient parameters";
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

        FssCentralLog.AddEntry($"FssCommandEntityCourseDelta.Execute -> {retString}");
        return retString;
    }
}
