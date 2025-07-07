using System;

public class GloXYZHeightmapSphere : GloXYZ
{
    public GloXYZPoint Center { get; private set; }

    // A 2D array for each integer azel angle, with the value being the height at that point
    public GloFloat2DArray Heightmap { get; private set; } = new (360, 180);

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYZHeightmapSphere(GloXYZPoint center, GloFloat2DArray hm)
    {
        Center    = center;
        Heightmap = hm;
    }

}