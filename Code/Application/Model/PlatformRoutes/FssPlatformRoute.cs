using System;

// FssPlatformRoute: Class representing a series of move instructions. Getting the desired position for a point in time can inform move
// updates in the kinetics object, so the route is an "aspriational" value, the kinetics is a "real" value, and functions continue to update
// between the two based on the performance characteristics of the platform.

// The route will run on a "simulation elapsed time in seconds", being able to provide a position for any given moment.

public abstract class FssPlatformRoute
{
    public string Name { set; get; } = "unnamed-route";

    public abstract FssLLAPoint PositionForTime(float simulationTimeSecs);
}

