using System;

#nullable enable

// FssPlatformElementOperations: Utility class to factor out all of the element searching and management operations, and avoid cluttering the platform class

public static class FssPlatformElementOperations
{
    public static FssPlatformElement? CreatePlatformElement(string platName, string elemName, string platElemType)
    {
        FssPlatformElement? newElem = null;

        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);
        if (platform == null)
            return newElem;

        if (platform.DoesElementExist(elemName))
            return newElem;

        switch(platElemType)
        {
            case "Dome":
                newElem = new FssPlatformElementRadarDome();
                break;
            case "Wedge":
                newElem = new FssPlatformElementRadarWedge();
                break;
            case "RecieverPatterns":
                newElem = new FssPlatformElementAntennaPatterns();
                break;
            default:
                break;
        }

        if (newElem != null)
            platform.AddElement(newElem);

        return newElem;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Access Operations
    // ---------------------------------------------------------------------------------------------

    // public static FssPlatformElement? ElementForName(FssPlatform platform, string elemname)
    // {
    //     foreach (var element in platform.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return element;
    //     }
    //     return null;
    // }

    // public static bool DoesElementExist(FssPlatform platform, string elemname)
    // {
    //     foreach (var element in platform.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return true;
    //     }
    //     return false;
    // }

    // public static void DeleteElement(FssPlatform platform, string elemname)
    // {
    //     foreach (var element in platform.Elements)
    //     {
    //         if (element.Name == elemname)
    //         {
    //             platform.Elements.Remove(element);
    //             return;
    //         }
    //     }
    // }

}