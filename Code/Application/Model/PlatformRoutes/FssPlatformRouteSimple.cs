using System;

// FssPlatformRouteSimple: Two points, and a speed to traverse between them. Straight line, no attitude changes.

public class FssPlatformRouteSimple : FssPlatformRoute
{
    public FssLLAPoint StartPoint { set; get; } = FssLLAPoint.Zero;
    public FssLLAPoint EndPoint { set; get; } = FssLLAPoint.Zero;
    public double SpeedMPS { set; get; } = 0;

    public override FssLLAPoint PositionForTime(float simulationTimeSecs)
    {
        // // Get the total route distance
        // double distRoute = StartPoint.CurvedDistanceToM(EndPoint);

        // // Return a default value if route is too short.
        // if (distRoute < FssConsts.ArbitraryMinValue)
        //     return StartPoint;

        // // Find the required distance for the time
        // double distForTime = SpeedMPS * simulationTimeSecs;

        // // Return the end point if the route is complete
        // if (distForTime > distRoute)
        //     return EndPoint;

        // // Calc the mid-route point
        // double distFraction = distForTime / distRoute;
        // FssLLAPoint routeLLA = StartPoint.RhumbLineLerp(EndPoint, distFraction);

        // return routeLLA;

        return FssLLAPoint.Zero;
    }


}