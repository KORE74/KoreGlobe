using System;

// The struct GloLLPoint stores Lat Long values from a bottom-left origin, which is useful for map tiles.
// Will convert between this and a GloLLAPoint where the origin is the "center".

public struct GloLLPoint
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }

    // --------------------------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // --------------------------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * GloConsts.RadsToDegsMultiplier; }
        set { LatRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * GloConsts.RadsToDegsMultiplier; }
        set { LonRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------
    // Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   GloLLPoint pos = new GloLLPoint() { latDegs = X, LonDegs = Y };

    public GloLLPoint(double laRads, double loRads)
    {
        LatRads = laRads;
        LonRads = loRads;
    }

    public GloLLPoint(GloLLAPoint llPos)
    {
        this.LatRads = llPos.LatRads;
        this.LonRads = llPos.LonRads;
    }

    public static GloLLPoint Zero
    {
        get { return new GloLLPoint { LatRads = 0.0, LonRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"({LatDegs:F3}, {LonDegs:F3})");
    }

}
