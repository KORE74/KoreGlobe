using System;
using System.Collections.Generic;

public class FssElementRoute : FssElement
{
    // A simple route being a list of FssLLAPoint values.
    public List<FssLLAPoint> RoutePoints { set; get; } = new List<FssLLAPoint>();

    public override string Type => "Route";

    // --------------------------------------------------------------------------------------------
    // #MARK Report
    // --------------------------------------------------------------------------------------------

    public override string Report()
    {
        return $"Route: {RoutePoints.Count} points // Start:{RoutePoints[0]} // End:{RoutePoints[RoutePoints.Count - 1]}";
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Update
    // ---------------------------------------------------------------------------------------------

    public void AddPoint(FssLLAPoint point)
    {
        RoutePoints.Add(point);
    }

    public void AddPoints(List<FssLLAPoint> points)
    {
        RoutePoints.AddRange(points);
    }

    public void Clear() => RoutePoints.Clear();

    // ---------------------------------------------------------------------------------------------
    // MARK: Query
    // ---------------------------------------------------------------------------------------------

    public List<FssLLAPoint> Points => RoutePoints;


    public int         PointCount()     => RoutePoints.Count;
    public FssLLAPoint Point(int index) => RoutePoints[index];



}

