using System;
using System.Collections.Generic;

public class GlobeRecieverPattern
{
    public string Name { get; set; } = "";
    public GlobeFloat2DArray SphereMagPattern = new GlobeFloat2DArray();
}

public class GlobePlatformElementRecieverPatterns : GlobePlatformElement
{
    // List of receivers
    public List<GlobeRecieverPattern> RecieverPatterns = new List<GlobeRecieverPattern>();

    // --------------------------------------------------------------------------------------------

    public List<string> GetRecieverPatternNames()
    {
        List<string> names = new();
        foreach (GlobeRecieverPattern reciever in RecieverPatterns)
            names.Add(reciever.Name);
        return names;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK Basic pattern mamangement
    // --------------------------------------------------------------------------------------------

    public GlobeRecieverPattern? GetRecieverPattern(string name)
    {
        foreach (GlobeRecieverPattern reciever in RecieverPatterns)
        {
            if (reciever.Name == name)
                return reciever;
        }
        return null;
    }

    public void AddRecieverPattern(GlobeRecieverPattern reciever)
    {
        RecieverPatterns.Add(reciever);
    }

    public void RemoveRecieverPattern(GlobeRecieverPattern reciever)
    {
        RecieverPatterns.Remove(reciever);
    }

    public void RemoveRecieverPattern(string name)
    {
        GlobeRecieverPattern? reciever = GetRecieverPattern(name);
        if (reciever != null)
            RecieverPatterns.Remove(reciever);
    }

    public void ClearRecieverPatterns()
    {
        RecieverPatterns.Clear();
    }
}
