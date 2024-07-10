using System.Collections.Generic;

// FssCommandVersion

public class FssCommandExit : FssCommand
{
    public FssCommandExit()
    {
        Signature.Add("exit");
    }

    public override string Execute(List<string> parameters)
    {
        FssEventDriver eventDriver = FssAppFactory.Instance.EventDriver;

        if (eventDriver != null)
        {
            eventDriver.ExitApplication();
            return "Exiting...";
        }
        else
        {
            return "Error: Event driver not found.";
        }
    }

}
