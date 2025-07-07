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
    // --------------------------------------------------------------------------------------------
    // MARK: Elevation Methods
    // --------------------------------------------------------------------------------------------

    private void LoadTileEleArr()
    {
        GloElevationTile? eleTile = GloElevationTileIO.ReadFromTextFile(Filepaths.EleArrFilepath);

        if (eleTile != null)
        {
            TileEleData = eleTile.ElevationData;

            // if we have a subsurface tile, set it to a lower resolution
            if (TileEleData.MaxVal() <= 0)
                TileEleData = new GloFloat2DArray(10, 10);

            // Write the simplified data back to the file, to be faster next time.
            eleTile.ElevationData = TileEleData;
            GloElevationTileIO.WriteToTextFile(eleTile, Filepaths.EleArrFilepath);
        }
        else
        {
            GloCentralLog.AddEntry($"Failed to load: {Filepaths.EleArrFilepath}");
        }

        TileEleData = GloFloat2DArrayOperations.CropToRange(TileEleData, new GloFloatRange(0f, 10000f));
    }

    // --------------------------------------------------------------------------------------------

    // private void SubsampleParentTileEle()
    // {
    //     GloLLPoint tileCenter = TileCode.LLBox.CenterPoint;
    //     int tileResLat = TileSizePointsPerLvl[TileCode.MapLvl];
    //     int tileResLon = GloElevationUtils.LonResForLat(tileResLat, tileCenter.LatRads);

    //     // Use the latitude resolution and latitude to figure out a longitude resolution the gives the "most square" tiles possible.
    //     // int tileResLon = GloElevationUtils.LonResForLat(tileResLat, lLBox.CenterPoint.LatDegs);

    //     if (ParentTile != null)
    //     {
    //         Glo2DGridPos tileGridPos = TileCode.GridPos;

    //         // Copy the parent's elevation data - regardless of its resolution, to pass that along to the child tiles
    //         //GloFloat2DArray RawParentTileEleData = ParentTile.ChildEleData[tileGridPos.PosX, tileGridPos.PosY];

    //         TileEleData = ParentTile.TileEleData.GetInterpolatedSubgrid(tileGridPos, tileResLon, tileResLat);
    //     }
    //     else
    //     {
    //         TileEleData = new GloFloat2DArray(tileResLon, tileResLat);
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Ele
    // --------------------------------------------------------------------------------------------

    public float GetLocalElevation(GloLLPoint pos)
    {
        GloLLBox llBounds = TileCode.LLBox;
        if (!llBounds.Contains(pos))
            return GloElevationUtils.InvalidEle;

        (float latFrac, float lonFrac) = llBounds.GetLatLonFraction(pos);

        return TileEleData.InterpolatedValue(latFrac, lonFrac);
    }

    public float GetElevation(GloLLPoint pos)
    {
        GloLLBox llBounds = TileCode.LLBox;
        if (!llBounds.Contains(pos))
            return GloElevationUtils.InvalidEle;

        // If we have child tiles all constructed, the look one up and return its elevation
        if (ChildTileDataAvailable)
        {
            foreach (GloZeroNodeMapTile currTile in ChildTiles)
            {
                if (currTile.IsPointInTile(pos))
                {
                    return currTile.GetElevation(pos);
                }
            }
        }
        // Else, return the local elevation value
        return GetLocalElevation(pos);
    }

}