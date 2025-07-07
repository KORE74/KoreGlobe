using System;

public struct GloAttitudeDelta
{
    public double RollRateClockwiseRadsPerSec { get; set; } // +/- rads clockwise from straight ahead
    public double PitchRateUpRadsPerSec       { get; set; } // +/- rads up from straight ahead
    public double YawRateClockwiseRadsPerSec  { get; set; } // +/- rads clockwise from straight ahead

    public double RollRateClockwiseDegsPerSec // +/- degs clockwise from straight ahead
    {
        get { return RollRateClockwiseRadsPerSec * GloConsts.RadsToDegsMultiplier; }
        set { RollRateClockwiseRadsPerSec = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double PitchRateUpDegsPerSec       // +/- degs up from straight ahead
    {
        get { return PitchRateUpRadsPerSec * GloConsts.RadsToDegsMultiplier; }
        set { PitchRateUpRadsPerSec = value * GloConsts.DegsToRadsMultiplier; }
    }

    public double YawRateClockwiseDegsPerSec  // +/- degs clockwise from straight ahead
    {
        get { return YawRateClockwiseRadsPerSec * GloConsts.RadsToDegsMultiplier; }
        set { YawRateClockwiseRadsPerSec = value * GloConsts.DegsToRadsMultiplier; }
    }

    public GloAttitudeDelta(double r, double p, double y)
    {
        this.RollRateClockwiseRadsPerSec = r;
        this.PitchRateUpRadsPerSec       = p;
        this.YawRateClockwiseRadsPerSec  = y;
    }

    // --------------------------------------------------------------------------------------------

    public static GloAttitudeDelta Zero
    {
        get { return new GloAttitudeDelta { RollRateClockwiseRadsPerSec = 0.0, PitchRateUpRadsPerSec = 0.0, YawRateClockwiseRadsPerSec = 0.0 }; }
    }

    public readonly bool IsZero()
    {
        bool rollZero  = GloValueUtils.IsZero(RollRateClockwiseRadsPerSec);
        bool pitchZero = GloValueUtils.IsZero(PitchRateUpRadsPerSec);
        bool yawZero   = GloValueUtils.IsZero(YawRateClockwiseRadsPerSec);

        return rollZero && pitchZero && yawZero;
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return string.Format(
            $"Roll(Clockwise):{RollRateClockwiseDegsPerSec:F2} deg/s, Pitch(Up):{PitchRateUpDegsPerSec:F2}deg/s, Yaw(Clockwise):{YawRateClockwiseDegsPerSec:F2}deg/s");
    }
}

