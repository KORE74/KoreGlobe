using System;
using System.Collections.Generic;

public class FssPlatformElementRoute : FssPlatformElement
{
    // A simple route being a list of FssLLAPoint values.
    public List<FssLLAPoint> RoutePoints { set; get; } = new List<FssLLAPoint>();

    // ---------------------------------------------------------------------------------------------
    // MARK: Update
    // ---------------------------------------------------------------------------------------------

    public void AddPoint(FssLLAPoint point)
    {
        RoutePoints.Add(point);
    }

    public void Clear() => RoutePoints.Clear();

    // ---------------------------------------------------------------------------------------------
    // MARK: Query
    // ---------------------------------------------------------------------------------------------

    public int         PointCount()     => RoutePoints.Count;
    public FssLLAPoint Point(int index) => RoutePoints[index];

    public override string Report()
    {
        return $"Route: {RoutePoints.Count} points // Start:{RoutePoints[0]} // End:{RoutePoints[RoutePoints.Count - 1]}";
    }

}

