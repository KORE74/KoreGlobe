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

    public static readonly int MaxMapLvl = 4;

    public static readonly int[] NumTilesVertPerLvl  = {  6, 6, 5, 5 };
    public static readonly int[] NumTilesHorizPerLvl = { 12, 6, 5, 5 };

    // Tile code string constants - Letter vertical from top-left down, Letter horizontal from top-left right. eg AB_CD
    public static readonly int MinTileCodeStrLen = 2;
    public static readonly int MaxTileCodeStrLen = 20;
    public static readonly char CodeSeparator = '_';
    public static readonly char[] LetterLookup = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' , 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'};

    // --------------------------------------------------------------------------------------------
    // Attributes
    // --------------------------------------------------------------------------------------------

    public List<FssLevelCodeElement> CodeList { get; private set; }

    public int MapLvl => CodeList.Count;

    // --------------------------------------------------------------------------------------------
    // Constructor
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

    public FssMapTileCode ChildCode(int x, int y)
    {
        FssMapTileCode newCode = new FssMapTileCode(this);

        int newLvl = newCode.CodeList.Count;

        if (newLvl >= MaxMapLvl) throw new InvalidOperationException("Maximum map level exceeded");

        newCode.AddLevelCode(x, y, newLvl);

        return newCode;
    }

    public static FssMapTileCode UndefinedTileCode()
    {
        return new FssMapTileCode();
    }

    // --------------------------------------------------------------------------------------------
    // Methods
    // --------------------------------------------------------------------------------------------

    public void AddLevelCode(int x, int y, int level)
    {
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x >= NumTilesHorizPerLvl[level]) x = NumTilesHorizPerLvl[level] - 1;
        if (y >= NumTilesVertPerLvl[level]) y = NumTilesVertPerLvl[level] - 1;

        FssLevelCodeElement l = new FssLevelCodeElement() { LatIndex = y, LonIndex = x };
        CodeList.Add(l);
    }

    public override string ToString()
    {
        return string.Join(CodeSeparator.ToString(), CodeList.ConvertAll(c => c.CodeString()));
    }

    // FssMapTileCode.CodeForIndex(0, 0) => "AA"

    public static string CodeForIndex(int y, int x)
    {
        return $"{LetterLookup[y]}{LetterLookup[x]}";
    }
}
