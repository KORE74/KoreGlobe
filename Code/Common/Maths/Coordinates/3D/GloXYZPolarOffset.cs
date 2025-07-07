using System;

// GloXYZPolarOffset: Class represents an angle defined offset in the abstract XYZ space.

public struct GloXYZPolarOffset
{
    public double Range  { get; set; }
    public double AzRads { get; set; }
    public double ElRads { get; set; }

    // Add properties to convert to/from degrees
    public double AzDegs
    {
        get { return AzRads * GloConsts.RadsToDegsMultiplier; }
        set { AzRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double ElDegs
    {
        get { return ElRads * GloConsts.RadsToDegsMultiplier; }
        set { ElRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public GloXYZPolarOffset(double a, double e, double r)
    {
        this.AzRads = a;
        this.ElRads = e;
        this.Range  = r;
    }

    public GloXYZPolarOffset(GloXYZVector xyz)
    {
        this = FromXYZ(xyz);
    }

    // Static property for a zero offset
    public static GloXYZPolarOffset Zero
    {
        get { return new GloXYZPolarOffset { AzRads = 0.0, ElRads = 0.0, Range = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Functions
    // --------------------------------------------------------------------------------------------

    public GloXYZPolarOffset Scale(double scaleFactor)
    {
        return new GloXYZPolarOffset(AzRads, ElRads, Range * scaleFactor);
    }

    public GloXYZPolarOffset WithRange(double newRange)
    {
        return new GloXYZPolarOffset(AzRads, ElRads, newRange);
    }

    public GloXYZPolarOffset Inverse()
    {
        return new GloXYZPolarOffset
        {
            AzRads = this.AzRads + Math.PI, // 180 degrees in radians
            ElRads = -this.ElRads,
            Range  = this.Range
        };
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public GloXYZVector ToXYZ()
    {
        double x = Range * Math.Cos(ElRads) * Math.Sin(AzRads);
        double y = Range * Math.Sin(ElRads);
        double z = Range * Math.Cos(ElRads) * Math.Cos(AzRads);

        return new GloXYZVector(x, y, z);
    }

    public static GloXYZPolarOffset FromXYZ(GloXYZVector xyz)
    {
        double mag = xyz.Magnitude;

        GloXYZPolarOffset newOffset = new GloXYZPolarOffset()
        {
            Range  = mag,
            AzRads = Math.Atan2(xyz.X, xyz.Z),
            ElRads = Math.Asin(xyz.Y / mag)
        };
        return newOffset;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"(AzDegs:{AzDegs:F2}, ElDegs:{ElDegs:F2}, RangeM:{Range:F2})");
    }

}
