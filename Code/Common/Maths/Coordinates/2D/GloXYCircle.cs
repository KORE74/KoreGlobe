using System;

public class GloXYCircle : GloXY
{
    public GloXYPoint Center { get; }
    public double     Radius { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Area          => Math.PI * Radius * Radius;
    public double Circumference => 2 * Math.PI * Radius;
    public double Diameter      => 2 * Radius;

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYCircle(double x, double y, double radius)
    {
        Center = new (x, y);
        Radius = radius;
    }

    public GloXYCircle(GloXYPoint center, double radius)
    {
        Center = center;
        Radius = radius;
    }

    public GloXYCircle(GloXYCircle circle)
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

    public bool Contains(GloXYPoint xy)
    {
        return Center.DistanceTo(xy) <= Radius;
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public GloXYCircle Offset(double x, double y)
    {
        return new GloXYCircle(Center.Offset(x, y), Radius);
    }

    public GloXYCircle Offset(GloXYVector xy)
    {
        return new GloXYCircle(Center.Offset(xy), Radius);
    }

    public GloXYCircle Offset(GloXYPolarOffset o)
    {
        return new GloXYCircle(Center.Offset(o), Radius);
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    public GloXYPoint PointAtAngle(double angleDegs)
    {
        double angleRads = angleDegs * GloConsts.DegsToRadsMultiplier;
        double x = Center.X + Radius * Math.Cos(angleRads);
        double y = Center.Y + Radius * Math.Sin(angleRads);
        return new GloXYPoint(x, y);
    }

}