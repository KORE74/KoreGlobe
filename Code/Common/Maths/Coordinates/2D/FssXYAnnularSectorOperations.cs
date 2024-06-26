using System;
using System.Collections.Generic;

#nullable enable

public static class FssXYAnnularSectorOperations
{
    // --------------------------------------------------------------------------------------------
    // Intersections
    // --------------------------------------------------------------------------------------------

    public static List<FssXYPoint> IntersectionPoints(FssXYAnnularSector sector, FssXYLine line)
    {
        List<FssXYPoint> intersectionPoints = new List<FssXYPoint>();

        // Break up the shape into two arcs and two lines, then use the intersection methods for each
        List<FssXYPoint> innerArcIntersections = FssXYArcOperations.IntersectionPoints(sector.InnerArc, line);
        List<FssXYPoint> outerArcIntersections = FssXYArcOperations.IntersectionPoints(sector.OuterArc, line);
        FssXYPoint? startIntersection          = FssXYLineOperations.Intersection(sector.StartInnerOuterLine, line);
        FssXYPoint? endIntersection            = FssXYLineOperations.Intersection(sector.EndInnerOuterLine, line);

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
