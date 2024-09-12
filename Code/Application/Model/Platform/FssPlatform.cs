using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

public class FssPlatform
{
    public string Name     { get; set; } = "Unknown-Name";
    public string Type     { get; set; } = "Unknown-Type";
    public string Category { get; set; } = "Unknown-Category"; // Such as airbourne, ground, etc. Allows us to select a correct icon, or default 3D model.

    // Kinetics object defines the initial and current position of the platform. Has to exist in all cases.
    public FssPlatformKinetics Kinetics { get; set; } = new FssPlatformKinetics();

    //private FssPlatformRoute? Route { get; set; } = null;

    private List<FssPlatformElement>? Elements { get; set; } = null;

    // --------------------------------------------------------------------------------------------

    // Accessors consructors

    // public FssPlatformRoute RouteObject
    // {
    //     get { Route ??= new FssPlatformRoute(); return Route; }
    // }

    public List<FssPlatformElement> ElementsList
    {
        get { Elements ??= new List<FssPlatformElement>(); return Elements; }
    }

    public List<string> ElementNames()
    {
        List<string> names = new List<string>();

        foreach (FssPlatformElement element in ElementsList)
            names.Add(element.Name);

        return names;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Basic Element Management
    // --------------------------------------------------------------------------------------------

    public void AddElement(FssPlatformElement element)
    {
        ElementsList.Add(element);
    }

    public void DeleteElement(FssPlatformElement element)
    {
        ElementsList.Remove(element);
    }

    // Loops in reverse to avoid issues with removing elements from a list while iterating over it
    public void DeleteElement(string name)
    {
        for (int i = ElementsList.Count - 1; i >= 0; i--)
        {
            if (ElementsList[i].Name == name)
            {
                ElementsList.RemoveAt(i);
            }
        }
    }

    public FssPlatformElement? ElementForName(string name)
    {
        foreach (FssPlatformElement element in ElementsList)
        {
            if (element.Name == name)
                return element;
        }
        return null;
    }

    public bool DoesElementExist(string name)
    {
        foreach (FssPlatformElement element in ElementsList)
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

    public string ElementReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Platform: {Name} Type: {Type}");
        sb.AppendLine($"- Elements: {ElementsList.Count}");

        foreach (FssPlatformElement element in ElementsList)
        {
            sb.AppendLine($"- Element: {element.Name} Type: {element.Type}");
        }

        return sb.ToString();
    }
}
