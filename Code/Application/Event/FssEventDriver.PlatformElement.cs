
using System;
using System.Collections.Generic;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Basic Element Management
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

    public List<string> PlatformElementNames(string platName)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return new List<string>();

        return platform.ElementNames();
    }

    // ---------------------------------------------------------------------------------------------

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
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public FssPlatformElement? GetElement(string platName, string elemName)
    {
        if (string.IsNullOrEmpty(platName) || string.IsNullOrEmpty(elemName))
            return null;

        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.ElementForName(elemName);
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
