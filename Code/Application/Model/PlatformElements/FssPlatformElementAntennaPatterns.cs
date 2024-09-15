using System;
using System.Text;
using System.Collections.Generic;

#nullable enable

public class FssAntennaPattern
{
    public string Name { get; set; } = "";
    public FssFloat2DArray SphereMagPattern = new FssFloat2DArray();
    public FssPolarOffset  PatternOffset    = new FssPolarOffset();

    public override string ToString()
    {
        return $"AntennaPattern:{Name} // Offset:{PatternOffset} // SphereMagPattern:{SphereMagPattern}";
    }
}

public class FssPlatformElementAntennaPatterns : FssPlatformElement
{
    public string Type {set; get; } = "AntennaPatterns";

    // List of receivers
    public List<FssAntennaPattern> AntennaPatterns = new List<FssAntennaPattern>();

    // --------------------------------------------------------------------------------------------
    // #MARK Named pattern mamangement
    // --------------------------------------------------------------------------------------------

    public List<string> PatternNames()
    {
        List<string> names = new();
        foreach (FssAntennaPattern pattern in AntennaPatterns)
            names.Add(pattern.Name);
        return names;
    }

    public FssAntennaPattern? PatternForName(string name)
    {
        foreach (FssAntennaPattern pattern in AntennaPatterns)
        {
            if (pattern.Name == name)
                return pattern;
        }
        return null;
    }

    public void RemoveAntennaPatterns(string name)
    {
        FssAntennaPattern? pattern = PatternForName(name);
        if (pattern != null)
            AntennaPatterns.Remove(pattern);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK Strightforward list management
    // --------------------------------------------------------------------------------------------

    public void AddAntennaPatterns(FssAntennaPattern pattern) => AntennaPatterns.Add(pattern);
    public void RemoveAntennaPatterns(FssAntennaPattern pattern) => AntennaPatterns.Remove(pattern);
    public void ClearAntennaPatterns() => AntennaPatterns.Clear();

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append($"Type: {Type}\n");
        sb.Append($"AntennaPatterns: {AntennaPatterns.Count}\n");
        foreach (FssAntennaPattern pattern in AntennaPatterns)
            sb.Append($"{pattern}\n");

        return sb.ToString();
    }
}
