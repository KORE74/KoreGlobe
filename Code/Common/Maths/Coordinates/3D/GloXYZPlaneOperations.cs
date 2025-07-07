using System;

#nullable enable

public static class GloXYZPlaneOperations
{
    // Check if three points are colinear
    public static bool PointsCollinear(GloXYZPoint p1, GloXYZPoint p2, GloXYZPoint p3)
    {
        // Validate inputs to avoid undefined lines (e.g., same start and end points)
        if (p1.Equals(p2) || p2.Equals(p3) || p1.Equals(p3))
        {
            throw new ArgumentException("Points must be distinct to determine colinearity.");
        }

        // Create two lines, then check if they are parallel
        var line1 = new GloXYZLine(p1, p2);
        var line2 = new GloXYZLine(p1, p3);

        return GloXYZLineOperations.IsParallel(line1, line2);
    }

    // Find the intersection point of a line with a plane, if it exists
    // public static GloXYZPoint? IntersectionWithLine(GloXYZPlane plane, GloXYZLine line)
    // {
    //     GloXYZVector planeNormal = plane.VecNormal;

    //     // Calculate the direction vector of the line
    //     GloXYZVector lineDir = line.P1.VectorTo(line.P2);

    //     // Calculate the dot product of the line direction and the plane normal
    //     double dotProduct = GloXYZVector.DotProduct(planeNormal, lineDir);

    //     // Check if the line is parallel to the plane
    //     if (Math.Abs(dotProduct) < GloConsts.ArbitraryMinDouble)
    //     {
    //         // Special case handling: Check if the line lies in the plane
    //         if (Math.Abs(GloXYZVector.DotProduct(planeNormal, plane.PntOrigin.VectorTo(line.P1))) < GloConsts.ArbitraryMinDouble)
    //         {
    //             // The line lies in the plane
    //             return line.P1; // or any point on the line
    //         }

    //         return null; // No intersection, line is parallel to the plane
    //     }

    //     // Calculate the parameter t at the intersection point
    //     double t = GloXYZPointOperations.DotProduct(planeNormal, GloXYZPoint.Diff(plane.P1, line.P1)) / dotProduct;

    //     // Calculate and return the intersection point
    //     return GloXYZPoint.Sum(line.P1, GloXYZPoint.Scale(lineDir, t));
    // }
}
