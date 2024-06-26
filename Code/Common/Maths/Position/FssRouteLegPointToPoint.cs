// FSSRouteLegPointToPoint: A class that has a start and end point for straight line route leg, along with speed.

using System;

public class FSSRouteLegPointToPoint
{
    public FssLLAPoint StartPoint;
    public FssLLAPoint EndPoint;
    public double SpeedMPS;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FSSRouteLegPointToPoint(FssLLAPoint startPoint, FssLLAPoint endPoint, double speedMPS)
    {
        StartPoint = startPoint;
        EndPoint   = endPoint;
        SpeedMPS   = speedMPS;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    public double GetDistanceM()
    {
        return StartPoint.CurvedDistanceToM(EndPoint);
    }

    public double GetDurationS()
    {
        return GetDistanceM() / SpeedMPS;
    }

    public FssLLAPoint CurrentPosition(double timeS)
    {
        double distanceM = SpeedMPS * timeS;
        double fraction = distanceM / GetDistanceM();
        return FssLLAPointOperations.RhumbLineInterpolation(StartPoint, EndPoint, fraction);
    }
}