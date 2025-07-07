using System;

#nullable enable

// GloPlatformElementOperations: Utility class to factor out all of the element searching and management operations, and avoid cluttering the platform class

public static class GloPlatformElementOperations
{
    public static GloPlatformElement? CreatePlatformElement(string platName, string elemName, string platElemType)
    {
        GloPlatformElement? newElem = null;

        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);
        if (platform == null)
            return newElem;

        if (platform.DoesElementExist(elemName))
            return newElem;

        switch(platElemType)
        {
            // case "Dome":
            //     newElem = new GloPlatformElementRadarDome();
            //     break;
            // case "Wedge":
            //     newElem = new GloPlatformElementRadarWedge();
            //     break;
            case "RecieverPatterns":
                newElem = new GloPlatformElementAntennaPatterns();
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

    // public static GloPlatformElement? ElementForName(GloPlatform platform, string elemname)
    // {
    //     foreach (var element in platform.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return element;
    //     }
    //     return null;
    // }

    // public static bool DoesElementExist(GloPlatform platform, string elemname)
    // {
    //     foreach (var element in platform.Elements)
    //     {
    //         if (element.Name == elemname)
    //             return true;
    //     }
    //     return false;
    // }

    // public static void DeleteElement(GloPlatform platform, string elemname)
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