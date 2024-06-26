using System;

public struct FssAttitudeDelta
{
    public double RollRateClockwiseRadsPerSec { get; set; } // +/- rads clockwise from straight ahead
    public double PitchRateUpRadsPerSec { get; set; }       // +/- rads up from straight ahead
    public double YawRateClockwiseRadsPerSec { get; set; }  // +/- rads clockwise from straight ahead

    public double RollRateClockwiseDegsPerSec // +/- degs clockwise from straight ahead
    {
        get { return RollRateClockwiseRadsPerSec * FssConsts.RadsToDegsMultiplier; }
        set { RollRateClockwiseRadsPerSec = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double PitchRateUpDegsPerSec       // +/- degs up from straight ahead
    {
        get { return PitchRateUpRadsPerSec * FssConsts.RadsToDegsMultiplier; }
        set { PitchRateUpRadsPerSec = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double YawRateClockwiseDegsPerSec  // +/- degs clockwise from straight ahead
    {
        get { return YawRateClockwiseRadsPerSec * FssConsts.RadsToDegsMultiplier; }
        set { YawRateClockwiseRadsPerSec = value * FssConsts.DegsToRadsMultiplier; }
    }

    public FssAttitudeDelta(double r, double p, double y)
    {
        this.RollRateClockwiseRadsPerSec = r;
        this.PitchRateUpRadsPerSec       = p;
        this.YawRateClockwiseRadsPerSec  = y;
    }
}
