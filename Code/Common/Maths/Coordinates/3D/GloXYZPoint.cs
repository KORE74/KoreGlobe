using System;
using System.Text.Json.Serialization;

// GloXYZPoint: A class to hold an XYZ position. Units are abstract, for the consumer to decide the context.
// This class is immutable, so all operations return a new object.

public struct GloXYZPoint
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

    [JsonIgnore]
    public double Length => Magnitude;

    // --------------------------------------------------------------------------------------------

    public GloXYZPoint(double xm, double ym, double zm)
    {
        this.X = xm;
        this.Y = ym;
        this.Z = zm;
    }

    // Return a zero point as a default value
    // Example: GloXYZPoint newPos = GloXYZPoint.Zero();
    public static GloXYZPoint Zero => new GloXYZPoint(0, 0, 0);

    // --------------------------------------------------------------------------------------------
    // Basic operations
    // --------------------------------------------------------------------------------------------

    public GloXYZPoint Offset(double x, double y, double z) => new GloXYZPoint(X + x, Y + y, Z + z);
    public GloXYZPoint Offset(GloXYZPoint shiftXYZ)         => new GloXYZPoint(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);
    public GloXYZPoint Subtract(GloXYZPoint inputXYZ)       => new GloXYZPoint(X - inputXYZ.X, Y - inputXYZ.Y, Z - inputXYZ.Z);
    public GloXYZPoint Scale(double scaleFactor)            => new GloXYZPoint(X * scaleFactor, Y * scaleFactor, Z * scaleFactor);

    public GloXYZPoint Offset(GloXYZVector shiftXYZ)        => new GloXYZPoint(X + shiftXYZ.X, Y + shiftXYZ.Y, Z + shiftXYZ.Z);

    public GloXYZPoint Normalize()
    {
        double mag = Magnitude;
        if (mag < GloConsts.ArbitraryMinDouble)
            return new GloXYZPoint(1, 0, 0);
        else
            return new GloXYZPoint(X / mag, Y / mag, Z / mag);
    }

    // --------------------------------------------------------------------------------------------

    public GloXYZVector XYZTo(GloXYZPoint remoteXYZ)
    {
        return new GloXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    public GloXYZVector VectorTo(GloXYZPoint remoteXYZ)
    {
        return new GloXYZVector(remoteXYZ.X - X, remoteXYZ.Y - Y, remoteXYZ.Z - Z);
    }

    public GloXYZVector UnitVectorTo(GloXYZPoint remoteXYZ)
    {
        GloXYZVector diff = XYZTo(remoteXYZ);
        return diff.Normalize();
    }


    public double DistanceTo(GloXYZPoint inputXYZ)
    {
        double diffX = X - inputXYZ.X;
        double diffY = Y - inputXYZ.Y;
        double diffZ = Z - inputXYZ.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }

    // --------------------------------------------------------------------------------------------
    // Polar Vectors
    // --------------------------------------------------------------------------------------------

    public GloXYZPolarOffset PolarOffsetTo(GloXYZPoint p)
    {
        GloXYZVector diff = XYZTo(p);

        GloXYZPolarOffset newOffset = GloXYZPolarOffset.FromXYZ(diff);

        return newOffset;
    }

    public GloXYZPoint PlusPolarOffset(GloXYZPolarOffset offset)
    {
        GloXYZVector diff = offset.ToXYZ();
        return new GloXYZPoint(X + diff.X, Y + diff.Y, Z + diff.Z);
    }

    // --------------------------------------------------------------------------------------------
    // operator overloads
    // --------------------------------------------------------------------------------------------

    // + operator overload
    public static GloXYZPoint operator +(GloXYZPoint a, GloXYZPoint b)         { return new GloXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

    // - operator overload for subtracting points
    public static GloXYZPoint operator -(GloXYZPoint a, GloXYZPoint b)         { return new GloXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }

    // * operator overload for scaling a
    public static GloXYZPoint operator *(GloXYZPoint a, double scaleFactor)    { return new GloXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }
    public static GloXYZPoint operator *(double scaleFactor, GloXYZPoint a)    { return new GloXYZPoint(a.X * scaleFactor, a.Y * scaleFactor, a.Z * scaleFactor); }

    // / operator overload for scaling a point
    public static GloXYZPoint operator /(GloXYZPoint a, double scaleFactor)    { return new GloXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }
    public static GloXYZPoint operator /(double scaleFactor, GloXYZPoint a)    { return new GloXYZPoint(a.X / scaleFactor, a.Y / scaleFactor, a.Z / scaleFactor); }

    // Vector operators
    public static GloXYZPoint operator -(GloXYZPoint a, GloXYZVector b)        { return new GloXYZPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z); }
    public static GloXYZPoint operator +(GloXYZPoint a, GloXYZVector b)        { return new GloXYZPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z); }

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

    public static GloXYZPoint Diff(GloXYZPoint inputXYZ1, GloXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return new GloXYZPoint(diffX, diffY, diffZ);
    }

    public static GloXYZPoint Sum(GloXYZPoint inputXYZ1, GloXYZPoint inputXYZ2)
    {
        double sumX = inputXYZ2.X + inputXYZ1.X;
        double sumY = inputXYZ2.Y + inputXYZ1.Y;
        double sumZ = inputXYZ2.Z + inputXYZ1.Z;

        return new GloXYZPoint(sumX, sumY, sumZ);
    }

    public static GloXYZPoint Scale(GloXYZPoint inputXYZ1, double scaleFactor)
    {
        double scaledX = inputXYZ1.X * scaleFactor;
        double scaledY = inputXYZ1.Y * scaleFactor;
        double scaledZ = inputXYZ1.Z * scaleFactor;

        return new GloXYZPoint(scaledX, scaledY, scaledZ);
    }

    // Static method to calculate the distance between two points
    public static double DistanceBetween(GloXYZPoint inputXYZ1, GloXYZPoint inputXYZ2)
    {
        double diffX = inputXYZ2.X - inputXYZ1.X;
        double diffY = inputXYZ2.Y - inputXYZ1.Y;
        double diffZ = inputXYZ2.Z - inputXYZ1.Z;

        return Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
    }
}
