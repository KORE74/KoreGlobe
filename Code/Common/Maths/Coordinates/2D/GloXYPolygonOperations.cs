using System;

#nullable enable

public static class GloXYPolygonOperations
{
    public static GloXYLine? ClipLineToPolygon(this GloXYLine line, GloXYPolygon polygon)
    {
        GloXYLine? clippedLine = null;
        GloXYPoint? p1 = null;
        GloXYPoint? p2 = null;

        for (int i = 0; i < polygon.Vertices.Count; i++)
        {
            GloXYPoint? intersection = GloXYLineOperations.Intersection(line, new GloXYLine(polygon.Vertices[i], polygon.Vertices[(i + 1) % polygon.Vertices.Count]));

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
            clippedLine = new GloXYLine(p1, p2);
        }

        return clippedLine;
    }
}