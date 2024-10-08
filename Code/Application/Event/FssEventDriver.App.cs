
using System;

using FssNetworking;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void ExitApplication()
    {
        FssAppFactory.Instance.ConsoleInterface.Stop();
        FssAppFactory.Instance.NetworkHub.endAllConnections();
        //FssAppFactory.Instance.NetworkHub.EndAllThread();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Sidebar view settings
    // ---------------------------------------------------------------------------------------------

    // Usage: FssAppFactory.Instance.EventDriver.SidebarSetBeamVisibility(true, true);

    public void SidebarSetBeamVisibility(bool rxVisible, bool txVisible)
    {
        // Set the UI State values
        FssGodotFactory.Instance.UIState.ShowRx = rxVisible;
        FssGodotFactory.Instance.UIState.ShowTx = txVisible;
    }

    // ---------------------------------------------------------------------------------------------

    // Usage: FssAppFactory.Instance.EventDriver.SidebarSetRouteVisibility(true);
    //        FssAppFactory.Instance.EventDriver.SidebarSetEmitterVisibility(true);
    //        FssAppFactory.Instance.EventDriver.SidebarSetAntennaPatternVisibility(true);


    public void SidebarSetRouteVisibility(bool routeVisible)                   => FssGodotFactory.Instance.UIState.ShowRoutes          = routeVisible;
    public void SidebarSetEmitterVisibility(bool emitterVisible)               => FssGodotFactory.Instance.UIState.ShowEmitters        = emitterVisible;
    public void SidebarSetAntennaPatternVisibility(bool antennaPatternVisible) => FssGodotFactory.Instance.UIState.ShowAntennaPatterns = antennaPatternVisible;

    // ---------------------------------------------------------------------------------------------

}

