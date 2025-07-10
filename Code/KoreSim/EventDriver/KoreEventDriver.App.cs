
using System;


using KoreCommon;

namespace KoreSim;

// Design Decisions:
// - The KoreEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class KoreEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public static void ExitApplication()
    {
        KoreSimFactory.Instance.ConsoleInterface.Stop();
        KoreSimFactory.Instance.NetworkHub.endAllConnections();
        //KoreSimFactory.Instance.NetworkHub.EndAllThread();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Sidebar view settings
    // ---------------------------------------------------------------------------------------------

    // Usage: KoreEventDriver.SidebarSetBeamVisibility(true, true);

    // public void SidebarSetBeamVisibility(bool rxVisible, bool txVisible)
    // {
    //     // Set the UI State values
    //     KoreGodotFactory.Instance.UIState.ShowRx = rxVisible;
    //     KoreGodotFactory.Instance.UIState.ShowTx = txVisible;
    // }

    // ---------------------------------------------------------------------------------------------

    // Usage: KoreEventDriver.SidebarSetRouteVisibility(true);
    //        KoreEventDriver.SidebarSetEmitterVisibility(true);
    //        KoreEventDriver.SidebarSetAntennaPatternVisibility(true);

    // public void SidebarSetRouteVisibility(bool routeVisible)                   => KoreGodotFactory.Instance.UIState.ShowRoutes          = routeVisible;
    // public void SidebarSetEmitterVisibility(bool emitterVisible)               => KoreGodotFactory.Instance.UIState.ShowEmitters        = emitterVisible;
    // public void SidebarSetAntennaPatternVisibility(bool antennaPatternVisible) => KoreGodotFactory.Instance.UIState.ShowAntennaPatterns = antennaPatternVisible;

    // public void SetCameraModeWorld()    => KoreGodotFactory.Instance.UIState.SetCameraModeWorld();
    // public void SetCameraModeChaseCam() => KoreGodotFactory.Instance.UIState.SetCameraModeChaseCam();
    // public void SetCameraModeAlignCam() => KoreGodotFactory.Instance.UIState.SetCameraModeAlignCam();


    // ---------------------------------------------------------------------------------------------

}

