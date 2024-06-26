using System;
using System.Collections.Generic;

public class FssXYPolyLine : FssXY
{
    public List<FssXYPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      FssXYPolyLine polyLine = new FssXYPolyLine (new List<FssXYPoint> { 
    //          new FssXYPoint(0, 0), 
    //          new FssXYPoint(1, 1), 
    //          new FssXYPoint(2, 2) });

    public FssXYPolyLine(List<FssXYPoint> points)
    {
        Points = points;
    }

    public FssXYPolyLine(FssXYPolyLine polyLine)
    {
        Points = new List<FssXYPoint>(polyLine.Points);
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

    public FssXYLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new FssXYLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<FssXYLine> SubLines()
    {
        List<FssXYLine> lines = new List<FssXYLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public FssXYPolyLine Offset(double x, double y)
    {
        List<FssXYPoint> newPoints = new List<FssXYPoint>();
        foreach (FssXYPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y));
        }
        return new FssXYPolyLine(newPoints);
    }

    public FssXYPolyLine Offset(FssXYPoint xy)
    {
        List<FssXYPoint> newPoints = new List<FssXYPoint>();
        foreach (FssXYPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new FssXYPolyLine(newPoints);
    }



}