
using System;

using FssNetworking;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public static void ExitApplication()
    {
        FssAppFactory.Instance.ConsoleInterface.Stop();
        FssAppFactory.Instance.NetworkHub.endAllConnections();
        //FssAppFactory.Instance.NetworkHub.EndAllThread();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Sidebar view settings
    // ---------------------------------------------------------------------------------------------

    // Usage: FssEventDriver.SidebarSetBeamVisibility(true, true);

    // public void SidebarSetBeamVisibility(bool rxVisible, bool txVisible)
    // {
    //     // Set the UI State values
    //     FssGodotFactory.Instance.UIState.ShowRx = rxVisible;
    //     FssGodotFactory.Instance.UIState.ShowTx = txVisible;
    // }

    // ---------------------------------------------------------------------------------------------

    // Usage: FssEventDriver.SidebarSetRouteVisibility(true);


    // public void SidebarSetRouteVisibility(bool routeVisible)                   => FssGodotFactory.Instance.UIState.ShowRoutes          = routeVisible;

    // ---------------------------------------------------------------------------------------------

}

