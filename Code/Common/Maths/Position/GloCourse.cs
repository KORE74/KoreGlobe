using System;

public struct GloCourse
{
    public double SpeedMps; // Metres per second
    public double HeadingRads;
    public double ClimbRateMps;

    // --------------------------------------------------------------------------------------------

    public double SpeedKph
    {
        get { return SpeedMps * GloWorldConsts.MPStoKPHMultiplier; }
        set { SpeedMps = value * GloWorldConsts.KPHtoMPSMultiplier; }
    }

    public double HeadingDegs
    {
        get { return HeadingRads * GloConsts.RadsToDegsMultiplier; }
        set { HeadingRads = (value % 360.0) * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public GloCourse(double inSpeedKph, double inHeadingDegs, double inClimbRateMtrSec)
    {
        SpeedMps     = inSpeedKph * GloWorldConsts.KPHtoMPSMultiplier;
        HeadingRads  = GloValueUtils.NormalizeAngle2Pi(inHeadingDegs * GloConsts.DegsToRadsMultiplier);
        ClimbRateMps = inClimbRateMtrSec;
    }

    public static GloCourse Zero
    {
        get { return new GloCourse { SpeedMps = 0.0, HeadingRads = 0.0, ClimbRateMps = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    public bool IsStationary()
    {
        return (SpeedMps < 0.001);
    }

    // --------------------------------------------------------------------------------------------

    public GloRangeBearing OffsetForTime(double elapsedSeconds)
    {
        return new GloRangeBearing(SpeedMps * elapsedSeconds, HeadingRads);
    }

    // Derive the course delta from two values and a duration
    public GloCourseDelta DeltaForTime(double elapsedSeconds, GloCourse newCourse)
    {
        double speedChange = (newCourse.SpeedMps - SpeedMps) / elapsedSeconds;
        double headingChange = ((newCourse.HeadingRads - HeadingRads) / elapsedSeconds) * GloConsts.RadsToDegsMultiplier;
        return new GloCourseDelta { SpeedChangeMpMps = speedChange, HeadingChangeClockwiseDegsSec = headingChange };
    }

    // Derive a new course from applying a duration for a period of time

    public GloCourse PlusDeltaForTime(GloCourseDelta delta, double elapsedSeconds)
    {
        double newSpeedMps = SpeedMps + delta.SpeedChangeMpMps * elapsedSeconds;
        double newHeadingDegs = HeadingDegs + (delta.HeadingChangeClockwiseDegsSec * -1.0 * elapsedSeconds);
        return new GloCourse(newSpeedMps * GloWorldConsts.MPStoKPHMultiplier, newHeadingDegs, ClimbRateMps);
    }

    // --------------------------------------------------------------------------------------------

    public GloAzElRange ToPolarOffset(double elapsedSeconds)
    {
        double rangeM = SpeedMps * elapsedSeconds;
        return new GloAzElRange(rangeM, HeadingRads, 0);
    }

    // --------------------------------------------------------------------------------------------

    // Lerp - for use cases where we move/smooth the course over time

    public static GloCourse Lerp(GloCourse start, GloCourse end, double t)
    {
        double speedMps     = GloValueUtils.Interpolate(start.SpeedMps,     end.SpeedMps,     t);
        double headingRads  = GloValueUtils.Interpolate(start.HeadingRads,  end.HeadingRads,  t);
        double climbRateMps = GloValueUtils.Interpolate(start.ClimbRateMps, end.ClimbRateMps, t);

        return new GloCourse() { SpeedMps = speedMps, HeadingRads = headingRads, ClimbRateMps = climbRateMps };
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        string speedStr = (IsStationary()) ? "Stationary" : $"Speed:{SpeedKph:F2}Kph";

        return $"{speedStr}, Heading:{HeadingDegs:F2}Degs, ClimbRate:{ClimbRateMps:F2}Mps";
    }
}



