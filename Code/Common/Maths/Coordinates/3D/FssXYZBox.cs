using System;

public class FssXYZBox : FssXYZ
{
    public FssXYZPoint Center { get; set; } = FssXYZPoint.Zero;

    public double Width    { get; set; } = 0;
    public double Height   { get; set; } = 0;
    public double Length   { get; set; } = 0;

    public enum EnumFace   { Top, Bottom, Left, Right, Front, Back }
    public enum EnumCorner { TopLeftFront, TopRightFront, BottomLeftFront, BottomRightFront, TopLeftBack, TopRightBack, BottomLeftBack, BottomRightBack }
    public enum EnumEdge   { TopFront, TopBack, TopLeft, TopRight, BottomFront, BottomBack, BottomLeft, BottomRight, FrontLeft, FrontRight, BackLeft, BackRight }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    public FssXYZBox()
    {
        Center = FssXYZPoint.Zero;

        Width  = 0;
        Height = 0;
        Length = 0;
    }

    public FssXYZBox(FssXYZPoint center, double width, double height, double length)
    {
        Center = center;
        Width  = width;
        Height = height;
        Length = length;
    }

    // --------------------------------------------------------------------------------------------

    // Constructors for common sizes

    public static FssXYZBox Zero
    {
        get { return new FssXYZBox(FssXYZPoint.Zero, 0, 0, 0); }
    }

    public static FssXYZBox One
    {
        get { return new FssXYZBox(FssXYZPoint.Zero, 1, 1, 1); }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Box Edits
    // --------------------------------------------------------------------------------------------

    // Scale the box by a factor - Treat the box as immutable and return a new box.

    public FssXYZBox Scale(double scale)
    {
        return new FssXYZBox(Center, Width * scale, Height * scale, Length * scale);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Offset methods
    // --------------------------------------------------------------------------------------------

    public double OffsetForwards  { get { return (Length / 2) - Center.Z; } }
    public double OffsetBackwards { get { return (Length / 2) + Center.Z; } }
    public double OffsetLeft      { get { return (Width / 2)  - Center.X; } }
    public double OffsetRight     { get { return (Width / 2)  + Center.X; } }
    public double OffsetUp        { get { return (Height / 2) - Center.Y; } }
    public double OffsetDown      { get { return (Height / 2) + Center.Y; } }

    public double LongestOffset()
    {
        double longestDimension = Math.Max(Width, Math.Max(Height, Length));
        return longestDimension / 2;
    }

    public double LongestDimension { get { return Math.Max(Width, Math.Max(Height, Length)); } }

    // --------------------------------------------------------------------------------------------
    // MARK: Corner methods
    // --------------------------------------------------------------------------------------------

    // Get the corner of the box - considering the width, height and length

    public FssXYZPoint Corner(EnumCorner corner)
    {
        double halfWidth  = Width  / 2;
        double halfHeight = Height / 2;
        double halfLength = Length  / 2;

        switch(corner)
        {
            case EnumCorner.TopLeftFront:     return new FssXYZPoint(Center.X - halfWidth, Center.Y + halfHeight, Center.Z - halfLength);
            case EnumCorner.TopRightFront:    return new FssXYZPoint(Center.X + halfWidth, Center.Y + halfHeight, Center.Z - halfLength);
            case EnumCorner.BottomLeftFront:  return new FssXYZPoint(Center.X - halfWidth, Center.Y - halfHeight, Center.Z - halfLength);
            case EnumCorner.BottomRightFront: return new FssXYZPoint(Center.X + halfWidth, Center.Y - halfHeight, Center.Z - halfLength);
            case EnumCorner.TopLeftBack:      return new FssXYZPoint(Center.X - halfWidth, Center.Y + halfHeight, Center.Z + halfLength);
            case EnumCorner.TopRightBack:     return new FssXYZPoint(Center.X + halfWidth, Center.Y + halfHeight, Center.Z + halfLength);
            case EnumCorner.BottomLeftBack:   return new FssXYZPoint(Center.X - halfWidth, Center.Y - halfHeight, Center.Z + halfLength);
            case EnumCorner.BottomRightBack:  return new FssXYZPoint(Center.X + halfWidth, Center.Y - halfHeight, Center.Z + halfLength);
            default:
                return Center;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Edge methods
    // --------------------------------------------------------------------------------------------

    // Get the edge of the box - considering the width, height and length.

    public FssXYZLine Edge(EnumEdge edge)
    {
        switch(edge)
        {
            case EnumEdge.TopFront:     return new FssXYZLine(Corner(EnumCorner.TopLeftFront),     Corner(EnumCorner.TopRightFront));
            case EnumEdge.TopBack:      return new FssXYZLine(Corner(EnumCorner.TopLeftBack),      Corner(EnumCorner.TopRightBack));
            case EnumEdge.TopLeft:      return new FssXYZLine(Corner(EnumCorner.TopLeftFront),     Corner(EnumCorner.TopLeftBack));
            case EnumEdge.TopRight:     return new FssXYZLine(Corner(EnumCorner.TopRightFront),    Corner(EnumCorner.TopRightBack));
            case EnumEdge.BottomFront:  return new FssXYZLine(Corner(EnumCorner.BottomLeftFront),  Corner(EnumCorner.BottomRightFront));
            case EnumEdge.BottomBack:   return new FssXYZLine(Corner(EnumCorner.BottomLeftBack),   Corner(EnumCorner.BottomRightBack));
            case EnumEdge.BottomLeft:   return new FssXYZLine(Corner(EnumCorner.BottomLeftFront),  Corner(EnumCorner.BottomLeftBack));
            case EnumEdge.BottomRight:  return new FssXYZLine(Corner(EnumCorner.BottomRightFront), Corner(EnumCorner.BottomRightBack));
            case EnumEdge.FrontLeft:    return new FssXYZLine(Corner(EnumCorner.TopLeftFront),     Corner(EnumCorner.BottomLeftFront));
            case EnumEdge.FrontRight:   return new FssXYZLine(Corner(EnumCorner.TopRightFront),    Corner(EnumCorner.BottomRightFront));
            case EnumEdge.BackLeft:     return new FssXYZLine(Corner(EnumCorner.TopLeftBack),      Corner(EnumCorner.BottomLeftBack));
            case EnumEdge.BackRight:    return new FssXYZLine(Corner(EnumCorner.TopRightBack),     Corner(EnumCorner.BottomRightBack));
            default:
                return new FssXYZLine(Center, Center);
        }
    }

    // --------------------------------------------------------------------------------------------

    // Override the ToString method to return a string representation of the box.

    public override string ToString()
    {
        return $"Center:{Center}, Width:{Width:F2}, Height:{Height:F2}, Length:{Length:F2}";
    }

}