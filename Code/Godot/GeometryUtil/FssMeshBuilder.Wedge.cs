using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddShellSegment(
        float azimuthMinDegs, float azimuthMaxDegs,
        float elevationMinDegs, float elevationMaxDegs,
        float distanceMin, float distanceMax,
        int resolutionAz, int resolutionEl)
    {
        // Lists to hold the points for the inside and outside surfaces
        Vector3[,] insidePoints  = new Vector3[resolutionAz + 1, resolutionEl + 1];
        Vector3[,] outsidePoints = new Vector3[resolutionAz + 1, resolutionEl + 1];

        // Lists to hold the points for the ribbons (edges)
        List<Vector3> topInsideEdge     = new List<Vector3>();
        List<Vector3> bottomInsideEdge  = new List<Vector3>();
        List<Vector3> leftInsideEdge    = new List<Vector3>();
        List<Vector3> rightInsideEdge   = new List<Vector3>();
        List<Vector3> topOutsideEdge    = new List<Vector3>();
        List<Vector3> bottomOutsideEdge = new List<Vector3>();
        List<Vector3> leftOutsideEdge   = new List<Vector3>();
        List<Vector3> rightOutsideEdge  = new List<Vector3>();

        // Generate points for the inside and outside surfaces, and the edges
        for (int y = 0; y <= resolutionEl; y++)
        {
            float currElDegs = Mathf.Lerp(elevationMinDegs, elevationMaxDegs, (float)y / resolutionEl);
            for (int x = 0; x <= resolutionAz; x++)
            {
                float currAzDegs = Mathf.Lerp(azimuthMinDegs, azimuthMaxDegs, (float)x / resolutionAz);

                float currAzRads = (float)FssValueUtils.DegsToRads(currAzDegs);
                float currElRads = (float)FssValueUtils.DegsToRads(currElDegs);

                Vector3 outsidePoint = FssGodotGeometryOperations.Vector3FromPolar(Vector3.Zero, currAzRads, currElRads, distanceMax);
                Vector3 insidePoint  = FssGodotGeometryOperations.Vector3FromPolar(Vector3.Zero, currAzRads, currElRads, distanceMin);

                insidePoints[x, y]  = insidePoint;
                outsidePoints[x, y] = outsidePoint;

                // Add points to the edge lists
                if (y == 0)            topInsideEdge.Add(insidePoint);
                if (y == resolutionEl) bottomInsideEdge.Add(insidePoint);
                if (x == 0)            leftInsideEdge.Add(insidePoint);
                if (x == resolutionAz) rightInsideEdge.Add(insidePoint);

                if (y == 0)            topOutsideEdge.Add(outsidePoint);
                if (y == resolutionEl) bottomOutsideEdge.Add(outsidePoint);
                if (x == 0)            leftOutsideEdge.Add(outsidePoint);
                if (x == resolutionAz) rightOutsideEdge.Add(outsidePoint);
            }
        }

        // Add the inside and outside surfaces
        AddSurface(insidePoints, true);
        AddSurface(outsidePoints, false);

        // Add ribbons for the edges
        AddRibbon(topInsideEdge, topOutsideEdge);
        AddRibbon(bottomOutsideEdge, bottomInsideEdge);
        AddRibbon(leftOutsideEdge, leftInsideEdge);
        AddRibbon(rightInsideEdge, rightOutsideEdge);
    }

    // ----------------------------------------------------------------------------------

    public void AddShellSegmentOutline(
        float azimuthMinDegs, float azimuthMaxDegs,
        float elevationMinDegs, float elevationMaxDegs,
        float distanceMin, float distanceMax,
        int resolutionAz, int resolutionEl,
        float insideRadius, float outsideRadius, int segments)
    {
        bool endsClosed = false;

        // Lists to hold the points for the edges
        List<Vector3> topInsideEdge     = new List<Vector3>();
        List<Vector3> bottomInsideEdge  = new List<Vector3>();
        List<Vector3> leftInsideEdge    = new List<Vector3>();
        List<Vector3> rightInsideEdge   = new List<Vector3>();

        List<Vector3> topOutsideEdge    = new List<Vector3>();
        List<Vector3> bottomOutsideEdge = new List<Vector3>();
        List<Vector3> leftOutsideEdge   = new List<Vector3>();
        List<Vector3> rightOutsideEdge  = new List<Vector3>();

        float azDeltaDegs = (azimuthMaxDegs - azimuthMinDegs) / (float)resolutionAz;

        FssCentralLog.AddEntry($"azDeltaDegs:{azDeltaDegs} azimuthMaxDegs:{azimuthMaxDegs} azimuthMinDegs:{azimuthMinDegs} resolutionAz:{resolutionAz}");

        // Generate points for the inside and outside edges
        for (int y = 0; y <= resolutionEl; y++)
        {
            float elDegs = Mathf.Lerp(elevationMinDegs, elevationMaxDegs, (float)y / resolutionEl);
            for (int x = 0; x <= resolutionAz; x++)
            {
                float azDegs = azimuthMinDegs + ((float)x * azDeltaDegs);

                Vector3 insidePoint  = FssGeoConvOperations.RwToGe(distanceMin, azDegs + azDeltaDegs, elDegs);
                Vector3 outsidePoint = FssGeoConvOperations.RwToGe(distanceMax, azDegs + azDeltaDegs, elDegs);

                // Store points in the edge lists
                if (y == 0)            topInsideEdge.Add(insidePoint);
                if (y == resolutionEl) bottomInsideEdge.Add(insidePoint);
                if (x == 0)            leftInsideEdge.Add(insidePoint);
                if (x == resolutionAz) rightInsideEdge.Add(insidePoint);

                if (y == 0)            topOutsideEdge.Add(outsidePoint);
                if (y == resolutionEl) bottomOutsideEdge.Add(outsidePoint);
                if (x == 0)            leftOutsideEdge.Add(outsidePoint);
                if (x == resolutionAz) rightOutsideEdge.Add(outsidePoint);
            }
        }

        // Add cylinders for the edges
        AddCylinders(topInsideEdge,   insideRadius, insideRadius, segments, endsClosed);
        AddCylinders(bottomInsideEdge,insideRadius, insideRadius, segments, endsClosed);
        AddCylinders(leftInsideEdge,  insideRadius, insideRadius, segments, endsClosed);
        AddCylinders(rightInsideEdge, insideRadius, insideRadius, segments, endsClosed);

        AddCylinders(topOutsideEdge,    outsideRadius, outsideRadius, segments, endsClosed);
        AddCylinders(bottomOutsideEdge, outsideRadius, outsideRadius, segments, endsClosed);
        AddCylinders(leftOutsideEdge,   outsideRadius, outsideRadius, segments, endsClosed);
        AddCylinders(rightOutsideEdge,  outsideRadius, outsideRadius, segments, endsClosed);

        // Link the inside and outside edges at the corners
        AddCylinder(topInsideEdge[0],               topOutsideEdge[0],               insideRadius, outsideRadius, segments, endsClosed);
        AddCylinder(bottomInsideEdge[0],            bottomOutsideEdge[0],            insideRadius, outsideRadius, segments, endsClosed);
        AddCylinder(topInsideEdge[resolutionAz],    topOutsideEdge[resolutionAz],    insideRadius, outsideRadius, segments, endsClosed);
        AddCylinder(bottomInsideEdge[resolutionAz], bottomOutsideEdge[resolutionAz], insideRadius, outsideRadius, segments, endsClosed);

        AddSphere(topInsideEdge[0],     insideRadius,  segments);
        AddSphere(topOutsideEdge[0],    outsideRadius, segments);
        AddSphere(bottomInsideEdge[0],  insideRadius,  segments);
        AddSphere(bottomOutsideEdge[0], outsideRadius, segments);

        AddSphere(topInsideEdge[resolutionAz],     insideRadius,  segments);
        AddSphere(topOutsideEdge[resolutionAz],    outsideRadius, segments);
        AddSphere(bottomInsideEdge[resolutionAz],  insideRadius,  segments);
        AddSphere(bottomOutsideEdge[resolutionAz], outsideRadius, segments);
    }

}
