
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

    public void AddElement(string entityName, string elemName, string elemType)
    {
        // Create a new ent
        //FssElementOperations.CreateElement(entityName, elemName, elemType);
    }

    public void AddElement(string entityName, string elemName, FssElement element)
    {
        // Get the ent
        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (ent == null)
            return;

        // Add the element to the ent
        ent.AddElement(element);
    }

    public void DeleteElement(string entityName, string elemName)
    {
        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (ent == null)
            return;

        ent.DeleteElement(elemName);
    }

    public List<string> ElementNamesForEntity(string entityName)
    {
        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (ent == null)
            return new List<string>();

        return ent.ElementNames();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public FssElement? GetElement(string entityName, string elemName)
    {
        if (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(elemName))
            return null;

        FssEntity? ent = FssAppFactory.Instance.EntityManager.EntityForName(entityName);

        if (ent == null)
            return null;

        return ent.ElementForName(elemName);
    }



    // public void SetPlatformStartLLA(string entityName, FssLLALocation loc)
    // {
    //     // Get the ent
    //     FssEntity? ent = FssAppFactory.Instance.PlatformManager.GetPlatformForName(entityName);

    //     if (ent == null)
    //         return;

    //     // Set the ent's start location
    //     ent.Motion.InitialLocation = loc;
    // }
}
