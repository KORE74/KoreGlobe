using System;
using System.Collections.Generic;

public static class FssXYArcOperations
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static List<FssXYPoint> IntersectionPoints(FssXYArc arc, FssXYLine line)
    {
        List<FssXYPoint> intersectionPoints = new List<FssXYPoint>();

        // Use the new ToCircle method
        FssXYCircle circle = arc.Circle;

        // Assume IntersectionPoints(FssXYCircle, FssXYLine) is implemented elsewhere
        List<FssXYPoint> circleIntersections = FssXYCircleOperations.IntersectionPoints(circle, line);

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

    public static FssXYPoint PointAtFraction(FssXYArc arc, double fraction)
    {
        double angle = arc.StartAngleRads + (arc.DeltaAngleRads * fraction);
        return FssXYPointOperations.OffsetPolar(arc.Center, arc.Radius, angle);
    }

    public static FssXYPolyLine ToPolyLine(FssXYArc arc, int numPoints)
    {
        List<FssXYPoint> points = new List<FssXYPoint>();

        double startAngle = arc.StartAngleRads;
        double endAngle   = arc.EndAngleRads;
        double angleIncrement = (endAngle - startAngle) / (numPoints - 1);

        for (int i = 0; i < numPoints; i++)
        {
            double angle = startAngle + (i * angleIncrement);
            points.Add( FssXYPointOperations.OffsetPolar(arc.Center, arc.Radius, angle) );
        }

        return new FssXYPolyLine(points);
    }

}