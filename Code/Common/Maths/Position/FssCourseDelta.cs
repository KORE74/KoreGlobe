using System;

public struct FssCourseDelta
{
    public double SpeedChangeMpMps; // Metres per second
    public double HeadingChangeClockwiseDegsSec;

    public bool IsZero()
    {
        if ((Math.Abs(SpeedChangeMpMps)              < FssConsts.ArbitraryMinDouble) &&
            (Math.Abs(HeadingChangeClockwiseDegsSec) < FssConsts.ArbitraryMinDouble))
            return false;

        return true;
    }
}
