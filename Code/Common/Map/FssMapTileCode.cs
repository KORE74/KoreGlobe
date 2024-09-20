using System;
using System.Collections.Generic;

// FssMapTileCode: A tile code class, dealing solely with the definition of a hierachical grid format.

public struct FssLevelCodeElement
{
    public int LatIndex;
    public int LonIndex;

    public string CodeString()
    {
        return $"{FssMapTileCode.LetterLookup[LatIndex]}{FssMapTileCode.LetterLookup[LonIndex]}";
    }
}

public class FssMapTileCode
{
    // --------------------------------------------------------------------------------------------
    // Constants
    // --------------------------------------------------------------------------------------------

    // FssMapTileCode.MaxMapLvl

    public static readonly int MaxMapLvl = 5;

    public static readonly int[] NumTilesVertPerLvl  = {  6, 6, 5, 5, 5, 5};
    public static readonly int[] NumTilesHorizPerLvl = { 12, 6, 5, 5, 5, 5};

    public static readonly double[] TileSizeDegsPerLvl  = {  30, 5, 1, 0.2, 0.04, 0.008 };

    // Tile code string constants - Letter vertical from top-left down, Letter horizontal from top-left right. eg AB_CD
    public static readonly int MinTileCodeStrLen = 2;
    public static readonly int MaxTileCodeStrLen = 20;
    public static readonly char CodeSeparator = '_';
    public static readonly char[] LetterLookup = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' , 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'};

    public static readonly string[] PathPerLvl = { "Lvl0_30x30", "Lvl1_5x5", "Lvl2_1x1", "Lvl3_0p2x0p2", "Lvl4_0p04x0p04", "Lvl5_0p008x0p008"};

    // --------------------------------------------------------------------------------------------
    // Attributes
    // --------------------------------------------------------------------------------------------

    public List<FssLevelCodeElement> CodeList { get; private set; }

    public int MapLvl     => (CodeList.Count - 1);

    public int HorizCount => NumTilesHorizPerLvl[MapLvl];
    public int VertCount  => NumTilesVertPerLvl[MapLvl];

    public int HorizIndex => CodeList[MapLvl].LonIndex;
    public int VertIndex  => CodeList[MapLvl].LatIndex;

    public Fss2DGridPos GridPos => new Fss2DGridPos(HorizCount, VertCount, HorizIndex, VertIndex);

    public FssLLBox LLBox => LLBoxForCode(this);

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public FssMapTileCode()
    {
        CodeList = new List<FssLevelCodeElement>();
    }

    public FssMapTileCode(int x, int y) : this()
    {
        AddLevelCode(x, y, 0);
    }

    public FssMapTileCode(FssMapTileCode parentCode)
    {
        CodeList = new List<FssLevelCodeElement>(parentCode.CodeList);
    }

    public static FssMapTileCode UndefinedTileCode()
    {
        return new FssMapTileCode();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edits
    // --------------------------------------------------------------------------------------------

    // We add the next level to the internal list, the level is used only for validation.

    public void AddLevelCode(int x, int y, int level)
    {
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x >= NumTilesHorizPerLvl[level]) x = NumTilesHorizPerLvl[level] - 1;
        if (y >= NumTilesVertPerLvl[level])  y = NumTilesVertPerLvl[level] - 1;

        FssLevelCodeElement l = new FssLevelCodeElement() { LatIndex = y, LonIndex = x };
        CodeList.Add(l);
    }

    public void AddLevelCode(int x, int y)
    {
        int nextMapLvl = MapLvl + 1;
        AddLevelCode(x, y, nextMapLvl);
    }

    public void SetLevelCode(int x, int y, int level)
    {
        // Basic level check - Levels are 0-based, so inputs will be values 0, 1, 2, 3, 4. 0 == 1 level in the code.
        if (CodeList.Count < (level+1)) throw new InvalidOperationException("Invalid level");

        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x >= NumTilesHorizPerLvl[level]) x = NumTilesHorizPerLvl[level] - 1;
        if (y >= NumTilesVertPerLvl[level])  y = NumTilesVertPerLvl[level] - 1;

        CodeList[level] = new FssLevelCodeElement() { LatIndex = y, LonIndex = x };
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Child Codes
    // --------------------------------------------------------------------------------------------

    public FssMapTileCode ChildCode(int x, int y)
    {
        FssMapTileCode newCode = new FssMapTileCode(this);

        int newLvl = newCode.CodeList.Count;

        if (newLvl >= MaxMapLvl) throw new InvalidOperationException("Maximum map level exceeded");

        newCode.AddLevelCode(x, y, newLvl);

        return newCode;
    }

    public List<FssMapTileCode> ChildCodesList()
    {
        List<FssMapTileCode> childCodes = new List<FssMapTileCode>();
        int newLvl = CodeList.Count;

        // We're just returning an empty list if we're at the max level
        if (newLvl <= MaxMapLvl)
        {
            for (int y = 0; y < NumTilesVertPerLvl[newLvl]; y++)
            {
                for (int x = 0; x < NumTilesHorizPerLvl[newLvl]; x++)
                {
                    FssMapTileCode newCode = new FssMapTileCode(this);
                    newCode.AddLevelCode(x, y, newLvl);
                    childCodes.Add(newCode);
                }
            }
        }
        return childCodes;
    }

    // Get the dimensions of the child codes list

    public int ChildCodesWidth()
    {
        if (MapLvl >= MaxMapLvl) return 0;
        return NumTilesHorizPerLvl[MapLvl + 1];
    }

    public int ChildCodesHeight()
    {
        if (MapLvl >= MaxMapLvl) return 0;
        return NumTilesVertPerLvl[MapLvl + 1];
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Misc Methods
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Join(CodeSeparator.ToString(), CodeList.ConvertAll(c => c.CodeString()));
    }

    public string ParentString()
    {
        if (CodeList.Count == 0) return "";
        return string.Join(CodeSeparator.ToString(), CodeList.GetRange(0, CodeList.Count - 1).ConvertAll(c => c.CodeString()));
    }

    // FssMapTileCode.CodeForIndex(0, 0) => "AA"

    public static string CodeForIndex(int y, int x)
    {
        return $"{LetterLookup[y]}{LetterLookup[x]}";
    }

    // --------------------------------------------------------------------------------------------

    // Usage: FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);

    public static FssLLBox LLBoxForCode(FssMapTileCode tileCode)
    {
        FssLLBox llbox = new FssLLBox();

        double topLeftLatDegs = 90;
        double topLeftLonDegs = -180;

        int currLvl = 0;
        foreach (FssLevelCodeElement currCodeLevel in tileCode.CodeList)
        {
            double deltaLatDegs = TileSizeDegsPerLvl[currLvl] * currCodeLevel.LatIndex;
            double deltaLonDegs = TileSizeDegsPerLvl[currLvl] * currCodeLevel.LonIndex;

            topLeftLatDegs -= deltaLatDegs;
            topLeftLonDegs += deltaLonDegs;

            currLvl++;
        }

        double finalTileSizeDegs = TileSizeDegsPerLvl[tileCode.MapLvl];
        llbox.MaxLatDegs = topLeftLatDegs;
        llbox.MinLatDegs = topLeftLatDegs - finalTileSizeDegs;
        llbox.MinLonDegs = topLeftLonDegs;
        llbox.MaxLonDegs = topLeftLonDegs + finalTileSizeDegs;

        return llbox;
    }
}
