using System;

#nullable enable

public static class GloXYLineOperations
{
    // Get the distance from a point to a line

    public static double ClosestDistanceTo(this GloXYLine line, GloXYPoint xy)
    {
        double x1 = line.P1.X;
        double y1 = line.P1.Y;
        double x2 = line.P2.X;
        double y2 = line.P2.Y;
        double x3 = xy.X;
        double y3 = xy.Y;

        double px = x2 - x1;
        double py = y2 - y1;
        double dAB = px * px + py * py;
        double u = ((x3 - x1) * px + (y3 - y1) * py) / dAB;
        double x = x1 + u * px;
        double y = y1 + u * py;

        return Math.Sqrt(Math.Pow(x - x3, 2) + Math.Pow(y - y3, 2));
    }

    // Its double precision maths, so a point will never *exactly* be on a line, just check that the distance is within a tolerance

    public static bool IsPointOnLine(this GloXYLine line, GloXYPoint xy, bool limitToLineSegment = true, double tolerance = 1e-6)
    {
        double x1 = line.P1.X;
        double y1 = line.P1.Y;
        double x2 = line.P2.X;
        double y2 = line.P2.Y;
        double x3 = xy.X;
        double y3 = xy.Y;

        // Calculate the projection of the point onto the line (x, y)
        double px = x2 - x1;
        double py = y2 - y1;
        double dAB = px * px + py * py;
        double u = ((x3 - x1) * px + (y3 - y1) * py) / dAB;
        double x = x1 + u * px;
        double y = y1 + u * py;

        // Check if the distance from the projected point to the actual point is within tolerance
        bool isWithinDistance = Math.Sqrt(Math.Pow(x - x3, 2) + Math.Pow(y - y3, 2)) <= tolerance;

        // If limiting to the line segment, check if the projected point is within the segment bounds
        if (limitToLineSegment)
        {
            bool isWithinSegment = u >= 0 && u <= 1;
            return isWithinSegment && isWithinDistance;
        }

        return isWithinDistance;
    }


    // Find the intersection point between two lines, which could be null.

    public static GloXYPoint? Intersection(GloXYLine line1, GloXYLine line2)
    {
        double x1 = line1.P1.X;
        double y1 = line1.P1.Y;
        double x2 = line1.P2.X;
        double y2 = line1.P2.Y;
        double x3 = line2.P1.X;
        double y3 = line2.P1.Y;
        double x4 = line2.P2.X;
        double y4 = line2.P2.Y;

        double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (d == 0)
            return null;

        double xi = ((x3 - x4) * (x1 * y2 - y1 * x2) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d;
        double yi = ((y3 - y4) * (x1 * y2 - y1 * x2) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d;

        return new GloXYPoint(xi, yi);
    }

    // determine if two lines are parallel

    public static bool IsParallel(GloXYLine line1, GloXYLine line2)
    {
        double x1 = line1.P1.X;
        double y1 = line1.P1.Y;
        double x2 = line1.P2.X;
        double y2 = line1.P2.Y;
        double x3 = line2.P1.X;
        double y3 = line2.P1.Y;
        double x4 = line2.P2.X;
        double y4 = line2.P2.Y;

        // Determine the difference in the gradients of the lines
        double d = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        // Check if the lines are parallel, within the tolerance of the system.
        return GloValueUtils.EqualsWithinTolerance(d, 0);
    }

    // Return a new line object, with adjustments to the end points. +ve extends the line away from the center, -ve reduces it.

    public static GloXYLine ExtendLine(GloXYLine line, double p1Dist, double p2Dist)
    {
        // Handle the case where the line has no length
        if (line.Length == 0) return line;

        GloXYPoint p1 = line.P1;
        GloXYPoint p2 = line.P2;

        // Get the delta and normalise it so we apply the delta in the direction of the line
        double dx = (p2.X - p1.X) / line.Length;
        double dy = (p2.Y - p1.Y) / line.Length;
        double lineLength = line.Length;

        double p1x = p1.X - (dx * p1Dist);
        double p1y = p1.Y - (dy * p1Dist);
        double p2x = p2.X + (dx * p2Dist);
        double p2y = p2.Y + (dy * p2Dist);

        return new GloXYLine(new GloXYPoint(p1x, p1y), new GloXYPoint(p2x, p2y));
    }

}
