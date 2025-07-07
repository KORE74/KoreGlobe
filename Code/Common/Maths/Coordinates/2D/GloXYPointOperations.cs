using System;

public static class GloXYPointOperations
{
    // Dot Product - the cosine of the angle between the two vectors
    // Considering both as lines from 0,0 to the points, this is the cosine of the angle between them
    // near +1 means the angle is near 0 degrees (absolute to remove sign)
    // wiki - https://en.wikipedia.org/wiki/Dot_product

    public static double DotProduct(GloXYPoint a, GloXYPoint b)
    {
        return (a.X * b.X) + (a.Y * b.Y);
    }

    // Angle from one point to another, useful when we start creating arcs.

    public static double Angle(GloXYPoint fromPos, GloXYPoint toPos)
    {
        double x = toPos.X - fromPos.X;
        double y = toPos.Y - fromPos.Y;
        return Math.Atan2(y, x);
    }

    // --------------------------------------------------------------------------------------------

    // Polar Offset.  Given a point and a distance and an angle, return the new point.
    // To work consistently with the creation of Arc points.

    // GloXYPointOperations.OffsetPolar(fromPos, distance, angleRads);

    public static GloXYPoint OffsetPolar(GloXYPoint fromPos, double distance, double angleRads)
    {
        // Normalise the angle, as may have been from a start value + delta
        // angleRads = GloNumericAngle<double>.NormalizeRads(angleRads);

        double x = fromPos.X + (distance * Math.Cos(angleRads));
        double y = fromPos.Y + (distance * Math.Sin(angleRads));
        return new GloXYPoint(x, y);
    }

    // --------------------------------------------------------------------------------------------


    // Given 3 points, ABC forming to lines AB and BC, find the angle between them.

    public static double AngleBetweenRads(GloXYPoint a, GloXYPoint b, GloXYPoint c)
    {
        double dx1 = a.X - b.X;
        double dy1 = a.Y - b.Y;
        double dx2 = c.X - b.X;
        double dy2 = c.Y - b.Y;

        double angle1Rads = Math.Atan2(dy1, dx1);
        double angle2Rads = Math.Atan2(dy2, dx2);

        double angleRads = angle2Rads - angle1Rads;
        if (angleRads < 0)
        {
            angleRads += 2 * Math.PI;
        }

        return angleRads;
    }

    // Given 3 points, ABC forming to lines AB and BC, find a point D that is equally inset between AB and BC
    // by parameter distance t.

    public static GloXYPoint InsetPoint(GloXYPoint a, GloXYPoint b, GloXYPoint c, double t)
    {
        // Calculate direction vectors for AB and BC
        double dxAB = b.X - a.X;
        double dyAB = b.Y - a.Y;
        double dxBC = c.X - b.X;
        double dyBC = c.Y - b.Y;

        // Normalize direction vectors
        double magAB = Math.Sqrt(dxAB * dxAB + dyAB * dyAB);
        double magBC = Math.Sqrt(dxBC * dxBC + dyBC * dyBC);
        dxAB /= magAB;
        dyAB /= magAB;
        dxBC /= magBC;
        dyBC /= magBC;

        // Calculate bisector vector
        double bisectorX = dxAB + dxBC;
        double bisectorY = dyAB + dyBC;
        double magBisector = Math.Sqrt(bisectorX * bisectorX + bisectorY * bisectorY);
        bisectorX /= magBisector;
        bisectorY /= magBisector;

        // Calculate inset point along the bisector
        double xInset = b.X + t * bisectorX;
        double yInset = b.Y + t * bisectorY;

        return new GloXYPoint(xInset, yInset);
    }
}