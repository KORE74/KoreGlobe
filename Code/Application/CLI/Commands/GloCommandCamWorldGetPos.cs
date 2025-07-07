using System.Collections.Generic;

using Godot;
// GloCommandVersion

public class GloCommandCamWordlGetPos : GloCommand
{
    public GloCommandCamWordlGetPos()
    {
        Signature.Add("cam");
        Signature.Add("worldgetpos");
    }

    public override string Execute(List<string> parameters)
    {
        // Stopping the model run
        if (GloAppFactory.Instance.SimClock.IsRunning)
        {
            GloCentralLog.AddEntry("Stopping model run");
            GloAppFactory.Instance.ModelRun.Stop();
        }

        // GloGodotFactory.Instance

        // Exiting the application - ending the threads
        GloCentralLog.AddEntry("Exiting the application");
        GloAppFactory.Instance.EventDriver.ExitApplication();

        return "Exiting the application";
    }
}
