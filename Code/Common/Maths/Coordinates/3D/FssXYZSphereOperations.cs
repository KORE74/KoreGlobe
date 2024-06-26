using System;

#nullable enable

public static class FssXYZSphereOperations
{
    // Determine if a line intersects with the boundary of a sphere. A line entirely within the sphere is not considered an intersection.

    public static bool LineIntersectsSphere(
        FssXYZLine line, 
        FssXYZSphere sphere, 
        out FssXYZPoint? intersection1, 
        out FssXYZPoint? intersection2)
    {
        intersection1 = null;
        intersection2 = null;

        // Compute line direction and the vector from sphere center to line start
        FssXYZPoint lineDir = line.Direction;
        FssXYZPoint sphereToLineStart = line.P1 - sphere.Center;

        // Calculate coefficients of the quadratic equation
        double a = FssXYZPointOperations.DotProduct(lineDir, lineDir);
        double b = 2 * FssXYZPointOperations.DotProduct(sphereToLineStart, lineDir);
        double c = FssXYZPointOperations.DotProduct(sphereToLineStart, sphereToLineStart) - (sphere.Radius * sphere.Radius);

        // Calculate the discriminant
        double discriminant = b * b - 4 * a * c;

        if (discriminant < 0)
        {
            // No intersection
            return false;
        }
        else
        {
            // Calculate the two intersection t values
            double t1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
            double t2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

            // Find intersection points
            intersection1 = line.P1 + t1 * lineDir;
            intersection2 = (discriminant == 0) ? intersection1 : line.P1 + t2 * lineDir;

            return true;
        }
    }
}