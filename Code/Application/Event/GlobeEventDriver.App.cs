
using System;

using FssNetworking;

// Design Decisions:
// - The GlobeEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GlobeEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void ExitApplication()
    {
        GlobeAppFactory.Instance.ConsoleInterface.Stop();
        GlobeAppFactory.Instance.NetworkCommsHub.endAllConnections();
        //GlobeAppFactory.Instance.NetworkCommsHub.EndAllThread();
    }

}