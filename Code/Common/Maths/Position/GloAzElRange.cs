using System;

// GloAzElRange: Class represents a straight line (no earth curvature) between two points.
using Godot;

public struct GloAzElRange
{
    public double RangeM { get; set; }
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

    public double RangeKm
    {
        get { return RangeM * GloWorldConsts.MetresToKmMultiplier; }
        set { RangeM = value * GloWorldConsts.KmToMetresMultiplier; }
    }

    public double AltOffsetM
    {
        get { return Math.Sin(ElRads) * RangeM; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public GloAzElRange(double rangeM, double azRads, double elRads)
    {
        this.RangeM = rangeM;
        this.AzRads = azRads;
        this.ElRads = elRads;
    }

    public GloAzElRange(double heightM, double widthM, double depthM, double orientationDeg)
    {
        // Calculate azimuth from orientation and width/depth ratio
        double angleRad = Math.Atan2(widthM, depthM) + orientationDeg * GloConsts.DegsToRadsMultiplier;

        // Wrap angle between 0 and 2*Pi
        AzRads = (angleRad + 2 * Math.PI) % (2 * Math.PI);

        // Calculate range (distance from origin) using Pythagorean theorem in 3D
        RangeM = Math.Sqrt(Math.Pow(heightM, 2) + Math.Pow(widthM, 2) + Math.Pow(depthM, 2));

        // Calculate elevation angle from height and range
        ElRads = Math.Asin(heightM / RangeM);
    }

    // Static property for a zero offset
    public static GloAzElRange Zero
    {
        get { return new GloAzElRange { RangeM = 0.0, AzRads = 0.0, ElRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Functions
    // --------------------------------------------------------------------------------------------

    public GloAzElRange Scale(double scaleFactor)
    {
        return new GloAzElRange(RangeM * scaleFactor, AzRads, ElRads);
    }

    public GloAzElRange WithRangeM(double newRangeM)
    {
        return new GloAzElRange(newRangeM, AzRads, ElRads);
    }

    public GloAzElRange Inverse()
    {
        return new GloAzElRange()
        {
            RangeM = this.RangeM,
            ElRads  = -this.ElRads,
            AzRads  = this.AzRads + Math.PI // 180 degrees in radians
        };
    }

    public GloAzElRange SimpleInterpolation(GloAzElRange toOffset, double fraction)
    {
        fraction = GloValueUtils.LimitToRange(fraction, 0, 1);
        double invFraction = 1 - fraction;

        GloAzElRange newOffset = new GloAzElRange();
        newOffset.AzRads = GloValueUtils.NormalizeAngle2Pi( (AzRads * invFraction) + (toOffset.AzRads * fraction) );
        newOffset.ElRads = (ElRads * invFraction) + (toOffset.ElRads * fraction);
        newOffset.RangeM = (RangeM * invFraction) + (toOffset.RangeM * fraction);

        return newOffset;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conversion
    // --------------------------------------------------------------------------------------------

    public GloXYZPoint ToXYZ()
    {
        double x = RangeM * Math.Cos(ElRads) * Math.Sin(AzRads);
        double y = RangeM * Math.Sin(ElRads);
        double z = RangeM * Math.Cos(ElRads) * Math.Cos(AzRads);

        return new GloXYZPoint(x, y, z);
    }

    public static GloAzElRange FromXYZ(GloXYZPoint xyz)
    {
        GloAzElRange newOffset = new GloAzElRange() {
            RangeM = xyz.Length,
            AzRads = Math.Atan2(xyz.X, xyz.Z),
            ElRads = Math.Asin(xyz.Y / xyz.Length)
        };

        return newOffset;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: String
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format($"(AzDegs:{AzDegs:F2}, ElDegs:{ElDegs:F2}, RangeM:{RangeM:F2})");
    }

}
