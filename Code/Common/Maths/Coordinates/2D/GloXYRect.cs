using System;

// Class defining a simple 2D rectangle, with no rotation

public struct GloXYRect
{
    // Main attributes
    public GloXYPoint TopLeft { get; }
    public GloXYPoint BottomRight { get; }

    // Derived attributes
    public double Width  => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;
    public double Left   => TopLeft.X;
    public double Right  => BottomRight.X;
    public double Top    => TopLeft.Y;
    public double Bottom => BottomRight.Y;
    public double Area   => Width * Height;

    public GloXYPoint TopRight     => new GloXYPoint(Right, Top);
    public GloXYPoint BottomLeft   => new GloXYPoint(Left, Bottom);

    public GloXYPoint Center       => new GloXYPoint((Left + Right) / 2, (Top + Bottom) / 2);
    public GloXYPoint TopCenter    => new GloXYPoint((Left + Right) / 2, Top);
    public GloXYPoint BottomCenter => new GloXYPoint((Left + Right) / 2, Bottom);
    public GloXYPoint LeftCenter   => new GloXYPoint(Left, (Top + Bottom) / 2);
    public GloXYPoint RightCenter  => new GloXYPoint(Right, (Top + Bottom) / 2);

    public GloXYLine TopLine       => new GloXYLine(TopLeft, TopRight);
    public GloXYLine BottomLine    => new GloXYLine(BottomLeft, BottomRight);
    public GloXYLine LeftLine      => new GloXYLine(TopLeft, BottomLeft);
    public GloXYLine RightLine     => new GloXYLine(TopRight, BottomRight);

    public static GloXYRect Zero => new GloXYRect(0, 0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYRect(double x1, double y1, double x2, double y2)
    {
        TopLeft     = new (x1, y1);
        BottomRight = new (x2, y2);
    }

    public GloXYRect(GloXYPoint topLeft, GloXYPoint bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public GloXYRect(GloXYRect rect)
    {
        TopLeft     = rect.TopLeft;
        BottomRight = rect.BottomRight;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Rect
    // --------------------------------------------------------------------------------------------

    public GloXYRect Offset(GloXYVector xyOffset)
    {
        GloXYPoint newTL = TopLeft.Offset(xyOffset);
        GloXYPoint newBR = BottomLeft.Offset(xyOffset);
        return new GloXYRect(newTL, newBR);
    }

    public GloXYRect Inset(double inset)
    {
        // Return a Zero rect if we would turn the rectangle inside out
        if (Width < inset * 2 || Height < inset * 2)
            return Zero;

        // Inset the rectangle by a given amount
        // - inset > 0 = smaller rectangle
        // - inset < 0 = larger rectangle
        return new GloXYRect(Left + inset, Top + inset, Right - inset, Bottom - inset);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Point
    // --------------------------------------------------------------------------------------------

    public GloXYPoint PointFromFraction(double xFraction, double yFraction)
    {
        // Get the point at a given fraction along the rectangle axis
        // - 0 = top/left, 1 = bottom/right, 0.5 = midpoint
        // - Values beyond 0->1 are allowed, to go outside the rectangle
        return new GloXYPoint(Left + (Width * xFraction), Top + (Height * yFraction));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Checks
    // --------------------------------------------------------------------------------------------

    public bool Contains(GloXYPoint xy)
    {
        return xy.X >= Left && xy.X <= Right && xy.Y >= Top && xy.Y <= Bottom;
    }

    public bool Intersects(GloXYRect other)
    {
        return !(other.Left > Right ||
                 other.Right < Left ||
                 other.Top > Bottom ||
                 other.Bottom < Top);
    }

}
