using System;

public struct FssAttitude
{
    public double RollClockwiseRads { get; set; } // +/- rads clockwise from straight ahead
    public double PitchUpRads { get; set; }       // +/- rads up from straight ahead
    public double YawClockwiseRads { get; set; }  // +/- rads clockwise from straight ahead

    // ------------------------------------------------------------------------

    public double RollClockwiseDegs // +/- degs clockwise from straight ahead
    {
        get { return RollClockwiseRads * FssConsts.RadsToDegsMultiplier; }
        set { RollClockwiseRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double PitchUpDegs       // +/- degs up from straight ahead
    {
        get { return PitchUpRads * FssConsts.RadsToDegsMultiplier; }
        set { PitchUpRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double YawClockwiseDegs  // +/- degs clockwise from straight ahead
    {
        get { return YawClockwiseRads * FssConsts.RadsToDegsMultiplier; }
        set { YawClockwiseRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    // ------------------------------------------------------------------------

    public FssAttitude(double r, double p, double y)
    {
        this.RollClockwiseRads = r;
        this.PitchUpRads       = p;
        this.YawClockwiseRads  = y;
    }

    public static FssAttitude Zero
    {
        get { return new FssAttitude { RollClockwiseRads = 0.0, PitchUpRads = 0.0, YawClockwiseRads = 0.0 }; }
    }

    public override string ToString()
    {
        return $"RollClockwiseDegs:{RollClockwiseDegs:F2}, PitchUpDegs:{PitchUpDegs:F2}, YawClockwiseDegs:{YawClockwiseDegs:F2}";
    }

    // ------------------------------------------------------------------------

    public FssAttitude PlusDeltaForTime(FssAttitudeDelta delta, double elapsedSeconds)
    {
        FssAttitude retAtt = new FssAttitude {
            RollClockwiseRads = RollClockwiseRads + delta.RollRateClockwiseRadsPerSec * elapsedSeconds,
            PitchUpRads       = PitchUpRads       + delta.PitchRateUpRadsPerSec       * elapsedSeconds,
            YawClockwiseRads  = YawClockwiseRads  + delta.YawRateClockwiseRadsPerSec  * elapsedSeconds,
        };

        return retAtt;
    }

    // ------------------------------------------------------------------------

    public FssPolarOffset ToPolarOffset(double distance)
    {
        double roll  = RollClockwiseRads;
        double pitch = PitchUpRads;
        double yaw   = YawClockwiseRads;

        // Trigonometric calculations to account for the roll
        double azimuth = Math.Atan2(Math.Sin(yaw) * Math.Cos(roll) + Math.Cos(yaw) * Math.Sin(roll) * Math.Sin(pitch),
                                    Math.Cos(yaw) * Math.Cos(roll) - Math.Sin(yaw) * Math.Sin(roll) * Math.Sin(pitch));

        double elevation = Math.Asin(Math.Cos(yaw) * Math.Sin(pitch) + Math.Sin(yaw) * Math.Sin(roll) * Math.Cos(pitch));

        return new FssPolarOffset(azimuth, elevation, distance);
    }

}
