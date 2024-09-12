using System;

// FssPolarOffset: Class represents a straight line (no earth curvature) between two points.
using Godot;

public struct FssPolarOffset
{
    public double RangeM { get; set; }
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

    public double RangeKm
    {
        get { return RangeM * FssPosConsts.MetresToKmMultiplier; }
        set { RangeM = value * FssPosConsts.KmToMetresMultiplier; }
    }

    public double AltOffsetM
    {
        get { return Math.Sin(ElRads) * RangeM; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public FssPolarOffset(double rangeM, double azRads, double elRads)
    {
        this.RangeM = rangeM;
        this.AzRads = azRads;
        this.ElRads = elRads;
    }

    public FssPolarOffset(double heightM, double widthM, double depthM, double orientationDeg)
    {
        // Calculate azimuth from orientation and width/depth ratio
        double angleRad = Math.Atan2(widthM, depthM) + orientationDeg * FssConsts.DegsToRadsMultiplier;

        // Wrap angle between 0 and 2*Pi
        AzRads = (angleRad + 2 * Math.PI) % (2 * Math.PI);

        // Calculate range (distance from origin) using Pythagorean theorem in 3D
        RangeM = Math.Sqrt(Math.Pow(heightM, 2) + Math.Pow(widthM, 2) + Math.Pow(depthM, 2));

        // Calculate elevation angle from height and range
        ElRads = Math.Asin(heightM / RangeM);
    }

    // Static property for a zero offset
    public static FssPolarOffset Zero
    {
        get { return new FssPolarOffset { RangeM = 0.0, AzRads = 0.0, ElRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Functions
    // --------------------------------------------------------------------------------------------

    public FssPolarOffset Scale(double scaleFactor)
    {
        return new FssPolarOffset(RangeM * scaleFactor, AzRads, ElRads);
    }

    public FssPolarOffset WithRangeM(double newRangeM)
    {
        return new FssPolarOffset(newRangeM, AzRads, ElRads);
    }

    public FssPolarOffset Inverse()
    {
        return new FssPolarOffset
        {
            RangeKm = this.RangeM,
            ElRads  = -this.ElRads,
            AzRads  = this.AzRads + Math.PI // 180 degrees in radians
        };
    }

    public FssPolarOffset SimpleInterpolation(FssPolarOffset toOffset, double fraction)
    {
        fraction = FssValueUtils.LimitToRange(fraction, 0, 1);
        double invFraction = 1 - fraction;

        FssPolarOffset newOffset = new FssPolarOffset();
        newOffset.AzRads = (AzRads * invFraction) + (toOffset.AzRads * fraction);
        newOffset.ElRads = (ElRads * invFraction) + (toOffset.ElRads * fraction);
        newOffset.RangeM = (RangeM * invFraction) + (toOffset.RangeM * fraction);

        return newOffset;
    }

    public FssXYZPoint ToXYZ()
    {
        double x = RangeM * Math.Cos(ElRads) * Math.Sin(AzRads);
        double y = RangeM * Math.Sin(ElRads);
        double z = RangeM * Math.Cos(ElRads) * Math.Cos(AzRads);

        return new FssXYZPoint(x, y, z);
    }

    public override string ToString()
    {
        return string.Format($"(AzDegs:{AzDegs:F2}, ElDegs:{ElDegs:F2}, RangeM:{RangeM:F2})");
    }

}
