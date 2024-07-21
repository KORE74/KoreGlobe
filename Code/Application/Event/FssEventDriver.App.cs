
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

}