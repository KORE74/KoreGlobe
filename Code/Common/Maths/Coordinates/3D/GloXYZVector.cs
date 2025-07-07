using System;

// GloXYZVector: A class to hold an XYZ vector. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

// An XYZVector is an abstract, relative difference in 3D position. An XYZPoint is an absolute position.
// - So you can't scale a position, but you can scale a vector. etc.

public struct GloXYZVector
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public double Magnitude
    {
        get
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }
        set
        {
            double currMag = Magnitude; // store, avoid caclulating twice
            if (currMag < GloConsts.ArbitraryMinDouble) // if too close to a div0
            {
                X = value; Y = 0; Z = 0;
            }
            else
            {
                double scaleFactor = value / currMag;
                X *= scaleFactor;
                Y *= scaleFactor;
                Z *= scaleFactor;
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public GloXYZVector(double xm, double ym, double zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // Return a zero point as a default value
    // Example: GloXYZVector newPos = GloXYZVector.Zero();
    public static GloXYZVector Zero => new GloXYZVector(0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Basic operations
    // --------------------------------------------------------------------------------------------

    public GloXYZVector Offset(double x, double y, double z) => new GloXYZVector(X + x, Y + y, Z + z);
    public GloXYZVector Offset(GloXYZVector shiftXYZ)        => new GloXYZVector(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);
    public GloXYZVector Subtract(GloXYZVector inputXYZ)      => new GloXYZVector(X - inputXYZ.X, Y - inputXYZ.Y, Z - inputXYZ.Z);
    public GloXYZVector Scale(double scaleFactor)            => new GloXYZVector(X * scaleFactor, Y * scaleFactor, Z * scaleFactor);
    public GloXYZVector Invert()                             => new GloXYZVector(-X, -Y, -Z);

    public GloXYZVector Normalize()
    {
        double mag = Magnitude;
        if (mag < GloConsts.ArbitraryMinDouble)
            return new GloXYZVector(1, 0, 0);
        else
            return new GloXYZVector(X / mag, Y / mag, Z / mag);
    }

    // --------------------------------------------------------------------------------------------

    public GloXYZVector XYZTo(GloXYZVector remoteXYZ)
    {
        return new GloXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    // --------------------------------------------------------------------------------------------
    // operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static GloXYZVector operator +(GloXYZVector a, GloXYZVector b)        { return new GloXYZVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // - operator overload for subtracting points
    public static GloXYZVector operator -(GloXYZVector a, GloXYZVector b)        { return new GloXYZVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

    // * operator overload for scaling a
    public static GloXYZVector operator *(GloXYZVector a, double scaleFactor)    { return new GloXYZVector(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }
    public static GloXYZVector operator *(double scaleFactor, GloXYZVector a)    { return new GloXYZVector(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }

    // / operator overload for scaling a point
    public static GloXYZVector operator /(GloXYZVector a, double scaleFactor)    { return new GloXYZVector(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }
    public static GloXYZVector operator /(double scaleFactor, GloXYZVector a)    { return new GloXYZVector(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }

    // --------------------------------------------------------------------------------------------
    // Conversion
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"({X:F2}, {Y:F2}, {Z:F2})(Mag {Magnitude:F2})";
    }

    // ========================================================================
    // static Ops
    // ========================================================================

    public static GloXYZVector Diff(GloXYZVector inputXYZ1, GloXYZVector inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return new GloXYZVector(diffX, diffY, diffZ);
    }

    public static GloXYZVector Sum(GloXYZVector inputXYZ1, GloXYZVector inputXYZ2)
    {
        double sumX = inputXYZ2.X + inputXYZ1.X;
        double sumY = inputXYZ2.Y + inputXYZ1.Y;
        double sumZ = inputXYZ2.Z + inputXYZ1.Z;

        return new GloXYZVector(sumX, sumY, sumZ);
    }

    public static GloXYZVector Scale(GloXYZVector inputXYZ1, double scaleFactor)
    {
        double scaledX = inputXYZ1.X * scaleFactor;
        double scaledY = inputXYZ1.Y * scaleFactor;
        double scaledZ = inputXYZ1.Z * scaleFactor;

        return new GloXYZVector(scaledX, scaledY, scaledZ);
    }

    // Usage: GloXYZVector.DotProduct(v1, v2)
    public static double DotProduct(GloXYZVector inputXYZ1, GloXYZVector inputXYZ2)
    {
        return (inputXYZ1.X * inputXYZ2.X) + (inputXYZ1.Y * inputXYZ2.Y) + (inputXYZ1.Z * inputXYZ2.Z);
    }

    public static GloXYZVector CrossProduct(GloXYZVector a, GloXYZVector b)
    {
        return new GloXYZVector(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }

}
