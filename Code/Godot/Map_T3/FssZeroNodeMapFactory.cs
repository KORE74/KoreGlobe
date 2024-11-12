using System;

using Godot;


// Class to take ownership of the complexity in creating ZeroNode map tiles.
public partial class FssZeroNodeMapFactory : Node3D
{

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // // create a new tile to test the zero node offset
        // FssAzElBox axElBox = new FssAzElBox() { MinAzDegs = -4, MinElDegs = 52, MaxAzDegs = -3, MaxElDegs = 53 };

        // FssFloat2DArray eledata = new FssFloat2DArray(100, 100);
        // eledata.SetRandomVals(10, -10);

        // FssZeroNodeMapTile tile = new FssZeroNodeMapTile() { RwAzElBox = axElBox, RwEleData = eledata };

        // AddChild(tile);

        CreateLvl0Tiles();

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }



    private void CreateLvl0Tiles()
    {
        for (int lonId = 0; lonId < 12; lonId++)
        {
            for (int latId = 0; latId < 6; latId++)
            {
                FssMapTileCode currTileCode = new FssMapTileCode(lonId, latId);
                FssFloat2DArray eleData = new(10, 10);
                eleData.SetRandomVals(100, 500);

                FssZeroNodeMapTile currTile = new FssZeroNodeMapTile() {
                    RwLLBox = currTileCode.LLBox,
                    RwEleData = eleData };

                AddChild(currTile);
            }
        }
    }

}



