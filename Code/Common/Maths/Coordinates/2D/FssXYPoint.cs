using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

public class FssXYPoint : FssXY
{
    public double X { get; }
    public double Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public FssXYPoint(FssXYPoint xy)
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

    public double DistanceTo(FssXYPoint xy)
    {
        return Math.Sqrt(Math.Pow(X - xy.X, 2) + Math.Pow(Y - xy.Y, 2));
    }

    // --------------------------------------------------------------------------------------------

    // return the angle FROM this point TO the given point - East / Positve X axis is zero
    public double AngleToRads(double x, double y)
    {
        if (FssValueUtils.EqualsWithinTolerance(x, X) && FssValueUtils.EqualsWithinTolerance(y, Y))
            return 0;
    
        return Math.Atan2(y - Y, x - X);
    }

    public double AngleToRads(FssXYPoint xy)
    {
        if (EqualsWithinTolerance(this, xy))
            return 0;

        // return the angle FROM this point TO the given point
        return Math.Atan2(xy.Y - Y, xy.X - X);
    }

    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public FssXYPoint Offset(double x, double y)
    {
        return new FssXYPoint(X + x, Y + y);
    }   

    public FssXYPoint Offset(FssXYPoint xy)
    {
        return new FssXYPoint(X + xy.X, Y + xy.Y);
    }

    public FssXYPoint Offset(FssXYPolarOffset o)
    {
        FssXYPoint xy = FssXYPolarOffsetOperations.ToXY(o);
        return Offset(xy);
    }

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static FssXYPoint Sum(FssXYPoint a, FssXYPoint b)
    {
        return new FssXYPoint(a.X + b.X, a.Y + b.Y);
    }

    public static FssXYPoint Diff(FssXYPoint a, FssXYPoint b)
    {
        return new FssXYPoint(a.X - b.X, a.Y - b.Y);
    }

    public static FssXYPoint Scale(FssXYPoint a, double b)
    {
        return new FssXYPoint(a.X * b, a.Y * b);
    }

    public static bool EqualsWithinTolerance(FssXYPoint a, FssXYPoint b, double tolerance = FssConsts.ArbitraryMinDouble)
    {
        return FssValueUtils.EqualsWithinTolerance(a.X, b.X, tolerance) && FssValueUtils.EqualsWithinTolerance(a.Y, b.Y, tolerance);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload  
    public static FssXYPoint operator +(FssXYPoint a, FssXYPoint b)  { return new FssXYPoint(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static FssXYPoint operator -(FssXYPoint a, FssXYPoint b)  { return new FssXYPoint(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static FssXYPoint operator *(FssXYPoint a, double b)        { return new FssXYPoint(a.X * b, a.Y * b); }
    public static FssXYPoint operator *(double b, FssXYPoint a)        { return new FssXYPoint(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static FssXYPoint operator /(FssXYPoint a, double b)        { return new FssXYPoint(a.X / b, a.Y / b); }
    public static FssXYPoint operator /(double b, FssXYPoint a)        { return new FssXYPoint(a.X / b, a.Y / b); }

} 