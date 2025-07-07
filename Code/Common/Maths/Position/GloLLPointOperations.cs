using System;
using System.Collections.Generic;

// GloLLAPointOperations: A static class to hold operations on GloLLAPoint objects that are not part of its
// core responsibilites. This class is static, all operations are stateless and return a new object.

public static class GloLLPointOperations
{
    // Calculates the angle between two points on a sphere
    public static double AngleBetween(GloLLPoint fromPoint, GloLLPoint toPoint)
    {
        double lat1 = fromPoint.LatRads;
        double lon1 = fromPoint.LonRads;
        double lat2 = toPoint.LatRads;
        double lon2 = toPoint.LonRads;

        double deltaLat = lat2 - lat1;
        double deltaLon = GloValueUtils.AngleDiffRads(lon1, lon2);

        double a = Math.Pow(Math.Sin(deltaLat / 2), 2) +
                   Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(deltaLon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return c;
    }

    // --------------------------------------------------------------------------------------------

    public static GloLLPoint RhumbLineInterpolation(GloLLPoint fromPoint, GloLLPoint toPoint, double fraction)
    {
        // This is a simple linear interpolation of the lat and lon, and a weighted average of the altitudes.
        // The fraction is the fraction of the distance from the fromPoint to the toPoint.

        double latDegs = fromPoint.LatDegs + ((toPoint.LatDegs - fromPoint.LatDegs) * fraction);
        double lonDegs = fromPoint.LonDegs + ((toPoint.LonDegs - fromPoint.LonDegs) * fraction);

        return new GloLLPoint() { LatDegs = latDegs, LonDegs = lonDegs };
    }

    // --------------------------------------------------------------------------------------------

    public static GloLLPoint GreatCircleInterpolation(GloLLPoint fromPoint, GloLLPoint toPoint, double fraction)
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

        return new GloLLPoint() { LatDegs = newLatDegs, LonDegs = newLonDegs };
    }

    // --------------------------------------------------------------------------------------------

    // Generate a list of points that form a full circle path
    public static List<GloLLPoint> GreatCirclePointList(GloLLPoint fromPoint, GloLLPoint toPoint, int numPoints)
    {
        var points = new List<GloLLPoint>();
        double angleBetween = AngleBetween(fromPoint, toPoint);

        // Divide 360 degrees by the angle between the two points, to get the maximum fraction we need to create the full great circle.
        double maxFraction = (2 * Math.PI) / angleBetween;

        // Calculate the fraction step based on the total fraction needed and the number of points
        double fractionStep = maxFraction / numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            points.Add(GreatCircleInterpolation(fromPoint, toPoint, fractionStep * i));
        }

        return points;
    }

    // --------------------------------------------------------------------------------------------

    // A quick/dirty "is facing camera" type check.
    // GloLLPointOperations.InSameHemisphere
    public static bool InSameHemisphere(GloLLPoint refPoint, GloLLPoint comparePoint)
    {
        if (GloValueUtils.Diff(refPoint.LatDegs, comparePoint.LatDegs) > 90)
            return false;
        if (GloValueUtils.Diff(refPoint.LonDegs, comparePoint.LonDegs) > 90)
            return false;

        return true;
    }

}
