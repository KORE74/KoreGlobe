using System;

namespace KoreCommon;

public struct KoreXYZPlane
{
    // Three points defining the plane
    public KoreXYZPoint PntOrigin { get; }
    public KoreXYZVector VecNormal { get; }
    public KoreXYZVector VecX { get; }
    public KoreXYZVector VecY { get; }

    // ---------------------------------------------------------------------------------------------
    // MARK: Constructors
    // ---------------------------------------------------------------------------------------------

    // Constructor
    public KoreXYZPlane(KoreXYZPoint pO, KoreXYZVector vN, KoreXYZVector vX, KoreXYZVector vY)
    {
        PntOrigin = pO;
        VecNormal = vN;
        VecX = vX;
        VecY = vY;
    }

    // ---------------------------------------------------------------------------------------------

    // Zero default constructor
    public static KoreXYZPlane Zero => new KoreXYZPlane(KoreXYZPoint.Zero, KoreXYZVector.Zero, KoreXYZVector.Zero, KoreXYZVector.Zero);

    // ---------------------------------------------------------------------------------------------

    // Given a normal and a Y (Up) axis, create the X axis and the plane.
    // Also normalises everything
    public static KoreXYZPlane MakePlane(KoreXYZPoint pO, KoreXYZVector vN, KoreXYZVector vY)
    {
        // Normalize the normal vector
        vN = vN.Normalize();
        vY = vY.Normalize();

        // Create the X axis vector, which is perpendicular to the normal and Y axis vectors
        KoreXYZVector vX = KoreXYZVector.CrossProduct(vN, vY);

        return new KoreXYZPlane(pO, vN, vX, vY);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Validation
    // ---------------------------------------------------------------------------------------------

    // Validate the inputs, make sure everything is perpendicular
    public bool IsValid()
    {
        // We can validate the plane, through the following criteria:
        // 1 - The normal vector and the X-axis and Y-axis vectors are all perpendicular to each other, resulting in a dot product of zero.

        double xAxisDotProduct = KoreXYZVector.DotProduct(VecNormal, VecX);
        double yAxisDotProduct = KoreXYZVector.DotProduct(VecNormal, VecY);
        double xyAxisDotProduct = KoreXYZVector.DotProduct(VecX, VecY);

        if (!KoreValueUtils.IsZero(xAxisDotProduct) ||
             !KoreValueUtils.IsZero(yAxisDotProduct) ||
             !KoreValueUtils.IsZero(xyAxisDotProduct))
            return false;

        return true;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: 2D Projection
    // ---------------------------------------------------------------------------------------------

    // take a 2D point, with reference to the plane, and project it to 3D
    public KoreXYZPoint Project2DTo3D(KoreXYPoint pnt2D)
    {
        // Convert the 2D point to 3D using the plane's axes and origin
        // X and Y are the local coordinates in the plane
        return PntOrigin
            .Offset(VecX.Scale(pnt2D.X))
            .Offset(VecY.Scale(pnt2D.Y));
    }

    // ---------------------------------------------------------------------------------------------

    // Take a 3D Point and project it to 2D - assuming any deviation from the 3D plane is parallel to the plane normal
    public KoreXYPoint Project3DTo2D(KoreXYZPoint pnt3D)
    {
        // Get the vector from the origin to the point
        KoreXYZVector vecToPoint = pnt3D.XYZTo(PntOrigin);

        // Project the vector onto the plane's X and Y axes
        double x = KoreXYZVector.DotProduct(vecToPoint, VecX);
        double y = KoreXYZVector.DotProduct(vecToPoint, VecY);

        return new KoreXYPoint(x, y);
    }

}
