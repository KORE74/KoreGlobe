using System;
using System.Collections.Generic;

#nullable enable

// FssMap is a top level class for a map data structure.
public class FssElevationSystem
{
    // public string RootDir = "";

    // FssElevationSystem.InvalidEle
    public static float InvalidEle      = -9999f;
    public static float InvalidEleCheck = -9990f; // For checking < or > comparisons

    private List<FssElevationTile> TileList = new List<FssElevationTile>();

    // public FssMapTileArray MapTiles;

    // --------------------------------------------------------------------------------------------
    // MARK: Get Tile
    // --------------------------------------------------------------------------------------------

    // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
    // Loop aross the points in a tile, populating the requested array.

    public FssElevationTile CreateTile(FssLLBox llBox, int latRes, int lonRes)
    {
        FssFloat2DArray data = new FssFloat2DArray(lonRes, latRes);

        double startLatDegs = llBox.MinLatDegs;
        double startLonDegs = llBox.MinLonDegs;
        double deltaLatDegs = llBox.DeltaLatDegs / latRes;
        double deltaLonDegs = llBox.DeltaLonDegs / lonRes;

        FssLLPoint pos = new FssLLPoint(0, 0);

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

        FssElevationTile newTile = new FssElevationTile() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
        // Loop across the points in a tile, populating the requested array.

        if (TileList.Count == 0) return InvalidEle;

        foreach (FssElevationTile tile in TileList)
        {
            if (tile.Contains(pos))
                return tile.ElevationAtPos(pos);
        }
        return InvalidEle;
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

    public FssElevationTile? LoadArcASCIIFileToTile(string filename, FssLLBox llBox)
    {
        FssElevationTile? newTile = ArcASCIIToTile(filename, llBox);
        if (newTile != null)
            TileList.Add(newTile);

        return newTile;
    }

    public static FssElevationTile? ArcASCIIToTile(string filename, FssLLBox llBox)
    {
        // Check the file exists
        if (!System.IO.File.Exists(filename)) return null;

        // Read the file
        FssFloat2DArray data = FssFloat2DArrayIO.LoadFromArcASIIGridFile(filename);

        // Create the tile
        FssElevationTile newTile = new FssElevationTile() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        string report = $"Elevation System Report: {TileList.Count} Tile(s)\n";

        foreach (FssElevationTile tile in TileList)
        {
            report += tile.Report() + "\n";
        }
        return report;
    }

}