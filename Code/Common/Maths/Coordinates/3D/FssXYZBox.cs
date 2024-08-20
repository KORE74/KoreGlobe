

public class FssXYZBox : FssXYZ
{
    public FssXYZPoint Centre { get; set; }

    public double Width  { get; set; }
    public double Height { get; set; }
    public double Depth  { get; set; }

    // public FssXYZVector Up      { get; set; }
    // public FssXYZVector Right   { get; set; }
    // public FssXYZVector Forward { get; set; }

    public enum Face   { Top, Bottom, Left, Right, Front, Back }
    public enum Corner { TopLeftFront, TopRightFront, BottomLeftFront, BottomRightFront, TopLeftBack, TopRightBack, BottomLeftBack, BottomRightBack }
    public enum Edge   { TopFront, TopBack, TopLeft, TopRight, BottomFront, BottomBack, BottomLeft, BottomRight, FrontLeft, FrontRight, BackLeft, BackRight }

    public FssXYZBox()
    {
        Centre = FssXYZPoint.Zero;

        Width  = 0;
        Height = 0;
        Depth  = 0;

        // Up      = new FssXYZVector(0, 1, 0);
        // Right   = new FssXYZVector(1, 0, 0);
        // Forward = new FssXYZVector(0, 0, 1);
    }

    public FssXYZBox(FssXYZPoint centre, double width, double height, double depth)
    {
        Centre = centre;
        Width  = width;
        Height = height;
        Depth  = depth;

        // Up      = new FssXYZVector(0, 1, 0);
        // Right   = new FssXYZVector(1, 0, 0);
        // Forward = new FssXYZVector(0, 0, 1);
    }

    // public FssXYZBox(FssXYZPoint centre, double width, double height, double depth, FssXYZVector up, FssXYZVector right, FssXYZVector forward)
    // {
    //     Centre = centre;
    //     Width  = width;
    //     Height = height;
    //     Depth  = depth;

    //     Up      = up;
    //     Right   = right;
    //     Forward = forward;
    // }

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
    // Corner methods
    // --------------------------------------------------------------------------------------------

    // Get the corner of the box - considering the width, height and depth, as well as the orientation vectors.

    // public FssXYZPoint GetCorner(Corner corner)
    // {
    //     switch (corner)
    //     {
    //         case Corner.TopLeftFront:
    //             return Centre + (Up * (Height / 2)) + (Right * (-Width / 2)) + (Forward * (Depth / 2));
    //         case Corner.TopRightFront:
    //             return Centre + (Up * (Height / 2)) + (Right * (Width / 2)) + (Forward * (Depth / 2));
    //         case Corner.BottomLeftFront:
    //             return Centre + (Up * (-Height / 2)) + (Right * (-Width / 2)) + (Forward * (Depth / 2));
    //         case Corner.BottomRightFront:
    //             return Centre + (Up * (-Height / 2)) + (Right * (Width / 2)) + (Forward * (Depth / 2));
    //         case Corner.TopLeftBack:
    //             return Centre + (Up * (Height / 2)) + (Right * (-Width / 2)) + (Forward * (-Depth / 2));
    //         case Corner.TopRightBack:
    //             return Centre + (Up * (Height / 2)) + (Right * (Width / 2)) + (Forward * (-Depth / 2));
    //         case Corner.BottomLeftBack:
    //             return Centre + (Up * (-Height / 2)) + (Right * (-Width / 2)) + (Forward * (-Depth / 2));
    //         case Corner.BottomRightBack:
    //             return Centre + (Up * (-Height / 2)) + (Right * (Width / 2)) + (Forward * (-Depth / 2));
    //         default:
    //             return Centre;
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // Edge methods
    // --------------------------------------------------------------------------------------------

    // Get the edge of the box - considering the width, height and depth, as well as the orientation vectors.

    // public FssXYZLine GetEdge(Edge edge)
    // {
    //     switch (edge)
    //     {
    //         case Edge.TopFront:
    //             return new FssXYZLine(GetCorner(Corner.TopLeftFront), GetCorner(Corner.TopRightFront));
    //         case Edge.TopBack:
    //             return new FssXYZLine(GetCorner(Corner.TopLeftBack), GetCorner(Corner.TopRightBack));
    //         case Edge.TopLeft:
    //             return new FssXYZLine(GetCorner(Corner.TopLeftFront), GetCorner(Corner.TopLeftBack));
    //         case Edge.TopRight:
    //             return new FssXYZLine(GetCorner(Corner.TopRightFront), GetCorner(Corner.TopRightBack));
    //         case Edge.BottomFront:
    //             return new FssXYZLine(GetCorner(Corner.BottomLeftFront), GetCorner(Corner.BottomRightFront));
    //         case Edge.BottomBack:
    //             return new FssXYZLine(GetCorner(Corner.BottomLeftBack), GetCorner(Corner.BottomRightBack));
    //         case Edge.BottomLeft:
    //             return new FssXYZLine(GetCorner(Corner.BottomLeftFront), GetCorner(Corner.BottomLeftBack));
    //         case Edge.BottomRight:
    //             return new FssXYZLine(GetCorner(Corner.BottomRightFront), GetCorner(Corner.BottomRightBack));
    //         case Edge.FrontLeft:
    //             return new FssXYZLine(GetCorner(Corner.TopLeftFront), GetCorner(Corner.BottomLeftFront));
    //         case Edge.FrontRight:
    //             return new FssXYZLine(GetCorner(Corner.TopRightFront), GetCorner(Corner.BottomRightFront));
    //         case Edge.BackLeft:
    //             return new FssXYZLine(GetCorner(Corner.TopLeftBack), GetCorner(Corner.BottomLeftBack));
    //         case Edge.BackRight:
    //             return new FssXYZLine(GetCorner(Corner.TopRightBack), GetCorner(Corner.BottomRightBack));
    //         default:
    //             return new FssXYZLine(Centre, Centre);
    //     }
    // }

}