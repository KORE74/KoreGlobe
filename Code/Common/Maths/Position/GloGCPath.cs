using System;

public class GloGCPath
{
    public GloXYZPoint StartPos     { get; private set; }
    public GloXYZPoint EndPos       { get; private set; }

    private double     RefAngleRads { get; set; } // Internal pre-calculated reference angle

    // Initialize the path with start and end positions
    public GloGCPath(GloLLAPoint startLLA, GloLLAPoint endLLA)
    {
        this.StartPos     = startLLA.ToXYZ();
        this.EndPos       = endLLA.ToXYZ();
        this.RefAngleRads = GloXYZPointOperations.AngleBetweenRads(this.StartPos, this.EndPos);
    }

    public GloGCPath(GloLLAPoint startLLA, double heading)
    {
        // Extrapolate point along heading to create second LLA point.
        // Assuming an arbitrary distance to create a meaningful second point.
        double arbitraryDistance = 10000; // 10 km for instance
        GloLLAPoint endLLA = startLLA.PlusRangeBearing(new GloRangeBearing(arbitraryDistance, heading));

        // Call other constructor to create object
        this.StartPos = startLLA.ToXYZ();
        this.EndPos   = endLLA.ToXYZ();
        this.RefAngleRads = GloXYZPointOperations.AngleBetweenRads(this.StartPos, this.EndPos);
    }

    // Calculate position along the path given a fraction
    public GloXYZPoint PositionAtFractionOfRoute(double fraction)
    {
        // Handle edge cases
        if (RefAngleRads < GloConsts.ArbitraryMinDouble)
            return StartPos;

        // Spherical Linear Interpolation (SLERP)
        return GloXYZPointOperations.Slerp(StartPos, EndPos, fraction);
    }

    public GloXYZPoint PositionAtFractionOfFullCircle(double fraction)
    {
        // Normalize the fraction to ensure it's between 0 and 1
        fraction = fraction % 1;

        // Scale the fraction to the full circle angle
        double angleForFullCircle = fraction * 2 * Math.PI;

        // Scale the angle relative to the reference angle
        double scaledAngle = angleForFullCircle / RefAngleRads;

        // Use the Slerp function for the scaled angle
        return GloXYZPointOperations.Slerp(StartPos, EndPos, scaledAngle);
    }

    public GloXYZPoint PositionAtDistance(double distAlongRouteM)
    {
        // Find the fraction of the path covered by the given distance
        double totalPathDistance = this.PathDistance();
        double fraction = distAlongRouteM / totalPathDistance;

        // Calculate the position at the given fraction
        return this.PositionAtFractionOfRoute(fraction);
    }

    // Calculate the total distance of the path
    public double PathDistance()
    {
        // Assuming radius is the same for both points or taking the average if different
        double radius = (StartPos.Magnitude + EndPos.Magnitude) / 2;
        return radius * RefAngleRads;
    }
}