using System;

// FssXYZPoint: A class to hold an XYZ position. Units are abstract.
// Class has operations to move the points as required by a 3D viewer.

public class FssXYZLine : FssXYZ
{
    // Read-only properties
    public FssXYZPoint P1 { get; }
    public FssXYZPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // MARK: Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double Length                   { get { return P1.DistanceTo(P2); } }
    public FssXYZPoint Direction           { get { return FssXYZPoint.Diff(P1, P2); } }
    public FssXYZPoint DirectionUnitVector { get { return FssXYZPoint.Scale(Direction, 1 / Length); } }
    public FssXYZPoint MidPoint            { get { return FssXYZPoint.Scale(FssXYZPoint.Sum(P1, P2), 0.5); } }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZLine(FssXYZPoint inP1, FssXYZPoint inP2)
    {
        P1 = inP1;
        P2 = inP2;
    }

    // --------------------------------------------------------------------------------------------
    // Normalize - set the line as a unit vector from P1 to P2, where P1 is Zero.
    // --------------------------------------------------------------------------------------------

    public FssXYZLine Normalize()
    {
        var direction = this.Direction;
        return new FssXYZLine(new FssXYZPoint(0, 0, 0), FssXYZPoint.Scale(direction, 1 / direction.Magnitude));
    }
}
