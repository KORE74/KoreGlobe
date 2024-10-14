using System;
using System.Collections.Generic;

using Godot;

// ZeroNode map tile: A tile placed at an offset from the zeronode.
public partial class FssZeroNodeMapTile : Node3D
{
    public FssAzElBox      RwAzElBox = FssAzElBox.Zero;
    public FssFloat2DArray RwEleData = new FssFloat2DArray();

    public void CreateMesh()
    {
        FssPolarDirection azElCenter = RwAzElBox.Center;
        double rwRadius = FssPosConsts.EarthRadiusM;
        FssLLAPoint rwLLACenter = new FssLLAPoint() { LatDegs = azElCenter.ElDegs, LonDegs = azElCenter.AzDegs, RadiusM = rwRadius };
        FssXYZPoint rwXYZCenter = rwLLACenter.ToXYZ();

        // Setup the loop control values
        int pointCountAz = RwEleData.Width;
        int pointCountEl = RwEleData.Height;
        List<double> azListRads = FssValueUtils.CreateRangeList(pointCountAz, RwAzElBox.MinAzRads, RwAzElBox.MaxAzRads);
        List<double> elListRads = FssValueUtils.CreateRangeList(pointCountEl, RwAzElBox.MinElRads, RwAzElBox.MaxElRads);

        for (int i = 0; i < pointCountAz; i++)
        {
            for (int j = 0; j < pointCountEl; j++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double azRads = azListRads[i];
                double elRads = elListRads[j];
                double ele    = rwRadius + RwEleData[i, j];
                FssLLAPoint rwLLAPointPos = new FssLLAPoint() { LatRads = elRads, LonRads = azRads, RadiusM = ele };
                FssXYZPoint rwXYZPointPos = rwLLACenter.ToXYZ();

                // Find the offset from the tile center.
                FssXYZPoint centerOffset = rwXYZCenter.XYZTo(rwXYZCenter);

                // Convert the Real-World position to the Game Engine position.
                Vector3 gePos = new Vector3(
                    (float)(centerOffset.X * FssZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(centerOffset.Y * FssZeroOffset.RwToGeDistanceMultiplierM),
                    (float)(centerOffset.Z * FssZeroOffset.RwToGeDistanceMultiplierM));
            }
        }
    }
}




