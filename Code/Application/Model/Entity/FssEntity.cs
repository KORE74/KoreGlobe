using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

public class FssEntity
{
    public string Name     { get; set; } = "Unknown-Name";

    // Kinetics object defines the initial and current position of the platform. Has to exist in all cases.
    public FssEntityKinetics Kinetics { get; set; } = new FssEntityKinetics();

    //private FssPlatformRoute? Route { get; set; } = null;

    private List<FssElement>? Elements { get; set; } = null;

    // --------------------------------------------------------------------------------------------

    // Accessors consructors

    // public FssPlatformRoute RouteObject
    // {
    //     get { Route ??= new FssPlatformRoute(); return Route; }
    // }

    public List<FssElement> ElementsList
    {
        get { Elements ??= new List<FssElement>(); return Elements; }
    }

    public List<string> ElementNames()
    {
        List<string> names = new List<string>();

        foreach (FssElement element in ElementsList)
            names.Add(element.Name);

        return names;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Basic Element Management
    // --------------------------------------------------------------------------------------------

    public void AddElement(FssElement element) => ElementsList.Add(element);
    public void DeleteElement(FssElement element) => ElementsList.Remove(element);

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

    public FssElement? ElementForName(string name)
    {
        foreach (FssElement element in ElementsList)
        {
            if (element.Name == name)
                return element;
        }
        return null;
    }

    public bool DoesElementExist(string name)
    {
        foreach (FssElement element in ElementsList)
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
        sb.AppendLine($"Entity: {Name} Type: {Type}");
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
        sb.AppendLine($"Entity: {Name} Type: {Type}");
        sb.AppendLine($"- Elements: {ElementsList.Count}");

        foreach (FssElement element in ElementsList)
        {
            sb.AppendLine($"- Element: {element.Name} Type: {element.Type}");
        }

        return sb.ToString();
    }
}
