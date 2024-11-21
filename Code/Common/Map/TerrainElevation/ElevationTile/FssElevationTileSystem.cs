using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

#nullable enable

// FssElevationTileSystem: A class to contain a list of map tile elevations that can be looked up when required.
// Tiles can be created from the ElevationPrepSystem, or loaded/saved to file.

public class FssElevationTileSystem
{
    // List of tile elevations, indexed by the tilecode string.
    private ConcurrentDictionary<string, FssElevationTile> TileList = new();

    // Time for textures to keep alive (in seconds).
    private const int KeepAliveTime = 10;
    private ConcurrentDictionary<string, int> LastAccessTime = new ConcurrentDictionary<string, int>();

    // --------------------------------------------------------------------------------------------
    // MARK: Tile Collection
    // --------------------------------------------------------------------------------------------

    public void AddTile(FssElevationTile tile)
    {
        string name          = tile.TileCode.TileCode;

        TileList[name]       = tile;
        LastAccessTime[name] = FssCentralTime.RuntimeIntSecs;
    }

    public bool HasTile(string name)
    {
        if (TileList.ContainsKey(name))
        {
            LastAccessTime[name] = FssCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    public FssElevationTile GetTile(string name)
    {
        if (TileList.ContainsKey(name))
        {
            LastAccessTime[name] = FssCentralTime.RuntimeIntSecs;
            return TileList[name];
        }
        return FssElevationTile.Zero;
    }

    public void DeleteTile(string name)
    {
        TileList.TryRemove(name, out _);
        LastAccessTime.TryRemove(name, out _);
    }

    public void DeleteAllTiles()
    {
        TileList.Clear();
        LastAccessTime.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Static Utils
    // --------------------------------------------------------------------------------------------

    public static FssFloat2DArray PrepTileData(FssElevationPrepSystem elePrepSystem, FssLLBox llBox, int latRes, int lonRes)
    {
        // Create lists of all the lats and lons we would iterate across
        FssFloat1DArray loopLats = FssFloat1DArrayOperations.ListForRange((float)llBox.MinLatDegs, (float)llBox.MaxLatDegs, latRes);
        FssFloat1DArray loopLons = FssFloat1DArrayOperations.ListForRange((float)llBox.MinLonDegs, (float)llBox.MaxLonDegs, lonRes);

        // Create the output 2D list
        FssFloat2DArray retEle = new FssFloat2DArray(latRes, lonRes);

        // Create the current position we'll move acorss the tile
        FssLLPoint currPos = new();

        // Nested loop across the lat and long lists of tile positions
        for (int latIdx = 0; latIdx < loopLats.Length; latIdx++)
        {
            currPos.LatDegs = loopLats[latIdx];
            for (int lonIdx = 0; lonIdx < loopLons.Length; lonIdx++)
            {
                currPos.LonDegs = loopLons[lonIdx];

                // Lookup the best value to this position and put it in our returned array
                retEle[latIdx, lonIdx] = elePrepSystem.ElevationAtPos(currPos);
            }
        }

        return retEle;
    }
}