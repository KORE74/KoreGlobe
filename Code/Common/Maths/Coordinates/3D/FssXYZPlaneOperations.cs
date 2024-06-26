using System;

#nullable enable

public static class FssXYZPlaneOperations
{
    // Check if three points are colinear
    public static bool PointsCollinear(FssXYZPoint p1, FssXYZPoint p2, FssXYZPoint p3)
    {
        // Validate inputs to avoid undefined lines (e.g., same start and end points)
        if (p1.Equals(p2) || p2.Equals(p3) || p1.Equals(p3))
        {
            throw new ArgumentException("Points must be distinct to determine colinearity.");
        }

        // Create two lines, then check if they are parallel
        var line1 = new FssXYZLine(p1, p2);
        var line2 = new FssXYZLine(p1, p3);

        return FssXYZLineOperations.IsParallel(line1, line2);
    }

    // Find the intersection point of a line with a plane, if it exists
    public static FssXYZPoint? IntersectionWithLine(FssXYZPlane plane, FssXYZLine line)
    {
        FssXYZPoint planeNormal = plane.Normal();

        // Calculate the direction vector of the line
        FssXYZPoint lineDir = FssXYZPoint.Diff(line.P2, line.P1);

        // Calculate the dot product of the line direction and the plane normal
        double dotProduct = FssXYZPointOperations.DotProduct(planeNormal, lineDir);

        // Check if the line is parallel to the plane
        if (Math.Abs(dotProduct) < FssConsts.ArbitraryMinDouble)
        {
            // Special case handling: Check if the line lies in the plane
            if (Math.Abs(FssXYZPointOperations.DotProduct(planeNormal, FssXYZPoint.Diff(line.P1, plane.P1))) < FssConsts.ArbitraryMinDouble)
            {
                // The line lies in the plane
                return line.P1; // or any point on the line
            }

            return null; // No intersection, line is parallel to the plane
        }

        // Calculate the parameter t at the intersection point
        double t = FssXYZPointOperations.DotProduct(planeNormal, FssXYZPoint.Diff(plane.P1, line.P1)) / dotProduct;

        // Calculate and return the intersection point
        return FssXYZPoint.Sum(line.P1, FssXYZPoint.Scale(lineDir, t));
    }
}
