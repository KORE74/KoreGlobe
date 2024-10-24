using System.Collections.Generic;

using Godot;
// FssCommandVersion

// public class FssCommandCamWordlGetPos : FssCommand
// {
//     public FssCommandCamWordlGetPos()
//     {
//         Signature.Add("cam");
//         Signature.Add("worldgetpos");
//     }

//     public override string Execute(List<string> parameters)
//     {
//         // Stopping the model run
//         if (FssAppFactory.Instance.SimClock.IsRunning)
//         {
//             FssCentralLog.AddEntry("Stopping model run");
//             FssAppFactory.Instance.ModelRun.Stop();
//         }

//         // FssGodotFactory.Instance

//         // Exiting the application - ending the threads
//         FssCentralLog.AddEntry("Exiting the application");
//         FssAppFactory.Instance.EventDriver.ExitApplication();

//         // Call the Godot exit function -- Maybe not the right place?
//         FssAppNode.Instance.ExitApplication();

//         return "Exiting the application";
//     }
// }
