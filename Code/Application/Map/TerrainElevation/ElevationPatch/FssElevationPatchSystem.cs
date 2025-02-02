using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

// FssElevationPatchSystem: A class to contain a number of elevation prep tiles, and supply an elevation value
// for a specific lat/long from the highest resolution (and assumed most accurate) tile it has for that location.

public class FssElevationPatchSystem
{
    // public string RootDir = "";

    private List<FssElevationPatch> TileList = new();

    // public FssMapTileArray MapTiles;

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
    // Loop aross the points in a tile, populating the requested array.

    public FssElevationPatch CreatePatch(FssLLBox llBox, int latRes, int lonRes)
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

        FssElevationPatch newTile = new() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    public float ElevationAtPos(FssLLPoint pos)
    {
        // Loop through the TileList, grabbing points from the highest resolution tile that contains the position.
        // Loop across the points in a tile, populating the requested array.

        if (TileList.Count == 0) return FssElevationConsts.InvalidEle;

        foreach (FssElevationPatch tile in TileList)
        {
            if (tile.Contains(pos))
                return tile.ElevationAtPos(pos);
        }
        return FssElevationConsts.InvalidEle;
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
    // MARK: Load Patch
    // --------------------------------------------------------------------------------------------

    public void LoadPatchFile(string inPatchFilepath)
    {
        FssElevationPatch? newpatch = FssElevationPatchIO.ReadFromTextFile(inPatchFilepath);
        if (newpatch != null)
        {
            TileList.Add(newpatch);
            SortTileList();
        }
    }

    public void CreatePatchFile(string inPatchFilepath, FssLLBox llBox, int latRes, int lonRes)
    {
        FssElevationPatch newPatch = CreatePatch(llBox, latRes, lonRes);
        FssElevationPatchIO.WriteToTextFile(newPatch, inPatchFilepath);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load / Save Arc ASCII Files
    // --------------------------------------------------------------------------------------------

    // An ASCII Arc file is a simple text file with a header followed by a grid of elevation values.
    // The top-left of the data is the top left of the map.

    public void LoadArcASCII(string filename, FssLLBox llBox)
    {
        FssElevationPatch? newTile = ArcASCIIToTile(filename, llBox);
        if (newTile != null)
        {
            TileList.Add(newTile);
            SortTileList();
        }
    }

    // --------------------------------------------------------------------------------------------

    private FssElevationPatch? ArcASCIIToTile(string filename, FssLLBox llBox)
    {
        // Check the file exists
        if (!System.IO.File.Exists(filename))
            return null;

        // Read the file
        FssFloat2DArray data = FssElevationPatchIO.LoadFromArcASIIGridFile(filename);

        // Orient the data in the 2D array, placing (maxlat, minLon) at top-left.
        FssFloat2DArray flippedData = FssFloat2DArray.FlipXAxis(data);

        // Create the tile
        FssElevationPatch newTile = new() { ElevationData = data, LLBox = llBox };

        return newTile;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        string report = $"Elevation System Report: {TileList.Count} Tile(s)\n";

        foreach (FssElevationPatch tile in TileList)
        {
            report += tile.Report() + "\n";
        }
        return report;
    }

}