using System;

// FssXYZVector: A class to hold an XYZ ve3ctor. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

public class FssXYZVector : FssXYZ
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
            if (currMag < FssConsts.ArbitraryMinDouble) // if too close to a div0
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

    public FssXYZVector(double xm, double ym, double zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // Return a zero point as a default value
    // Example: FssXYZVector newPos = FssXYZVector.Zero();
    public static FssXYZVector Zero() => new FssXYZVector(0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Basic operations
    // --------------------------------------------------------------------------------------------

    public FssXYZVector Offset(double x, double y, double z)
    {
        return new FssXYZVector(X + x, Y + y, Z + z);
    }
    public FssXYZVector Offset(FssXYZVector shiftXYZ)
    {
        return new FssXYZVector(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);
    }
    public FssXYZVector Subtract(FssXYZVector inputXYZ)
    {
        return new FssXYZVector(X - inputXYZ.X, Y - inputXYZ.Y, Z - inputXYZ.Z);
    }
    public FssXYZVector Scale(double scaleFactor)
    {
        return new FssXYZVector(X * scaleFactor, Y * scaleFactor, Z * scaleFactor);
    }

    public FssXYZVector Normalize()
    {
        double mag = Magnitude;
        if (mag < FssConsts.ArbitraryMinDouble)
            return new FssXYZVector(1, 0, 0);
        else
            return new FssXYZVector(X / mag, Y / mag, Z / mag);
    }

    // --------------------------------------------------------------------------------------------

    public FssXYZVector XYZTo(FssXYZVector remoteXYZ)
    {
        return new FssXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    // --------------------------------------------------------------------------------------------
    // operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static FssXYZVector operator +(FssXYZVector a, FssXYZVector b)         { return new FssXYZVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // - operator overload for subtracting points
    public static FssXYZVector operator -(FssXYZVector a, FssXYZVector b)         { return new FssXYZVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

    // * operator overload for scaling a
    public static FssXYZVector operator *(FssXYZVector a, double scaleFactor)    { return new FssXYZVector(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }
    public static FssXYZVector operator *(double scaleFactor, FssXYZVector a)    { return new FssXYZVector(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }

    // / operator overload for scaling a point
    public static FssXYZVector operator /(FssXYZVector a, double scaleFactor)    { return new FssXYZVector(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }
    public static FssXYZVector operator /(double scaleFactor, FssXYZVector a)    { return new FssXYZVector(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }

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

    public static FssXYZVector Diff(FssXYZVector inputXYZ1, FssXYZVector inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return new FssXYZVector(diffX, diffY, diffZ);
    }

    public static FssXYZVector Sum(FssXYZVector inputXYZ1, FssXYZVector inputXYZ2)
    {
        double sumX = inputXYZ2.X + inputXYZ1.X;
        double sumY = inputXYZ2.Y + inputXYZ1.Y;
        double sumZ = inputXYZ2.Z + inputXYZ1.Z;

        return new FssXYZVector(sumX, sumY, sumZ);
    }

    public static FssXYZVector Scale(FssXYZVector inputXYZ1, double scaleFactor)
    {
        double scaledX = inputXYZ1.X * scaleFactor;
        double scaledY = inputXYZ1.Y * scaleFactor;
        double scaledZ = inputXYZ1.Z * scaleFactor;

        return new FssXYZVector(scaledX, scaledY, scaledZ);
    }

}
