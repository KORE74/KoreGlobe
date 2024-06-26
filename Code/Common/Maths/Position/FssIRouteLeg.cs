
// Route legs are the building blocks of a route. They are the segments of the route that connect two points.
// We need operations to establish the entry and exit crietia of each leg and the position/attitude along it to 
// control the platform.

public interface FssIRouteLeg
{
    // Position Operations
    public FssLLAPoint StartPoint();
    public FssLLAPoint EndPoint();
    public FssLLAPoint PositionAtTime(double legtimeS);

    // Attitude Operations
    public FssAttitude StartAttitude();
    public FssAttitude EndAttitude();
    public FssAttitude AttitudeAtTime(double legtimeS);

    // Attitude Delta Operations
    public FssAttitudeDelta StartAttitudeDelta();
    public FssAttitudeDelta EndAttitudeDelta();
    public FssAttitudeDelta AttitudeDeltaAtTime(double legtimeS);
}
