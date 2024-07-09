
using System;

using FssNetworking;

// Design Decisions:
// - The GlobeEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GlobeEventDriver
{

    // Unit Testing
    public GlobeTestLog testLog = GlobeTestCenter.RunCoreTests();

    // ---------------------------------------------------------------------------------------------
    // Constructor
    // ---------------------------------------------------------------------------------------------

    public GlobeEventDriver()
    {
    }

    // ---------------------------------------------------------------------------------------------
    // Basic tile management
    // ---------------------------------------------------------------------------------------------

    public void CreateTileEle(string tilecodeStr, int horizRes)
    {
        if (String.IsNullOrEmpty(tilecodeStr))
        {
            Console.WriteLine("CreateTileEle: Invalid tilecodeStr: >{tilecodeStr}<");
            return;
        }
        if ((tilecodeStr.Length < 2) || (tilecodeStr.Length > 20))
        {
            Console.WriteLine("CreateTileEle: Invalid tilecodeStr length: >{tilecodeStr}<");
            return;
        }

        GlobeMapTileCode newTileCode = GlobeMapTileCode.Parse(tilecodeStr);
        if (newTileCode == GlobeMapTileCode.Zero)
        {
            Console.WriteLine("CreateTileEle: Invalid tilecodeStr: >{tilecodeStr}<");
            return;
        }
        //MapEleManager.CreateTileEle(newTileCode, horizRes);

        //string tileEleFilename = System.IO.Path.Combine(MapData.RootDir, "Ele", "Level0_45x40Degs", tilecodeStr + ".bin");
    }

}