using Godot;
using System;
using System.IO;
using System.Threading.Tasks;

// Usage: FssTileInfo newTileInfo = new FssTileInfo() { TileCodeStruct = tilecodestr, TileLLBounds = llbox };

// public struct FssTileInfo
// {
//     public FssMapTileCode TileCodeStruct;
//     public FssLLBox TileLLBounds;
// }

public partial class FssMapManager : Node3D
{
    // Common map path point (save the config being queried excessively) // FssMapManager.MapRootPath
    public static string MapRootPath = "";

    // Load reference point - The position against which map tile load operations are judged
    // FssMapManager.LoadRefLLA
    public static FssLLAPoint LoadRefLLA = new FssLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    public static FssXYZPoint LoadRefXYZ => LoadRefLLA.ToXYZ();
    public static bool ShowDebug = false;
    public static int CurrMaxMapLvl = 1;

    // Debug
    private FssLLAPoint pos  = new FssLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    private FssCourse Course = new FssCourse() { HeadingDegs = 90, SpeedMps = 1 };
    Node3D ModelNode         = null;
    Node3D ModelResourceNode = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Setup this node
        Name = "MapManager";

        LoadConfig();

        for (int lonId = 0; lonId < 12; lonId++)
        {
            for (int latId = 0; latId < 6; latId++)
            {
                FssMapTileNode newChildTile = new FssMapTileNode(new FssMapTileCode(lonId, latId));
                AddChild(newChildTile);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Config Load
    // --------------------------------------------------------------------------------------------

    private static void LoadConfig()
    {
        // Read the debug flag from config
        var config = FssCentralConfig.Instance;

        // Read each of the params
        ShowDebug     = config.GetParam<bool>("ShowDebug");
        CurrMaxMapLvl = config.GetParam<int>("MaxMapLvl");
        MapRootPath   = config.GetParam<string>("MapRootPath");

        // Apply some validation to the max map level
        CurrMaxMapLvl = FssValueUtils.Clamp(CurrMaxMapLvl, 0, FssMapTileCode.MaxMapLvl);

        if (!Directory.Exists(FssMapManager.MapRootPath))
           FssCentralLog.AddEntry("$FssMapManager.MapRootPath not found: {FssMapManager.MapRootPath}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Config Update
    // --------------------------------------------------------------------------------------------

    public static void SetDebug(bool debug)
    {
        ShowDebug = debug;

        // Save the debug flag to config
        var config = FssCentralConfig.Instance;
        config.SetParam("ShowDebug", ShowDebug);
    }

    public static void SetMaxMapLvl(int maxMapLvl)
    {
        CurrMaxMapLvl = FssValueUtils.Clamp(maxMapLvl, 0, FssMapTileCode.MaxMapLvl);

        // Save the max map level to config
        var config = FssCentralConfig.Instance;
        config.SetParam("MaxMapLvl", CurrMaxMapLvl);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------


}
