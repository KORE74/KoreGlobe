using System;
using System.Collections.Generic;

using Godot;

// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class GloZeroNodeMapManager : Node3D
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

        // double tileSizeDegs = 1;

        // for (int i=0; i< 10; i+=1)
        // {
        //     double az1 = (i * tileSizeDegs);
        //     double az2 = az1 + tileSizeDegs;

        //     for (int j=0; j<10; j++)
        //     {
        //         double el1 = 40 + (j * tileSizeDegs);
        //         double el2 = el1 + tileSizeDegs;

        //         // create a new tile to test the zero node offset
        //         GloAzElBox axElBox = new GloAzElBox() { MinAzDegs = az1, MinElDegs = el1, MaxAzDegs = az2, MaxElDegs = el2 };

        //         GloFloat2DArray eledata = new GloFloat2DArray(100, 100);
        //         eledata.SetRandomVals(10, -10);

        //         GloZeroNodeMapTile tile = new GloZeroNodeMapTile() { RwAzElBox = axElBox, RwEleData = eledata };
        //         tile.Name = $"TestTile-{i:F2}";

        //         AddChild(tile);
        //     }
        // }
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

        for (int latId = 0; latId < 6; latId++)
        {
            for (int lonId = 0; lonId < 12; lonId++)
            {
                GloMapTileCode currTileCode = new GloMapTileCode(lonId, latId);
                GloLLBox lLBox = currTileCode.LLBox;

                int tileResLon = GloElevationPrepTile.GetLongitudeResolution(tileResLat, lLBox.CenterPoint.LatDegs);

                GloFloat2DArray eledata = new GloFloat2DArray(tileResLon, tileResLat);
                eledata.SetRandomVals(5000, 5500);

                GloZeroNodeMapTile tile = new GloZeroNodeMapTile(currTileCode) { RwEleData = eledata };
                tile.Name = $"{currTileCode}";
                AddChild(tile);
            }
        }
    }
}


