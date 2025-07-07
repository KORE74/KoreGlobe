using System.Collections.Generic;

// GloCommandVersion

public class GloCommandPlatTestScenario : GloCommand
{
    public GloCommandPlatTestScenario()
    {
        Signature.Add("plat");
        Signature.Add("test");
    }

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandPlatTestScenario");
        GloAppFactory.Instance.EventDriver.SetupTestPlatforms();

        int num = GloAppFactory.Instance.EventDriver.NumPlatforms();
        return $"GloCommandPlatTestScenario:\n Number of Platforms: {num}";
    }

}
