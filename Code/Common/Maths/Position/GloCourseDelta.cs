using System;

public struct GloCourseDelta
{
    public double SpeedChangeMpMps; // Metres per metres per second
    public double HeadingChangeClockwiseDegsSec;

    // --------------------------------------------------------------------------------------------

    public static GloCourseDelta Zero
    {
        get { return new GloCourseDelta { SpeedChangeMpMps = 0.0, HeadingChangeClockwiseDegsSec = 0.0 }; }
    }

    public readonly bool IsZero()
    {
        bool speedZero         = GloValueUtils.IsZero(SpeedChangeMpMps);
        bool headingChangeZero = GloValueUtils.IsZero(HeadingChangeClockwiseDegsSec);

        return speedZero && headingChangeZero;
    }
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        if (IsZero()) return "Zero Delta";

        return $"SpeedChangeMpMps: {SpeedChangeMpMps:F3}, HeadingChangeClockwiseDegsSec: {HeadingChangeClockwiseDegsSec:F3}";
    }
}
