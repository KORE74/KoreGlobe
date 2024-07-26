using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// FssMeshBuilder.SurfaceWedge: Assemble wedge shaped meshes with a 2d surface array on the outer spherical surface.

public partial class FssMeshBuilder
{

/*

UVs are defined as follows:

    (0,0)   (1,0)
    +--------+
    |        |
    |        |
    +--------+
    (0,1)   (1,1)

*/


    public void AddSurface(
        float azMinDegs, float azMaxDegs,
        float elMinDegs, float elMaxDegs,
        float surfaceRadius, float surfaceScale,
        FssFloat2DArray surfaceArray,
        bool flipTriangles = false)
    {
        int resolutionEl = surfaceArray.Height;
        int resolutionAz = surfaceArray.Width;

        // Create a 2D array to hold the points for the surface
        Vector3[,] points = new Vector3[resolutionEl, resolutionAz];
        int [,] indices   = new int[resolutionEl, resolutionAz];

        // [0,0] is the top left corner of the surface, and [0,0]UV is the top left corner of the UV map
        // We'll need to adjust the elevation and azimuth values to match this iteration across the 2D array.

        float elIncrement = (elMaxDegs - elMinDegs) / (resolutionEl - 1);
        float azIncrement = (azMaxDegs - azMinDegs) / (resolutionAz - 1);

        for (int y = 0; y < resolutionEl; y++)
        {
            for (int x = 0; x < resolutionAz; x++)
            {
                float currElDegs = elMaxDegs - (float)y * elIncrement; // Note we go from top to bottom
                float currAzDegs = azMinDegs + (float)x * azIncrement;
                float currRadius = surfaceRadius + (surfaceArray[y, x] * surfaceScale);

                points[y, x] = FssGeoConvOperations.RealWorldToGodot(currRadius, currElDegs, currAzDegs);
            }
        }

        for (int y = 0; y < resolutionEl; y++)
        {
            float yfrac = (float)y / (resolutionEl-1); // 0 y is the top row.

            for (int x = 0; x < resolutionAz; x++)
            {
                float xfrac = (float)x / (resolutionAz-1);

                indices[y, x] = AddVertex(points[y, x]);
                AddNormal(points[y, x].Normalized());
                AddUV(new Vector2(yfrac, xfrac));
            }
        }

        for (int y = 0; y < resolutionEl-1; y++)
        {
            for (int x = 0; x < resolutionAz-1; x++)
            {
                int i1 = indices[y,     x];
                int i2 = indices[y,     x + 1];
                int i3 = indices[y + 1, x];
                int i4 = indices[y + 1, x + 1];

                // Create two MeshData.Triangles using the four MeshData.Vertices just added
                if (flipTriangles)
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
                else
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
            }
        }
    }

    public void AddSurfaceWedge(
        float azMinDegs, float azMaxDegs,
        float elMinDegs, float elMaxDegs,
        float radiusMin, float radiusMax,
        FssFloat2DArray outerSurfaceDelta)
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
