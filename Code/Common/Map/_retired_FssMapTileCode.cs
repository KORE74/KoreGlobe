// ﻿using System;
// using System.Collections.Generic;
// using System.Reflection.Metadata;
// using System.Linq;
// using System.Text;

/*
Tile Code System:
- We start with all single letters per axis per tile level, and also letters vertically.
- The code is <col><row>, basically <x><y> in a 2D grid, in order from the top left corner.
- Its always Lat/Lon, so the first letter is the vertical axis, and the second is the horizontal axis.
- So AA is the top left tile, AB is the tile to the right of that, and BA is the tile below the top left.
- This scheme is also designed to show an alphabetically arranged grid of tiles in Windows Explorer.

The top level of tiles is 360x180 degrees, and each tile is 30x30 degrees, for a 72 element of 12x6 tiles.
The next level is 6x6 of 5x5 degree tiles, then 5x5 of 1x1 degree tiles, then 5x5 of 0.2x0.2 degree tiles.

We need to easily translate between any given tilecode, and a structure of the tile's lat/lon box.
- We'll use "map tile Lat/Lon" numbers that have a 0,0 at the top left, and then convert those to a
  "geographic lat/lon" number that has a conventional 0,0 in the middle as we see in maps.
- We'll use degrees exclusively here, as its a more intuitive unit.

Design Desicions:
- The class will be immutable, and will be created from a tile code string, or from a level and a lat/lon position.
- Any returned single position is for the center of the tile.
- "Map" lat/lons and "Geo" lat/lons are always in degrees, and will always be prefixed.
*/

// public class FssMapTileCode_Retired
// {
    // // Attributes with defaults
    // public bool   IsValid     { get; private set;} = false;
    // public int    MapLvl      { get; private set;} = -1;

    // public FssLLPoint TopLeftPos { get; private set;}
    // public double WidthDegs  => ((MapLvl >= 0) && (MapLvl <= MaxMapLvl)) ? TileSizeDegsPerLvl[MapLvl] : 0;
    // public double HeightDegs => ((MapLvl >= 0) && (MapLvl <= MaxMapLvl)) ? TileSizeDegsPerLvl[MapLvl] : 0;
  
    // public string TileCodeStr { get; private set;} = "<undefined>";

  
  
//     // --------------------------------------------------------------------------------------------
//     // Tile Code Constants
//     // --------------------------------------------------------------------------------------------

//     public static readonly int MaxMapLvl = 4;

//     // Number of tile per level within their parent tile
//     public static readonly int[]    NumTilesVertPerLvl  = {  6, 6, 5, 5 };
//     public static readonly int[]    NumTilesHorizPerLvl = { 12, 6, 5, 5 };
//     public static readonly double[] TileSizeDegsPerLvl  = { 30.0, 5.0, 1.0, 0.2};

//     // Position constants - +/- 180 degrees longtude and +/- 90 degrees latitude
//     public static readonly double MinLatDegs =  -90.0;
//     public static readonly double MinLonDegs = -180.0;
//     public static readonly double MaxLatDegs =   90.0;
//     public static readonly double MaxLonDegs =  180.0;

//     // Tile code string constants - Letter vertical from top-left down, number horizontal from top-left right. eg A4_F9
//     public static readonly int MinTileCodeStrLen = 2;
//     public static readonly int MaxTileCodeStrLen = 20;
//     public static readonly char CodeSeparator = '_';
//     public static readonly char[] LetterLookup = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' , 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'};

//     // --------------------------------------------------------------------------------------------
//     // Constructor
//     // --------------------------------------------------------------------------------------------

//     // Given any lat/lon and a tile level, get the tile for that location.

//     public FssMapTileCode(int lvl, double latDegs, double lonDegs)
//     {
//         // Find the top-left 
//         TopLeftPos = MapTileTopLeft(lvl, latDegs, lonDegs);

//         // Set the level and lat/lon
//         MapLvl  = lvl;

//         // Set the tile code string
//         TileCodeStr = GetFullTileCodeStr(lvl, TopLeftPos.LatDegs, TopLeftPos.LonDegs);

//         // Set the valid flag
//         IsValid = true;
//     }

//     public FssMapTileCode(string tileCodeStr)
//     {
//         // Check the string is valid
//         IsValid = ValidateInput(tileCodeStr);
//         if (!IsValid) return;

//         // Set the tile code string
//         TileCodeStr = tileCodeStr;

//         // Set the level and lat/lon
//         MapLvl  = tileCodeStr.Length;
//         LatDegs = 0.0;
//         LonDegs = 0.0;

//         // Set the valid flag
//         IsValid = true;
//     }

//     public static FssMapTileCode UndefinedTileCode()
//     {
//         return new FssMapTileCode(-1, -1.0, -1.0);
//     }

//     // --------------------------------------------------------------------------------------------
//     // Tile top left defined from position and level
//     // --------------------------------------------------------------------------------------------

//     // Return the top left lat/lon for the given level and lat/lon

//     public FssLLPoint MapTileTopLeft(int lvl, double inputLatDegs, double inputLonDegs)
//     {
//         // Ensure level is within specified range.
//         if (lvl < 0 || lvl >= MaxMapLvl) throw new ArgumentOutOfRangeException(nameof(lvl), "Level is out of range.");

//         // Use the predefined tile size for the level.
//         double tileSizeDegs = TileSizeDegsPerLvl[lvl];

//         // Calculate the global tileRow and tileCol based on the tilesize, and then divide to be relevant to this tile level
//         int tileRow = ((int)(inputLatDegs / tileSizeDegs)) % NumTilesVertPerLvl[lvl];
//         int tileCol = ((int)(inputLonDegs / tileSizeDegs)) % NumTilesHorizPerLvl[lvl];

//         // Calculate the top-left corner of the tile in map coordinates
//         double mapLat = tileRow * tileSizeDegs;
//         double mapLon = tileCol * tileSizeDegs;

//         FssLLPoint retPos = new FssLLPoint() { LatDegs = mapLat, LonDegs = mapLon };
//         return retPos;
//     }

//     // --------------------------------------------------------------------------------------------
//     // Tile code string defined from position
//     // --------------------------------------------------------------------------------------------

//     // Return the tile code for the given level and lat/lon

//     public static string GetTileCodeStr(int lvl, double inputLatDegs, double inputLonDegs)
//     {
//         // Ensure level is within specified range.
//         if (lvl < 0 || lvl >= MaxMapLvl) throw new ArgumentOutOfRangeException(nameof(lvl), "Level is out of range.");

//         // Use the predefined tile size for the level.
//         double tileSizeDegs = TileSizeDegsPerLvl[lvl];

//         // Calculate the global tileRow and tileCol based on the tilesize, and then divide to be relevant to this tile level
//         int tileRow = ((int)(inputLatDegs / tileSizeDegs)) % NumTilesVertPerLvl[lvl];
//         int tileCol = ((int)(inputLonDegs / tileSizeDegs)) % NumTilesHorizPerLvl[lvl];

//         // Generate the tile code using the row and column indices.
//         char rowCode = LetterLookup[tileRow];
//         char colCode = LetterLookup[tileCol];

//         return $"{rowCode}{colCode}";
//     }

//     // Create the full tile code across all the levels for the given lat/lon

//     public string GetFullTileCodeStr(int lvl, double inputLatDegs, double inputLonDegs)
//     {
//         StringBuilder tileCodeStr = new StringBuilder();

//         for (int i = 0; i <= lvl; i++) // Adjusted loop to include the requested level
//         {
//             if (i > 0) tileCodeStr.Append(CodeSeparator); // Prepend the separator for subsequent levels
//             tileCodeStr.Append(GetTileCodeStr(i, inputLatDegs, inputLonDegs));
//         }

//         return tileCodeStr.ToString();
//     }

//     // --------------------------------------------------------------------------------------------
//     // Tile position defined from string
//     // --------------------------------------------------------------------------------------------

//     public (double mapLat, double mapLon) MapLatLonForTileCodeString(string tileCode)
//     {
//         double currentLatOffset = 0.0, currentLonOffset = 0.0;
//         int levelDepth = tileCode.Count(c => c == CodeSeparator) + 1;
//         string[] codeParts = tileCode.Split(CodeSeparator);

//         for (int level = 0; level < levelDepth; level++)
//         {
//             if (level >= TileSizeDegsPerLvl.Length)
//                 throw new ArgumentException("Tile code level exceeds available tile sizes.", nameof(tileCode));

//             double tileSizeLat = 180.0 / Math.Pow(NumTilesVertPerLvl[level], level); // Adjust for cumulative effect of levels
//             double tileSizeLon = 360.0 / Math.Pow(NumTilesHorizPerLvl[level], level);

//             if (level < codeParts.Length)
//             {
//                 string part = codeParts[level];
//                 int latIndex = Array.IndexOf(LetterLookup, part[0]);
//                 int lonIndex = Array.IndexOf(LetterLookup, part[1]);

//                 if (latIndex == -1 || lonIndex == -1)
//                     throw new ArgumentException("Invalid tile code part.", nameof(tileCode));

//                 currentLatOffset += latIndex * tileSizeLat;
//                 currentLonOffset += lonIndex * tileSizeLon;
//             }
//         }

//         // Calculate the top-left corner of the tile in map coordinates
//         double mapLat = currentLatOffset;
//         double mapLon = currentLonOffset;

//         return (mapLat, mapLon);
//     }

//     // --------------------------------------------------------------------------------------------

//     private bool ValidateInput(int lvl, double mapLatDegs, double mapLonDegs)
//     {
//         return (lvl >= 0) && (lvl <= MaxMapLvl) &&
//                (mapLatDegs >= MinLatDegs) && (mapLatDegs <= MaxLatDegs) &&
//                (mapLonDegs >= MinLonDegs) && (mapLonDegs <= MaxLonDegs);
//     }

//     private bool ValidateInput(string tileCodeStr)
//     {
//         bool validLength = tileCodeStr.Length >= MinTileCodeStrLen && tileCodeStr.Length <= MaxTileCodeStrLen;
//         bool validChars  = tileCodeStr.All(c => char.IsLetterOrDigit(c) && char.IsUpper(c));

//         return validLength && validChars;
//     }

//     // --------------------------------------------------------------------------------------------

// }



