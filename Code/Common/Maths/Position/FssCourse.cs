using System;

public struct FssCourse
{
    public double SpeedMps; // Metres per second
    public double HeadingRads;

    // ------------------------------------------------------------------------

    public double SpeedKph
    {
        get { return SpeedMps * FssPosConsts.MPStoKPHMultiplier; }
        set { SpeedMps = value * FssPosConsts.KPHtoMPSMultiplier; }
    }

    public double HeadingDegs
    {
        get { return HeadingRads * FssConsts.RadsToDegsMultiplier; }
        set { HeadingRads = (value % 360.0) * FssConsts.DegsToRadsMultiplier; }
    }

    // ------------------------------------------------------------------------

    public FssCourse(double inSpeedKph, double inHeadingDegs)
    {
        SpeedMps    = inSpeedKph * FssPosConsts.KPHtoMPSMultiplier;
        HeadingRads = FssValueUtils.NormalizeAngle2Pi(inHeadingDegs * FssConsts.DegsToRadsMultiplier);
    }

    // ------------------------------------------------------------------------

    public bool IsStationary()
    {
        return (SpeedMps < 0.001);
    }

    // ------------------------------------------------------------------------

    public FssRangeBearing OffsetForTime(double elapsedSeconds)
    {
        return new FssRangeBearing(SpeedMps * elapsedSeconds, HeadingRads);
    }

    // Derive the course delta from two values and a duration
    public FssCourseDelta DeltaForTime(double elapsedSeconds, FssCourse newCourse)
    {
        double speedChange = (newCourse.SpeedMps - SpeedMps) / elapsedSeconds;
        double headingChange = ((newCourse.HeadingRads - HeadingRads) / elapsedSeconds) * FssConsts.RadsToDegsMultiplier;
        return new FssCourseDelta { SpeedChangeMpMps = speedChange, HeadingChangeClockwiseDegsSec = headingChange };
    }

    // Derive a new course from applying a duration for a period of time

    public FssCourse PlusDeltaForTime(FssCourseDelta delta, double elapsedSeconds)
    {
        double newSpeedMps = SpeedMps + delta.SpeedChangeMpMps * elapsedSeconds;
        double newHeadingDegs = HeadingDegs + (delta.HeadingChangeClockwiseDegsSec * -1.0 * elapsedSeconds);
        return new FssCourse(newSpeedMps * FssPosConsts.MPStoKPHMultiplier, newHeadingDegs);
    }

    // ------------------------------------------------------------------------

    public FssPolarOffset ToPolarOffset(double elapsedSeconds)
    {
        double rangeM = SpeedMps * elapsedSeconds;
        return new FssPolarOffset(rangeM, HeadingRads, 0);
    }

    // ------------------------------------------------------------------------

    public override string ToString()
    {
        return $"Speed: {SpeedKph:F2} Kph, Heading: {HeadingDegs:F2} Degs";
    }
}
