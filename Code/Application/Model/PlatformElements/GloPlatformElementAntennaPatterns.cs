using System;
using System.Text;
using System.Collections.Generic;

#nullable enable

// ------------------------------------------------------------------------------------------------

public class GloAntennaPattern
{
    public string          PortName { get; set; } = "";
    public GloFloat2DArray SphereMagPattern = new GloFloat2DArray();
    public GloAzElRange  PatternOffset    = new GloAzElRange();

    public override string ToString()
    {
        return $"PortName:{PortName} // Offset:{PatternOffset} // SphereMagPattern:{SphereMagPattern}";
    }
}

// ------------------------------------------------------------------------------------------------

public class GloPlatformElementAntennaPatterns : GloPlatformElement
{
    public string Type {set; get; } = "AntennaPatterns";

    // List of receivers
    public List<GloAntennaPattern> PatternList = new List<GloAntennaPattern>();

    // --------------------------------------------------------------------------------------------
    // MARK Named pattern mamangement
    // --------------------------------------------------------------------------------------------

    public List<string> PatternNames()
    {
        List<string> names = new();
        foreach (GloAntennaPattern pattern in PatternList)
            names.Add(pattern.PortName);
        return names;
    }

    public GloAntennaPattern? PatternForPortName(string name)
    {
        foreach (GloAntennaPattern pattern in PatternList)
        {
            if (pattern.PortName == name)
                return pattern;
        }
        return null;
    }

    public void RemoveAntennaPatterns(string name)
    {
        GloAntennaPattern? pattern = PatternForPortName(name);
        if (pattern != null)
            PatternList.Remove(pattern);
    }

    // --------------------------------------------------------------------------------------------
    // MARK Strightforward list management
    // --------------------------------------------------------------------------------------------

    public void AddAntennaPattern(GloAntennaPattern pattern) => PatternList.Add(pattern);
    public void RemoveAntennaPattern(GloAntennaPattern pattern) => PatternList.Remove(pattern);
    public void ClearAntennaPattern() => PatternList.Clear();

    // --------------------------------------------------------------------------------------------
    // MARK Report
    // --------------------------------------------------------------------------------------------

    public override string Report()
    {
        StringBuilder sb = new();

        sb.Append($"Type: {Type} // Name: {Name}\n");
        sb.Append($"AntennaPatterns: {PatternList.Count}\n");
        foreach (GloAntennaPattern pattern in PatternList)
            sb.Append($"{pattern}\n");

        return sb.ToString();
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append($"Type: {Type} // Name: {Name}\n");
        sb.Append($"AntennaPatterns: {PatternList.Count}\n");
        foreach (GloAntennaPattern pattern in PatternList)
            sb.Append($"{pattern}\n");

        return sb.ToString();
    }
}
