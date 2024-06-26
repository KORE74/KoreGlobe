using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// FssMeshBuilder.SurfaceWedge: Assemble wedge shaped meshes with a 2d surface array on the outer spherical surface.

public partial class FssMeshBuilder
{
    public void AddSurfaceWedge(
        float azMinDegs, float azMaxDegs,
        float elMinDegs, float elMaxDegs,
        float radiusMin, float radiusMax,
        Float2DArray outerSurfaceDelta)
    {
        // Lists to hold the points for the inside and outside surfaces
        List<Vector3> insideSurfacePoints  = new List<Vector3>();
        List<Vector3> outsideSurfacePoints = new List<Vector3>();

        // Lists to hold the points for the ribbons (edges)
        List<Vector3> topInsideEdge     = new List<Vector3>();
        List<Vector3> bottomInsideEdge  = new List<Vector3>();
        List<Vector3> leftInsideEdge    = new List<Vector3>();
        List<Vector3> rightInsideEdge   = new List<Vector3>();
        List<Vector3> topOutsideEdge    = new List<Vector3>();
        List<Vector3> bottomOutsideEdge = new List<Vector3>();
        List<Vector3> leftOutsideEdge   = new List<Vector3>();
        List<Vector3> rightOutsideEdge  = new List<Vector3>();

        int resolutionEl = outerSurfaceDelta.Height;
        int resolutionAz = outerSurfaceDelta.Width;

        // Generate points for the inside and outside surfaces, and the edges
        for (int y = 0; y <= resolutionEl; y++)
        {
            float currElDegs = Mathf.Lerp(elMinDegs, elMaxDegs, (float)y / resolutionEl);
            for (int x = 0; x <= resolutionAz; x++)
            {
                float currAzDegs       = Mathf.Lerp(azMinDegs, azMaxDegs, (float)x / resolutionAz);
                float outsidePointDelta = outerSurfaceDelta[x, y];

                Vector3 insidePoint  = FssGeoConvOperations.RealWorldToGodot(radiusMin, currAzDegs, currElDegs);
                Vector3 outsidePoint = FssGeoConvOperations.RealWorldToGodot(radiusMax + outsidePointDelta, currAzDegs, currElDegs);
              
                insideSurfacePoints.Add(insidePoint);
                outsideSurfacePoints.Add(outsidePoint);

                // Add points to the edge lists, to make ribbons from them later in the other sides of the shape
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
        AddSurface(resolutionAz, resolutionEl, insideSurfacePoints, true);
        AddSurface(resolutionAz, resolutionEl, outsideSurfacePoints);

        // Add ribbons for the edges
        AddRibbon(topOutsideEdge,   topInsideEdge);
        AddRibbon(bottomInsideEdge, bottomOutsideEdge);
        AddRibbon(leftInsideEdge,   leftOutsideEdge);
        AddRibbon(rightOutsideEdge, rightInsideEdge);
    }

}
