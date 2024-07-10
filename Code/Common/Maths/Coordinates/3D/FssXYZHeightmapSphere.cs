using System;

public class FssXYZHeightmapSphere : FssXYZ
{
    public FssXYZPoint Center { get; private set; }

    // A 2D array for each integer azel angle, with the value being the height at that point
    public FssFloat2DArray Heightmap { get; private set; } = new (360, 180);

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZHeightmapSphere(FssXYZPoint center, FssFloat2DArray hm)
    {
        Center    = center;
        Heightmap = hm;
    }

}