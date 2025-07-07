using System;

// Design Decisions:
// - Zero angle is "east" (3 o'clock) and angles increase anti-clockwise.

// Point = a position, absolute.
// Vector = an offset, relative, scalable.

public struct GloXYVectorI
{
    public int X { get; }
    public int Y { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYVectorI(int x, int y)
    {
        X = x;
        Y = y;
    }

    public GloXYVectorI(GloXYVectorI xy)
    {
        X = xy.X;
        Y = xy.Y;
    }

    // Zero default constructor
    public static GloXYVectorI Zero => new GloXYVectorI(0, 0);

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Return a new point offset by an XY amount.

    public GloXYVectorI Offset(int x, int y) => new GloXYVectorI(X + x, Y + y);
    public GloXYVectorI Offset(GloXYVectorI xy)     => new GloXYVectorI(X + xy.X, Y + xy.Y);

    // --------------------------------------------------------------------------------------------
    // static methods
    // --------------------------------------------------------------------------------------------

    public static GloXYVectorI Sum(GloXYVectorI a, GloXYVectorI b)  => new GloXYVectorI(a.X + b.X, a.Y + b.Y);
    public static GloXYVectorI Diff(GloXYVectorI a, GloXYVectorI b) => new GloXYVectorI(a.X - b.X, a.Y - b.Y);
    public static GloXYVectorI Scale(GloXYVectorI a, int b)         => new GloXYVectorI(a.X * b, a.Y * b);

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static GloXYVectorI operator +(GloXYVectorI a, GloXYVectorI b) { return new GloXYVectorI(a.X + b.X, a.Y + b.Y); }

    // - operator overload for subtracting points
    public static GloXYVectorI operator -(GloXYVectorI a, GloXYVectorI b) { return new GloXYVectorI(a.X - b.X, a.Y - b.Y); }

    // * operator overload for scaling point in relation to origin
    public static GloXYVectorI operator *(GloXYVectorI a, int b)      { return new GloXYVectorI(a.X * b, a.Y * b); }
    public static GloXYVectorI operator *(int b, GloXYVectorI a)      { return new GloXYVectorI(a.X * b, a.Y * b); }

    // / operator overload for scaling point in relation to origin
    public static GloXYVectorI operator /(GloXYVectorI a, int b)      { return new GloXYVectorI(a.X / b, a.Y / b); }
    public static GloXYVectorI operator /(int b, GloXYVectorI a)      { return new GloXYVectorI(a.X / b, a.Y / b); }

}