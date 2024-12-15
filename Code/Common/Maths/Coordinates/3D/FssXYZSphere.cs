using System;

public class FssXYZSphere : FssXYZ
{
    public FssXYZPoint Center { get; private set; }
    public double Radius { get; private set; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Volume      { get { return 4 / 3 * Math.PI * Radius * Radius * Radius; } }
    public double SurfaceArea { get { return 4 * Math.PI * Radius * Radius; } }
    public double Diameter    { get { return 2 * Radius; } }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZSphere(FssXYZPoint center, double radius)
    {
        if (radius <= 0)
        {
            throw new ArgumentException("Radius must be positive.", nameof(radius));
        }

        Center = center;
        Radius = radius;
    }

    public bool ContainsPoint(FssXYZPoint point)
    {
        return (point - Center).Magnitude <= Radius;
    }


    // Additional methods can be added here as needed
}