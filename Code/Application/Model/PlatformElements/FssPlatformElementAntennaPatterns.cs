using System;
using System.Text;
using System.Collections.Generic;

#nullable enable

public class FssAntennaPattern
{
    public string          PortName { get; set; } = "";
    public FssFloat2DArray SphereMagPattern = new FssFloat2DArray();
    public FssPolarOffset  PatternOffset    = new FssPolarOffset();

    public override string ToString()
    {
        return $"PortName:{PortName} // Offset:{PatternOffset} // SphereMagPattern:{SphereMagPattern}";
    }
}

public class FssPlatformElementAntennaPatterns : FssPlatformElement
{
    public string Type {set; get; } = "AntennaPatterns";

    // List of receivers
    public List<FssAntennaPattern> PatternList = new List<FssAntennaPattern>();

    // --------------------------------------------------------------------------------------------
    // #MARK Named pattern mamangement
    // --------------------------------------------------------------------------------------------

    public List<string> PatternNames()
    {
        List<string> names = new();
        foreach (FssAntennaPattern pattern in PatternList)
            names.Add(pattern.PortName);
        return names;
    }

    public FssAntennaPattern? PatternForPortName(string name)
    {
        foreach (FssAntennaPattern pattern in PatternList)
        {
            if (pattern.PortName == name)
                return pattern;
        }
        return null;
    }

    public void RemoveAntennaPatterns(string name)
    {
        FssAntennaPattern? pattern = PatternForPortName(name);
        if (pattern != null)
            PatternList.Remove(pattern);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK Strightforward list management
    // --------------------------------------------------------------------------------------------

    public void AddAntennaPattern(FssAntennaPattern pattern) => PatternList.Add(pattern);
    public void RemoveAntennaPattern(FssAntennaPattern pattern) => PatternList.Remove(pattern);
    public void ClearAntennaPattern() => PatternList.Clear();

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append($"Type: {Type}\n");
        sb.Append($"AntennaPatterns: {PatternList.Count}\n");
        foreach (FssAntennaPattern pattern in PatternList)
            sb.Append($"{pattern}\n");

        return sb.ToString();
    }
}
