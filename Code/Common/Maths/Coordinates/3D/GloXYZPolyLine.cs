using System;
using System.Collections.Generic;

public class GloXYZPolyLine : GloXYZ
{
    public List<GloXYZPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      GloXYPolyLine polyLine = new GloXYPolyLine (new List<GloXYPoint> {
    //          new GloXYPoint(0, 0),
    //          new GloXYPoint(1, 1),
    //          new GloXYPoint(2, 2) });

    public GloXYZPolyLine(List<GloXYZPoint> points)
    {
        Points = points;
    }

    public GloXYZPolyLine(GloXYZPolyLine polyLine)
    {
        Points = new List<GloXYZPoint>(polyLine.Points);
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

    public GloXYZLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new GloXYZLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<GloXYZLine> SubLines()
    {
        List<GloXYZLine> lines = new List<GloXYZLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public GloXYZPolyLine Offset(double x, double y, double z)
    {
        List<GloXYZPoint> newPoints = new List<GloXYZPoint>();
        foreach (GloXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y, z));
        }
        return new GloXYZPolyLine(newPoints);
    }

    public GloXYZPolyLine Offset(GloXYZPoint xy)
    {
        List<GloXYZPoint> newPoints = new List<GloXYZPoint>();
        foreach (GloXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new GloXYZPolyLine(newPoints);
    }

}