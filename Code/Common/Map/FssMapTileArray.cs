using System;

public class FssMapTileArray
{
    public FssLLBox LLBox;
    public FssMapTile[,] TileArray;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssMapTileArray(FssLLBox llbox, int newMapLvl, Float2DArray eleData)
    {
        // Simple copy the LLBox
        LLBox = llbox;

        // Create the tile array
        int numTilesHoriz = FssMapTileCode.NumTilesHorizPerLvl[newMapLvl];
        int numTilesVert  = FssMapTileCode.NumTilesVertPerLvl[newMapLvl];
        TileArray = new FssMapTile[numTilesHoriz, numTilesVert];

        int tileRes       = 200;
        Float2DArray[,] childEleData = eleData.GetInterpolatedSubGridCellWithOverlap(numTilesHoriz, numTilesVert, tileRes, tileRes);
    }

    // --------------------------------------------------------------------------------------------
    // Simple Accessors
    // --------------------------------------------------------------------------------------------

    public void SetTile(int row, int col, FssMapTile tile)
    {
        if (row < 0 || row >= TileArray.GetLength(0) || col < 0 || col >= TileArray.GetLength(1))
            throw new Exception("TSTileArray.SetTile: Invalid row or col");
        TileArray[row, col] = tile;
    }

    public FssMapTile GetTile(int row, int col)
    {
        if (row < 0 || row >= TileArray.GetLength(0) || col < 0 || col >= TileArray.GetLength(1))
            throw new Exception("TSTileArray.GetTile: Invalid row or col");
        return TileArray[row, col];
    }

    public FssMapTile GetTile(FssLLPoint pos)
    {
        int row = (int)((pos.LatDegs - LLBox.MinLatDegs) / LLBox.DeltaLatDegs);
        int col = (int)((pos.LonDegs - LLBox.MinLonDegs) / LLBox.DeltaLonDegs);

        return TileArray[row, col];
    }
}