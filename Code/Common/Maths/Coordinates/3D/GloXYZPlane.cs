using System;

public struct GloXYZPlane
{
    // Three points defining the plane
    public GloXYZPoint  PntOrigin { get; }
    public GloXYZVector VecNormal { get; }
    public GloXYZVector VecX      { get; }
    public GloXYZVector VecY      { get; }

    // ---------------------------------------------------------------------------------------------
    // MARK: Constructors
    // ---------------------------------------------------------------------------------------------

    // Constructor
    public GloXYZPlane(GloXYZPoint pO, GloXYZVector vN, GloXYZVector vX, GloXYZVector vY)
    {
        PntOrigin = pO;
        VecNormal = vN;
        VecX      = vX;
        VecY      = vY;
    }

    // ---------------------------------------------------------------------------------------------

    // Zero default constructor
    public static GloXYZPlane Zero => new GloXYZPlane(GloXYZPoint.Zero, GloXYZVector.Zero, GloXYZVector.Zero, GloXYZVector.Zero);

    // ---------------------------------------------------------------------------------------------

    // Given a normal and a Y (Up) axis, create the X axis and the plane.
    // Also normalises everything
    public static GloXYZPlane MakePlane(GloXYZPoint pO, GloXYZVector vN, GloXYZVector vY)
    {
        // Normalize the normal vector
        vN = vN.Normalize();
        vY = vY.Normalize();

        // Create the X axis vector, which is perpendicular to the normal and Y axis vectors
        GloXYZVector vX = GloXYZVector.CrossProduct(vN, vY);

        return new GloXYZPlane(pO, vN, vX, vY);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Validation
    // ---------------------------------------------------------------------------------------------

    // Validate the inputs, make sure everything is perpendicular
    public bool IsValid()
    {
        // We can validate the plane, through the following criteria:
        // 1 - The normal vector and the X-axis and Y-axis vectors are all perpendicular to each other, resulting in a dot product of zero.

        double xAxisDotProduct  = GloXYZVector.DotProduct(VecNormal, VecX);
        double yAxisDotProduct  = GloXYZVector.DotProduct(VecNormal, VecY);
        double xyAxisDotProduct = GloXYZVector.DotProduct(VecX, VecY);

        if ( !GloValueUtils.IsZero(xAxisDotProduct) ||
             !GloValueUtils.IsZero(yAxisDotProduct) ||
             !GloValueUtils.IsZero(xyAxisDotProduct))
            return false;

        return true;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: 2D Projection
    // ---------------------------------------------------------------------------------------------

    // take a 2D point, with reference to the plane, and project it to 3D
    public GloXYZPoint Project2DTo3D(GloXYPoint pnt2D)
    {
        // Convert the 2D point to 3D using the plane's axes and origin
        // X and Y are the local coordinates in the plane
        return PntOrigin
            .Offset(VecX.Scale(pnt2D.X))
            .Offset(VecY.Scale(pnt2D.Y));
    }

    // ---------------------------------------------------------------------------------------------

    // Take a 3D Point and project it to 2D - assuming any deviation from the 3D plane is parallel to the plane normal
    public GloXYPoint Project3DTo2D(GloXYZPoint pnt3D)
    {
        // Get the vector from the origin to the point
        GloXYZVector vecToPoint = pnt3D.XYZTo(PntOrigin);

        // Project the vector onto the plane's X and Y axes
        double x = GloXYZVector.DotProduct(vecToPoint, VecX);
        double y = GloXYZVector.DotProduct(vecToPoint, VecY);

        return new GloXYPoint(x, y);
    }

}
