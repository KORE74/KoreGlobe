using System;

// GloXYArc: class representing a 2D arc, which can be clockwise or anti-clockwise in direction.
// Will use radians natively and the distance units are abstract.
// Will use GloValueUtils.Angle to ensure angles are wrapped and differences are calculated correctly.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

public class GloXYArc : GloXY
{
    public GloXYPoint Center     { get; }
    public double Radius         { get; }
    public double StartAngleRads { get; }
    public double DeltaAngleRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double EndAngleRads       { get { return StartAngleRads + DeltaAngleRads; } }

    public double Diameter           { get { return 2 * Radius; } }
    public double AngleSpanRads      { get { return GloValueUtils.AngleDiffRads(StartAngleRads, EndAngleRads); } }
    public double LengthCurved       { get { return Radius * AngleSpanRads; } }
    public double LengthStraightLine { get { return Math.Sqrt(Diameter * Diameter + LengthCurved * LengthCurved); } }

    public GloXYPoint StartPoint     { get { return GloXYPointOperations.OffsetPolar(Center, Radius, StartAngleRads); } }
    public GloXYPoint EndPoint       { get { return GloXYPointOperations.OffsetPolar(Center, Radius, EndAngleRads); } }

    public double StartAngleDegs     { get { return GloValueUtils.RadsToDegs(StartAngleRads); } }
    public double EndAngleDegs       { get { return GloValueUtils.RadsToDegs(EndAngleRads); } }
    public double AngleSpanDegs      { get { return GloValueUtils.RadsToDegs(AngleSpanRads); } }

    public GloXYCircle Circle        { get { return new GloXYCircle(Center, Radius); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYArc(GloXYPoint center, double radius, double startAngleRads, double deltaAngleRads)
    {
        Center         = center;
        Radius         = radius;
        StartAngleRads = startAngleRads;
        DeltaAngleRads = deltaAngleRads;
    }

    public GloXYArc(GloXYArc arc)
    {
        Center         = arc.Center;
        Radius         = arc.Radius;
        StartAngleRads = arc.StartAngleRads;
        DeltaAngleRads = arc.DeltaAngleRads;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public GloXYArc offset(double x, double y)
    {
        return new GloXYArc(Center.Offset(x, y), Radius, StartAngleRads, DeltaAngleRads);
    }

    // return if the angle is within the arc

    public bool ContainsAngle(double angleRads)
    {
        // return GloValueUtils.IsAngleInRangeRads(angleRads, StartAngleDegs, EndAngleDegs);

        return GloValueUtils.IsAngleInRangeRadsDelta(angleRads, StartAngleDegs, DeltaAngleRads);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------


}