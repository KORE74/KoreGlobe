
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // Command Execution
    // ---------------------------------------------------------------------------------------------

    public void AddPlatformElement(string platName, string elemName, string platElemType)
    {
        // Create a new platform
        FssPlatformElementOperations.CreatePlatformElement(platName, elemName, platElemType);
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
    // Add scans
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


