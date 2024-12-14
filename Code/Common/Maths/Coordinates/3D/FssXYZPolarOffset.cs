using System;

// FssXYZPolarOffset: Class represents an angle defined offset in the abstract XYZ space.

public struct FssXYZPolarOffset
{
    public double Range  { get; set; }
    public double AzRads { get; set; }
    public double ElRads { get; set; }

    // Add properties to convert to/from degrees
    public double AzDegs
    {
        get { return AzRads * FssConsts.RadsToDegsMultiplier; }
        set { AzRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double ElDegs
    {
        get { return ElRads * FssConsts.RadsToDegsMultiplier; }
        set { ElRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public FssXYZPolarOffset(double a, double e, double r)
    {
        this.AzRads = a;
        this.ElRads = e;
        this.Range  = r;
    }

    // Static property for a zero offset
    public static FssXYZPolarOffset Zero
    {
        get { return new FssXYZPolarOffset { AzRads = 0.0, ElRads = 0.0, Range = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Functions
    // --------------------------------------------------------------------------------------------

    public FssXYZPolarOffset Scale(double scaleFactor)
    {
        return new FssXYZPolarOffset(AzRads, ElRads, Range * scaleFactor);
    }

    public FssXYZPolarOffset WithRange(double newRange)
    {
        return new FssXYZPolarOffset(AzRads, ElRads, newRange);
    }

    public FssXYZPolarOffset Inverse()
    {
        return new FssXYZPolarOffset
        {
            AzRads  = this.AzRads + Math.PI, // 180 degrees in radians
            ElRads  = -this.ElRads,
            Range   = this.Range
        };
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public FssXYZPoint ToXYZ()
    {
        double x = Range * Math.Cos(ElRads) * Math.Sin(AzRads);
        double y = Range * Math.Sin(ElRads);
        double z = Range * Math.Cos(ElRads) * Math.Cos(AzRads);

        return new FssXYZPoint(x, y, z);
    }

    public static FssXYZPolarOffset FromXYZ(FssXYZVector xyz)
    {
        FssXYZPolarOffset newOffset = new FssXYZPolarOffset()
        {
            Range  = xyz.Length,
            AzRads = Math.Atan2(xyz.X, xyz.Z),
            ElRads = Math.Asin(xyz.Y / xyz.Length)
        };
        return newOffset;
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: String
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"(AzDegs:{AzDegs:F2}, ElDegs:{ElDegs:F2}, RangeM:{Range:F2})");
    }

}
