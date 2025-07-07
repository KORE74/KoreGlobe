// GloXYZArc: Class representing a 3D arc, based on an origin point, start azimuth and elevation, and deltas for azimuth and elevation.

// Design Decisions:
// - Zero azimuth angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Elevation angle is zero on the horizontal plane and increases upwards.
// - Class will be immutable, as operations will return new instances.

public class GloXYZArc : GloXYZ
{
    public GloXYZPoint Origin        { get; }
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

    public double StartAzimuthDegs   { get { return GloValueUtils.RadsToDegs(StartAzimuthRads); } }
    public double DeltaAzimuthDegs   { get { return GloValueUtils.RadsToDegs(DeltaAzimuthRads); } }
    public double EndAzimuthDegs     { get { return GloValueUtils.RadsToDegs(EndAzimuthRads); } }

    public double StartElevationDegs { get { return GloValueUtils.RadsToDegs(StartElevationRads); } }
    public double DeltaElevationDegs { get { return GloValueUtils.RadsToDegs(DeltaElevationRads); } }
    public double EndElevationDegs   { get { return GloValueUtils.RadsToDegs(EndElevationRads); } }

    public double AzimuthSpanRads    { get { return GloValueUtils.AngleDiffRads(StartAzimuthRads, EndAzimuthRads); } }
    public double AzimuthSpanDegs    { get { return GloValueUtils.RadsToDegs(AzimuthSpanRads); } }

    public double ElevationSpanRads  { get { return GloValueUtils.AngleDiffRads(StartElevationRads, EndElevationRads); } }
    public double ElevationSpanDegs  { get { return GloValueUtils.RadsToDegs(ElevationSpanRads); } }

    public GloXYZPoint StartPoint    { get { return GloXYZPointOperations.OffsetAzEl(Origin, Radius, StartAzimuthRads, StartElevationRads); } }
    public GloXYZPoint EndPoint      { get { return GloXYZPointOperations.OffsetAzEl(Origin, Radius, EndAzimuthRads,   EndElevationRads); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYZArc(GloXYZPoint origin, double radius, double startAzimuthRads, double deltaAzimuthRads, double startElevationRads, double deltaElevationRads)
    {
        Origin             = origin;
        Radius             = radius;
        StartAzimuthRads   = startAzimuthRads;
        DeltaAzimuthRads   = deltaAzimuthRads;
        StartElevationRads = startElevationRads;
        DeltaElevationRads = deltaElevationRads;
    }

    public GloXYZArc(GloXYZArc arc)
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

    public GloXYZArc Offset(double x, double y, double z)
    {
        return new GloXYZArc(Origin.Offset(x, y, z), Radius, StartAzimuthRads, DeltaAzimuthRads, StartElevationRads, DeltaElevationRads);
    }

    // return the angle between the two points in the plane of the points, in radians. Essentially a 2D angle and a 2D cross product.
    public double Angle()
    {
        return GloXYZPointOperations.AngleBetweenRads(Origin, StartPoint, EndPoint);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
