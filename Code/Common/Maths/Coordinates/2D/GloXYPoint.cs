using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

// Point = a position, absolute.
// Vector = an offset, relative, scalable.

public class GloXYPoint
{
    public double X { get; }
    public double Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public GloXYPoint(GloXYPoint xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Absolute value of the magnitude of the distance from this point, to the specified point.

    public double DistanceTo(double x, double y)
    {
        return Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
    }

    public double DistanceTo(GloXYPoint xy)
    {
        return Math.Sqrt(Math.Pow(X - xy.X, 2) + Math.Pow(Y - xy.Y, 2));
    }

    // --------------------------------------------------------------------------------------------

    // return the angle FROM this point TO the given point - East / Positve X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (GloValueUtils.EqualsWithinTolerance(x, X) && GloValueUtils.EqualsWithinTolerance(y, Y))
            return 0;

        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(GloXYPoint xy)
    {
        if (EqualsWithinTolerance(this, xy))
            return 0;

        // return the angle FROM this point TO the given point
        return Math.Atan2(xy.Y - Y, xy.X - X);
    }

    public double AngleToDegs(double x, double y) => GloValueUtils.RadsToDegs(AngleToRads(x, y));
    public double AngleToDegs(GloXYPoint xy)      => GloValueUtils.RadsToDegs(AngleToRads(xy));

    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public GloXYPoint Offset(double x, double y) => new GloXYPoint(X + x, Y + y);
    public GloXYPoint Offset(GloXYVector xy)     => new GloXYPoint(X + xy.X, Y + xy.Y);
    public GloXYPoint Offset(GloXYPolarOffset o) => Offset(GloXYPolarOffsetOperations.ToXY(o));

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static GloXYPoint Sum(GloXYPoint a, GloXYPoint b)  => new GloXYPoint(a.X + b.X, a.Y + b.Y);
    public static GloXYPoint Diff(GloXYPoint a, GloXYPoint b) => new GloXYPoint(a.X - b.X, a.Y - b.Y);
    //public static GloXYPoint Scale(GloXYPoint a, double b)    => new GloXYPoint(a.X * b, a.Y * b);

    public static bool EqualsWithinTolerance(GloXYPoint a, GloXYPoint b, double tolerance = GloConsts.ArbitraryMinDouble)
    {
        return GloValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && GloValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static GloXYPoint operator +(GloXYPoint a, GloXYPoint b)  { return new GloXYPoint(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static GloXYPoint operator -(GloXYPoint a, GloXYPoint b)  { return new GloXYPoint(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static GloXYPoint operator *(GloXYPoint a, double b)      { return new GloXYPoint(a.X * b, a.Y * b); }
    public static GloXYPoint operator *(double b, GloXYPoint a)      { return new GloXYPoint(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static GloXYPoint operator /(GloXYPoint a, double b)      { return new GloXYPoint(a.X / b, a.Y / b); }
    public static GloXYPoint operator /(double b, GloXYPoint a)      { return new GloXYPoint(a.X / b, a.Y / b); }

}