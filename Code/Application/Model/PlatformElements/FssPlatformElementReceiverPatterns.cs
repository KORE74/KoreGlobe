using System;
using System.Collections.Generic;

#nullable enable

public class FssRecieverPattern
{
    public string Name { get; set; } = "";
    public FssFloat2DArray SphereMagPattern = new FssFloat2DArray();
}

public class FssPlatformElementRecieverPatterns : FssPlatformElement
{
    // List of receivers
    public List<FssRecieverPattern> RecieverPatterns = new List<FssRecieverPattern>();

    // --------------------------------------------------------------------------------------------

    public List<string> GetRecieverPatternNames()
    {
        List<string> names = new();
        foreach (FssRecieverPattern reciever in RecieverPatterns)
            names.Add(reciever.Name);
        return names;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK Basic pattern mamangement
    // --------------------------------------------------------------------------------------------

    public FssRecieverPattern? GetRecieverPattern(string name)
    {
        foreach (FssRecieverPattern reciever in RecieverPatterns)
        {
            if (reciever.Name == name)
                return reciever;
        }
        return null;
    }

    public void AddRecieverPattern(FssRecieverPattern reciever)
    {
        RecieverPatterns.Add(reciever);
    }

    public void RemoveRecieverPattern(FssRecieverPattern reciever)
    {
        RecieverPatterns.Remove(reciever);
    }

    public void RemoveRecieverPattern(string name)
    {
        FssRecieverPattern? reciever = GetRecieverPattern(name);
        if (reciever != null)
            RecieverPatterns.Remove(reciever);
    }

    public void ClearRecieverPatterns()
    {
        RecieverPatterns.Clear();
    }
}
