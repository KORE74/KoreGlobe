using System;

// GloLLBox: Class to represent a 3D box, curving to a spherical surface. Deals with points

// in the box based on simple angle and distance comparisons.
// Can also be used to represent a box out from an entity, such as a radar wedge.

public struct GloLLBox
{
    public double MinLatRads { get; set; }
    public double MinLonRads { get; set; }
    public double MaxLatRads { get; set; }
    public double MaxLonRads { get; set; }

    public double MinLatDegs
    {
        get { return MinLatRads * GloConsts.RadsToDegsMultiplier; }
        set { MinLatRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double MinLonDegs
    {
        get { return MinLonRads * GloConsts.RadsToDegsMultiplier; }
        set { MinLonRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double MaxLatDegs
    {
        get { return MaxLatRads * GloConsts.RadsToDegsMultiplier; }
        set { MaxLatRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double MaxLonDegs
    {
        get { return MaxLonRads * GloConsts.RadsToDegsMultiplier; }
        set { MaxLonRads = value * GloConsts.DegsToRadsMultiplier; }
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

    public GloLLPoint PosTopLeft
    {
        get { return new GloLLPoint() { LatRads = MaxLatRads, LonRads = MinLonRads }; }
        set { MaxLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public GloLLPoint PosTopRight
    {
        get { return new GloLLPoint() { LatRads = MaxLatRads, LonRads = MaxLonRads }; }
        set { MaxLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    public GloLLPoint PosBottomLeft
    {
        get { return new GloLLPoint() { LatRads = MinLatRads, LonRads = MinLonRads }; }
        set { MinLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public GloLLPoint PosBottomRight
    {
        get { return new GloLLPoint() { LatRads = MinLatRads, LonRads = MaxLonRads }; }
        set { MinLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    // --------------------------------------------------------------------------------------------

    public GloLLPoint CenterPoint => new GloLLPoint() { LatDegs = (MinLatDegs + MaxLatDegs) / 2.0, LonDegs = (MinLonDegs + MaxLonDegs) / 2.0 };

    // --------------------------------------------------------------------------------------------

    public GloLLBox(double tlLatRads, double tlLonRads, double latHeightRads, double lonWidthRads)
    {
        MinLatRads = tlLatRads;
        MinLonRads = tlLonRads;
        MaxLatRads = tlLatRads + latHeightRads;
        MaxLonRads = tlLonRads + lonWidthRads;
    }

    public static GloLLBox GlobalBox => new GloLLBox(-Math.PI / 2.0, -Math.PI, Math.PI, 2.0 * Math.PI);

    public static GloLLBox Zero => new GloLLBox(0.0, 0.0, 0.0, 0.0);

    // --------------------------------------------------------------------------------------------

    public bool Contains(GloLLPoint inPos)
    {
        if (inPos.LatRads < MinLatRads) return false;
        if (inPos.LatRads > MaxLatRads) return false;
        if (inPos.LonRads < MinLonRads) return false;
        if (inPos.LonRads > MaxLonRads) return false;

        return true;
    }

    public (float, float) GetLatLonFraction(GloLLPoint inPos)
    {
        float latFrac = (float)((inPos.LatRads - MinLatRads) / DeltaLatRads);
        float lonFrac = (float)((inPos.LonRads - MinLonRads) / DeltaLonRads);

        return (latFrac, lonFrac);
    }

    // --------------------------------------------------------------------------------------------

    public GloLLBox ShiftBox(double adjustLatDegs, double adjustLonDegs, double adjustAltM)
    {
        return new GloLLBox()
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

