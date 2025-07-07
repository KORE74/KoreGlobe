using System;
using System.Collections.Generic;

#nullable enable

public static class GloXYAnnularSectorOperations
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static List<GloXYPoint> IntersectionPoints(GloXYAnnularSector sector, GloXYLine line)
    {
        List<GloXYPoint> intersectionPoints = new List<GloXYPoint>();

        // Break up the shape into two arcs and two lines, then use the intersection methods for each
        List<GloXYPoint> innerArcIntersections = GloXYArcOperations.IntersectionPoints(sector.InnerArc, line);
        List<GloXYPoint> outerArcIntersections = GloXYArcOperations.IntersectionPoints(sector.OuterArc, line);
        GloXYPoint? startIntersection          = GloXYLineOperations.Intersection(sector.StartInnerOuterLine, line);
        GloXYPoint? endIntersection            = GloXYLineOperations.Intersection(sector.EndInnerOuterLine, line);

        // Consolidate the results into one list
        if (startIntersection != null)
            intersectionPoints.Add(startIntersection);
        if (endIntersection != null)
            intersectionPoints.Add(endIntersection);
        intersectionPoints.AddRange(innerArcIntersections);
        intersectionPoints.AddRange(outerArcIntersections);

        return intersectionPoints;
    }


}
