
// Route legs are the building blocks of a route. They are the segments of the route that connect two points.
// We need operations to establish the entry and exit crietia of each leg and the position/attitude along it to
// control the platform.

public class FssRouteLeg
{
    // Position Operations
    public FssLLAPoint StartPoint { get; set; }
    public FssLLAPoint EndPoint   { get; set; }

    // Attitude Operations
    public FssAttitude StartAttitude { get; set; }
    public FssAttitude EndAttitude   { get; set; }

    // Attitude Delta Operations
    public FssAttitudeDelta StartAttitudeDelta { get; set; }
    public FssAttitudeDelta EndAttitudeDelta   { get; set; }

    // Distance based Operations
    public double GetDistanceM() => 0;
    public double GetDurationS() => 0;


    // Time based Operations - time is only relevant for the leg, zero being the start point.
    public FssLLAPoint      PositionAtTime(double legtimeS)      => FssLLAPoint.Zero;
    public FssAttitude      AttitudeAtTime(double legtimeS)      => FssAttitude.Zero;
    public FssAttitudeDelta AttitudeDeltaAtTime(double legtimeS) => FssAttitudeDelta.Zero;
}
