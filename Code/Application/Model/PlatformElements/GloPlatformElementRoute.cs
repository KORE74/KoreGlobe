using System;
using System.Collections.Generic;

public class GloPlatformElementRoute : GloPlatformElement
{
    // A simple route being a list of GloLLAPoint values.
    public List<GloLLAPoint> RoutePoints { set; get; } = new List<GloLLAPoint>();

    public override string Type => "Route";

    // --------------------------------------------------------------------------------------------
    // MARK Report
    // --------------------------------------------------------------------------------------------

    public override string Report()
    {
        return $"Route: {RoutePoints.Count} points // Start:{RoutePoints[0]} // End:{RoutePoints[RoutePoints.Count - 1]}";
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Update
    // ---------------------------------------------------------------------------------------------

    public void AddPoint(GloLLAPoint point)
    {
        RoutePoints.Add(point);
    }

    public void AddPoints(List<GloLLAPoint> points)
    {
        RoutePoints.AddRange(points);
    }

    public void Clear() => RoutePoints.Clear();

    // ---------------------------------------------------------------------------------------------
    // MARK: Query
    // ---------------------------------------------------------------------------------------------

    public List<GloLLAPoint> Points => RoutePoints;


    public int         PointCount()     => RoutePoints.Count;
    public GloLLAPoint Point(int index) => RoutePoints[index];



}

