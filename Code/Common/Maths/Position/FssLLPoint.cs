using System;

// The struct FssLLPoint stores Lat Long values from a bottom-left origin, which is useful for map tiles.
// Will convert between this and a FssLLAPoint where the origin is the "center".

public struct FssLLPoint
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }

    // ------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // ------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * FssConsts.RadsToDegsMultiplier; }
        set { LatRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * FssConsts.RadsToDegsMultiplier; }
        set { LonRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    // ------------------------------------------------------------------------
    // Constructors - different options and units
    // ------------------------------------------------------------------------

    // Note that fields can be set:
    //   FssLLPoint pos = new FssLLPoint() { latDegs = X, LonDegs = Y };

    public FssLLPoint(double laRads, double loRads)
    {
        LatRads = laRads;
        LonRads = loRads;
    }

    public static FssLLPoint Zero
    {
        get { return new FssLLPoint { LatRads = 0.0, LonRads = 0.0 }; }
    }

    // ------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"({LatDegs:F3}, {LonDegs:F3})");
    }

}
