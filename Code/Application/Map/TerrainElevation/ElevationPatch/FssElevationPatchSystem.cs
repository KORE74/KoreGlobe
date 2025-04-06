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
        // Create lists of all the lats and lons we would iterate across
        FssFloat1DArray loopLatDegs = FssFloat1DArrayOperations.ListForRange((float)llBox.MinLatDegs, (float)llBox.MaxLatDegs, latRes);
        FssFloat1DArray loopLonDegs = FssFloat1DArrayOperations.ListForRange((float)llBox.MinLonDegs, (float)llBox.MaxLonDegs, lonRes);

        // Create the output 2D list
        FssFloat2DArray eleData = new FssFloat2DArray(latRes, lonRes);

        // Create the current position we'll move across the tile
        FssLLPoint currPos = new();

        // Store lengths, avoid repeated property access
        int latLength = loopLatDegs.Length;
        int lonLength = loopLonDegs.Length;

        // Nested loop across the lat and long lists of tile positions
        for (int latIdx = 0; latIdx < latLength; latIdx++)
        {
            currPos.LatDegs = loopLatDegs[latIdx];

            for (int lonIdx = 0; lonIdx < lonLength; lonIdx++)
            {
                currPos.LonDegs = loopLonDegs[lonIdx];

                // Lookup the best value to this position and put it in our returned array
                eleData[latIdx, lonIdx] = ElevationAtPos(currPos);
            }
        }
        FssElevationPatch newTile = new() { ElevationData = eleData, LLBox = llBox };

        return newTile;
    }

    public void AddPatch(FssElevationPatch newpatch)
    {
        TileList.Add(newpatch);
        SortTileList();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation
    // --------------------------------------------------------------------------------------------

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
    // MARK: Sort List
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
        FssElevationPatch? newTile = ArcASCIIToPatch(filename, llBox);
        if (newTile != null)
        {
            TileList.Add(newTile);
            SortTileList();
        }
    }

    // --------------------------------------------------------------------------------------------

    private FssElevationPatch? ArcASCIIToPatch(string filename, FssLLBox llBox)
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