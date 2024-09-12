using System;
using System.Collections.Generic;

#nullable enable

public class FssAntennaPatterns : FssPlatformElement
{
    public string Name { get; set; } = "";
    public FssFloat2DArray SphereMagPattern = new FssFloat2DArray();
    public FssPolarOffset  PatternOffset    = new FssPolarOffset();
}

public class FssPlatformElementAntennaPatterns : FssPlatformElement
{
    public string Type {set; get; } = "AntennaPatterns";

    // List of receivers
    public List<FssAntennaPatterns> AntennaPatterns = new List<FssAntennaPatterns>();

    // --------------------------------------------------------------------------------------------

    public List<string> GetAntennaPatternsNames()
    {
        List<string> names = new();
        foreach (FssAntennaPatterns pattern in AntennaPatterns)
            names.Add(pattern.Name);
        return names;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK Basic pattern mamangement
    // --------------------------------------------------------------------------------------------

    public FssAntennaPatterns? GetAntennaPatterns(string name)
    {
        foreach (FssAntennaPatterns pattern in AntennaPatterns)
        {
            if (pattern.Name == name)
                return pattern;
        }
        return null;
    }

    public void AddAntennaPatterns(FssAntennaPatterns pattern)
    {
        AntennaPatterns.Add(pattern);
    }

    public void RemoveAntennaPatterns(FssAntennaPatterns pattern)
    {
        AntennaPatterns.Remove(pattern);
    }

    public void RemoveAntennaPatterns(string name)
    {
        FssAntennaPatterns? pattern = GetAntennaPatterns(name);
        if (pattern != null)
            AntennaPatterns.Remove(pattern);
    }

    public void ClearAntennaPatterns()
    {
        AntennaPatterns.Clear();
    }



    public override string ToString()
    {
        return $"AntennaPatterns: {AntennaPatterns.Count}";
    }

}