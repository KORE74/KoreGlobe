using System;
using System.Collections.Generic;

public class GloXYPolyLine : GloXY
{
    public List<GloXYPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      GloXYPolyLine polyLine = new GloXYPolyLine (new List<GloXYPoint> {
    //          new GloXYPoint(0, 0),
    //          new GloXYPoint(1, 1),
    //          new GloXYPoint(2, 2) });

    public GloXYPolyLine(List<GloXYPoint> points)
    {
        Points = points;
    }

    public GloXYPolyLine(GloXYPolyLine polyLine)
    {
        Points = new List<GloXYPoint>(polyLine.Points);
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    public double Length()
    {
        double length = 0;
        for (int i = 0; i < Points.Count - 1; i++)
        {
            length += Points[i].DistanceTo(Points[i + 1]);
        }
        return length;
    }

    // --------------------------------------------------------------------------------------------
    // Sublines
    // --------------------------------------------------------------------------------------------

    public int NumSubLines()
    {
        return Points.Count - 1;
    }

    public GloXYLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new GloXYLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<GloXYLine> SubLines()
    {
        List<GloXYLine> lines = new List<GloXYLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public GloXYPolyLine Offset(double x, double y)
    {
        List<GloXYPoint> newPoints = new List<GloXYPoint>();
        foreach (GloXYPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y));
        }
        return new GloXYPolyLine(newPoints);
    }

    public GloXYPolyLine Offset(GloXYVector xy)
    {
        List<GloXYPoint> newPoints = new List<GloXYPoint>();
        foreach (GloXYPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new GloXYPolyLine(newPoints);
    }



}