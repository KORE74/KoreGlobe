using System;
using System.Collections.Generic;

// A common class that represents a route. Each leg on the route could be from a separate
// class, so we will use common interface for each, but their implementation will be different.

// Design Decisions:
// - We will use a list of legs to represent the route.

public class FssRoute
{
    public List<FSSRouteLegPointToPoint> Legs;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssRoute(List<FSSRouteLegPointToPoint> legs)
    {
        Legs = legs;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    public double GetDistanceM()
    {
        double distanceM = 0;
        foreach (FSSRouteLegPointToPoint leg in Legs)
        {
            distanceM += leg.GetDistanceM();
        }
        return distanceM;
    }

    public double GetDurationS()
    {
        double durationS = 0;
        foreach (FSSRouteLegPointToPoint leg in Legs)
        {
            durationS += leg.GetDurationS();
        }
        return durationS;
    }

    public FssLLAPoint CurrentPosition(double timeS)
    {
        double timeRemainingS = timeS;
        foreach (FSSRouteLegPointToPoint leg in Legs)
        {
            if (timeRemainingS < leg.GetDurationS())
            {
                return leg.CurrentPosition(timeRemainingS);
            }
            timeRemainingS -= leg.GetDurationS();
        }
        return Legs[Legs.Count - 1].EndPoint;
    }

    public FssAttitude CurrentAttitude(double timeS)
    {   
        // This is a placeholder for now. We will implement this later.
        return new FssAttitude(0, 0, 0);
    }
}