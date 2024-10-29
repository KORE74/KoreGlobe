using System;
using System.Collections.Generic;

// A common class that represents a route. Each leg on the route could be from a separate
// class, so we will use common interface for each, but their implementation will be different.

// Design Decisions:
// - We will use a list of legs to represent the route.

public class FssRoute
{
    public List<FssRouteLeg> Legs;

    // --------------------------------------------------------------------------------------------
    // Constructors
    // --------------------------------------------------------------------------------------------

    public FssRoute(List<FssRouteLeg> legs)
    {
        Legs = legs;
    }

    // --------------------------------------------------------------------------------------------
    // Public Methods
    // --------------------------------------------------------------------------------------------

    public double GetDistanceM()
    {
        double distanceM = 0;
        foreach (FssRouteLeg leg in Legs)
        {
            distanceM += leg.GetDistanceM();
        }
        return distanceM;
    }

    public double GetDurationS()
    {
        double durationS = 0;
        foreach (FssRouteLeg leg in Legs)
        {
            durationS += leg.GetDurationS();
        }
        return durationS;
    }

    public FssLLAPoint PositionAtTime(double routeTimeS)
    {
        double timeRemainingS = routeTimeS;
        foreach (FssRouteLeg leg in Legs)
        {
            if (timeRemainingS < leg.GetDurationS())
            {
                return leg.PositionAtTime(timeRemainingS);
            }
            timeRemainingS -= leg.GetDurationS();
        }
        return Legs[Legs.Count - 1].EndPoint;
    }

    public FssAttitude CurrentAttitude(double routeTimeS)
    {
        // This is a placeholder for now. We will implement this later.
        return new FssAttitude(0, 0, 0);
    }
}