
using System;

using FssNetworking;

// Design Decisions:
// - The GlobeEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GlobeEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void AddPlatformElement(string platName, string elemName, string platElemType)
    {
        // Create a new platform
        GlobePlatformElementOperations.CreatePlatformElement(platName, elemName, platElemType);
    }


    public void PlatformAddSizerBox(string platName, string platType)
    {
        // Get the platform
        GlobePlatform? platform = GlobeAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        // fixed elemName for the box
        string elemName = "SizerBox";

        // Get the element
        GlobePlatformElement? element = platform.ElementForName(elemName);

        if (element == null)
            return;

        // Set the element's size
        return;
    }

    // ---------------------------------------------------------------------------------------------
    // Add scans
    // ---------------------------------------------------------------------------------------------

    public void PlatformAddScanHemisphere(string platName, string elemName, double DetectionRangeKms)
    {

    }


    public void PlatformAddScanWedge(string platName, string elemName, double DetectionRangeKms, double DetectionRangeRxMtrs, GlobeAzElBox azElBox)
    {

    }


    public void PlatformAddScanConical(string platName, string elemName, double DetectionRangeKms, string targetName)
    {

    }



    // public void SetPlatformStartLLA(string platName, GlobeLLALocation loc)
    // {
    //     // Get the platform
    //     GlobePlatform? platform = GlobeAppFactory.Instance.PlatformManager.GetPlatformForName(platName);

    //     if (platform == null)
    //         return;

    //     // Set the platform's start location
    //     platform.Motion.InitialLocation = loc;
    // }
}


