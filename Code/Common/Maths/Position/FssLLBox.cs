using System;

// FssLLBox: Class to represent a 3D box, curving to a spherical surface. Deals with points

// in the box based on simple angle and distance comparisons.
// Can also be used to represent a box out from an entity, such as a radar wedge.

public struct FssLLBox
{
    public double MinLatRads { get; set; }
    public double MinLonRads { get; set; }
    public double MaxLatRads { get; set; }
    public double MaxLonRads { get; set; }

    public double MinLatDegs
    {
        get { return MinLatRads * FssConsts.RadsToDegsMultiplier; }
        set { MinLatRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double MinLonDegs
    {
        get { return MinLonRads * FssConsts.RadsToDegsMultiplier; }
        set { MinLonRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double MaxLatDegs
    {
        get { return MaxLatRads * FssConsts.RadsToDegsMultiplier; }
        set { MaxLatRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double MaxLonDegs
    {
        get { return MaxLonRads * FssConsts.RadsToDegsMultiplier; }
        set { MaxLonRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public double MidLatDegs => (MinLatDegs + MaxLatDegs) / 2.0;
    public double MidLonDegs => (MinLonDegs + MaxLonDegs) / 2.0;

    // --------------------------------------------------------------------------------------------

    public double DeltaLatRads
    {
        get { return MaxLatRads - MinLatRads; }
        set { MaxLatRads = MinLatRads + value; }
    }
    public double DeltaLonRads
    {
        get { return MaxLonRads - MinLonRads; }
        set { MaxLonRads = MinLonRads + value; }
    }
    public double DeltaLatDegs
    {
        get { return MaxLatDegs - MinLatDegs; }
        set { MaxLatDegs = MinLatDegs + value; }
    }
    public double DeltaLonDegs
    {
        get { return MaxLonDegs - MinLonDegs; }
        set { MaxLonDegs = MinLonDegs + value; }
    }

    // --------------------------------------------------------------------------------------------

    public double HalfDeltaLonRads => DeltaLonRads / 2.0;
    public double HalfDeltaLatRads => DeltaLatRads / 2.0;
    public double HalfDeltaLonDegs => DeltaLonDegs / 2.0;
    public double HalfDeltaLatDegs => DeltaLatDegs / 2.0;

    public double LargestHalfDeltaDegs => Math.Max(HalfDeltaLatDegs, HalfDeltaLonDegs);

    // --------------------------------------------------------------------------------------------

    public FssLLPoint PosTopLeft
    {
        get { return new FssLLPoint() { LatRads = MaxLatRads, LonRads = MinLonRads }; }
        set { MaxLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public FssLLPoint PosTopRight
    {
        get { return new FssLLPoint() { LatRads = MaxLatRads, LonRads = MaxLonRads }; }
        set { MaxLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    public FssLLPoint PosBottomLeft
    {
        get { return new FssLLPoint() { LatRads = MinLatRads, LonRads = MinLonRads }; }
        set { MinLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public FssLLPoint PosBottomRight
    {
        get { return new FssLLPoint() { LatRads = MinLatRads, LonRads = MaxLonRads }; }
        set { MinLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    // --------------------------------------------------------------------------------------------

    public FssLLPoint CenterPoint => new FssLLPoint() { LatDegs = (MinLatDegs + MaxLatDegs) / 2.0, LonDegs = (MinLonDegs + MaxLonDegs) / 2.0 };

    // --------------------------------------------------------------------------------------------

    public FssLLBox(double tlLatRads, double tlLonRads, double latHeightRads, double lonWidthRads)
    {
        MinLatRads     = tlLatRads;
        MinLonRads     = tlLonRads;
        MaxLatRads     = tlLatRads + latHeightRads;
        MaxLonRads     = tlLonRads + lonWidthRads;
    }

    public static FssLLBox FssbalBox => new FssLLBox(-Math.PI / 2.0, -Math.PI, Math.PI, 2.0 * Math.PI);

    public static FssLLBox Zero => new FssLLBox(0.0, 0.0, 0.0, 0.0);

    // --------------------------------------------------------------------------------------------

    public bool Contains(FssLLPoint inPos)
    {
        if (inPos.LatRads < MinLatRads) return false;
        if (inPos.LatRads > MaxLatRads) return false;
        if (inPos.LonRads < MinLonRads) return false;
        if (inPos.LonRads > MaxLonRads) return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------

    public FssLLBox ShiftBox(double adjustLatDegs, double adjustLonDegs, double adjustAltM)
    {
        return new FssLLBox()
        {
            MinLatDegs = this.MinLatDegs + adjustLatDegs,
            MaxLatDegs = this.MaxLatDegs + adjustLatDegs,
            MinLonDegs = this.MinLonDegs + adjustLonDegs,
            MaxLonDegs = this.MaxLonDegs + adjustLonDegs,
        };
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"[{MinLatDegs:F3}, {MinLonDegs:F3}, {MaxLatDegs:F3}, {MaxLonDegs:F3}]";
    }

    public string ToStringVerbose()
    {
        return $"[MinLat:{MinLatDegs:F3}Degs, MinLon:{MinLonDegs:F3}Degs, MaxLat:{MaxLatDegs:F3}Degs, MaxLon:{MaxLonDegs:F3}Degs]";
    }

}

