using System.Collections.Generic;

// GloCommandPlatDelete

public class GloCommandPlatCourse : GloCommand
{
    public GloCommandPlatCourse()
    {
        Signature.Add("plat");
        Signature.Add("course");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <HeadingDegs> <SpeedKph>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "GloCommandPlatCourse.Execute -> insufficient parameters";
        }

        string platName    = parameters[0];
        double headingDegs = double.Parse(parameters[1]);
        double speedKph    = double.Parse(parameters[2]);

        string retString = "";

        if (GloAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            GloCourse newCourse = new GloCourse() { HeadingDegs = headingDegs, SpeedKph = speedKph };

            GloAppFactory.Instance.EventDriver.SetPlatformCourse(platName, newCourse);
            retString = $"Platform {platName} Updated: Course: {newCourse}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        GloCentralLog.AddEntry($"GloCommandPlatCourse.Execute -> {retString}");
        return retString;
    }
}
