using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// FssMeshBuilder.SurfaceWedge: Assemble wedge shaped meshes with a 2d surface array on the outer spherical surface.

public partial class FssMeshBuilder
{
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

        for (int y = 0; y < resolutionEl; y++)
        {
            for (int x = 0; x < resolutionAz; x++)
            {
                float currAzDegs = Mathf.Lerp(azMinDegs, azMaxDegs, (float)x / resolutionAz);
                float currElDegs = Mathf.Lerp(elMinDegs, elMaxDegs, (float)y / resolutionEl);
                float currRadius = surfaceRadius + (surfaceArray[x, y] * surfaceScale);

                points[y, x] = FssGeoConvOperations.RealWorldToGodot(currRadius, currAzDegs, currElDegs);
            }
        }

        for (int y = 0; y < resolutionEl-1; y++)
        {
            float yfrac = (float)y / (resolutionEl-1);

            for (int x = 0; x < resolutionAz-1; x++)
            {
                float xfrac = (float)x / (resolutionAz-1);

                Vector3 p1 = points[y,     x];
                Vector3 p2 = points[y,     x + 1];
                Vector3 p3 = points[y + 1, x];
                Vector3 p4 = points[y + 1, x + 1];

                Vector2 uv1 = new Vector2((float)y       / (resolutionEl - 1) * -1f, (float)x       / (resolutionAz - 1));
                Vector2 uv2 = new Vector2((float)y       / (resolutionEl - 1) * -1f, (float)(x + 1) / (resolutionAz - 1));
                Vector2 uv3 = new Vector2((float)(y + 1) / (resolutionEl - 1) * -1f, (float)x       / (resolutionAz - 1));
                Vector2 uv4 = new Vector2((float)(y + 1) / (resolutionEl - 1) * -1f, (float)(x + 1) / (resolutionAz - 1));

                // Add points to MeshData.Vertices list and record the index of each point
                int i1 = AddVertex(p1);
                AddNormal(p1.Normalized());
                AddUV(uv1);

                int i2 = AddVertex(p2);
                AddNormal(p2.Normalized());
                AddUV(uv2);

                int i3 = AddVertex(p3);
                AddNormal(p3.Normalized());
                AddUV(uv3);

                int i4 = AddVertex(p4);
                AddNormal(p4.Normalized());
                AddUV(uv4);

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
