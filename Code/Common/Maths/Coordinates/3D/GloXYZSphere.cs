using System;

public struct GloXYZSphere
{
    public GloXYZPoint Center { get; private set; }
    public double      Radius { get; private set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Volume      { get { return 4 / 3 * Math.PI * Radius * Radius * Radius; } }
    public double SurfaceArea { get { return 4 * Math.PI * Radius * Radius; } }
    public double Diameter    { get { return 2 * Radius; } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYZSphere(GloXYZPoint center, double radius)
    {
        if (radius <= 0)
        {
            throw new ArgumentException("Radius must be positive.", nameof(radius));
        }

        Center = center;
        Radius = radius;
    }

    public bool ContainsPoint(GloXYZPoint point)
    {
        return (point - Center).Magnitude <= Radius;
    }

    // --------------------------------------------------------------------------------------------
    //
    // --------------------------------------------------------------------------------------------

    public GloXYZPoint SurfacePoint(double AzDegs, double ElDegs)
    {
        double azRads = AzDegs * GloConsts.DegsToRadsMultiplier;
        double elRads = ElDegs * GloConsts.DegsToRadsMultiplier;

        double x = Center.X + Radius * Math.Cos(elRads) * Math.Cos(azRads);
        double y = Center.Y + Radius * Math.Cos(elRads) * Math.Sin(azRads);
        double z = Center.Z + Radius * Math.Sin(elRads);

        return new GloXYZPoint(x, y, z);
    }

    // Additional methods can be added here as needed
}