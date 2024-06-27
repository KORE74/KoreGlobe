public static class FssMapTileOperations
{
    // public static FssMapTileArray CreatChildTiles(FssMapTile parentTile)
    // {
        
    // }
    
    public static readonly double[] TileSizeDegsPerLvl  = { 30.0, 5.0, 1.0, 0.2};
    
    public static FssLLBox LLBoxForTileCode(FssMapTileCode tileCode)
    {
        double currTopLeftLatDegs = 90;
        double currTopLeftLonDegs = -180;
    
        int currLvl = 0;    
        foreach (var currCode in tileCode.CodeList)
        {
            currTopLeftLatDegs -= currCode.LatIndex* TileSizeDegsPerLvl[currLvl];
            currTopLeftLonDegs += currCode.LonIndex * TileSizeDegsPerLvl[currLvl];
        }

        double bottomRightLatDegs = currTopLeftLatDegs - TileSizeDegsPerLvl[currLvl];
        double bottomRightLonDegs = currTopLeftLonDegs + TileSizeDegsPerLvl[currLvl];
        
        return new FssLLBox(currTopLeftLatDegs, currTopLeftLonDegs, bottomRightLatDegs, bottomRightLonDegs); 
    }
    
    // Load a tile from a set path, returning null on fail
    // public static FssMapTile? LoadTile(string filepath)
    // {
    //     // Check if the file exists
    //     if (!File.Exists(filepath))
    //     {
    //         Console.WriteLine($"FssMapTileOperations.LoadTile: File does not exist: {filepath}");
    //         return null;
    //     }


    //     // Read the 2D Array
    //     Float2DArray? eleData = Float2DArrayIO.LoadFromBinaryFile(filepath)

    //     if (eleData != null)
    //     {
    //         Console.WriteLine($"FssMapTileOperations.LoadTile: Failed to load eleData from file: {filepath}");
    //         return null;
    //     }
    //     FssMapTile tile = new FssMapTile()
    //     {
    //         EleData = eleData
    //     };

    //     // Return the tile
    //     return tile;
    // }
}
