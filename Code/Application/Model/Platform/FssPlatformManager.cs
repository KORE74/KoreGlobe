using System;
using System.Collections.Generic;

using FssJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public class FssPlatformManager
{
    private List<FssPlatform> PlatfomList = new();

    // --------------------------------------------------------------------------------------------

    public FssPlatformManager()
    {
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Platform List
    // --------------------------------------------------------------------------------------------

    // Considering the platform list by index

    public int NumPlatforms()
    {
        return PlatfomList.Count;
    }

    public void DeleteAllPlatforms()
    {
        PlatfomList.Clear();
    }

    // --------------------------------------------------------------------------------------------

    public FssPlatform? Add(string platname)
    {
        // Check if platform exists
        if (PlatForName(platname) == null)
        {
            FssPlatform newPlat = new FssPlatform() { Name = platname };
            PlatfomList.Add(newPlat);
            return newPlat;
        }
        return null;
    }

    // Looping through the list using the index, and in reverse order to more safely delete a platform
    public void Delete(string platname)
    {
        for (int i = PlatfomList.Count - 1; i >= 0; i--)
        {
            if (PlatfomList[i].Name == platname)
            {
                PlatfomList.RemoveAt(i);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public FssPlatform? PlatForName(string platname)
    {
        foreach (FssPlatform currPlat in PlatfomList)
        {
            if (currPlat.Name == platname)
                return currPlat;
        }
        return null;
    }

    public FssPlatform? PlatForIndex(int index)
    {
        if (index < 0 || index >= PlatfomList.Count)
            return null;

        return PlatfomList[index];
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Platform name * index
    // --------------------------------------------------------------------------------------------

    public string PlatNameForIndex(int index)
    {
        if (index < 0 || index >= PlatfomList.Count)
            return string.Empty;

        return PlatfomList[index].Name;
    }

    public int PlatIndexForName(string platname)
    {
        for (int i = 0; i < PlatfomList.Count; i++)
        {
            if (PlatfomList[i].Name == platname)
                return i;
        }
        return -1;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: PlatIDs & Names
    // --------------------------------------------------------------------------------------------
    // Id being the 1-based user presented version of the list index

    public string PlatNameForId(int platId)
    {
        int platIndex = platId - 1;
        if (platIndex < 0 || platIndex >= PlatfomList.Count)
            return "- - -";

        return PlatfomList[platIndex].Name;
    }

    public string PlatIdForName(string platname)
    {
        for (int i = 0; i < PlatfomList.Count; i++)
        {
            if (PlatfomList[i].Name == platname)
                return (i + 1).ToString();
        }
        return string.Empty;
    }

    public int PlatIdNext(int currPlatId)
    {
        int numPlats = FssAppFactory.Instance.PlatformManager.NumPlatforms();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId < minId)  return minId; // Move up to min if below it
        if (currPlatId >= maxId) return minId; // wrap around to min if at or above max
        return currPlatId + 1;                 // Move up one if mid-range
    }

    public int PlatIdPrev(int currPlatId)
    {
        int numPlats = FssAppFactory.Instance.PlatformManager.NumPlatforms();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId <= minId) return maxId; // wrap around to max if at or below min
        if (currPlatId > maxId)  return maxId; // Move down to max if above it
        return currPlatId - 1;                 // Move down one if mid-range
    }

    public bool DoesPlatformExist(string platname)
    {
        return PlatForName(platname) != null;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Elements
    // --------------------------------------------------------------------------------------------

    public bool DoesElementExist(string platname, string elemname)
    {
        FssPlatform? plat = PlatForName(platname);
        if (plat == null)
            return false;

        return plat.DoesElementExist(elemname);
    }

    public bool AddElement(string platname, FssPlatformElement newElem)
    {
        // Return false if we don't find the platform
        FssPlatform? plat = PlatForName(platname);
        if (plat == null)
            return false;

        plat.AddElement(newElem);
        return true;
    }

    public void DeleteElement(string platname, string elemname)
    {
        FssPlatform? plat = PlatForName(platname);
        if (plat == null)
            return;

        plat.DeleteElement(elemname);
    }

    public FssPlatformElement? ElementForName(string platname, string elemname)
    {
        FssPlatform? plat = PlatForName(platname);
        if (plat == null)
            return null;

        return plat.ElementForName(elemname);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Reset
    // --------------------------------------------------------------------------------------------

    // Reset (or set) all the current platform positions

    public void Reset()
    {
        foreach (FssPlatform currPlat in PlatfomList)
        {
            if (currPlat.Kinetics != null)
                currPlat.Kinetics.ResetPosition();
        }
    }


    // --------------------------------------------------------------------------------------------
    // #MARK: Update
    // --------------------------------------------------------------------------------------------

    public void UpdateKinetics()
    {
        double elapsedSeconds = FssAppFactory.Instance.SimClock.ElapsedTimeSinceMark();
        FssAppFactory.Instance.SimClock.MarkTime();

        foreach (FssPlatform currPlat in PlatfomList)
        {
            if (currPlat.Kinetics != null)
                currPlat.Kinetics.UpdateForDuration((float)elapsedSeconds);
        }

        // Log the LLA of the first platform, to demonstrate some movement.
        if (PlatfomList.Count > 0)
        {
            FssPlatform firstPlat = PlatfomList[0];
            FssLLAPoint currPos = firstPlat.Kinetics.CurrPosition;

            string LLAStr = $"Lat: {currPos.LatDegs:0.0000}, Lon: {currPos.LonDegs:0.0000}, Alt: {currPos.AltMslM:0.0000}";

            FssCentralLog.AddEntry($"Platform: {firstPlat.Name} at {LLAStr}");
        }
    }

    // --------------------------------------------------------------------------------------------

}


