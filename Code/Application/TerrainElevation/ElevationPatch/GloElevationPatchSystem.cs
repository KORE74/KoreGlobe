using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

// GloElevationPatchSystem: A class to contain a number of elevation prep tiles, and supply an elevation value
// for a specific lat/long from the highest resolution (and assumed most accurate) tile it has for that location.

public class GloElevationPatchSystem
{
    // GloElevationPatchSystem.InvalidEle

    private List<GloElevationPatch> PatchList = new();

    // --------------------------------------------------------------------------------------------
    // MARK: Load Arc ASCII Files
    // --------------------------------------------------------------------------------------------

    // An ASCII Arc file is a simple text file with a header followed by a grid of elevation values.
    // The top-left of the data is the top left of the map.

    public GloElevationPatch? ArcASCIIToTile(string filename, GloLLBox llBox)
    {
        // Check the file exists
        if (!System.IO.File.Exists(filename))
        {
            GloCentralLog.AddEntry($"ArcASCIIToTile: File not found: {filename}");
            return null;
        }

        // Read the file
        GloFloat2DArray data = GloElevationPatchIO.LoadFromArcASIIGridFile(filename);

        // Orient the data in the 2D array, placing (maxlat, minLon) at top-left.
        //GloFloat2DArray flippedData = GloFloat2DArray.FlipXAxis(data);

        // Create the tile
        GloElevationPatch newTile = new() { ElevationData = data, LLBox = llBox };

        // Add tile to internal list and sort in descending order of resolution
        PatchList.Add(newTile);
        SortPatchList();

        GloCentralLog.AddEntry($"ArcASCIIToTile: Loaded {filename} {llBox}");

        return newTile;
    }


    public GloElevationPatch CreateNewPatch(GloLLBox llBox, int numLatPoints, int numLonPoints)
    {
        GloElevationPatch newTile = new() { ElevationData = new GloFloat2DArray(numLonPoints, numLatPoints), LLBox = llBox };

        // Create the value range for lat and longs that we'll cover in the new patch
        GloNumericRange<double> latRange = new GloNumericRange<double>(llBox.MinLatDegs, llBox.MaxLatDegs);
        GloNumericRange<double> lonRange = new GloNumericRange<double>(llBox.MinLonDegs, llBox.MaxLonDegs);

        // Turn the range into a list of lat/lon values we can iterate over.
        GloNumeric1DArray<double> latArray = new GloNumeric1DArray<double>(latRange, numLatPoints, direction:GloNumeric1DArray<double>.ListDirection.Reverse);
        GloNumeric1DArray<double> lonArray = new GloNumeric1DArray<double>(lonRange, numLonPoints);

        for (int i = 0; i < latArray.Length; i++)
        {
            for (int j = 0; j < lonArray.Length; j++)
            {
                GloLLPoint newPoint = new GloLLPoint() { LatDegs = latArray[i], LonDegs = lonArray[j] };
                newTile.ElevationData[j, i] = ElevationAtPos(newPoint);
            }
        }
        return newTile;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Sort Tile List
    // --------------------------------------------------------------------------------------------

    // Sort the PatchList from highest resolution to lowest, as returned by the TileRes function.

    public void SortPatchList()
    {
        PatchList.Sort((a, b) => a.TileRes().CompareTo(b.TileRes()));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Empty Tile List
    // --------------------------------------------------------------------------------------------

    public void AddPatch(GloElevationPatch newPatch)
    {
        PatchList.Add(newPatch);
        SortPatchList();
    }

    public void EmptyPatchList()
    {
        PatchList.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation At Pos
    // --------------------------------------------------------------------------------------------

    public float ElevationAtPos(GloLLPoint pos)
    {
        // Loop through the PatchList, grabbing points from the highest resolution tile that contains the position.
        // Loop across the points in a tile, populating the requested array.

        if (PatchList.Count == 0) return GloElevationUtils.InvalidEle;

        foreach (GloElevationPatch tile in PatchList)
        {
            if (tile.Contains(pos))
                return tile.ElevationAtPos(pos);
        }
        return GloElevationUtils.InvalidEle;
    }

    public string ElevationAtPosWithReport(GloLLPoint pos)
    {
        StringBuilder sb = new StringBuilder();
        float retEle = GloElevationUtils.InvalidEle;

        sb.AppendLine($"Elevation at position: {pos}");

        foreach (GloElevationPatch tile in PatchList)
        {
            sb.AppendLine($"Considering Tile: {tile.Report()}");
            if (tile.Contains(pos))
            {
                sb.AppendLine($"position in Tile");

                retEle = tile.ElevationAtPos(pos);
                sb.AppendLine($"Elevation: {retEle}");
            }
            else
            {
                sb.AppendLine($"position NOT in Tile");
            }
        }
        sb.AppendLine($"Concluding Elevation: {retEle}");
        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        string report = $"Elevation System Report: {PatchList.Count} Tile(s)\n";

        foreach (GloElevationPatch tile in PatchList)
        {
            report += tile.Report() + "\n";
        }
        return report;
    }

}