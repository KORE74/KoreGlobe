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
        // Stopping the model run
        if (FssAppFactory.Instance.SimClock.IsRunning)
        {
            FssCentralLog.AddEntry("Stopping model run");
            FssAppFactory.Instance.ModelRun.Stop();
        }

        // Exiting the application
        FssCentralLog.AddEntry("Exiting the application");
        FssAppFactory.Instance.EventDriver.ExitApplication();

        return "Exiting the application";
    }
}
