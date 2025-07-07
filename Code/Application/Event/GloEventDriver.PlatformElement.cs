
using System;
using System.Collections.Generic;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Basic Element Management
    // ---------------------------------------------------------------------------------------------

    public void AddPlatformElement(string platName, string elemName, string platElemType)
    {
        // Create a new platform
        GloPlatformElementOperations.CreatePlatformElement(platName, elemName, platElemType);
    }

    public void AddPlatformElement(string platName, string elemName, GloPlatformElement element)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        // Add the element to the platform
        platform.AddElement(element);
    }

    public void DeletePlatformElement(string platName, string elemName)
    {
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        platform.DeleteElement(elemName);
    }

    public List<string> PlatformElementNames(string platName)
    {
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return new List<string>();

        return platform.ElementNames();
    }

    // ---------------------------------------------------------------------------------------------

    public void PlatformAddSizerBox(string platName, string platType)
    {
        // Get the platform
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return;

        // fixed elemName for the box
        string elemName = "SizerBox";

        // Get the element
        GloPlatformElement? element = platform.ElementForName(elemName);

        if (element == null)
            return;

        // Set the element's size
        return;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public GloPlatformElement? GetElement(string platName, string elemName)
    {
        if (string.IsNullOrEmpty(platName) || string.IsNullOrEmpty(elemName))
            return null;

        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
            return null;

        return platform.ElementForName(elemName);
    }



    // public void SetPlatformStartLLA(string platName, GloLLALocation loc)
    // {
    //     // Get the platform
    //     GloPlatform? platform = GloAppFactory.Instance.PlatformManager.GetPlatformForName(platName);

    //     if (platform == null)
    //         return;

    //     // Set the platform's start location
    //     platform.Motion.InitialLocation = loc;
    // }
}
