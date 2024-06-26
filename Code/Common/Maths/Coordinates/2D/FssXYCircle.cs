using System;

public class FssXYCircle : FssXY
{
    public FssXYPoint Center { get; }
    public double Radius { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Area          => Math.PI * Radius * Radius;
    public double Circumference => 2 * Math.PI * Radius;
    public double Diameter      => 2 * Radius;

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYCircle(double x, double y, double radius)
    {
        Center = new (x, y);
        Radius = radius;
    }

    public FssXYCircle(FssXYPoint center, double radius)
    {
        Center = center;
        Radius = radius;
    }

    public FssXYCircle(FssXYCircle circle)
    {
        Center = circle.Center;
        Radius = circle.Radius;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public bool Contains(double x, double y)
    {
        return Center.DistanceTo(x, y) <= Radius;
    }

    public bool Contains(FssXYPoint xy)
    {
        return Center.DistanceTo(xy) <= Radius;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public FssXYCircle Offset(double x, double y)
    {
        return new FssXYCircle(Center.Offset(x, y), Radius);
    }

    public FssXYCircle Offset(FssXYPoint xy)
    {
        return new FssXYCircle(Center.Offset(xy), Radius);
    }

    public FssXYCircle Offset(FssXYPolarOffset o)
    {
        return new FssXYCircle(Center.Offset(o), Radius);
    }

    // --------------------------------------------------------------------------------------------
    // Operator overloads
    // --------------------------------------------------------------------------------------------

}