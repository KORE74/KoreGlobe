using System;
using System.Collections.Generic;
using System.Text;

using GloJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public partial class GloPlatformManager
{
    private List<GloPlatform> PlatfomList = new();

    // --------------------------------------------------------------------------------------------

    public GloPlatformManager()
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Platform List
    // --------------------------------------------------------------------------------------------

    // Considering the platform list by index

    public int  NumPlatforms()       => PlatfomList.Count;
    public void DeleteAllPlatforms() => PlatfomList.Clear();

    // --------------------------------------------------------------------------------------------

    public GloPlatform? Add(string platname)
    {
        // Check if platform exists
        if (PlatForName(platname) == null)
        {
            GloPlatform newPlat = new GloPlatform() { Name = platname };
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

    public GloPlatform? PlatForName(string platname)
    {
        foreach (GloPlatform currPlat in PlatfomList)
        {
            if (currPlat.Name == platname)
                return currPlat;
        }
        return null;
    }

    public GloPlatform? PlatForIndex(int index)
    {
        if (index < 0 || index >= PlatfomList.Count)
            return null;

        return PlatfomList[index];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Platform name * index
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
    // MARK: PlatIDs & Names
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
        int numPlats = GloAppFactory.Instance.PlatformManager.NumPlatforms();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId < minId)  return minId; // Move up to min if below it
        if (currPlatId >= maxId) return minId; // wrap around to min if at or above max
        return currPlatId + 1;                 // Move up one if mid-range
    }

    public int PlatIdPrev(int currPlatId)
    {
        int numPlats = GloAppFactory.Instance.PlatformManager.NumPlatforms();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId <= minId) return maxId; // wrap around to max if at or below min
        if (currPlatId > maxId)  return maxId; // Move down to max if above it
        return currPlatId - 1;                 // Move down one if mid-range
    }

    public bool DoesPlatExist(string platname)
    {
        return PlatForName(platname) != null;
    }

    public List<string> PlatNameList()
    {
        List<string> platNames = new();
        foreach (GloPlatform currPlat in PlatfomList)
        {
            platNames.Add(currPlat.Name);
        }
        return platNames;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elements
    // --------------------------------------------------------------------------------------------

    public bool DoesElementExist(string platname, string elemname)
    {
        GloPlatform? plat = PlatForName(platname);
        if (plat == null)
            return false;

        return plat.DoesElementExist(elemname);
    }

    public bool AddElement(string platname, GloPlatformElement newElem)
    {
        // Return false if we don't find the platform
        GloPlatform? plat = PlatForName(platname);
        if (plat == null)
            return false;

        plat.AddElement(newElem);
        return true;
    }

    public void DeleteElement(string platname, string elemname)
    {
        GloPlatform? plat = PlatForName(platname);
        if (plat == null)
            return;

        plat.DeleteElement(elemname);
    }

    public GloPlatformElement? ElementForName(string platname, string elemname)
    {
        GloPlatform? plat = PlatForName(platname);
        if (plat == null)
            return null;

        return plat.ElementForName(elemname);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reports
    // --------------------------------------------------------------------------------------------

    public string PlatformPositionsReport()
    {
        string report = "Platform Positions Report\n";

        foreach (GloPlatform currPlat in PlatfomList)
        {
            if (currPlat.Kinetics != null)
            {
                GloLLAPoint currPos = currPlat.Kinetics.CurrPosition;
                report += $"Platform:{currPlat.Name} at LatDegs:{currPos.LatDegs:0.0000}, LonDegs:{currPos.LonDegs:0.0000}, AltMslM:{currPos.AltMslM:0.0000}, RadiusM:{currPos.RadiusM}\n";
            }
        }

        return report;
    }

    public string PlatformElementsReport()
    {
        string report = "Platform Elements Report\n";

        foreach (GloPlatform currPlat in PlatfomList)
        {
            report += $"Platform: {currPlat.Name}\n";

            foreach (GloPlatformElement currElem in currPlat.ElementsList)
            {
                report += $"- Element: {currElem.Name} Type: {currElem.Type}\n";
            }
        }

        return report;
    }

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Platform Manager Report");
        sb.AppendLine("=======================");
        sb.AppendLine();

        sb.AppendLine("Platform List");
        sb.AppendLine("-------------");
        foreach (GloPlatform currPlat in PlatfomList)
        {
            sb.AppendLine($"Platform: {currPlat.Name}");
            sb.AppendLine($"- CurrPosition: {currPlat.Kinetics?.CurrPosition}");
            sb.AppendLine($"- StartPosition: {currPlat.Kinetics?.StartPosition}");
            sb.AppendLine($"- CurrAttitude: {currPlat.Kinetics?.CurrAttitude}");
            sb.AppendLine($"- CurrCourse: {currPlat.Kinetics?.CurrCourse}");
            sb.AppendLine($"- CurrCourseDelta: {currPlat.Kinetics?.CurrCourseDelta}");
        }

        sb.AppendLine();
        sb.AppendLine("Platform Elements");
        sb.AppendLine("-----------------");
        foreach (GloPlatform currPlat in PlatfomList)
        {
            sb.AppendLine($"Platform: {currPlat.Name}");
            foreach (GloPlatformElement currElem in currPlat.ElementsList)
            {
                sb.AppendLine($"- Element: {currElem.Report()}");
            }
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reset
    // --------------------------------------------------------------------------------------------

    // Reset (or set) all the current platform positions

    public void Reset()
    {
        foreach (GloPlatform currPlat in PlatfomList)
        {
            if (currPlat.Kinetics != null)
                currPlat.Kinetics.ResetPosition();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void UpdateKinetics()
    {
        double elapsedSeconds = GloAppFactory.Instance.SimClock.ElapsedTimeSinceMark();
        GloAppFactory.Instance.SimClock.MarkTime();

        foreach (GloPlatform currPlat in PlatfomList)
        {
            if (currPlat.Kinetics != null)
                currPlat.Kinetics.UpdateForDuration((float)elapsedSeconds);
        }

        // Log the LLA of the first platform, to demonstrate some movement.
        if (PlatfomList.Count > 0)
        {
            GloPlatform firstPlat = PlatfomList[0];
            GloLLAPoint currPos = firstPlat.Kinetics.CurrPosition;

            string LLAStr = $"Lat: {currPos.LatDegs:0.0000}, Lon: {currPos.LonDegs:0.0000}, Alt: {currPos.AltMslM:0.0000}";

            GloCentralLog.AddEntry($"Platform: {firstPlat.Name} at {LLAStr}");
        }
    }

    // --------------------------------------------------------------------------------------------

}


