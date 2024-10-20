using System;

public struct FssCourse
{
    public double GroundSpeedMps;
    public double HeadingRads;
    public double ClimbRateMps;

    // ------------------------------------------------------------------------

    public double GroundSpeedKph
    {
        get { return GroundSpeedMps * FssPosConsts.MPStoKPHMultiplier; }
        set { GroundSpeedMps = value * FssPosConsts.KPHtoMPSMultiplier; }
    }

    public double HeadingDegs
    {
        get { return HeadingRads * FssConsts.RadsToDegsMultiplier; }
        set { HeadingRads = (value % 360.0) * FssConsts.DegsToRadsMultiplier; }
    }

    // ------------------------------------------------------------------------

    public FssCourse(double inSpeedKph, double inHeadingDegs, double inClimbRateMtrSec)
    {
        GroundSpeedMps = inSpeedKph * FssPosConsts.KPHtoMPSMultiplier;
        HeadingRads    = FssValueUtils.NormalizeAngle2Pi(inHeadingDegs * FssConsts.DegsToRadsMultiplier);
        ClimbRateMps   = inClimbRateMtrSec;
    }

    public static FssCourse Zero
    {
        get { return new FssCourse { GroundSpeedMps = 0.0, HeadingRads = 0.0, ClimbRateMps = 0.0 }; }
    }

    // ------------------------------------------------------------------------

    public bool IsStationary()
    {
        return (GroundSpeedMps < 0.001) && (ClimbRateMps < 0.001);
    }

    // ------------------------------------------------------------------------

    public FssRangeBearing OffsetForTime(double elapsedSeconds)
    {
        return new FssRangeBearing(GroundSpeedMps * elapsedSeconds, HeadingRads);
    }

    // Derive the course delta from two values and a duration
    public FssCourseDelta DeltaForTime(double elapsedSeconds, FssCourse newCourse)
    {
        double speedChange = (newCourse.GroundSpeedMps - GroundSpeedMps) / elapsedSeconds;
        double headingChange = ((newCourse.HeadingRads - HeadingRads) / elapsedSeconds) * FssConsts.RadsToDegsMultiplier;
        return new FssCourseDelta { SpeedChangeMpMps = speedChange, HeadingChangeClockwiseDegsSec = headingChange };
    }

    // Derive a new course from applying a duration for a period of time

    public FssCourse PlusDeltaForTime(FssCourseDelta delta, double elapsedSeconds)
    {
        double newSpeedMps = GroundSpeedMps + delta.SpeedChangeMpMps * elapsedSeconds;
        double newHeadingDegs = HeadingDegs + (delta.HeadingChangeClockwiseDegsSec * -1.0 * elapsedSeconds);
        return new FssCourse(newSpeedMps * FssPosConsts.MPStoKPHMultiplier, newHeadingDegs, ClimbRateMps);
    }

    // ------------------------------------------------------------------------

    public FssPolarOffset ToPolarOffset(double elapsedSeconds)
    {
        double rangeM = GroundSpeedMps * elapsedSeconds;
        return new FssPolarOffset(rangeM, HeadingRads, 0);
    }

    // ------------------------------------------------------------------------

    public override string ToString()
    {
        string speedStr = (IsStationary()) ? "Stationary" : $"Speed:{GroundSpeedKph:F2}Kph";

        return $"{speedStr}, Heading:{HeadingDegs:F2}Degs, ClimbRate:{ClimbRateMps:F2}Mps";
    }
}



