using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

public class GlobePlatform
{
    public string Name { get; set; } = "Unknown-Name";
    public string Type { get; set; } = "Unknown-Type";

    // Kinetics object defines the initial and current position of the platform. Has to exist in all cases.
    public GlobePlatformKinetics Kinetics { get; set; } = new GlobePlatformKinetics();

    //private GlobePlatformRoute? Route { get; set; } = null;

    private List<GlobePlatformElement>? Elements { get; set; } = null;

    // --------------------------------------------------------------------------------------------

    // Accessors consructors

    // public GlobePlatformRoute RouteObject
    // {
    //     get { Route ??= new GlobePlatformRoute(); return Route; }
    // }

    public List<GlobePlatformElement> ElementsList
    {
        get { Elements ??= new List<GlobePlatformElement>(); return Elements; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Basic Element Management
    // --------------------------------------------------------------------------------------------

    public void AddElement(GlobePlatformElement element)
    {
        ElementsList.Add(element);
    }

    public void DeleteElement(GlobePlatformElement element)
    {
        ElementsList.Remove(element);
    }

    public void DeleteElement(string name)
    {
        foreach (GlobePlatformElement element in ElementsList)
        {
            if (element.Name == name)
                ElementsList.Remove(element);
        }
    }

    public GlobePlatformElement? ElementForName(string name)
    {
        foreach (GlobePlatformElement element in ElementsList)
        {
            if (element.Name == name)
                return element;
        }
        return null;
    }

    public bool DoesElementExist(string name)
    {
        foreach (GlobePlatformElement element in ElementsList)
        {
            if (element.Name == name)
                return true;
        }
        return false;
    }

    // --------------------------------------------------------------------------------------------

    public string PositionReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Platform: {Name} Type: {Type}");
        sb.AppendLine($"- InitialLocation: {Kinetics.StartPosition}");
        sb.AppendLine($"- CurrPosition: {Kinetics.CurrPosition}");
        sb.AppendLine($"- CurrAttitude: {Kinetics.CurrAttitude}");
        sb.AppendLine($"- CurrCourse: {Kinetics.CurrCourse}");
        sb.AppendLine($"- CurrCourseDelta: {Kinetics.CurrCourseDelta}");
        sb.AppendLine($"- CurrAttitudeDelta: {Kinetics.CurrAttitudeDelta}");

        return sb.ToString();
    }
}
