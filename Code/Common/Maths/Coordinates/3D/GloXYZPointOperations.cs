using System;

public static class GloXYZPointOperations
{
    // Dot Product - the cosine of the angle between the two vectors
    // Considering both as lines from 0,0 to the points, this is the cosine of the angle between them
    // near +1 means the angle is near 0 degrees (absolute to remove sign)
    // wiki - https://en.wikipedia.org/wiki/Dot_product

    public static double DotProduct(GloXYZPoint a, GloXYZPoint b)
    {
        return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
    }

    // Cross Product - the vector perpendicular to both input vectors
    // wiki - https://en.wikipedia.org/wiki/Cross_product

    public static GloXYZPoint CrossProduct(GloXYZPoint a, GloXYZPoint b)
    {
        double x = (a.Y * b.Z) - (a.Z * b.Y);
        double y = (a.Z * b.X) - (a.X * b.Z);
        double z = (a.X * b.Y) - (a.Y * b.X);

        return new GloXYZPoint(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    public static double AngleBetweenRads(GloXYZPoint a, GloXYZPoint b)
    {
        double dotProduct = DotProduct(a, b);
        double magA = a.Magnitude;
        double magB = b.Magnitude;

        double cosAngle = dotProduct / (magA * magB);

        // Handle edge cases
        if (cosAngle > 1.0)
            cosAngle = 1.0;
        else if (cosAngle < -1.0)
            cosAngle = -1.0;

        return Math.Acos(cosAngle);
    }

    // Get the angle between lines origin-A and origin-B
    public static double AngleBetweenRads(GloXYZPoint origin, GloXYZPoint a, GloXYZPoint b)
    {
        GloXYZPoint vecA = a - origin;
        GloXYZPoint vecB = b - origin;

        return AngleBetweenRads(vecA, vecB);
    }

    // --------------------------------------------------------------------------------------------

    // usage: GloXYZPoint a = new GloXYZPoint(1, 2, 3);
    //        GloXYZPoint c = GloXYZPointOperations.OffsetAzEl(a, 10, GloValueUtils.DegsToRads(45), GloValueUtils.DegsToRads(30));

    public static GloXYZPoint OffsetAzEl(GloXYZPoint origin, double radius, double azimuthRads, double elevationRads)
    {
        double x = origin.X + radius * Math.Cos(azimuthRads) * Math.Cos(elevationRads);
        double y = origin.Y + radius * Math.Sin(azimuthRads) * Math.Cos(elevationRads);
        double z = origin.Z + radius * Math.Sin(elevationRads);

        return new GloXYZPoint(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    // Usage: GloXYZPoint c = GloXYZPointOperations.Lerp(from, to, 0.1);

    public static GloXYZPoint Lerp(GloXYZPoint fromPoint, GloXYZPoint toPoint, double fraction)
    {
        // Handle edge cases
        if (fraction <= 0.0) return fromPoint;
        if (fraction >= 1.0) return toPoint;

        // Calculate the interpolated point
        double x = fromPoint.X + (toPoint.X - fromPoint.X) * fraction;
        double y = fromPoint.Y + (toPoint.Y - fromPoint.Y) * fraction;
        double z = fromPoint.Z + (toPoint.Z - fromPoint.Z) * fraction;

        return new GloXYZPoint(x, y, z);
    }

    // --------------------------------------------------------------------------------------------

    public static GloXYZPoint Slerp(GloXYZPoint a, GloXYZPoint b, double fraction)
    {
        // Handle edge cases
        if (fraction <= 0.0) return a;
        if (fraction >= 1.0) return b;

        // Normalize input points (assuming they are not already normalized)
        a = a.Normalize();
        b = b.Normalize();

        // Calculate the angle between the two points
        double angle = AngleBetweenRads(a, b);

        if (Math.Abs(angle) < GloConsts.ArbitraryMinDouble)
            return a;  // The points are colinear

        // Compute the Slerp
        double sinAngle = Math.Sin(angle);
        double sinFractionAngle = Math.Sin(fraction * angle);
        double sinComplementAngle = Math.Sin((1 - fraction) * angle);

        // Calculate the interpolated point
        GloXYZPoint interpolated = (a * sinComplementAngle + b * sinFractionAngle) / sinAngle;

        return interpolated;
    }

    // --------------------------------------------------------------------------------------------

    public static GloXYZPoint InsetPoint(GloXYZPoint a, GloXYZPoint b, GloXYZPoint c, double t)
    {
        // Calculate direction vectors for AB and BC, including the Z dimension
        double dxAB = b.X - a.X;
        double dyAB = b.Y - a.Y;
        double dzAB = b.Z - a.Z; // Include Z dimension
        double dxBC = c.X - b.X;
        double dyBC = c.Y - b.Y;
        double dzBC = c.Z - b.Z; // Include Z dimension

        // Normalize direction vectors
        double magAB = Math.Sqrt(dxAB * dxAB + dyAB * dyAB + dzAB * dzAB); // Include Z dimension
        double magBC = Math.Sqrt(dxBC * dxBC + dyBC * dyBC + dzBC * dzBC); // Include Z dimension
        dxAB /= magAB;
        dyAB /= magAB;
        dzAB /= magAB; // Normalize Z component
        dxBC /= magBC;
        dyBC /= magBC;
        dzBC /= magBC; // Normalize Z component

        // Calculate bisector vector, including the Z dimension
        double bisectorX = dxAB + dxBC;
        double bisectorY = dyAB + dyBC;
        double bisectorZ = dzAB + dzBC; // Include Z dimension
        double magBisector = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY + bisectorZ * bisectorZ); // Include Z dimension
        bisectorX /= magBisector;
        bisectorY /= magBisector;
        bisectorZ /= magBisector; // Normalize Z component

        // Calculate inset point along the bisector, including the Z dimension
        double xInset = b.X + t * bisectorX;
        double yInset = b.Y + t * bisectorY;
        double zInset = b.Z + t * bisectorZ; // Include Z dimension

        return new GloXYZPoint(xInset, yInset, zInset);
    }

    // --------------------------------------------------------------------------------------------

    public static GloXYZPoint RotateAboutAxis(GloXYZPoint point, GloXYZPoint axis, double angleRads)
    {
        // Normalize the axis vector
        double axisLength = Math.Sqrt(axis.X * axis.X + axis.Y * axis.Y + axis.Z * axis.Z);
        if (axisLength == 0) throw new ArgumentException("Axis cannot be a zero vector.", nameof(axis));
        double axisX = axis.X / axisLength;
        double axisY = axis.Y / axisLength;
        double axisZ = axis.Z / axisLength;

        // Compute the cosine and sine of the angle
        double cosAngle = Math.Cos(angleRads);
        double sinAngle = Math.Sin(angleRads);

        // Compute the rotated coordinates
        double newX = (cosAngle + (1 - cosAngle) * axisX * axisX) * point.X
                    + ((1 - cosAngle) * axisX * axisY - axisZ * sinAngle) * point.Y
                    + ((1 - cosAngle) * axisX * axisZ + axisY * sinAngle) * point.Z;

        double newY = ((1 - cosAngle) * axisY * axisX + axisZ * sinAngle) * point.X
                    + (cosAngle + (1 - cosAngle) * axisY * axisY) * point.Y
                    + ((1 - cosAngle) * axisY * axisZ - axisX * sinAngle) * point.Z;

        double newZ = ((1 - cosAngle) * axisZ * axisX - axisY * sinAngle) * point.X
                    + ((1 - cosAngle) * axisZ * axisY + axisX * sinAngle) * point.Y
                    + (cosAngle + (1 - cosAngle) * axisZ * axisZ) * point.Z;

        return new GloXYZPoint(newX, newY, newZ);
    }

}
