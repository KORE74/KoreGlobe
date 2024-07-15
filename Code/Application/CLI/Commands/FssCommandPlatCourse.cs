using System.Collections.Generic;

// FssCommandPlatDelete

public class FssCommandPlatCourse : FssCommand
{
    public FssCommandPlatCourse()
    {
        Signature.Add("plat");
        Signature.Add("course");
    }

    public override string HelpString => $"{SignatureString} <platform_name> <HeadingDegs> <SpeedKph>";

    public override string Execute(List<string> parameters)
    {
        if (parameters.Count < 3)
        {
            return "FssCommandPlatCourse.Execute -> insufficient parameters";
        }

        string platName    = parameters[0];
        double headingDegs = double.Parse(parameters[1]);
        double speedKph    = double.Parse(parameters[2]);

        string retString = "";

        if (FssAppFactory.Instance.EventDriver.DoesPlatformExist(platName))
        {
            FssCourse newCourse = new FssCourse() { HeadingDegs = headingDegs, SpeedKph = speedKph };

            FssAppFactory.Instance.EventDriver.SetPlatformCourse(platName, newCourse);
            retString = $"Platform {platName} Updated: Course: {newCourse}.";
        }
        else
        {
            retString = $"Platform {platName} not found.";
        }

        FssCentralLog.AddEntry($"FssCommandPlatCourse.Execute -> {retString}");
        return retString;
    }
}
