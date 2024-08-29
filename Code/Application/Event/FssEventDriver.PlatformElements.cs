
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Command Execution
    // ---------------------------------------------------------------------------------------------

    public void AddPlatformElement(string platName, string elemName, string platElemType)
    {
        // Create a new platform
        FssPlatformElementOperations.CreatePlatformElement(platName, elemName, platElemType);
    }

    public void DeletePlatformElement(string platName, string elemName)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        platform.DeleteElement(elemName);
    }


    public void PlatformAddSizerBox(string platName, string platType)
    {
        // Get the platform
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        // fixed elemName for the box
        string elemName = "SizerBox";

        // Get the element
        FssPlatformElement? element = platform.ElementForName(elemName);

        if (element == null)
            return;

        // Set the element's size
        return;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Add scans
    // ---------------------------------------------------------------------------------------------

    public void PlatformAddScanHemisphere(string platName, string elemName, double DetectionRangeKms)
    {

    }


    public void PlatformAddScanWedge(string platName, string elemName, double DetectionRangeKms, double DetectionRangeRxMtrs, FssAzElBox azElBox)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddScanWedge: Platform {platName} not found.");
            return;
        }

        // Get the element
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);

        // Create the element
        FssPlatformElementOperations.CreatePlatformElement(platName, elemName, "ScanWedge");


    }


    public void PlatformAddScanConical(string platName, string elemName, double DetectionRangeKms, string targetName)
    {

    }


    public void PlatformAddAntennaPattern(string platName, string elemName, FssPolarOffset offset, FssAzElBox azElBox)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddAntennaPattern: Platform {platName} not found.");
            return;
        }



    }


    // ---------------------------------------------------------------------------------------------
    // MARK: Route Helpers
    // ---------------------------------------------------------------------------------------------


    public void PlatformAddRoute(string platName, string elemName)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddRoute: Platform {platName} not found.");
            return;
        }

        // Look for an existing route with the same name
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddRoute: Route {elemName} already exists.");
            return;
        }

        // Create the route
        FssPlatformElementRoute route = new FssPlatformElementRoute() { Name = elemName };
        platform.AddElement(route);
    }

    public void PlatformAddRoutePoint(string platName, string elemName, FssLLAPoint point)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddRoutePoint: Platform {platName} not found.");
            return;
        }

        // Get the route
        FssPlatformElementRoute? route = platform.ElementForName(elemName) as FssPlatformElementRoute;
        if (route == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddRoutePoint: Route {elemName} not found.");
            return;
        }

        // Add the point
        route.AddPoint(point);
    }

    public void PlatformClearRoute(string platName, string elemName)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformClearRoute: Platform {platName} not found.");
            return;
        }

        // Get the route
        FssPlatformElementRoute? route = platform.ElementForName(elemName) as FssPlatformElementRoute;
        if (route == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformClearRoute: Route {elemName} not found.");
            return;
        }

        // Clear the route
        route.Clear();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public static string ElementNameForBeam(string platName, string emitName, string beamName)
    {
        return $"{platName}_{emitName}_{beamName}";
    }


    // public void SetPlatformStartLLA(string platName, FssLLALocation loc)
    // {
    //     // Get the platform
    //     FssPlatform? platform = FssAppFactory.Instance.PlatformManager.GetPlatformForName(platName);

    //     if (platform == null)
    //         return;

    //     // Set the platform's start location
    //     platform.Motion.InitialLocation = loc;
    // }
}
