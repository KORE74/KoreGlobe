using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

// FssElevationPrepSystem: A class to contain a number of elevation prep tiles, and supply an elevation value
// for a specific lat/long from the highest resolution (and assumed most accurate) tile it has for that location.

public class FssElevationPrepSystem
{
    // public string RootDir = "";

    // FssElevationPrepSystem.InvalidEle
    public static float InvalidEle      = -9999f;
    public static float InvalidEleCheck = -9990f; // For checking < or > comparisons

    private List<FssElevationPrepTile> TileList = new();

    // public FssMapTileArray MapTiles;

    // --------------------------------------------------------------------------------------------
    // MARK: Get Tile
    // --------------------------------------------------------------------------------------------

    // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
    // Loop aross the points in a tile, populating the requested array.

    public FssElevationPrepTile CreatePrepTile(FssLLBox llBox, int latRes, int lonRes)
    {
        FssFloat2DArray data = new(lonRes, latRes);

        double startLatDegs = llBox.MinLatDegs;
        double startLonDegs = llBox.MinLonDegs;
        double deltaLatDegs = llBox.DeltaLatDegs / latRes;
        double deltaLonDegs = llBox.DeltaLonDegs / lonRes;

        FssLLPoint pos = new(0, 0);

        // Loop through each of the lat and long positions of the destination tile
        for (int latIdx = 0; latIdx < latRes; latIdx++)
        {
            for (int lonIdx = 0; lonIdx < lonRes; lonIdx++)
            {
                pos.LatDegs = startLatDegs + latIdx * deltaLatDegs;
                pos.LonDegs = startLonDegs + lonIdx * deltaLonDegs;

                // Query the ordered source tile list for the elevation for the new position
                data[lonIdx, latIdx] = ElevationAtPos(pos);
            }
        }

        FssElevationPrepTile newTile = new() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
        // Loop across the points in a tile, populating the requested array.

        if (TileList.Count == 0) return InvalidEle;

        foreach (FssElevationPrepTile tile in TileList)
        {
            if (tile.Contains(pos))
                return tile.ElevationAtPos(pos);
        }
        return InvalidEle;
    }

    public string ElevationAtPosWithReport(FssLLPoint pos)
    {
        StringBuilder sb = new StringBuilder();
        float retEle = InvalidEle;

        sb.AppendLine($"Elevation at position: {pos}");

        foreach (FssElevationPrepTile tile in TileList)
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
    // MARK: Sort Tile List
    // --------------------------------------------------------------------------------------------

    // Sort the TileList from highest resolution to lowest, as returned by the TileRes function.

    public void SortTileList()
    {
        TileList.Sort((a, b) => a.TileRes().CompareTo(b.TileRes()));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load / Save Arc ASCII Files
    // --------------------------------------------------------------------------------------------

    // An ASCII Arc file is a simple text file with a header followed by a grid of elevation values.
    // The top-left of the data is the top left of the map.

    public FssElevationPrepTile? ArcASCIIToTile(string filename, FssLLBox llBox)
    {
        // Check the file exists
        if (!System.IO.File.Exists(filename))
            return null;

        // Read the file
        FssFloat2DArray data = FssElevationPrepTileIO.LoadFromArcASIIGridFile(filename);

        // Orient the data in the 2D array, placing (maxlat, minLon) at top-left.
        FssFloat2DArray flippedData = FssFloat2DArray.FlipXAxis(data);

        // Create the tile
        FssElevationPrepTile newTile = new() { ElevationData = data, LLBox = llBox };

        // Add tile to internal list and sort in descending order of resolution
        TileList.Add(newTile);
        SortTileList();

        return newTile;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        string report = $"Elevation System Report: {TileList.Count} Tile(s)\n";

        foreach (FssElevationPrepTile tile in TileList)
        {
            report += tile.Report() + "\n";
        }
        return report;
    }

}