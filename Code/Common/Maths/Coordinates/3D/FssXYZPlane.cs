using System;

public class FssXYZPlane : FssXYZ
{
    // Three points defining the plane
    public FssXYZPoint P1 { get; }
    public FssXYZPoint P2 { get; }
    public FssXYZPoint P3 { get; }

    // Constructor
    public FssXYZPlane(FssXYZPoint inP1, FssXYZPoint inP2, FssXYZPoint inP3)
    {
        // Check if the points are collinear
        if (FssXYZPlaneOperations.PointsCollinear(inP1, inP2, inP3))
            throw new ArgumentException("Points are collinear");

        P1 = inP1;
        P2 = inP2;
        P3 = inP3;
    }

    // Method to calculate the normal vector of the plane
    public FssXYZPoint Normal()
    {
        // Calculate the normal using cross product of vectors (P2-P1) and (P3-P1)
        return FssXYZPointOperations.CrossProduct(FssXYZPoint.Diff(P2, P1), FssXYZPoint.Diff(P3, P1));
    }

    // Method to calculate the distance of a point from a plane
    public static double DistanceFromPlane(FssXYZPoint point, FssXYZPlane plane)
    {
        // Calculate the normal vector of the plane
        FssXYZPoint planeNormal = FssXYZPointOperations.CrossProduct(FssXYZPoint.Diff(plane.P2, plane.P1), FssXYZPoint.Diff(plane.P3, plane.P1));

        // Normalize the normal vector
        double normalMagnitude = planeNormal.Magnitude;
        FssXYZPoint normalizedNormal = FssXYZPoint.Scale(planeNormal, 1 / normalMagnitude);

        // Calculate distance from the plane
        // Distance = DotProduct(normalizedNormal, (point - any point on the plane))
        return Math.Abs(FssXYZPointOperations.DotProduct(normalizedNormal, FssXYZPoint.Diff(point, plane.P1)));
    }


}
