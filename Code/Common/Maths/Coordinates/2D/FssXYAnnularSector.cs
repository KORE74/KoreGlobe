// FssXYArcSegmentBox: Class representing a 2D "box", based on an inner and outer radius, and start and end angles.
// Will have operations around the creation and manipulation of these boxes.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responibilites will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class FssXYAnnularSector : FssXY
{
    public FssXYPoint Center { get; }
    public double InnerRadius { get; }
    public double OuterRadius { get; }
    public double StartAngleRads { get; }
    public double DeltaAngleRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double EndAngleRads   { get { return StartAngleRads + DeltaAngleRads; } }

    public double StartAngleDegs  { get { return FssValueUtils.RadsToDegs(StartAngleRads); } }
    public double DeltaAngleDegs  { get { return FssValueUtils.RadsToDegs(DeltaAngleRads); } }
    public double EndAngleDegs    { get { return FssValueUtils.RadsToDegs(EndAngleRads); } }

    public double AngleSpanRads { get { return FssValueUtils.AngleDiffRads(StartAngleRads, EndAngleRads); } }
    public double AngleSpanDegs   { get { return FssValueUtils.RadsToDegs(AngleSpanRads); } }

    public FssXYPoint StartInnerPoint { get { return FssXYPointOperations.OffsetPolar(Center, InnerRadius, StartAngleRads); } }
    public FssXYPoint EndInnerPoint   { get { return FssXYPointOperations.OffsetPolar(Center, InnerRadius, EndAngleRads); } }
    public FssXYPoint StartOuterPoint { get { return FssXYPointOperations.OffsetPolar(Center, OuterRadius, StartAngleRads); } }
    public FssXYPoint EndOuterPoint   { get { return FssXYPointOperations.OffsetPolar(Center, OuterRadius, EndAngleRads); } }

    public FssXYLine StartInnerOuterLine { get { return new FssXYLine(StartInnerPoint, StartOuterPoint); } }
    public FssXYLine EndInnerOuterLine   { get { return new FssXYLine(EndInnerPoint, EndOuterPoint); } }

    public FssXYCircle InnerCircle { get { return new FssXYCircle(Center, InnerRadius); } }
    public FssXYCircle OuterCircle { get { return new FssXYCircle(Center, OuterRadius); } }

    public FssXYArc InnerArc { get { return new FssXYArc(Center, InnerRadius, StartAngleRads, EndAngleRads); } }
    public FssXYArc OuterArc { get { return new FssXYArc(Center, OuterRadius, StartAngleRads, EndAngleRads); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYAnnularSector(FssXYPoint center, double innerRadius, double outerRadius, double startAngleRads, double deltaAngleRads)
    {
        // Swap the inner and outer radii if they are the wrong way around
        if (innerRadius > outerRadius) 
            (innerRadius, outerRadius) = (outerRadius, innerRadius);

        Center         = center;
        InnerRadius    = innerRadius;
        OuterRadius    = outerRadius;
        StartAngleRads = startAngleRads;
        DeltaAngleRads = deltaAngleRads;
    }

    public FssXYAnnularSector(FssXYAnnularSector arc)
    {
        Center         = arc.Center;
        InnerRadius    = arc.InnerRadius;
        OuterRadius    = arc.OuterRadius;
        StartAngleRads = arc.StartAngleRads;
        DeltaAngleRads = arc.DeltaAngleRads;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYAnnularSector offset(double x, double y)
    {
        return new FssXYAnnularSector(Center.Offset(x, y), InnerRadius, OuterRadius, StartAngleRads, DeltaAngleRads);
    }

    // Check if a point is within the arc box, first by checking distance against the two radii, then by checking the angle.

    public bool Contains(FssXYPoint checkPos)
    {
        // Containing the point is three checks:
        // 1 - Its inside the outer radius
        // 2 - Its outside the inner radius
        // 3 - Its within the start and end angles
        bool isInsideOuterRadius = OuterCircle.Contains(checkPos);
        if (!isInsideOuterRadius)
            return false;

        bool isOutsideInnerRadius = !InnerCircle.Contains(checkPos);
        if (!isOutsideInnerRadius)
            return false;

        bool isWithinAngles = InnerArc.ContainsAngle(Center.AngleToRads(checkPos));
        if (!isWithinAngles)
            return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------


}