using System;
using System.Collections.Generic;

using Godot;

// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class GloZeroNodeMapManager : Node3D
{
    // Root game engine on which to parent map tiles
    private Node ZeroNode;
    public static int CurrMaxMapLvl = 5; // default value - updated from config in constructor

    // Common map path point (save the config being queried excessively) // GloZeroNodeMapManager.MapRootPath
    public static string MapRootPath = "";

    // Map load ref position
    public static GloLLAPoint LoadRefLLA = new GloLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
    public static GloXYZPoint LoadRefXYZ => LoadRefLLA.ToXYZ();
    public static float       DistanceToHorizonM = 0;

    // Tile Action counter - Sets up a number of tile creation actions to be performed per _Process call.
    public GloActionCounter ActionCounter = new GloActionCounter(10);

    // lvl0 tile list
    private List<GloZeroNodeMapTile> Lvl0Tiles = new List<GloZeroNodeMapTile>();

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public GloZeroNodeMapManager(Node zeroNode)
    {
        ZeroNode = zeroNode;

        // Read the debug flag from config
        var config = GloCentralConfig.Instance;

        CurrMaxMapLvl = config.GetParam<int>("MaxMapLvl");
        MapRootPath   = config.GetParam<string>("MapRootPath");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialise the Manager node itself
        Name = "ZeroNodeMapManager";
        CreateLvl0Tiles();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        ActionCounter.Refresh(5); // Tile create actions per update cycle (frame)
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lvl0 Tiles
    // --------------------------------------------------------------------------------------------

    // Initialising call to create the top level tiles, which then recursively service the creation and display of all the lower level tiles.

    public void CreateLvl0Tiles()
    {
        if (ZeroNode == null)
        {
            GD.PrintErr("ERROR: ZeroNodeMapManager ZeroNode Null");
            return;
        }

        GloCentralLog.AddEntry("Creating Lvl0 Tiles");

        for (int latId = 0; latId < 6; latId++)
        {
            for (int lonId = 0; lonId < 12; lonId++)
            {
                GloMapTileCode currTileCode = new GloMapTileCode(lonId, latId);

                GloZeroNodeMapTile tile = new GloZeroNodeMapTile(currTileCode);
                AddChild(tile);
                Lvl0Tiles.Add(tile);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation Query
    // --------------------------------------------------------------------------------------------

    private GloZeroNodeMapTile? GetLvl0Tile(GloMapTileCode tileCode)
    {
        // Get the name of the first lvl0
        string tileName = tileCode.ToString();

        GloMapTileCode? lvl0Code = GloMapTileCode.CodeToLvl(tileCode, 0);
        if (lvl0Code == null) return null;

        // Get the tile node for this tile code
        return (GloZeroNodeMapTile)FindChild(lvl0Code.ToString());
    }

    // Get the elevation at a given LL point, as loaded by the map tiles.

    public float GetElevation(GloLLPoint llPoint)
    {
        GloMapTileCode? tileCode = new GloMapTileCode(llPoint.LatDegs, llPoint.LonDegs, 0);

        GloZeroNodeMapTile? lvl0Tile = GetLvl0Tile(tileCode);

        if (lvl0Tile == null) return GloElevationUtils.InvalidEle;

        float ele = GloElevationUtils.InvalidEle;

        foreach (GloZeroNodeMapTile currTile in Lvl0Tiles)
        {
            if (currTile.IsPointInTile(llPoint) )
            {
                ele = currTile.GetElevation(llPoint);
                break;
            }
        }

        return lvl0Tile.GetElevation(llPoint);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public static void SetLoadRefLLA(GloLLAPoint newLoadRefLLA)
    {
        LoadRefLLA = newLoadRefLLA;

        DistanceToHorizonM = (float)(GloWorldOperations.DistanceToHorizonM(LoadRefLLA.AltMslM));

        if (DistanceToHorizonM < 10000) GloZeroNodeMapManager.DistanceToHorizonM = 10000; // minimise value at 10km
    }




    public static void SetMaxMapLvl(int maxMapLvl)
    {
        CurrMaxMapLvl = GloValueUtils.Clamp(maxMapLvl, 0, GloMapTileCode.MaxMapLvl);

        // Save the max map level to config
        var config = GloCentralConfig.Instance;
        config.SetParam("MaxMapLvl", CurrMaxMapLvl);
    }

    public void UpdateInfoVisibility(bool infoVisible)
    {
        GD.Print($"GloZeroNodeMapManager: UpdateInfoVisibility {infoVisible}");

        foreach (GloZeroNodeMapTile tile in Lvl0Tiles)
        {
            tile.UpdateInfoVisibility(infoVisible);
            tile.UpdateChildInfoVisibility(infoVisible);
        }
    }


}