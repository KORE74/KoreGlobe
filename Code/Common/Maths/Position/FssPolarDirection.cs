using System;

// FssPolarOffset: Class represents a straight line (no earth curvature) between two points.
public struct FssPolarDirection
{
    public enum AngleWrapMode { NegativePiToPi, ZeroToTwoPi }

    public double AzRads { get; set; }
    public double ElRads { get; set; }
    public AngleWrapMode WrapMode { get; set; }

    // Add properties to convert to/from degrees
    public double AzDegs
    {
        get { return AzRads * FssConsts.RadsToDegsMultiplier; }
        set { AzRads = WrapAngle(value * FssConsts.DegsToRadsMultiplier, WrapMode); }
    }

    public double ElDegs
    {
        get { return ElRads * FssConsts.RadsToDegsMultiplier; }
        set { ElRads = WrapAngle(value * FssConsts.DegsToRadsMultiplier, WrapMode); }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Note: Constructor takes radians
    public FssPolarDirection(double azRads, double elRads, AngleWrapMode range = AngleWrapMode.ZeroToTwoPi)
    {
        this.WrapMode = wrapMode;
        this.AzRads = WrapAngle(azRads, range);
        this.ElRads = WrapAngle(elRads, range);
    }

    // Static property for a zero offset
    public static FssPolarDirection Zero
    {
        get { return new FssPolarDirection { AzRads = 0.0, ElRads = 0.0, WrapMode = AngleWrapMode.ZeroToTwoPi }; }
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Functions
    // --------------------------------------------------------------------------------------------

    public FssPolarDirection Inverse()
    {
        return new FssPolarDirection
        {
            ElRads  = WrapAngle(-this.ElRads, WrapMode),
            AzRads  = WrapAngle(this.AzRads + Math.PI, WrapMode),
            WrapMode = this.WrapMode
        };
    }

    public override string ToString()
    {
        return string.Format($"(Az:{AzDegs:F2}Degs, El:{ElDegs:F2}Degs)");
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Helper Functions
    // --------------------------------------------------------------------------------------------

    private static double WrapAngle(double angleRads, AngleWrapMode range)
    {
        switch (range)
        {
            case AngleWrapMode.ZeroToTwoPi:
                angleRads = angleRads % (2 * Math.PI);
                if (angleRads < 0)
                    angleRads += 2 * Math.PI;
                return angleRads;
            case AngleWrapMode.NegativePiToPi:
            default:
                while (angleRads <= -Math.PI)
                {
                    angleRads += 2 * Math.PI;
                }
                while (angleRads > Math.PI)
                {
                    angleRads -= 2 * Math.PI;
                }
                return angleRads;
        }
    }
}
