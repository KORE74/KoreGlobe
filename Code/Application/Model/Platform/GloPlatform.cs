using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

public class GloPlatform
{
    public string Name     { get; set; } = "Unknown-Name";
    public string Type     { get; set; } = "Unknown-Type";
    public string Category { get; set; } = "Unknown-Category"; // Such as airbourne, ground, etc. Allows us to select a correct icon, or default 3D model.

    // Kinetics object defines the initial and current position of the platform. Has to exist in all cases.
    public GloPlatformKinetics Kinetics { get; set; } = new GloPlatformKinetics();

    //private GloPlatformRoute? Route { get; set; } = null;

    private List<GloPlatformElement>? Elements { get; set; } = null;

    private GloStringDictionary Properties { get; set; } = new GloStringDictionary();

    // --------------------------------------------------------------------------------------------

    // Accessors consructors

    // public GloPlatformRoute RouteObject
    // {
    //     get { Route ??= new GloPlatformRoute(); return Route; }
    // }

    public List<GloPlatformElement> ElementsList
    {
        get { Elements ??= new List<GloPlatformElement>(); return Elements; }
    }

    public List<string> ElementNames()
    {
        List<string> names = new List<string>();

        foreach (GloPlatformElement element in ElementsList)
            names.Add(element.Name);

        return names;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Properties
    // --------------------------------------------------------------------------------------------

    public bool HasProperty(string key)               => Properties.Has(key);
    public void SetProperty(string key, string value) => Properties.Set(key, value);

    public string GetProperty(string key)
    {
        return Properties.Get(key, "<MissingProperty>");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Basic Element Management
    // --------------------------------------------------------------------------------------------

    public void AddElement(GloPlatformElement element)    => ElementsList.Add(element);
    public void DeleteElement(GloPlatformElement element) => ElementsList.Remove(element);

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

    public GloPlatformElement? ElementForName(string name)
    {
        foreach (GloPlatformElement element in ElementsList)
        {
            if (element.Name == name)
                return element;
        }
        return null;
    }

    public bool DoesElementExist(string name)
    {
        foreach (GloPlatformElement element in ElementsList)
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

        foreach (GloPlatformElement element in ElementsList)
        {
            sb.AppendLine($"- Element: {element.Name} Type: {element.Type}");
        }

        return sb.ToString();
    }
}
