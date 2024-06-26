using System;

public class FssGCPath
{
    public FssXYZPoint StartPos { get; private set; }
    public FssXYZPoint EndPos { get; private set; }
    private double RefAngle { get; set; } // Pre-calculated reference angle

    // Initialize the path with start and end positions
    public FssGCPath(FssLLAPoint startLLA, FssLLAPoint endLLA)
    {
        this.StartPos = startLLA.ToXYZ();
        this.EndPos = endLLA.ToXYZ();
        this.RefAngle = FssXYZPointOperations.AngleBetweenRads(this.StartPos, this.EndPos);
    }

    public FssGCPath(FssLLAPoint startLLA, double heading)
    {
        // Extrapolate point along heading to create second LLA point.
        // Assuming an arbitrary distance to create a meaningful second point.
        double arbitraryDistance = 10000; // 10 km for instance
        FssLLAPoint endLLA = startLLA.PlusRangeBearing(new FssRangeBearing(arbitraryDistance, heading));

        // Call other constructor to create object
        this.StartPos = startLLA.ToXYZ();
        this.EndPos   = endLLA.ToXYZ();
        this.RefAngle = FssXYZPointOperations.AngleBetweenRads(this.StartPos, this.EndPos);
    }

    // Calculate position along the path given a fraction
    public FssXYZPoint PositionAtFractionOfRoute(double fraction)
    {
        // Handle edge cases
        if (RefAngle < FssConsts.ArbitraryMinDouble)
            return StartPos;

        // Spherical Linear Interpolation (SLERP)
        return FssXYZPointOperations.Slerp(StartPos, EndPos, fraction);
    }

    public FssXYZPoint PositionAtFractionOfFullCircle(double fraction)
    {
        // Normalize the fraction to ensure it's between 0 and 1
        fraction = fraction % 1;

        // Scale the fraction to the full circle angle
        double angleForFullCircle = fraction * 2 * Math.PI;

        // Scale the angle relative to the reference angle
        double scaledAngle = angleForFullCircle / RefAngle;

        // Use the Slerp function for the scaled angle
        return FssXYZPointOperations.Slerp(StartPos, EndPos, scaledAngle);
    }

    public FssXYZPoint PositionAtDistance(double distAlongRouteM)
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
        return radius * RefAngle;
    }
}