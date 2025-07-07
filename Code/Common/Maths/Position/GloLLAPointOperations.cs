using System;
using System.Collections.Generic;

// GloLLAPointOperations: A static class to hold operations on GloLLAPoint objects that are not part of its
// core responsibilites. This class is static, all operations are stateless and return a new object.

// Design Decisions:
// - The LLA Point code uses a struct rather than an immutable class, as the constructor options with flexible units
//   are simply too useful. We rely on the struct's pass by value to avoid issues with mutability.

public static class GloLLAPointOperations
{
    public static GloLLAPoint RhumbLineInterpolation(GloLLAPoint fromPoint, GloLLAPoint toPoint, double fraction)
    {
        // This is a simple linear interpolation of the lat and lon, and a weighted average of the altitudes.
        // The fraction is the fraction of the distance from the fromPoint to the toPoint.

        double latDegs = fromPoint.LatDegs + (toPoint.LatDegs - fromPoint.LatDegs) * fraction;
        double lonDegs = fromPoint.LonDegs + (toPoint.LonDegs - fromPoint.LonDegs) * fraction;
        double alt     = fromPoint.AltMslM + (toPoint.AltMslM - fromPoint.AltMslM) * fraction;

        return new GloLLAPoint() { LatDegs = latDegs, LonDegs = lonDegs, AltMslM = alt };
    }

    // --------------------------------------------------------------------------------------------

    public static List<GloLLAPoint> DividedRhumbLine(GloLLAPoint fromPoint, GloLLAPoint toPoint, int returnedListCount)
    {
        // Ensure the list count is at least 2, as the list needs to include both endpoints.
        if (returnedListCount < 2)
        {
            throw new ArgumentException("returnedListCount must be 2 or greater.");
        }

        List<GloLLAPoint> result = new List<GloLLAPoint>();

        // Add the first point (fromPoint)
        result.Add(fromPoint);

        // Calculate the step fraction for each interpolation point
        double stepFraction = 1.0 / (returnedListCount - 1);

        // Interpolate points along the rhumb line
        for (int i = 1; i < returnedListCount - 1; i++)
        {
            double fraction = i * stepFraction;
            GloLLAPoint interpolatedPoint = RhumbLineInterpolation(fromPoint, toPoint, fraction);
            result.Add(interpolatedPoint);
        }

        // Add the last point (toPoint)
        result.Add(toPoint);

        return result;
    }

    // --------------------------------------------------------------------------------------------

    // Usage: List<GloLLAPoint> pointList = GloLLAPointOperations.DividedGCLine(fromPoint, toPoint, 10)

    public static List<GloLLAPoint> DividedGCLine(GloLLAPoint fromPoint, GloLLAPoint toPoint, int returnedListCount)
    {
        // Ensure the list count is at least 2, as the list needs to include both endpoints.
        if (returnedListCount < 2)
        {
            throw new ArgumentException("returnedListCount must be 2 or greater.");
        }

        List<GloLLAPoint> result = new List<GloLLAPoint>();

        // Add the first point (fromPoint)
        result.Add(fromPoint);

        // Calculate the step fraction for each interpolation point
        double stepFraction = 1.0 / (returnedListCount - 1);

        // Interpolate points along the rhumb line
        for (int i = 1; i < returnedListCount - 1; i++)
        {
            double fraction = i * stepFraction;
            GloLLAPoint interpolatedPoint = GreatCircleInterpolation(fromPoint, toPoint, fraction);
            result.Add(interpolatedPoint);
        }

        // Add the last point (toPoint)
        result.Add(toPoint);

        return result;
    }

    // --------------------------------------------------------------------------------------------

    public static GloLLAPoint StraightLineInterpolation(GloLLAPoint fromPoint, GloLLAPoint toPoint, double fraction)
    {
        GloXYZPoint fromXYZ = fromPoint.ToXYZ();
        GloXYZPoint toXYZ   = toPoint.ToXYZ();
        GloXYZPoint interpXYZ = GloXYZPointOperations.Lerp(fromXYZ, toXYZ, fraction);

        return GloLLAPoint.FromXYZ(interpXYZ);
    }

    // --------------------------------------------------------------------------------------------

    public static GloLLAPoint GreatCircleInterpolation(GloLLAPoint fromPoint, GloLLAPoint toPoint, double fraction)
    {
        // Convert latitude and longitude from degrees to radians
        double lat1 = fromPoint.LatRads;
        double lon1 = fromPoint.LonRads;
        double lat2 = toPoint.LatRads;
        double lon2 = toPoint.LonRads;

        // Compute spherical interpolation
        double delta = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((lat2 - lat1) / 2), 2) +
                                              Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon2 - lon1) / 2), 2)));
        double A = Math.Sin((1 - fraction) * delta) / Math.Sin(delta);
        double B = Math.Sin(fraction * delta) / Math.Sin(delta);

        double x = A * Math.Cos(lat1) * Math.Cos(lon1) + B * Math.Cos(lat2) * Math.Cos(lon2);
        double y = A * Math.Cos(lat1) * Math.Sin(lon1) + B * Math.Cos(lat2) * Math.Sin(lon2);
        double z = A * Math.Sin(lat1) + B * Math.Sin(lat2);

        double newLatRads = Math.Atan2(z, Math.Sqrt(x * x + y * y));
        double newLonRads = Math.Atan2(y, x);

        // Convert the interpolated point back to degrees
        double newLatDegs = newLatRads * (180.0 / Math.PI);
        double newLonDegs = newLonRads * (180.0 / Math.PI);

        // Interpolate altitude
        double newAlt = fromPoint.AltMslM + (toPoint.AltMslM - fromPoint.AltMslM) * fraction;

        return new GloLLAPoint() { LatDegs = newLatDegs, LonDegs = newLonDegs, AltMslM = newAlt };
    }
}
