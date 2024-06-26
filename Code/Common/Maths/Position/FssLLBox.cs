using System;

// Class for a Map Tile Lat Long Box. Altitude/Radius will always be assumes to be MSL, or just not considered.

public struct FssLLBox
{
    // Top left LL position
    public double TLLatRads { get; set; }
    public double TLLonRads { get; set; }

    // Box size
    public double LatHeightRads { get; set; }
    public double LonWidthRads  { get; set; }

    // Derived properties
    public readonly double MinLatRads => TLLatRads - LatHeightRads;
    public readonly double MaxLatRads => TLLatRads;
    public readonly double MinLonRads => TLLonRads;
    public readonly double MaxLonRads => TLLonRads + LonWidthRads;

    public readonly double MinLatDegs => MinLatRads * (180.0 / Math.PI);
    public readonly double MaxLatDegs => MaxLatRads * (180.0 / Math.PI);
    public readonly double MinLonDegs => MinLonRads * (180.0 / Math.PI);
    public readonly double MaxLonDegs => MaxLonRads * (180.0 / Math.PI);

    public readonly double DeltaLatDegs => LatHeightRads * (180.0 / Math.PI);
    public readonly double DeltaLonDegs => LonWidthRads * (180.0 / Math.PI);

    // ------------------------------------------------------------------------

    // Top Left, Top Right, Bottom Left, Bottom Right properties
    public FssLLPoint TopLeft     => new FssLLPoint() { LatRads = MaxLatRads, LonRads = MinLonRads };
    public FssLLPoint TopRight    => new FssLLPoint() { LatRads = MaxLatRads, LonRads = MaxLonRads };
    public FssLLPoint BottomLeft  => new FssLLPoint() { LatRads = MinLatRads, LonRads = MinLonRads };
    public FssLLPoint BottomRight => new FssLLPoint() { LatRads = MinLatRads, LonRads = MaxLonRads };

    // --------------------------------------------------------------------------------------------
    // #MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public FssLLBox(double tlLatRads, double tlLonRads, double latHeightRads, double lonWidthRads)
    {
        TLLatRads     = tlLatRads;
        TLLonRads     = tlLonRads;
        LatHeightRads = latHeightRads;
        LonWidthRads  = lonWidthRads;
    }

    public static FssLLBox GlobalBox => new FssLLBox(-Math.PI / 2.0, -Math.PI, Math.PI, 2.0 * Math.PI);

    public static FssLLBox ZeroBox => new FssLLBox(0.0, 0.0, 0.0, 0.0);

    // ------------------------------------------------------------------------

    public bool Contains(FssLLPoint posLL)
    {
        if (posLL.LatDegs < MinLatDegs) return false;
        if (posLL.LatDegs > MaxLatDegs) return false;
        if (posLL.LonDegs < MinLonDegs) return false;
        if (posLL.LonDegs > MaxLonDegs) return false;

        return true;
    }

    // ------------------------------------------------------------------------

    // Returns the center point of the box
    public FssLLPoint CenterPoint()
    {
        return new FssLLPoint()
        {
            LatDegs = (MinLatDegs + MaxLatDegs) / 2.0,
            LonDegs = (MinLonDegs + MaxLonDegs) / 2.0
        };
    }

    // ------------------------------------------------------------------------

    // Shifts the box by given latitude and longitude adjustments
    public FssLLBox ShiftBox(double adjustLatDegs, double adjustLonDegs)
    {
        return new FssLLBox()
        {
            TLLatRads     = this.TLLatRads + adjustLatDegs * (Math.PI / 180.0),
            TLLonRads     = this.TLLonRads + adjustLonDegs * (Math.PI / 180.0),
            LatHeightRads = this.LatHeightRads,
            LonWidthRads  = this.LonWidthRads
        };
    }

    // ------------------------------------------------------------------------

    public override string ToString()
    {
        return $"[MinLatDegs:{MinLatDegs:F3}, MinLonDegs:{MinLonDegs:F3}, MaxLatDegs:{MaxLatDegs:F3}, MaxLonDegs:{MaxLonDegs:F3}]";
    }
}
