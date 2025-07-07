using System;

// KoreXYZPoint: A class to hold an XYZ position. Units are abstract.
// Class has operations to move the points as required by a 3D viewer.

namespace KoreCommon;

public struct KoreXYZLine
{
    // Read-only properties
    public KoreXYZPoint P1 { get; }
    public KoreXYZPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // MARK: Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Length { get { return P1.DistanceTo(P2); } }
    public KoreXYZVector DirectionVector { get { return P1.VectorTo(P2); } }
    public KoreXYZVector DirectionUnitVector { get { return P1.UnitVectorTo(P2); } }
    public KoreXYZPoint MidPoint { get { return Fraction(0.5); } }

    // --------------------------------------------------------------------------------------------
    // Position methods
    // --------------------------------------------------------------------------------------------

    public KoreXYZPoint Fraction(double fraction)
    {
        KoreXYZVector direction = P1.XYZTo(P2);
        KoreXYZVector scaledDirection = direction * fraction;
        KoreXYZPoint newPnt = P1.Offset(scaledDirection);
        return newPnt;
    }

    // Extrapolate the line by a distance. -ve is back from P1, +ve is forward from P2
    public KoreXYZPoint ExtrapolateDistance(double distance)
    {
        KoreXYZVector directionUnitVector = P1.XYZTo(P2).Normalize();
        KoreXYZVector scaledDirection = directionUnitVector * distance;

        if (distance < 0)
        {
            // Backtrack from P1
            scaledDirection = scaledDirection.Invert();
            return P1.Offset(scaledDirection);
        }
        else
        {
            // Move forward from P1
            return P2.Offset(scaledDirection);
        }

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public KoreXYZLine(KoreXYZPoint inP1, KoreXYZPoint inP2)
    {
        P1 = inP1;
        P2 = inP2;
    }

    // --------------------------------------------------------------------------------------------
    // Normalize - set the line as a unit vector from P1 to P2, where P1 is Zero.
    // --------------------------------------------------------------------------------------------

    public KoreXYZLine Normalize()
    {
        var direction = this.DirectionUnitVector;
        return new KoreXYZLine(KoreXYZPoint.Zero, KoreXYZPoint.Zero.Offset(direction));
    }
}
