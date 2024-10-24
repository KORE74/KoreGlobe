using System;
using System.Collections.Generic;
using System.Text;

using FssJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public partial class FssEntityManager
{
    private List<FssEntity> EntityList = new();

    // --------------------------------------------------------------------------------------------

    public FssEntityManager()
    {
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Platform List
    // --------------------------------------------------------------------------------------------

    // Considering the platform list by index

    public int  NumEntities()       => EntityList.Count;
    public void DeleteAllEntities() => EntityList.Clear();

    // --------------------------------------------------------------------------------------------

    public FssEntity? Add(string entityname)
    {
        // Check if platform exists
        if (EntityForName(entityname) == null)
        {
            FssEntity newEntity = new FssEntity() { Name = entityname };
            EntityList.Add(newEntity);
            return newEntity;
        }
        return null;
    }

    // Looping through the list using the index, and in reverse order to more safely delete a platform
    public void Delete(string entityname)
    {
        for (int i = EntityList.Count - 1; i >= 0; i--)
        {
            if (EntityList[i].Name == entityname)
            {
                EntityList.RemoveAt(i);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public FssEntity? EntityForName(string entityname)
    {
        foreach (FssEntity currPlat in EntityList)
        {
            if (currPlat.Name == entityname)
                return currPlat;
        }
        return null;
    }

    public FssEntity? EntityForIndex(int index)
    {
        if (index < 0 || index >= EntityList.Count)
            return null;

        return EntityList[index];
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Platform name * index
    // --------------------------------------------------------------------------------------------

    public string EntityNameForIndex(int index)
    {
        if (index < 0 || index >= EntityList.Count)
            return string.Empty;

        return EntityList[index].Name;
    }

    public int EntityIndexForName(string entityname)
    {
        for (int i = 0; i < EntityList.Count; i++)
        {
            if (EntityList[i].Name == entityname)
                return i;
        }
        return -1;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: PlatIDs & Names
    // --------------------------------------------------------------------------------------------
    // Id being the 1-based user presented version of the list index

    public string EntityNameForId(int entityId)
    {
        int entityIndex = entityId - 1;
        if (entityIndex < 0 || entityIndex >= EntityList.Count)
            return "- - -";

        return EntityList[entityIndex].Name;
    }

    public string EntityIdForName(string entityname)
    {
        for (int i = 0; i < EntityList.Count; i++)
        {
            if (EntityList[i].Name == entityname)
                return (i + 1).ToString();
        }
        return string.Empty;
    }

    public int EntityIdNext(int currPlatId)
    {
        int numPlats = FssAppFactory.Instance.EntityManager.NumEntities();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId < minId)  return minId; // Move up to min if below it
        if (currPlatId >= maxId) return minId; // wrap around to min if at or above max
        return currPlatId + 1;                 // Move up one if mid-range
    }

    public int EntityIdPrev(int currPlatId)
    {
        int numPlats = FssAppFactory.Instance.EntityManager.NumEntities();
        int minId    = (numPlats > 0) ? 1 : 0;
        int maxId    = (numPlats > 0) ? numPlats: 0;

        if (numPlats == 0)       return 0;     // return 0 if no platforms
        if (currPlatId <= minId) return maxId; // wrap around to max if at or below min
        if (currPlatId > maxId)  return maxId; // Move down to max if above it
        return currPlatId - 1;                 // Move down one if mid-range
    }

    public bool DoesEntityExist(string entityname)
    {
        return EntityForName(entityname) != null;
    }

    public List<string> EntityNameList()
    {
        List<string> entityNames = new();
        foreach (FssEntity currPlat in EntityList)
        {
            entityNames.Add(currPlat.Name);
        }
        return entityNames;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Elements
    // --------------------------------------------------------------------------------------------

    public bool DoesElementExist(string entityname, string elemname)
    {
        FssEntity? plat = EntityForName(entityname);
        if (plat == null)
            return false;

        return plat.DoesElementExist(elemname);
    }

    public bool AddElement(string entityname, FssElement newElem)
    {
        // Return false if we don't find the platform
        FssEntity? plat = EntityForName(entityname);
        if (plat == null)
            return false;

        plat.AddElement(newElem);
        return true;
    }

    public void DeleteElement(string entityname, string elemname)
    {
        FssEntity? plat = EntityForName(entityname);
        if (plat == null)
            return;

        plat.DeleteElement(elemname);
    }

    public FssElement? ElementForName(string entityname, string elemname)
    {
        FssEntity? plat = EntityForName(entityname);
        if (plat == null)
            return null;

        return plat.ElementForName(elemname);
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Reports
    // --------------------------------------------------------------------------------------------

    public string PlatformPositionsReport()
    {
        string report = "Platform Positions Report\n";

        foreach (FssEntity currPlat in EntityList)
        {
            if (currPlat.Kinetics != null)
            {
                FssLLAPoint currPos = currPlat.Kinetics.CurrPosition;
                report += $"Platform:{currPlat.Name} at LatDegs:{currPos.LatDegs:0.0000}, LonDegs:{currPos.LonDegs:0.0000}, AltMslM:{currPos.AltMslM:0.0000}, RadiusM:{currPos.RadiusM}\n";
            }
        }

        return report;
    }

    public string PlatformElementsReport()
    {
        string report = "Platform Elements Report\n";

        foreach (FssEntity currPlat in EntityList)
        {
            report += $"Platform: {currPlat.Name}\n";

            foreach (FssElement currElem in currPlat.ElementsList)
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
        foreach (FssEntity currPlat in EntityList)
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
        foreach (FssEntity currPlat in EntityList)
        {
            sb.AppendLine($"Platform: {currPlat.Name}");
            foreach (FssElement currElem in currPlat.ElementsList)
            {
                sb.AppendLine($"- Element: {currElem.Report()}");
            }
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Reset
    // --------------------------------------------------------------------------------------------

    // Reset (or set) all the current platform positions

    public void Reset()
    {
        foreach (FssEntity currPlat in EntityList)
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

        foreach (FssEntity currPlat in EntityList)
        {
            if (currPlat.Kinetics != null)
                currPlat.Kinetics.UpdateForDuration((float)elapsedSeconds);
        }

        // Log the LLA of the first platform, to demonstrate some movement.
        if (EntityList.Count > 0)
        {
            FssEntity firstPlat = EntityList[0];
            FssLLAPoint currPos = firstPlat.Kinetics.CurrPosition;

            string LLAStr = $"Lat: {currPos.LatDegs:0.0000}, Lon: {currPos.LonDegs:0.0000}, Alt: {currPos.AltMslM:0.0000}";

            FssCentralLog.AddEntry($"Platform: {firstPlat.Name} at {LLAStr}");
        }
    }

    // --------------------------------------------------------------------------------------------

}


