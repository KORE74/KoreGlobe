using System;
using System.Collections.Generic;

public class FssXYZPolyLine : FssXYZ
{
    public List<FssXYZPoint> Points { get; }

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    // Example declaration:
    //      FssXYPolyLine polyLine = new FssXYPolyLine (new List<FssXYPoint> { 
    //          new FssXYPoint(0, 0), 
    //          new FssXYPoint(1, 1), 
    //          new FssXYPoint(2, 2) });

    public FssXYZPolyLine(List<FssXYZPoint> points)
    {
        Points = points;
    }

    public FssXYZPolyLine(FssXYZPolyLine polyLine)
    {
        Points = new List<FssXYZPoint>(polyLine.Points);
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

    public FssXYZLine SubLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= NumSubLines())
            throw new ArgumentOutOfRangeException("lineIndex", "lineIndex must be between 0 and NumSubLines() - 1");

        return new FssXYZLine(Points[lineIndex], Points[lineIndex + 1]);
    }

    // return a list of line objects, for a caller to then handle separately

    public List<FssXYZLine> SubLines()
    {
        List<FssXYZLine> lines = new List<FssXYZLine>();
        for (int i = 0; i < NumSubLines(); i++)
        {
            lines.Add(SubLine(i));
        }
        return lines;
    }

    // --------------------------------------------------------------------------------------------
    // Edits
    // --------------------------------------------------------------------------------------------

    public FssXYZPolyLine Offset(double x, double y, double z)
    {
        List<FssXYZPoint> newPoints = new List<FssXYZPoint>();
        foreach (FssXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(x, y, z));
        }
        return new FssXYZPolyLine(newPoints);
    }

    public FssXYZPolyLine Offset(FssXYZPoint xy)
    {
        List<FssXYZPoint> newPoints = new List<FssXYZPoint>();
        foreach (FssXYZPoint point in Points)
        {
            newPoints.Add(point.Offset(xy));
        }
        return new FssXYZPolyLine(newPoints);
    }



}