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
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        StringBuilder sb = new();

        sb.AppendLine($"TileList: {TileList.Count} tiles");
        foreach (var tile in TileList)
        {
            sb.AppendLine($"- {tile.Key}");
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------

    // Loop through all of the tiles, checking the bounding box for containing the point, then return the value
    // from the tile with the highest map level.

    public float ElevationAtPos(FssLLPoint pos)
    {
        FssElevationTile? bestTile = null;
        foreach (var tile in TileList.Values)
        {
            if (tile.LLBox.Contains(pos))
            {
                // If this tile has a higher level (more detailed) than the current bestTile, replace it.
                if (bestTile == null || tile.TileCode.MapLvl > bestTile.TileCode.MapLvl)
                {
                    bestTile = tile;
                }
            }
        }

        if (bestTile != null)
        {
            return bestTile.ElevationAtPos(pos);
        }

        return FssElevationPatchSystem.InvalidEle; // Default value if no suitable tile is found.
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Static Utils
    // --------------------------------------------------------------------------------------------

    // We want to have square map tiles as much as possible, so setup a routine to
    // Function to return the number of points for longitude given a latitude.
    // Usage: int lonRes = FssElevationTileSystem.GetLonRes(latRes, latDegs);
    // public static int GetLonRes(int latRes, double latDegs)
    // {
    //     // Clamp latitude to valid range (-90 to 90 degrees).
    //     latDegs = Math.Max(-90.0, Math.Min(90.0, latDegs));

    //     // Convert latitude to radians for easier trigonometric calculations.
    //     double latRads = Math.Abs(latDegs) * (Math.PI / 180.0);

    //     // Calculate a scaling factor based on the cosine of the latitude.
    //     // Near the equator (latitude = 0), factor is 1.0 (full resolution).
    //     // Near the poles (latitude = +/-90), factor approaches 0 (lower resolution).
    //     double scale = Math.Cos(latRads);

    //     // Calculate the adjusted resolution for longitude.
    //     int adjustedLonRes = (int)Math.Max(1, Math.Round(latRes * scale));

    //     return adjustedLonRes;
    // }

    public static FssFloat2DArray PrepTileData(FssElevationPatchSystem elePrepSystem, FssLLBox llBox, int latRes, int lonRes)
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