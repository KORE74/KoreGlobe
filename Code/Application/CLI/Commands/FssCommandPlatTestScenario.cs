using System.Collections.Generic;

// FssCommandVersion

public class FssCommandPlatTestScenario : FssCommand
{
    public FssCommandPlatTestScenario()
    {
        Signature.Add("plat");
        Signature.Add("test");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandPlatTestScenario");
        FssAppFactory.Instance.EventDriver.SetupTestPlatforms();

        int num = FssAppFactory.Instance.EventDriver.NumPlatforms();
        return $"FssCommandPlatTestScenario:\n Number of Platforms: {num}";
    }

}
