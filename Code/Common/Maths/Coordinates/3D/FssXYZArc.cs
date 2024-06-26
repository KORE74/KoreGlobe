// FssXYZArc: Class representing a 3D arc, based on an origin point, start azimuth and elevation, and deltas for azimuth and elevation.

// Design Decisions:
// - Zero azimuth angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Elevation angle is zero on the horizontal plane and increases upwards.
// - Class will be immutable, as operations will return new instances.

public class FssXYZArc : FssXYZ
{
    public FssXYZPoint Origin        { get; }
    public double Radius             { get; }
    public double StartAzimuthRads   { get; }
    public double StartElevationRads { get; }
    public double DeltaAzimuthRads   { get; }
    public double DeltaElevationRads { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double EndAzimuthRads     { get { return StartAzimuthRads + DeltaAzimuthRads; } }
    public double EndElevationRads   { get { return StartElevationRads + DeltaElevationRads; } }

    public double StartAzimuthDegs   { get { return FssValueUtils.RadsToDegs(StartAzimuthRads); } }
    public double DeltaAzimuthDegs   { get { return FssValueUtils.RadsToDegs(DeltaAzimuthRads); } } 
    public double EndAzimuthDegs     { get { return FssValueUtils.RadsToDegs(EndAzimuthRads); } }
    
    public double StartElevationDegs { get { return FssValueUtils.RadsToDegs(StartElevationRads); } }
    public double DeltaElevationDegs { get { return FssValueUtils.RadsToDegs(DeltaElevationRads); } }
    public double EndElevationDegs   { get { return FssValueUtils.RadsToDegs(EndElevationRads); } }

    public double AzimuthSpanRads    { get { return FssValueUtils.AngleDiffRads(StartAzimuthRads, EndAzimuthRads); } }
    public double AzimuthSpanDegs    { get { return FssValueUtils.RadsToDegs(AzimuthSpanRads); } }
    
    public double ElevationSpanRads  { get { return FssValueUtils.AngleDiffRads(StartElevationRads, EndElevationRads); } }
    public double ElevationSpanDegs  { get { return FssValueUtils.RadsToDegs(ElevationSpanRads); } }

    public FssXYZPoint StartPoint    { get { return FssXYZPointOperations.OffsetAzEl(Origin, Radius, StartAzimuthRads, StartElevationRads); } }
    public FssXYZPoint EndPoint      { get { return FssXYZPointOperations.OffsetAzEl(Origin, Radius, EndAzimuthRads,   EndElevationRads); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZArc(FssXYZPoint origin, double radius, double startAzimuthRads, double deltaAzimuthRads, double startElevationRads, double deltaElevationRads)
    {
        Origin             = origin;
        Radius             = radius;
        StartAzimuthRads   = startAzimuthRads;
        DeltaAzimuthRads   = deltaAzimuthRads;
        StartElevationRads = startElevationRads;
        DeltaElevationRads = deltaElevationRads;
    }

    public FssXYZArc(FssXYZArc arc)
    {
        Origin             = arc.Origin;
        Radius             = arc.Radius;
        StartAzimuthRads   = arc.StartAzimuthRads;
        DeltaAzimuthRads   = arc.DeltaAzimuthRads;
        StartElevationRads = arc.StartElevationRads;
        DeltaElevationRads = arc.DeltaElevationRads;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYZArc Offset(double x, double y, double z)
    {
        return new FssXYZArc(Origin.Offset(x, y, z), Radius, StartAzimuthRads, DeltaAzimuthRads, StartElevationRads, DeltaElevationRads);
    }

    // return the angle between the two points in the plane of the points, in radians. Essentially a 2D angle and a 2D cross product.
    public double Angle()
    {
        return FssXYZPointOperations.AngleBetweenRads(Origin, StartPoint, EndPoint);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
