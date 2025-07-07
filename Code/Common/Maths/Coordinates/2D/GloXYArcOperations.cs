using System;
using System.Collections.Generic;

public static class GloXYArcOperations
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static List<GloXYPoint> IntersectionPoints(GloXYArc arc, GloXYLine line)
    {
        List<GloXYPoint> intersectionPoints = new List<GloXYPoint>();

        // Use the new ToCircle method
        GloXYCircle circle = arc.Circle;

        // Assume IntersectionPoints(GloXYCircle, GloXYLine) is implemented elsewhere
        List<GloXYPoint> circleIntersections = GloXYCircleOperations.IntersectionPoints(circle, line);

        foreach (var point in circleIntersections)
        {
            double angleToPoint = arc.Center.AngleToRads(point);

            // Use the new IsAngleInRange method
            if (arc.ContainsAngle(angleToPoint))
                intersectionPoints.Add(point);
        }

        return intersectionPoints;
    }

    // --------------------------------------------------------------------------------------------
    // Conversions
    // --------------------------------------------------------------------------------------------

    public static GloXYPoint PointAtFraction(GloXYArc arc, double fraction)
    {
        double angle = arc.StartAngleRads + (arc.DeltaAngleRads * fraction);
        return GloXYPointOperations.OffsetPolar(arc.Center, arc.Radius, angle);
    }

    public static GloXYPolyLine ToPolyLine(GloXYArc arc, int numPoints)
    {
        List<GloXYPoint> points = new List<GloXYPoint>();

        double startAngle = arc.StartAngleRads;
        double endAngle   = arc.EndAngleRads;
        double angleIncrement = (endAngle - startAngle) / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            double angle = startAngle + (i * angleIncrement);
            points.Add( GloXYPointOperations.OffsetPolar(arc.Center, arc.Radius, angle) );
        }

        return new GloXYPolyLine(points);
    }

}