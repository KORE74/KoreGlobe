
#nullable enable

public static class FssMapTileOperations
{
    // public static FssMapTileArray CreatChildTiles(FssMapTile parentTile)
    // {

    // }

    public static readonly double[] TileSizeDegsPerLvl  = { 30.0, 5.0, 1.0, 0.2};

    // --------------------------------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------

    // Take an input string that we think tries to represent a tile code, and return the tile code or null

    // Usage: FssMapTileCode? tileCode = FssMapTileOperations.TileCodeFromString("AA_AA_AA");

    public static FssMapTileCode? TileCodeFromString(string inStrCode)
    {
        // A tile code is typically in the format AA_AA_AA. Split the string into its main level parts.
        string[] parts = inStrCode.Split('_');

        // Each part has to be two characters long
        foreach (var part in parts)
        {
            if (part.Length != 2)
                return null;
        }

        // Create a blank tilecode.
        FssMapTileCode newTileCode = new();

        // Each character has to be a letter we turn into an index.
        foreach (var part in parts)
        {
            // Convert and validate on the first two characters being letters
            int firstIndex  = FssStringOperations.NumberForLetter(part[0]);
            int secondIndex = FssStringOperations.NumberForLetter(part[1]);
            if (firstIndex == -1 || secondIndex == -1)
                return null;

            newTileCode.AddLevelCode(secondIndex, firstIndex); // Note the letter ordering is reversed. BF is 2nd row (Y), then 6th column (X)
        }
        return newTileCode;
    }


    // Turn a letter into its index in the alphabet. Needs to be case insensitive.


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
    //     FssFloat2DArray? eleData = Float2DArrayIO.LoadFromBinaryFile(filepath)

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
