using System;

// GlobePlatformRouteSimple: Two points, and a speed to traverse between them. Straight line, no attitude changes.

public class GlobePlatformRouteSimple : GlobePlatformRoute
{
    public GlobeLLA StartPoint { set; get; } = GlobeLLA.Zero;
    public GlobeLLA EndPoint { set; get; } = GlobeLLA.Zero;
    public double SpeedMPS { set; get; } = 0;

    public override GlobeLLA PositionForTime(float simulationTimeSecs)
    {
        // Get the total route distance
        double distRoute = StartPoint.CurvedDistanceToM(EndPoint);

        // Return a default value if route is too short.
        if (distRoute < GlobeConsts.ArbitraryMinValue)
            return StartPoint;

        // Find the required distance for the time
        double distForTime = SpeedMPS * simulationTimeSecs;

        // Return the end point if the route is complete
        if (distForTime > distRoute)
            return EndPoint;

        // Calc the mid-route point
        double distFraction = distForTime / distRoute;
        GlobeLLA routeLLA = StartPoint.RhumbLineLerp(EndPoint, distFraction);

        return routeLLA;
    }


}