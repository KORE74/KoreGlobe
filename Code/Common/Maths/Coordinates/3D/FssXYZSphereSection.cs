// FssXYZSphereWedge: Class representing a 3D "wedge", based on an inner and outer radius, height, and start and end angles.
// Will have operations around the creation and manipulation of these wedges.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responsibilities will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class FssXYZSphereSection : FssXYZ
{
    public FssXYZPoint Center { get; set; }
    public double Radius { get; set; }
    public double StartAzRads { get; set; } // Using start and delta (clockwise) angles to better accomodate wrapping
    public double StartElRads { get; set; }
    public double DeltaAzRads { get; set; }
    public double DeltaElRads { get; set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double StartAzDegs
    {
         get { return FssValueUtils.RadsToDegs(StartAzRads); }
         set { StartAzRads = FssValueUtils.DegsToRads(value); }
    }
    public double DeltaAzDegs
    {
        get { return FssValueUtils.RadsToDegs(DeltaAzRads); }
        set { DeltaAzRads = FssValueUtils.DegsToRads(value); }
    }
    public double StartElDegs
    {
        get { return FssValueUtils.RadsToDegs(StartElRads); }
        set { StartElRads = FssValueUtils.DegsToRads(value); }
    }
    public double DeltaElDegs
    {
        get { return FssValueUtils.RadsToDegs(DeltaElRads); }
        set { DeltaElRads = FssValueUtils.DegsToRads(value); }
    }

    public double EndAzRads         { get { return StartAzRads + DeltaAzRads; } }
    public double EndAzDegs         { get { return FssValueUtils.RadsToDegs(EndAzRads); } }

    public double EndElRads         { get { return StartElRads + DeltaElRads; } }
    public double EndElDegs         { get { return FssValueUtils.RadsToDegs(EndElRads); } }

    public FssDoubleRange AzRangeDegs      { get { return new FssDoubleRange(StartAzDegs, EndAzDegs); } }
    public FssDoubleRange ElRangeDegs      { get { return new FssDoubleRange(StartElDegs, EndElDegs); } }
    public FssDoubleRange AzRangeRads      { get { return new FssDoubleRange(StartAzRads, EndAzRads); } }
    public FssDoubleRange ElRangeRads      { get { return new FssDoubleRange(StartElRads, EndElRads); } }

    public FssXYZPoint BaseStartPoint { get { return Center.PlusPolarOffset(new (StartAzRads, StartElRads, Radius)); } }
    public FssXYZPoint BaseEndPoint   { get { return Center.PlusPolarOffset(new (EndAzRads,   StartElRads, Radius)); } }
    public FssXYZPoint TopStartPoint  { get { return Center.PlusPolarOffset(new (StartAzRads, EndElRads,   Radius)); } }
    public FssXYZPoint TopEndPoint    { get { return Center.PlusPolarOffset(new (EndAzRads,   EndElRads,   Radius)); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZSphereWedge() : this(FssXYZPoint.Zero, 0, 0, 0, 0, 0) { }

    public FssXYZSphereWedge(
        FssXYZPoint center, double radius,
        double startAzRads, double deltaAzRads, double startElRads, double deltaElRads)
    {
        Center      = center;
        InnerRadius = radius;
        StartAzRads = startAzRads;
        DeltaAzRads = deltaAzRads;
        StartElRads = startElRads;
        DeltaElRads = deltaElRads;
    }

    public FssXYZSphereWedge(FssXYZSphereWedge wedge)
    {
        Center      = wedge.Center;
        InnerRadius = wedge.Radius;
        StartAzRads = wedge.StartAzRads;
        DeltaAzRads = wedge.DeltaAzRads;
        StartElRads = wedge.StartElRads;
        DeltaElRads = wedge.DeltaElRads;
    }

    public static FssXYZSphereWedge Zero => new FssXYZSphereWedge(FssXYZPoint.Zero, 0, 0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYZSphereWedge Offset(double x, double y, double z)
    {
        return new FssXYZSphereWedge(Center.Offset(x, y, z), Radius, StartAzRads, DeltaAzRads, StartElRads, DeltaElRads);
    }

    // // Check if a point is within the wedge by checking distance from the center, angles, and height.
    public bool Contains(FssXYZPolarOffset offset)
    {
        if (!AzRangeDegs.Contains(offset.AzDegs)) return false;
        if (!ElRangeDegs.Contains(offset.ElDegs)) return false;

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
