// GloXYArcSegmentBox: Class representing a 2D "box", based on an inner and outer radius, and start and end angles.
// Will have operations around the creation and manipulation of these boxes.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responibilites will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class GloXYAnnularSector : GloXY
{
    public GloXYPoint Center { get; }
    public double InnerRadius { get; }
    public double OuterRadius { get; }
    public double StartAngleRads { get; }
    public double DeltaAngleRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double EndAngleRads    { get { return StartAngleRads + DeltaAngleRads; } }

    public double StartAngleDegs  { get { return GloValueUtils.RadsToDegs(StartAngleRads); } }
    public double DeltaAngleDegs  { get { return GloValueUtils.RadsToDegs(DeltaAngleRads); } }
    public double EndAngleDegs    { get { return GloValueUtils.RadsToDegs(EndAngleRads); } }

    public double AngleSpanRads   { get { return GloValueUtils.AngleDiffRads(StartAngleRads, EndAngleRads); } }
    public double AngleSpanDegs   { get { return GloValueUtils.RadsToDegs(AngleSpanRads); } }

    public GloXYPoint StartInnerPoint { get { return GloXYPointOperations.OffsetPolar(Center, InnerRadius, StartAngleRads); } }
    public GloXYPoint EndInnerPoint   { get { return GloXYPointOperations.OffsetPolar(Center, InnerRadius, EndAngleRads); } }
    public GloXYPoint StartOuterPoint { get { return GloXYPointOperations.OffsetPolar(Center, OuterRadius, StartAngleRads); } }
    public GloXYPoint EndOuterPoint   { get { return GloXYPointOperations.OffsetPolar(Center, OuterRadius, EndAngleRads); } }

    public GloXYLine StartInnerOuterLine { get { return new GloXYLine(StartInnerPoint, StartOuterPoint); } }
    public GloXYLine EndInnerOuterLine   { get { return new GloXYLine(EndInnerPoint, EndOuterPoint); } }

    public GloXYCircle InnerCircle { get { return new GloXYCircle(Center, InnerRadius); } }
    public GloXYCircle OuterCircle { get { return new GloXYCircle(Center, OuterRadius); } }

    public GloXYArc InnerArc { get { return new GloXYArc(Center, InnerRadius, StartAngleRads, EndAngleRads); } }
    public GloXYArc OuterArc { get { return new GloXYArc(Center, OuterRadius, StartAngleRads, EndAngleRads); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYAnnularSector(GloXYPoint center, double innerRadius, double outerRadius, double startAngleRads, double deltaAngleRads)
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

    public GloXYAnnularSector(GloXYAnnularSector arc)
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

    public GloXYAnnularSector offset(double x, double y)
    {
        return new GloXYAnnularSector(Center.Offset(x, y), InnerRadius, OuterRadius, StartAngleRads, DeltaAngleRads);
    }

    // Check if a point is within the arc box, first by checking distance against the two radii, then by checking the angle.

    public bool Contains(GloXYPoint checkPos)
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