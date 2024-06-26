using System;
using System.Collections.Generic;

/// <summary>
/// Represents an immutable 2D polygon on a Fss. The polygon is considered closed,
/// with an implicit final line connecting the last and first points. There's no need
/// to repeat the first point at the end of the vertices list.
/// </summary>
public class FssXYPolygon : FssXY
{
    public IReadOnlyList<FssXYPoint> Vertices { get; }

    public FssXYPolygon(IEnumerable<FssXYPoint> vertices)
    {
        Vertices = new List<FssXYPoint>(vertices).AsReadOnly();
    }

    // --------------------------------------------------------------------------------------------
    // Public methods
    // --------------------------------------------------------------------------------------------

    // Determine the area of the polygon - for a simple non-intersecting case.

    public double Area()
    {
        double area = 0;
        for (int i = 0; i < Vertices.Count; i++)
        {
            int j = (i + 1) % Vertices.Count;
            area += Vertices[i].X * Vertices[j].Y - Vertices[j].X * Vertices[i].Y;
        }
        return Math.Abs(area / 2); // Absolute value for area
    }

    // offset each point by a given x y

    public FssXYPolygon Offset(double x, double y)
    {
        List<FssXYPoint> newVertices = new List<FssXYPoint>();
        foreach (FssXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(x, y));
        }
        return new FssXYPolygon(newVertices);
    }    

    // offset each point in the polygon by the given vector

    public FssXYPolygon Offset(FssXYPoint xy)
    {
        List<FssXYPoint> newVertices = new List<FssXYPoint>();
        foreach (FssXYPoint vertex in Vertices)
        {
            newVertices.Add(vertex.Offset(xy));
        }
        return new FssXYPolygon(newVertices);
    }
}
