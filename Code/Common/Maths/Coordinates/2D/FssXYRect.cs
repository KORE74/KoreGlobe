using System;

// Class defining a simple 2D rectangle, with no rotation

public class FssXYRect : FssXY
{
    // Main attributes
    public FssXYPoint TopLeft { get; }
    public FssXYPoint BottomRight { get; }

    // Derived attributes
    public double Width  => BottomRight.X - TopLeft.X;
    public double Height => BottomRight.Y - TopLeft.Y;
    public double Left   => TopLeft.X;
    public double Right  => BottomRight.X;
    public double Top    => TopLeft.Y;
    public double Bottom => BottomRight.Y;
    public double Area   => Width * Height;

    public FssXYPoint TopRight     => new FssXYPoint(Right, Top);
    public FssXYPoint BottomLeft   => new FssXYPoint(Left, Bottom);

    public FssXYPoint Center       => new FssXYPoint((Left + Right) / 2, (Top + Bottom) / 2);
    public FssXYPoint TopCenter    => new FssXYPoint((Left + Right) / 2, Top);
    public FssXYPoint BottomCenter => new FssXYPoint((Left + Right) / 2, Bottom);
    public FssXYPoint LeftCenter   => new FssXYPoint(Left, (Top + Bottom) / 2);
    public FssXYPoint RightCenter  => new FssXYPoint(Right, (Top + Bottom) / 2);

    public FssXYLine TopLine       => new FssXYLine(TopLeft, TopRight);
    public FssXYLine BottomLine    => new FssXYLine(BottomLeft, BottomRight);
    public FssXYLine LeftLine      => new FssXYLine(TopLeft, BottomLeft);
    public FssXYLine RightLine     => new FssXYLine(TopRight, BottomRight);

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYRect(double x1, double y1, double x2, double y2)
    {
        TopLeft     = new (x1, y1);
        BottomRight = new (x2, y2);
    }

    public FssXYRect(FssXYPoint topLeft, FssXYPoint bottomRight)
    {
        TopLeft     = topLeft;
        BottomRight = bottomRight;
    }

    public FssXYRect(FssXYRect rect)
    {
        TopLeft     = rect.TopLeft;
        BottomRight = rect.BottomRight;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public bool Contains(FssXYPoint xy)
    {
        return xy.X >= Left && xy.X <= Right && xy.Y >= Top && xy.Y <= Bottom;
    }

}
