using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

// Point = a position, absolute.
// Vector = an offset, relative, scalable.

public struct GloXYVector
{
    public double X { get; }
    public double Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYVector(double x, double y)
    {
        X = x;
        Y = y;
    }

    public GloXYVector(GloXYVector xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    // Zero default constructor
    public static GloXYVector Zero => new GloXYVector(0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // return the angle FROM this point TO the given point - East / Positve X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (GloValueUtils.EqualsWithinTolerance(x, X) && GloValueUtils.EqualsWithinTolerance(y, Y))
            return 0;

        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(GloXYVector xy)
    {
        if (EqualsWithinTolerance(this, xy))
            return 0;

        // return the angle FROM this point TO the given point
        return Math.Atan2(xy.Y - Y, xy.X - X);
    }

    public double AngleToDegs(double x, double y) => GloValueUtils.RadsToDegs(AngleToRads(x, y));
    public double AngleToDegs(GloXYVector xy)     => GloValueUtils.RadsToDegs(AngleToRads(xy));

    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public GloXYVector Offset(double x, double y) => new GloXYVector(X + x, Y + y);
    public GloXYVector Offset(GloXYVector xy)     => new GloXYVector(X + xy.X, Y + xy.Y);
    public GloXYVector Offset(GloXYPolarOffset o) => Offset(GloXYPolarOffsetOperations.ToXY(o));

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static GloXYVector Sum(GloXYVector a, GloXYVector b)  => new GloXYVector(a.X + b.X, a.Y + b.Y);
    public static GloXYVector Diff(GloXYVector a, GloXYVector b) => new GloXYVector(a.X - b.X, a.Y - b.Y);
    public static GloXYVector Scale(GloXYVector a, double b)     => new GloXYVector(a.X * b, a.Y * b);

    public static bool EqualsWithinTolerance(GloXYVector a, GloXYVector b, double tolerance = GloConsts.ArbitraryMinDouble)
    {
        return GloValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && GloValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static GloXYVector operator +(GloXYVector a, GloXYVector b) { return new GloXYVector(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static GloXYVector operator -(GloXYVector a, GloXYVector b) { return new GloXYVector(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static GloXYVector operator *(GloXYVector a, double b)      { return new GloXYVector(a.X * b, a.Y * b); }
    public static GloXYVector operator *(double b, GloXYVector a)      { return new GloXYVector(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static GloXYVector operator /(GloXYVector a, double b)      { return new GloXYVector(a.X / b, a.Y / b); }
    public static GloXYVector operator /(double b, GloXYVector a)      { return new GloXYVector(a.X / b, a.Y / b); }

}