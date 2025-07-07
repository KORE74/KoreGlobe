using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class GloZeroNodeMapTile : Node3D
{
    public bool ChildTilesExist()
    {
        return ChildTiles.Count > 0;
    }

    private bool DoChildTilesExist()
    {
        return ChildTiles.Count > 0;
    }

    // --------------------------------------------------------------------------------------------

    // Get the list of child tile names, then loop through, finding these nodes, and querying their IsDone property
    private bool AreChildTilesLoaded()
    {
        // return false if there are no child tiles
        if (ChildTiles.Count == 0)
            return false;

        // Loop through the list of child node names, and query the IsDone property of each node.
        foreach (GloZeroNodeMapTile currTile in ChildTiles)
        {
            // Query the IsDone property of the node
            if (!currTile.ConstructionComplete)
                return false;
        }

        // Return true, we've not found any false criteria in the search
        return true;
    }

    // --------------------------------------------------------------------------------------------

    private async void DetermineChildTileAvailability(GloMapTileCode tileCode)
    {
        // Pause the thread, being a good citizen with lots of tasks around.
        await Task.Yield();

        ChildTileDataAvailable = false;

        // loop through all the child tiles for the input tile code, and ensure that they are all available

        // Compile the list of child node names.
        List<GloMapTileCode> childTileCodes = TileCode.ChildCodesList();

        foreach (GloMapTileCode currChildCode in childTileCodes)
        {
            // Get the filenames for each child tile
            GloMapTileFilepaths currChildFilepaths = new GloMapTileFilepaths(currChildCode); // Figure out the file paths for the tile

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // Check if the files exist
            if (!currChildFilepaths.EleArrFileExists)
            {
                ChildTileDataAvailable = false;
                return;
            }
        }

        // No failure to find the elevation tile
        ChildTileDataAvailable = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: LL Checks
    // --------------------------------------------------------------------------------------------

    public bool IsPointInTile(GloLLPoint ll)   => RwTileLLBox.Contains(ll);
    public bool IsPointInTile(GloLLAPoint lla) => RwTileLLBox.Contains(new GloLLPoint() { LatDegs = lla.LatDegs, LonDegs = lla.LonDegs });

}