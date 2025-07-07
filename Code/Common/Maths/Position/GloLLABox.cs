using System;

// GloLLABox: Class to represent a 3D box, curving to a spherical surface. Deals with points in the
// box based on simple angle and distance comparisons. Can also be used to represent a box out from
// an entity, such as a radar wedge.

public struct GloLLABox
{
    public double MinLatRads { get; set; }
    public double MinLonRads { get; set; }
    public double MaxLatRads { get; set; }
    public double MaxLonRads { get; set; }

    public double MinRadiusM { get; set; }
    public double MaxRadiusM { get; set; }

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

    public double ArcLatRads { get { return MaxLatRads - MinLatRads; } }
    public double ArcLonRads { get { return MaxLonRads - MinLonRads; } }
    public double ArcLatDegs { get { return MaxLatDegs - MinLatDegs; } }
    public double ArcLonDegs { get { return MaxLonDegs - MinLonDegs; } }

    public double AltRangeM  { get { return MaxRadiusM - MinRadiusM; } }

    // --------------------------------------------------------------------------------------------

    public GloLLAPoint PosTopLeft
    {
        get { return new GloLLAPoint() { LatRads = MaxLatRads, LonRads = MinLonRads }; }
        set { MaxLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public GloLLAPoint PosTopRight
    {
        get { return new GloLLAPoint() { LatRads = MaxLatRads, LonRads = MaxLonRads }; }
        set { MaxLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    public GloLLAPoint PosBottomLeft
    {
        get { return new GloLLAPoint() { LatRads = MinLatRads, LonRads = MinLonRads }; }
        set { MinLatRads = value.LatRads; MinLonRads = value.LonRads; }
    }

    public GloLLAPoint PosBottomRight
    {
        get { return new GloLLAPoint() { LatRads = MinLatRads, LonRads = MaxLonRads }; }
        set { MinLatRads = value.LatRads; MaxLonRads = value.LonRads; }
    }

    // --------------------------------------------------------------------------------------------

    public bool Contains(GloLLAPoint InputLLA)
    {
        if (InputLLA.LatDegs < MinLatDegs) return false;
        if (InputLLA.LatDegs > MaxLatDegs) return false;
        if (InputLLA.LonDegs < MinLonDegs) return false;
        if (InputLLA.LonDegs > MaxLonDegs) return false;
        if (InputLLA.RadiusM < MinRadiusM) return false;
        if (InputLLA.RadiusM > MaxRadiusM) return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------

    public GloLLAPoint CenterPoint()
    {
        return new GloLLAPoint()
        {
            LatDegs = (MinLatDegs + MaxLatDegs) / 2.0,
            LonDegs = (MinLonDegs + MaxLonDegs) / 2.0,
            AltMslM = (MaxRadiusM + MinRadiusM) / 2.0,
        };
    }

    // --------------------------------------------------------------------------------------------

    public GloLLABox ShiftBox(double adjustLatDegs, double adjustLonDegs, double adjustAltM)
    {
        return new GloLLABox()
        {
            MinLatDegs = this.MinLatDegs + adjustLatDegs,
            MaxLatDegs = this.MaxLatDegs + adjustLatDegs,
            MinLonDegs = this.MinLonDegs + adjustLonDegs,
            MaxLonDegs = this.MaxLonDegs + adjustLonDegs,
            MaxRadiusM = this.MaxRadiusM + adjustAltM,
            MinRadiusM = this.MinRadiusM + adjustAltM
        };
    }

    // --------------------------------------------------------------------------------------------

    public string BoxToString()
    {
        return $"[{MinLatDegs:F3}, {MinLonDegs:F3}, {MaxLatDegs:F3}, {MaxLonDegs:F3}]";
    }
}

