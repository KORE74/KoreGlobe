using System;

// GlobePlatformElementOperations: Utility class to factor out all of the element searching and management operations, and avoid cluttering the platform class

public static class GlobePlatformElementOperations
{
    public static GlobePlatformElement? CreatePlatformElement(string platName, string elemName, string platElemType)
    {
        GlobePlatformElement? newElem = null;

        GlobePlatform? platform = GlobeAppFactory.Instance.PlatformManager.PlatForName(platName);
        if (platform == null)
            return newElem;

        if (platform.DoesElementExist(elemName))
            return newElem;

        switch(platElemType)
        {
            case "Dome":
                newElem = new GlobePlatformElementRadarDome();
                break;
            case "Wedge":
                newElem = new GlobePlatformElementRadarWedge();
                break;
            case "RecieverPatterns":
                newElem = new GlobePlatformElementRecieverPatterns();
                break;
            default:
                break;
        }

        if (newElem != null)
            platform.AddElement(newElem);
        
        return newElem;
    }

}

//     public static GlobePlatformElement? ElementForName(GlobePlatform platform, string elemname)
//     {
//         foreach (var element in platform.Elements)
//         {
//             if (element.Name == elemname)
//                 return element;
//         }
//         return null;
//     }

//     public static bool DoesElementExist(GlobePlatform platform, string elemname)
//     {
//         foreach (var element in platform.Elements)
//         {
//             if (element.Name == elemname)
//                 return true;
//         }
//         return false;
//     }

