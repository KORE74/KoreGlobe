
using System;

using GloNetworking;

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void ExitApplication()
    {
        GloAppFactory.Instance.ConsoleInterface.Stop();
        GloAppFactory.Instance.NetworkHub.endAllConnections();
        //GloAppFactory.Instance.NetworkHub.EndAllThread();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Sidebar view settings
    // ---------------------------------------------------------------------------------------------

    // Usage: GloAppFactory.Instance.EventDriver.SidebarSetBeamVisibility(true, true);

    public void SidebarSetBeamVisibility(bool rxVisible, bool txVisible)
    {
        // Set the UI State values
        GloGodotFactory.Instance.UIState.ShowRx = rxVisible;
        GloGodotFactory.Instance.UIState.ShowTx = txVisible;
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: GloAppFactory.Instance.EventDriver.SidebarSetRouteVisibility(true);
    //        GloAppFactory.Instance.EventDriver.SidebarSetEmitterVisibility(true);
    //        GloAppFactory.Instance.EventDriver.SidebarSetAntennaPatternVisibility(true);


    public void SidebarSetRouteVisibility(bool routeVisible)                   => GloGodotFactory.Instance.UIState.ShowRoutes          = routeVisible;
    public void SidebarSetEmitterVisibility(bool emitterVisible)               => GloGodotFactory.Instance.UIState.ShowEmitters        = emitterVisible;
    public void SidebarSetAntennaPatternVisibility(bool antennaPatternVisible) => GloGodotFactory.Instance.UIState.ShowAntennaPatterns = antennaPatternVisible;

    public void SetCameraModeWorld()    => GloGodotFactory.Instance.UIState.SetCameraModeWorld();
    public void SetCameraModeChaseCam() => GloGodotFactory.Instance.UIState.SetCameraModeChaseCam();
    public void SetCameraModeAlignCam() => GloGodotFactory.Instance.UIState.SetCameraModeAlignCam();


    // ---------------------------------------------------------------------------------------------

}

