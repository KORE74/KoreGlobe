using System;

// FssTileCode2: A tile code class, dealing solely with the definition of a hierachical grid format.

public struct FssLevelCode
{
    public int LatIndex;
    public int LonLondex;

    public string CodeString()
    {
        return $"{FssTileCode2.LetterLookup[LatIndex]}{FssTileCode2.LetterLookup[LonLondex]}";
    }
}
  
public class FssTileCode2
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
    
    public List<FssLevelCode> Code { get; private set; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------
    
    public FssTileCode2()
    {
        Code = new List<FssLevelCode>();
    }
    
    public FssTileCode2(int x, int y) : this()
    {
        AddLevelCode(x, y, 0);
    }

    public FssTileCode2(FssTileCode2 parentCode)
    {
        Code = new List<FssLevelCode>(parentCode.Code);
    }

    public FssTileCode2 ChildCode(int x, int y)
    {
        FssTileCode2 newCode = new FssTileCode2(this);

        int newLvl = newCode.Code.Count;

        if (newLvl >= MaxMapLvl) throw new InvalidOperationException("Maximum map level exceeded");

        newCode.AddLevelCode(x, y, newLvl);

        return newCode;
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

        FssLevelCode l = new FssLevelCode() { LatIndex = y, LonLondex = x };
        Code.Add(l);
    }

    public override string ToString()
    {
        return string.Join(CodeSeparator.ToString(), Code.ConvertAll(c => c.CodeString()));
    }
}
