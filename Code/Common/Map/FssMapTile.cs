using System;

#nullable enable

public class FssMapTile
{
    // Essential tile data
    public FssMapTileCode TileCode;
    public FssLLBox LLBox;

    // Tile Data
    public Float2DArray EleData = new Float2DArray(1,1);

    // Child Tile Data - can legitimately be null for tiles with no child details.
    public FssMapTileArray? ChildTileArray;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssMapTile()
    {
        LLBox = FssLLBox.GlobalBox;
        TileCode = FssMapTileCode.UndefinedTileCode();
    }

    public FssMapTile(FssMapTileCode tileCode, FssLLBox mapBox)
    {
        LLBox = mapBox;
        TileCode = tileCode;

        EleData = new Float2DArray();
    }

    public void SetTileResolution(int tileResLong, int tileResLat)
    {
        EleData = EleData.GetInterpolatedGrid(tileResLong, tileResLat);
    }

    // --------------------------------------------------------------------------------------------

    public void ConfigChildTileArray(int numChildTilesLong, int numChildTilesLat, int resLong, int resLat)
    {
        ChildTileArray = new FssMapTileArray(LLBox, TileCode.MapLvl+1, EleData);
    }

    public void DeleteChildTileArray()
    {
        ChildTileArray = null;
    }

    // --------------------------------------------------------------------------------------------

    public void SetEleValue(FssLLPoint pos, float value)
    {
        if (LLBox.Contains(pos))
        {
            float latFrac = (float)((pos.LatDegs - LLBox.MinLatDegs) / LLBox.DeltaLatDegs);
            float lonFrac = (float)((pos.LonDegs - LLBox.MinLonDegs) / LLBox.DeltaLonDegs);

            EleData.SetFractionalValue(lonFrac, latFrac, value);
        }
    }

}
