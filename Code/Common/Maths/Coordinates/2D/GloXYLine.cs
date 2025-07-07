using System;

public class GloXYLine : GloXY
{
    public GloXYPoint P1 { get; }
    public GloXYPoint P2 { get; }

    // --------------------------------------------------------------------------------------------
    // Read only / derived attributes
    // --------------------------------------------------------------------------------------------

    public double     Length    => P1.DistanceTo(P2);
    public GloXYPoint Direction => P2 - P1; // return a position as the vector from 0,0

    public GloXYPoint DirectionUnitVector { // return a direction, with a magniude of 1
        get
        {
            if (Length == 0) return new GloXYPoint(0, 0);
            return Direction * (1 / Length);
        }
    }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public GloXYLine(double x1, double y1, double x2, double y2)
    {
        P1 = new GloXYPoint(x1, y1);
        P2 = new GloXYPoint(x2, y2);
    }

    public GloXYLine(GloXYPoint p1, GloXYPoint p2)
    {
        P1 = p1;
        P2 = p2;
    }

    public GloXYLine(GloXYLine line)
    {
        P1 = line.P1;
        P2 = line.P2;
    }

    // --------------------------------------------------------------------------------------------
    // Position methods
    // --------------------------------------------------------------------------------------------

    // Get the centre point of the line

    public GloXYPoint MidPoint()
    {
        return new GloXYPoint((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
    }

    public GloXYPoint Fraction(double fraction)
    {
        // Get the point at a given fraction along the line
        // - 0 = P1, 1 = P2, 0.5 = midpoint
        // - We allow -ve value, backtracking from before P1, along the P1-P2 line.
        // - We allow > 1, to go past P2, along the P1-P2 line.

        double dx = P2.X - P1.X;
        double dy = P2.Y - P1.Y;

        double newX = P1.X + (dx * fraction);
        double newY = P1.Y + (dy * fraction);

        return new GloXYPoint(newX, newY);
    }

    // Extrapolate the line by a distance. -ve is back from P1, +ve is forward from P2
    public GloXYPoint ExtrapolateDistance(double distance)
    {
        double dx = P2.X - P1.X;
        double dy = P2.Y - P1.Y;

        double newX = P1.X + (dx * distance / Length);
        double newY = P1.Y + (dy * distance / Length);

        return new GloXYPoint(newX, newY);
    }

    // --------------------------------------------------------------------------------------------
    // Line methods
    // --------------------------------------------------------------------------------------------

    // Return a new line object, will all points offset by an XY amount.

    public GloXYLine Offset(double x, double y)
    {
        return new GloXYLine(P1.Offset(x, y), P2.Offset(x, y));
    }

    public GloXYLine Offset(GloXYVector xy)
    {
        return new GloXYLine(P1.Offset(xy), P2.Offset(xy));
    }

    // --------------------------------------------------------------------------------------------
    // Misc methods
    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"({P1.X:F3}, {P1.Y:F3}) -> ({P2.X:F3}, {P2.Y:F3})";
    }
}