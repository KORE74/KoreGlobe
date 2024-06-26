using System;

#nullable enable

public static class FssXYPolygonOperations
{
    public static FssXYLine? ClipLineToPolygon(this FssXYLine line, FssXYPolygon polygon)
    {
        FssXYLine? clippedLine = null;
        FssXYPoint? p1 = null;
        FssXYPoint? p2 = null;

        for (int i = 0; i < polygon.Vertices.Count; i++)
        {
            FssXYPoint? intersection = FssXYLineOperations.Intersection(line, new FssXYLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count]));

            if (intersection != null)
            {
                if (p1 == null)
                {
                    p1 = intersection;
                }
                else if (p2 == null)
                {
                    p2 = intersection;
                }
                else
                {
                    if (p1.DistanceTo(line.P1) > p2.DistanceTo(line.P1))
                    {
                        p1 = p2;
                        p2 = intersection;
                    }
                    else
                    {
                        p2 = intersection;
                    }
                }
            }
        }

        if (p1 != null && p2 != null)
        {
            clippedLine = new FssXYLine(p1, p2);
        }

        return clippedLine;
    }
}