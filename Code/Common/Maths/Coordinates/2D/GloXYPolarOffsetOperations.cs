using System;

// Design Decisions:
// - Performs GloXYPolarOffset functionality thats not prt of the core class.

public static class GloXYPolarOffsetOperations
{
    public static GloXYVector ToXY(GloXYPolarOffset o)
    {
        double x = o.Distance * Math.Cos(o.AngleRads);
        double y = o.Distance * Math.Sin(o.AngleRads);
        return new GloXYVector(x, y);
    }

    // Turn the XY position into an offset from 0,0

    public static GloXYPolarOffset FromXYVector(GloXYVector p)
    {
        double distance  = Math.Sqrt(p.X * p.X + p.Y * p.Y);
        double angleRads = Math.Atan2(p.Y, p.X);
        return new GloXYPolarOffset(angleRads, distance);
    }

    // public static GloXYPolarOffset ToPolarOffset(GloXYPoint fromPoint, GloXYPoint toPoint)
    // {
    //     double xDiff = toPoint.X - fromPoint.X;
    //     double yDiff = toPoint.Y - fromPoint.Y;
    //     double distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
    //     double angleRads = Math.Atan2(yDiff, xDiff);
    //     return new GloXYPolarOffset(angleRads, distance);
    // }
}
