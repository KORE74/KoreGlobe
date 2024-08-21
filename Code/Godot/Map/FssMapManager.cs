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
    private string MapRootPath = "";

    // Load reference point - The position against which map tile load operations are judged
    // FssMapManager.LoadRefLLA
    public static FssLLAPoint LoadRefLLA = new FssLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    public static FssXYZPoint LoadRefXYZ => LoadRefLLA.ToXYZ();

    // Debug
    private FssLLAPoint pos  = new FssLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    private FssCourse Course = new FssCourse() { HeadingDegs = 90, SpeedMps = 1 };
    Node3D ModelNode         = null;
    Node3D ModelResourceNode = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Setup this node
        Name = "MapManager";

        // Read the config values
        var config = FssCentralConfig.Instance;
        MapRootPath = config.GetParam<string>("MapRootPath");

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

}
