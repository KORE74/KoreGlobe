using System;

public struct GloAttitude
{
    public double RollClockwiseRads { get; set; } // +/- rads clockwise from straight ahead
    public double PitchUpRads       { get; set; } // +/- rads up from straight ahead
    public double YawClockwiseRads  { get; set; } // +/- rads clockwise from straight ahead

    // --------------------------------------------------------------------------------------------

    public double RollClockwiseDegs // +/- degs clockwise from straight ahead
    {
        get { return RollClockwiseRads * GloConsts.RadsToDegsMultiplier; }
        set { RollClockwiseRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double PitchUpDegs       // +/- degs up from straight ahead
    {
        get { return PitchUpRads * GloConsts.RadsToDegsMultiplier; }
        set { PitchUpRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double YawClockwiseDegs  // +/- degs clockwise from straight ahead
    {
        get { return YawClockwiseRads * GloConsts.RadsToDegsMultiplier; }
        set { YawClockwiseRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public GloAttitude(double r, double p, double y)
    {
        this.RollClockwiseRads = r;
        this.PitchUpRads       = p;
        this.YawClockwiseRads  = y;
    }

    public static GloAttitude Zero
    {
        get { return new GloAttitude { RollClockwiseRads = 0.0, PitchUpRads = 0.0, YawClockwiseRads = 0.0 }; }
    }

    public bool IsZero()
    {
        return (
            GloValueUtils.IsZero(RollClockwiseRads) &&
            GloValueUtils.IsZero(PitchUpRads) &&
            GloValueUtils.IsZero(YawClockwiseRads)
        );
    }

    public override string ToString()
    {
        return $"RollClockwiseDegs:{RollClockwiseDegs:F2}, PitchUpDegs:{PitchUpDegs:F2}, YawClockwiseDegs:{YawClockwiseDegs:F2}";
    }

    // --------------------------------------------------------------------------------------------

    public GloAttitude PlusDeltaForTime(GloAttitudeDelta delta, double elapsedSeconds)
    {
        GloAttitude retAtt = new GloAttitude {
            RollClockwiseRads = RollClockwiseRads + delta.RollRateClockwiseRadsPerSec * elapsedSeconds,
            PitchUpRads       = PitchUpRads       + delta.PitchRateUpRadsPerSec       * elapsedSeconds,
            YawClockwiseRads  = YawClockwiseRads  + delta.YawRateClockwiseRadsPerSec  * elapsedSeconds,
        };

        return retAtt;
    }

    // --------------------------------------------------------------------------------------------

    public GloAzElRange ToPolarOffset(double distance)
    {
        double roll  = RollClockwiseRads;
        double pitch = PitchUpRads;
        double yaw   = YawClockwiseRads;

        // Trigonometric calculations to account for the roll
        double azimuth = Math.Atan2(Math.Sin(yaw) * Math.Cos(roll) + Math.Cos(yaw) * Math.Sin(roll) * Math.Sin(pitch),
                                    Math.Cos(yaw) * Math.Cos(roll) - Math.Sin(yaw) * Math.Sin(roll) * Math.Sin(pitch));

        double elevation = Math.Asin(Math.Cos(yaw) * Math.Sin(pitch) + Math.Sin(yaw) * Math.Sin(roll) * Math.Cos(pitch));

        return new GloAzElRange(azimuth, elevation, distance);
    }

}
