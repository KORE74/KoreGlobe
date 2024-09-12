using System;

public struct FssCourseDelta
{
    public double SpeedChangeMpMps; // Metres per second
    public double HeadingChangeClockwiseDegsSec;


    public static FssCourseDelta Zero
    {
        get { return new FssCourseDelta { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 }; }
    }

    public bool IsZero()
    {
        if ((Math.Abs(SpeedChangeMpMps)              < FssConsts.ArbitraryMinDouble) &&
            (Math.Abs(HeadingChangeClockwiseDegsSec) < FssConsts.ArbitraryMinDouble))
            return false;

        return true;
    }

    public override string ToString()
    {
        if (IsZero()) return "Zero Delta";

        return $"SpeedChangeMpMps: {SpeedChangeMpMps:F3}, HeadingChangeClockwiseDegsSec: {HeadingChangeClockwiseDegsSec:F3}";
    }
}
