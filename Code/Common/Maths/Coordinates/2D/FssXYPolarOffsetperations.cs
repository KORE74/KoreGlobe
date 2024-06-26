using System;

// Design Decisions:
// - Performs FssXYPolarOffset functionality thats not prt of the core class.

public static class FssXYPolarOffsetOperations
{
    public static FssXYPoint ToXY(FssXYPolarOffset o)
    {
        double x = o.Distance * Math.Cos(o.AngleRads);
        double y = o.Distance * Math.Sin(o.AngleRads);
        return new FssXYPoint(x, y);
    }

    // Turn the XY position into an offset from 0,0

    public static FssXYPolarOffset FromXY(FssXYPoint p)
    {
        double distance  = Math.Sqrt(p.X * p.X + p.Y * p.Y);
        double angleRads = Math.Atan2(p.Y, p.X);
        return new FssXYPolarOffset(angleRads, distance);
    }

    public static FssXYPolarOffset OffsetToPoint(FssXYPoint fromPoint, FssXYPoint toPoint)
    {
        double xDiff = toPoint.X - fromPoint.X;
        double yDiff = toPoint.Y - fromPoint.Y;
        double distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        double angleRads = Math.Atan2(yDiff, xDiff);
        return new FssXYPolarOffset(angleRads, distance);
    }
}
