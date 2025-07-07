using System;

// GloXYZPoint: A class to hold an XYZ position. Units are abstract.
// Class has operations to move the points as required by a 3D viewer.

public struct GloXYZLine
{
    // Read-only properties
    public GloXYZPoint P1 { get; }
    public GloXYZPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // MARK: Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Length                    { get { return P1.DistanceTo(P2);   } }
    public GloXYZVector DirectionVector     { get { return P1.VectorTo(P2);     } }
    public GloXYZVector DirectionUnitVector { get { return P1.UnitVectorTo(P2); } }
    public GloXYZPoint MidPoint             { get { return Fraction(0.5);       } }

    // --------------------------------------------------------------------------------------------
    // Position methods
    // --------------------------------------------------------------------------------------------

    public GloXYZPoint Fraction(double fraction)
    {
        GloXYZVector direction       = P1.XYZTo(P2);
        GloXYZVector scaledDirection = direction * fraction;
        GloXYZPoint  newPnt          = P1.Offset(scaledDirection);
        return newPnt;
    }

    // Extrapolate the line by a distance. -ve is back from P1, +ve is forward from P2
    public GloXYZPoint ExtrapolateDistance(double distance)
    {
        GloXYZVector directionUnitVector = P1.XYZTo(P2).Normalize();
        GloXYZVector scaledDirection     = directionUnitVector * distance;

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

    public GloXYZLine(GloXYZPoint inP1, GloXYZPoint inP2)
    {
        P1 = inP1;
        P2 = inP2;
    }

    // --------------------------------------------------------------------------------------------
    // Normalize - set the line as a unit vector from P1 to P2, where P1 is Zero.
    // --------------------------------------------------------------------------------------------

    public GloXYZLine Normalize()
    {
        var direction = this.DirectionUnitVector;
        return new GloXYZLine(GloXYZPoint.Zero, GloXYZPoint.Zero.Offset(direction));
    }
}
