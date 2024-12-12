using System;
using System.Text.Json.Serialization;

// FssXYZPoint: A class to hold an XYZ position. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

public class FssXYZPoint : FssXYZ
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    [JsonIgnore]
    public double Magnitude {
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

    public FssXYZPoint(double xm, double ym, double zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // Return a zero point as a default value
    // Example: FssXYZPoint newPos = FssXYZPoint.Zero();
    public static FssXYZPoint Zero => new FssXYZPoint(0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Basic operations
    // --------------------------------------------------------------------------------------------

    public FssXYZPoint Offset(double x, double y, double z)
    {
        return new FssXYZPoint(X + x, Y + y, Z + z);
    }
    public FssXYZPoint Offset(FssXYZPoint shiftXYZ)
    {
        return new FssXYZPoint(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);
    }
    public FssXYZPoint Subtract(FssXYZPoint inputXYZ)
    {
        return new FssXYZPoint(X - inputXYZ.X, Y - inputXYZ.Y, Z - inputXYZ.Z);
    }
    public FssXYZPoint Scale(double scaleFactor)
    {
        return new FssXYZPoint(X * scaleFactor, Y * scaleFactor, Z * scaleFactor);
    }

    public FssXYZPoint Normalize()
    {
        double mag = Magnitude;
        if (mag < FssConsts.ArbitraryMinDouble)
            return new FssXYZPoint(1, 0, 0);
        else
            return new FssXYZPoint(X / mag, Y / mag, Z / mag);
    }

    // --------------------------------------------------------------------------------------------

    public FssXYZPoint XYZTo(FssXYZPoint remoteXYZ)
    {
        return new FssXYZPoint(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    public double DistanceTo(FssXYZPoint inputXYZ)
    {
        double diffX = X - inputXYZ.X;
        double diffY = Y - inputXYZ.Y;
        double diffZ = Z - inputXYZ.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }

    // --------------------------------------------------------------------------------------------
    // Polar Vectors
    // --------------------------------------------------------------------------------------------

    public FssXYZPolarOffset PolarOffsetTo(FssXYZPoint p)
    {
        FssXYZPoint diff = XYZTo(p);

        FssXYZPolarOffset newOffset = FssXYZPolarOffset.FromXYZ(diff);

        return newOffset;
    }

    public FssXYZPoint PlusPolarOffset(GloXYZPolarOffset offset)
    {
        FssXYZPoint diff = offset.ToXYZ();
        return new FssXYZPoint(X + diff.X, Y + diff.Y, Z + diff.Z);
    }

    // --------------------------------------------------------------------------------------------
    // operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static FssXYZPoint operator +(FssXYZPoint a, FssXYZPoint b)         { return new FssXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // - operator overload for subtracting points
    public static FssXYZPoint operator -(FssXYZPoint a, FssXYZPoint b)         { return new FssXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

    // * operator overload for scaling a
    public static FssXYZPoint operator *(FssXYZPoint a, double scaleFactor)    { return new FssXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }
    public static FssXYZPoint operator *(double scaleFactor, FssXYZPoint a)    { return new FssXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }

    // / operator overload for scaling a point
    public static FssXYZPoint operator /(FssXYZPoint a, double scaleFactor)    { return new FssXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }
    public static FssXYZPoint operator /(double scaleFactor, FssXYZPoint a)    { return new FssXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }


    // Vector operators
    public static FssXYZPoint operator -(FssXYZPoint a, FssXYZVector b)       { return new FssXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
    public static FssXYZPoint operator +(FssXYZPoint a, FssXYZVector b)       { return new FssXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }


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

    public static FssXYZPoint Diff(FssXYZPoint inputXYZ1, FssXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return new FssXYZPoint(diffX, diffY, diffZ);
    }

    public static FssXYZPoint Sum(FssXYZPoint inputXYZ1, FssXYZPoint inputXYZ2)
    {
        double sumX = inputXYZ2.X + inputXYZ1.X;
        double sumY = inputXYZ2.Y + inputXYZ1.Y;
        double sumZ = inputXYZ2.Z + inputXYZ1.Z;

        return new FssXYZPoint(sumX, sumY, sumZ);
    }

    public static FssXYZPoint Scale(FssXYZPoint inputXYZ1, double scaleFactor)
    {
        double scaledX = inputXYZ1.X * scaleFactor;
        double scaledY = inputXYZ1.Y * scaleFactor;
        double scaledZ = inputXYZ1.Z * scaleFactor;

        return new FssXYZPoint(scaledX, scaledY, scaledZ);
    }

    // Static method to calculate the distance between two points
    public static double DistanceBetween(FssXYZPoint inputXYZ1, FssXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }
}
