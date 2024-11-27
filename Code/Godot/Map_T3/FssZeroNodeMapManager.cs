using System;
using System.Collections.Generic;

using Godot;

// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class FssZeroNodeMapManager : Node3D
{
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

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Lvl0 Tiles
    // --------------------------------------------------------------------------------------------

    // Initialising call to create the top level tiles, which then recursively service the creation and display of all the lower level tiles.

    public void CreateLvl0Tiles()
    {
        int tileResLat = 30;

        FssCentralLog.AddEntry("Creating Lvl0 Tiles");

        for (int latId = 0; latId < 6; latId++)
        {
            for (int lonId = 0; lonId < 12; lonId++)
            {
                FssMapTileCode currTileCode = new FssMapTileCode(lonId, latId);
                FssLLBox lLBox = currTileCode.LLBox;

                int tileResLon = FssElevationPrepTile.GetLongitudeResolution(tileResLat, lLBox.CenterPoint.LatDegs);

                FssFloat2DArray eledata = new FssFloat2DArray(tileResLon, tileResLat);
                eledata.SetRandomVals(5000, 5500);

                FssZeroNodeMapTile tile = new FssZeroNodeMapTile(currTileCode) { RwEleData = eledata };
                tile.Name = $"{currTileCode}";
                AddChild(tile);
            }
        }
    }
}


