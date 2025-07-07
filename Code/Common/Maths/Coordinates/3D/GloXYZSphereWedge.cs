// GloXYZSphereWedge: Class representing a 3D "wedge", based on an inner and outer radius, height, and start and end angles.
// Will have operations around the creation and manipulation of these wedges.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responsibilities will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class GloXYZSphereWedge : GloXYZ
{
    public GloXYZPoint Center { get; set; }
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }
    public double StartAzRads { get; set; } // Using start and delta (clockwise) angles to better accomodate wrapping
    public double StartElRads { get; set; }
    public double DeltaAzRads { get; set; }
    public double DeltaElRads { get; set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double StartAzDegs
    {
         get { return GloValueUtils.RadsToDegs(StartAzRads); }
         set { StartAzRads = GloValueUtils.DegsToRads(value); }
    }
    public double DeltaAzDegs
    {
        get { return GloValueUtils.RadsToDegs(DeltaAzRads); }
        set { DeltaAzRads = GloValueUtils.DegsToRads(value); }
    }
    public double StartElDegs
    {
        get { return GloValueUtils.RadsToDegs(StartElRads); }
        set { StartElRads = GloValueUtils.DegsToRads(value); }
    }
    public double DeltaElDegs
    {
        get { return GloValueUtils.RadsToDegs(DeltaElRads); }
        set { DeltaElRads = GloValueUtils.DegsToRads(value); }
    }

    public double EndAzRads         { get { return StartAzRads + DeltaAzRads; } }
    public double EndAzDegs         { get { return GloValueUtils.RadsToDegs(EndAzRads); } }

    public double EndElRads         { get { return StartElRads + DeltaElRads; } }
    public double EndElDegs         { get { return GloValueUtils.RadsToDegs(EndElRads); } }

    public GloDoubleRange AzRangeDegs      { get { return new GloDoubleRange(StartAzDegs, EndAzDegs); } }
    public GloDoubleRange ElRangeDegs      { get { return new GloDoubleRange(StartElDegs, EndElDegs); } }
    public GloDoubleRange AzRangeRads      { get { return new GloDoubleRange(StartAzRads, EndAzRads); } }
    public GloDoubleRange ElRangeRads      { get { return new GloDoubleRange(StartElRads, EndElRads); } }
    public GloDoubleRange RadiusRange      { get { return new GloDoubleRange(InnerRadius, OuterRadius); } }

    public GloXYZPoint BaseStartInnerPoint { get { return Center.PlusPolarOffset(new (StartAzRads, StartElRads, InnerRadius)); } }
    public GloXYZPoint BaseEndInnerPoint   { get { return Center.PlusPolarOffset(new (EndAzRads,   StartElRads, InnerRadius)); } }
    public GloXYZPoint BaseStartOuterPoint { get { return Center.PlusPolarOffset(new (StartAzRads, StartElRads, OuterRadius)); } }
    public GloXYZPoint BaseEndOuterPoint   { get { return Center.PlusPolarOffset(new (EndAzRads,   StartElRads, OuterRadius)); } }

    public GloXYZPoint TopStartInnerPoint  { get { return Center.PlusPolarOffset(new (StartAzRads, EndElRads,   InnerRadius)); } }
    public GloXYZPoint TopEndInnerPoint    { get { return Center.PlusPolarOffset(new (EndAzRads,   EndElRads,   InnerRadius)); } }
    public GloXYZPoint TopStartOuterPoint  { get { return Center.PlusPolarOffset(new (StartAzRads, EndElRads,   OuterRadius)); } }
    public GloXYZPoint TopEndOuterPoint    { get { return Center.PlusPolarOffset(new (EndAzRads,   EndElRads,   OuterRadius)); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYZSphereWedge() : this(GloXYZPoint.Zero, 0, 0, 0, 0, 0, 0) { }

    public GloXYZSphereWedge(
        GloXYZPoint center, double innerRadius, double outerRadius,
        double startAzRads, double deltaAzRads, double startElRads, double deltaElRads)
    {
        // Swap the inner and outer radii if they are the wrong way around
        if (innerRadius > outerRadius)
            (innerRadius, outerRadius) = (outerRadius, innerRadius);

        Center      = center;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        StartAzRads = startAzRads;
        DeltaAzRads = deltaAzRads;
        StartElRads = startElRads;
        DeltaElRads = deltaElRads;
    }

    public GloXYZSphereWedge(GloXYZSphereWedge wedge)
    {
        Center      = wedge.Center;
        InnerRadius = wedge.InnerRadius;
        OuterRadius = wedge.OuterRadius;
        StartAzRads = wedge.StartAzRads;
        DeltaAzRads = wedge.DeltaAzRads;
        StartElRads = wedge.StartElRads;
        DeltaElRads = wedge.DeltaElRads;
    }

    public static GloXYZSphereWedge Zero => new GloXYZSphereWedge(GloXYZPoint.Zero, 0, 0, 0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public GloXYZSphereWedge Offset(double x, double y, double z)
    {
        return new GloXYZSphereWedge(Center.Offset(x, y, z), InnerRadius, OuterRadius, StartAzRads, DeltaAzRads, StartElRads, DeltaElRads);
    }

    // // Check if a point is within the wedge by checking distance from the center, angles, and height.
    public bool Contains(GloXYZPolarOffset offset)
    {
        if (!AzRangeDegs.Contains(offset.AzDegs)) return false;
        if (!ElRangeDegs.Contains(offset.ElDegs)) return false;
        if (!RadiusRange.Contains(offset.Range))  return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
