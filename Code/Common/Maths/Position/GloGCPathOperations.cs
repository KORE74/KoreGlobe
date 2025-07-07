using System;
using System.Collections.Generic;

// GloLLAPointOperations: A static class to hold operations on GloLLAPoint objects that are not part of its
// core responsibilites. This class is static, all operations are stateless and return a new object.

// Design Decisions:
// - The LLA Point code uses a struct rather than an immutable class, as the constructor options with flexible units
//   are simply too useful. We rely on the struct's pass by value to avoid issues with mutability.

public static class GloGCPathOperations
{
    // Usage: GloGCPathOperations.PointsOnGCPath(startLLA, endLLA, 1000)
    public static List<GloLLAPoint> PointsOnGCPath(GloLLAPoint startLLA, GloLLAPoint endLLA, double sepDistM)
    {
        // Create the returning list and add the start point
        List<GloLLAPoint> retList = new List<GloLLAPoint>();
        retList.Add(startLLA);

        // Create the GC path and find its distance, to determine the intermediary points along the path
        GloGCPath    gcPath   = new GloGCPath(startLLA, endLLA);
        double       pathLenM = gcPath.PathDistance();
        //const double minDistM = 1000; // 1 km minimum distance

        // Calculate the number of points to add
        int numPoints = (int)(pathLenM / sepDistM);
        double stepFraction = 1.0 / numPoints;

        for (int i = 1; i < numPoints; i++)
        {
            double distAlongPath = i * sepDistM;
            GloLLAPoint newPoint = GloLLAPoint.FromXYZ(gcPath.PositionAtFractionOfRoute(stepFraction));
            retList.Add(newPoint);
        }
        retList.Add(endLLA);

        return retList;
    }

    public static List<GloLLAPoint> PointsOnGCPath2(GloLLAPoint startLLA, GloLLAPoint endLLA, int numPoints)
    {
        // Create the returning list and add the start point
        List<GloLLAPoint> retList = new List<GloLLAPoint>();
        retList.Add(startLLA);

        // Create the GC path and find its distance, to determine the intermediary points along the path
        GloGCPath    gcPath   = new GloGCPath(startLLA, endLLA);

        // Calculate the number of points to add
        double stepFraction = 1.0 / numPoints;

        for (int i = 1; i < numPoints; i++)
        {
            double currFraction = stepFraction * i;
            GloLLAPoint newPoint = GloLLAPoint.FromXYZ(gcPath.PositionAtFractionOfRoute(stepFraction));
            retList.Add(newPoint);
        }
        retList.Add(endLLA);

        return retList;
    }

}