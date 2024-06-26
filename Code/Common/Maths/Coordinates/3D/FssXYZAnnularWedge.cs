// FssXYZAnnularWedge: Class representing a 3D "wedge", based on an inner and outer radius, height, and start and end angles.
// Will have operations around the creation and manipulation of these wedges.

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.
// - Anything beyond the core responsibilities will be in a separate "operations" class.
// - Class will be immutable, as operations will return new instances.

public class FssXYZAnnularWedge : FssXYZ
{
    public FssXYZPoint Center { get; }
    public double InnerRadius { get; }
    public double OuterRadius { get; }
    public double StartAzimuthRads { get; }
    public double DeltaAzimuthRads { get; }
    public double StartElevationRads { get; }
    public double DeltaElevationRads { get; }
    public double Height { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes (increasing complexity order)
    // --------------------------------------------------------------------------------------------

    public double EndAzimuthRads           { get { return StartAzimuthRads + DeltaAzimuthRads; } }
    public double EndElevationRads         { get { return StartElevationRads + DeltaElevationRads; } }
      
    public double StartAzimuthDegs         { get { return FssValueUtils.RadsToDegs(StartAzimuthRads); } }
    public double DeltaAzimuthDegs         { get { return FssValueUtils.RadsToDegs(DeltaAzimuthRads); } }
    public double EndAzimuthDegs           { get { return FssValueUtils.RadsToDegs(EndAzimuthRads); } }
          
    public double StartElevationDegs       { get { return FssValueUtils.RadsToDegs(StartElevationRads); } }
    public double DeltaElevationDegs       { get { return FssValueUtils.RadsToDegs(DeltaElevationRads); } }
    public double EndElevationDegs         { get { return FssValueUtils.RadsToDegs(EndElevationRads); } }
      
    public double AzimuthSpanRads          { get { return FssValueUtils.AngleDiffRads(StartAzimuthRads, EndAzimuthRads); } }
    public double AzimuthSpanDegs          { get { return FssValueUtils.RadsToDegs(AzimuthSpanRads); } }
          
    public double ElevationSpanRads        { get { return FssValueUtils.AngleDiffRads(StartElevationRads, EndElevationRads); } }
    public double ElevationSpanDegs        { get { return FssValueUtils.RadsToDegs(ElevationSpanRads); } }

    // Points at the base (z = 0) and top (z = height) of the wedge
    public FssXYZPoint BaseStartInnerPoint { get { return FssXYZPointOperations.OffsetAzEl(Center, InnerRadius, StartAzimuthRads, StartElevationRads); } }
    public FssXYZPoint BaseEndInnerPoint   { get { return FssXYZPointOperations.OffsetAzEl(Center, InnerRadius, EndAzimuthRads,   StartElevationRads); } }
    public FssXYZPoint BaseStartOuterPoint { get { return FssXYZPointOperations.OffsetAzEl(Center, OuterRadius, StartAzimuthRads, StartElevationRads); } }
    public FssXYZPoint BaseEndOuterPoint   { get { return FssXYZPointOperations.OffsetAzEl(Center, OuterRadius, EndAzimuthRads,   StartElevationRads); } }

    public FssXYZPoint TopStartInnerPoint  { get { return FssXYZPointOperations.OffsetAzEl(Center, InnerRadius, StartAzimuthRads, EndElevationRads).Offset(0, 0, Height); } }
    public FssXYZPoint TopEndInnerPoint    { get { return FssXYZPointOperations.OffsetAzEl(Center, InnerRadius, EndAzimuthRads,   EndElevationRads).Offset(0, 0, Height); } }
    public FssXYZPoint TopStartOuterPoint  { get { return FssXYZPointOperations.OffsetAzEl(Center, OuterRadius, StartAzimuthRads, EndElevationRads).Offset(0, 0, Height); } }
    public FssXYZPoint TopEndOuterPoint    { get { return FssXYZPointOperations.OffsetAzEl(Center, OuterRadius, EndAzimuthRads,   EndElevationRads).Offset(0, 0, Height); } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZAnnularWedge(FssXYZPoint center, double innerRadius, double outerRadius, double startAzimuthRads, double deltaAzimuthRads, double startElevationRads, double deltaElevationRads, double height)
    {
        // Swap the inner and outer radii if they are the wrong way around
        if (innerRadius > outerRadius)
            (innerRadius, outerRadius) = (outerRadius, innerRadius);

        Center             = center;
        InnerRadius        = innerRadius;
        OuterRadius        = outerRadius;
        StartAzimuthRads   = startAzimuthRads;
        DeltaAzimuthRads   = deltaAzimuthRads;
        StartElevationRads = startElevationRads;
        DeltaElevationRads = deltaElevationRads;
        Height             = height;
    }

    public FssXYZAnnularWedge(FssXYZAnnularWedge wedge)
    {
        Center             = wedge.Center;
        InnerRadius        = wedge.InnerRadius;
        OuterRadius        = wedge.OuterRadius;
        StartAzimuthRads   = wedge.StartAzimuthRads;
        DeltaAzimuthRads   = wedge.DeltaAzimuthRads;
        StartElevationRads = wedge.StartElevationRads;
        DeltaElevationRads = wedge.DeltaElevationRads;
        Height             = wedge.Height;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYZAnnularWedge Offset(double x, double y, double z)
    {
        return new FssXYZAnnularWedge(Center.Offset(x, y, z), InnerRadius, OuterRadius, StartAzimuthRads, DeltaAzimuthRads, StartElevationRads, DeltaElevationRads, Height);
    }

    // // Check if a point is within the wedge by checking distance from the center, angles, and height.
    // public bool Contains(FssXYZPoint checkPos)
    // {
    //     // Check if the point is within the base and top planes
    //     bool isWithinHeight = checkPos.Z >= Center.Z && checkPos.Z <= (Center.Z + Height);
    //     if (!isWithinHeight)
    //         return false;

    //     // Containing the point is three checks:
    //     // 1 - It's inside the outer radius
    //     // 2 - It's outside the inner radius
    //     // 3 - It's within the start and end azimuth angles
    //     bool isInsideOuterRadius = OuterRadius >= Center.DistanceTo(checkPos);
    //     if (!isInsideOuterRadius)
    //         return false;

    //     bool isOutsideInnerRadius = InnerRadius <= Center.DistanceTo(checkPos);
    //     if (!isOutsideInnerRadius)
    //         return false;

    //     bool isWithinAzimuthAngles = FssValueUtils.IsAngleBetween(StartAzimuthRads, EndAzimuthRads, Center.AzimuthToRads(checkPos));
    //     if (!isWithinAzimuthAngles)
    //         return false;

    //     bool isWithinElevationAngles = FssValueUtils.IsAngleBetween(StartElevationRads, EndElevationRads, Center.ElevationToRads(checkPos));
    //     if (!isWithinElevationAngles)
    //         return false;

    //     return true;
    // }


    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // Implement operator overloads if necessary
}
