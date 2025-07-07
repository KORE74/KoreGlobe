using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

#nullable enable

// GloElevationTileSystem: A class to contain a list of map tile elevations that can be looked up when required.
// Tiles can be created from the ElevationPrepSystem, or loaded/saved to file.

public class GloElevationTileSystem
{
    // List of tile elevations, indexed by the tilecode string.
    private ConcurrentDictionary<string, GloElevationTile> TileList = new();

    // Time for textures to keep alive (in seconds).
    private const int KeepAliveTime = 10;
    private ConcurrentDictionary<string, int> LastAccessTime = new ConcurrentDictionary<string, int>();

    // --------------------------------------------------------------------------------------------
    // MARK: Tile Collection
    // --------------------------------------------------------------------------------------------

    public void AddTile(GloElevationTile tile)
    {
        string name = tile.TileCode.ToString();
        if (String.IsNullOrEmpty(name))
        {
            GloCentralLog.AddEntry($"TileCode name Empty, not adding tile");
        }
        else
        {
            TileList[name]       = tile;
            LastAccessTime[name] = GloCentralTime.RuntimeIntSecs;
        }
    }

    public bool HasTile(string name)
    {
        if (TileList.ContainsKey(name))
        {
            LastAccessTime[name] = GloCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    public GloElevationTile GetTile(string name)
    {
        if (TileList.ContainsKey(name))
        {
            LastAccessTime[name] = GloCentralTime.RuntimeIntSecs;
            return TileList[name];
        }
        return GloElevationTile.Zero;
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

    public static GloElevationTile CreateTile(GloElevationPatchSystem elePrepSystem, GloMapTileCode tileCode)
    {
        // Setup the tile defining values
        GloLLBox llBox      = tileCode.LLBox;
        int      tileResLat = 50;
        int      tileResLon = GloElevationUtils.LonResForLat(tileResLat, llBox.CenterPoint.LatDegs);

        // Big operation: get the 2D array of elevations, which itself may require interpolation across nested arrays.
        GloFloat2DArray eleData = PrepTileData(elePrepSystem, llBox, tileResLat, tileResLon);

        // Create the new tile and add it to the collection
        GloElevationTile newTile = new() {
            TileCode      = tileCode,
            ElevationData = eleData,
            LLBox         = llBox
        };

        return newTile;
    }

    public static GloFloat2DArray PrepTileData(GloElevationPatchSystem elePrepSystem, GloLLBox llBox, int latRes, int lonRes)
    {
        // Create lists of all the lats and lons we would iterate across
        GloFloat1DArray loopLats = GloFloat1DArrayOperations.ListForRange((float)llBox.MinLatDegs, (float)llBox.MaxLatDegs, latRes);
        GloFloat1DArray loopLons = GloFloat1DArrayOperations.ListForRange((float)llBox.MinLonDegs, (float)llBox.MaxLonDegs, lonRes);

        // Flip the latitudes, so we know the top and bottom of the output arr files are intuitively the right way up.
        GloFloat1DArray reverselats = loopLats.Reverse();

        // Create the output 2D list
        // [X, Y]
        GloFloat2DArray retEle = new GloFloat2DArray(lonRes, latRes);

        // Create the current position we'll move acorss the tile
        GloLLPoint currPos = new();

        // Nested loop across the lat and long lists of tile positions
        for (int latIdx = 0; latIdx < reverselats.Length; latIdx++)
        {
            currPos.LatDegs = reverselats[latIdx];
            for (int lonIdx = 0; lonIdx < loopLons.Length; lonIdx++)
            {
                currPos.LonDegs = loopLons[lonIdx];

                // Lookup the best value to this position and put it in our returned array
                // [X, Y]
                retEle[lonIdx, latIdx] = elePrepSystem.ElevationAtPos(currPos);
            }
        }

        return retEle;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Query Elevation
    // --------------------------------------------------------------------------------------------

    // Find the most precise elevation value we keep stored, for any given lat long.
    // Reasonably expensive in terms of the calculations and lookups involved.

    public float FindElevation(double latDegs, double lonDegs)
    {
        // Start at the highest map level
        int currentLevel = GloMapTileCode.MaxMapLvl - 1;

        // Create the most specific tile code for the given lat/lon
        GloMapTileCode currentTileCode = new(latDegs, lonDegs, currentLevel);

        while (currentTileCode.MapLvl > 0)
        {
            // Check if the tile exists in the system
            if (HasTile(currentTileCode.TileCode))
            {
                // Get the tile and find elevation at the lat/lon within the tile
                var tile = GetTile(currentTileCode.TileCode);

                // Lookup and return the elevation value
                return tile.GetElevation(latDegs, lonDegs);
            }

            // If we're at the global Lvl0 map level and still not found a level, there's no where else to go but return a default/invlid value.
            if (currentTileCode.MapLvl == 0)
                return GloElevationUtils.InvalidEle;

            // If not found, move to the parent tile and try again
            currentTileCode = currentTileCode.ParentCode();
        }

        // Default return value if logic fails
        return GloElevationUtils.InvalidEle;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public string Report()
    {
        var reportBuilder = new StringBuilder();

        reportBuilder.AppendLine("GloElevationTileSystem Report:");
        reportBuilder.AppendLine("----------------------------------------");

        foreach (var tileEntry in TileList)
        {
            string tileCode = tileEntry.Key;
            GloElevationTile tile = tileEntry.Value;
            int lastAccess = LastAccessTime.ContainsKey(tileCode) ? LastAccessTime[tileCode] : -1;

            reportBuilder.Append($"Tile Code: {tileCode}");
            //reportBuilder.AppendLine($"Elevation Data: {tile.GetElevationSummary()}");
            reportBuilder.Append($"Last Accessed: {lastAccess} seconds since runtime start");
            //reportBuilder.AppendLine("----------------------------------------");

            reportBuilder.Append('\n');
        }

        return reportBuilder.ToString();
    }


}
