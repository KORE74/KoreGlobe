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

    // Add a surface, that is just the top edge.

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
        Vector3[,] points = new Vector3[resolutionAz, resolutionEl];
        int [,] indices   = new int[resolutionAz, resolutionEl];

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
                float currRadius = surfaceRadius + (surfaceArray[x, y] * surfaceScale);

                FssLLAPoint llap = new FssLLAPoint() {
                    LatDegs = currElDegs,
                    LonDegs = currAzDegs,
                    RadiusM = currRadius
                };
                points[x, y] = FssGeoConvOperations.RwToGe(llap);
            }
        }

        for (int y = 0; y < resolutionEl; y++)
        {
            float yfrac = (float)y / (resolutionEl-1); // 0 y is the top row.
            yfrac = FssValueUtils.ScaleVal(yfrac, 0f, 1f, 0.001f, 0.999f);

            for (int x = 0; x < resolutionAz; x++)
            {
                float xfrac = (float)x / (resolutionAz-1);
                xfrac = FssValueUtils.ScaleVal(xfrac, 0f, 1f, 0.001f, 0.999f);

                indices[x, y] = AddVertex(points[x, y]);
                AddNormal(points[x, y].Normalized());
                AddUV(new Vector2(xfrac, yfrac));
            }
        }

        for (int y = 0; y < resolutionEl-1; y++)
        {
            for (int x = 0; x < resolutionAz-1; x++)
            {
                int i1 = indices[x,     y];
                int i2 = indices[x,     y + 1];
                int i3 = indices[x + 1, y];
                int i4 = indices[x + 1, y + 1];

                // Create two MeshData.Triangles using the four MeshData.Vertices just added
                if (flipTriangles)
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
                else
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Add the sides of a wedge shape, connecting the top and bottom surfaces
    public void AddSurfaceWedgeSides(
        float azMinDegs, float azMaxDegs,
        float elMinDegs, float elMaxDegs,
        float surfaceRadius, float surfaceScale, float innerRadius,
        FssFloat2DArray surfaceArray,
        bool flipTriangles = false)
    {
        int resolutionEl = surfaceArray.Height;
        int resolutionAz = surfaceArray.Width;

        // Create a 2D array to hold the points for the surface
        Vector3[,] points = new Vector3[resolutionAz, resolutionEl];
        int [,] indices   = new int[resolutionAz, resolutionEl];

        // [0,0] is the top left corner of the surface, and [0,0]UV is the top left corner of the UV map
        // We'll need to adjust the elevation and azimuth values to match this iteration across the 2D array.

        FssFloat1DArray topEdge    = surfaceArray.GetEdge(FssFloat2DArray.Edge.Top);
        FssFloat1DArray bottomEdge = surfaceArray.GetEdge(FssFloat2DArray.Edge.Bottom);
        FssFloat1DArray leftEdge   = surfaceArray.GetEdge(FssFloat2DArray.Edge.Left);
        FssFloat1DArray rightEdge  = surfaceArray.GetEdge(FssFloat2DArray.Edge.Right);

        // Top
        {
            // Loop across the top edge, creating the surface and lower edge points, that we then add as a ribbon.
            float uvX  = 0.0f;
            float uvY1 = 0.001f;
            float uvY2 = 0.002f;

            float azInc = (azMaxDegs - azMinDegs) / (resolutionAz - 1);

            // Create lists for the top and bottom points
            List<int> topIds    = new List<int>();
            List<int> bottomIds = new List<int>();
            for (int x = 0; x < resolutionAz; x++)
            {
                uvX = (float)x / (resolutionAz-1);
                uvX = FssValueUtils.ScaleVal(uvX, 0f, 1f, 0.001f, 0.999f);

                float currAzDegs   = azMinDegs + (float)x * azInc;
                float topEdgeDelta = topEdge[x] * surfaceScale;

                topIds.Add( AddVertex(FssGeoConvOperations.RwToGe(surfaceRadius + topEdgeDelta, elMaxDegs, currAzDegs)) );
                bottomIds.Add( AddVertex(FssGeoConvOperations.RwToGe(innerRadius, elMaxDegs, currAzDegs)) );

                AddUV(new Vector2(uvX, uvY1));
                AddUV(new Vector2(uvX, uvY2));
            }

            // Loop through the points to add the resulting triangles/squares
            for (int i = 0; i < topIds.Count-1; i++)
            {
                int i1 = topIds[i];
                int i2 = topIds[i + 1];
                int i3 = bottomIds[i];
                int i4 = bottomIds[i + 1];

                if (flipTriangles)
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
                else
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
            }
        }

        // Bottom
        {
            // Loop across the top edge, creating the surface and lower edge points, that we then add as a ribbon.
            float uvX  = 0.0f;
            float uvY1 = 0.998f;
            float uvY2 = 0.999f;

            float azInc = (azMaxDegs - azMinDegs) / (resolutionAz - 1);

            // Create lists for the top and bottom points
            List<int> topIds    = new List<int>();
            List<int> bottomIds = new List<int>();
            for (int x = 0; x < resolutionAz; x++)
            {
                uvX = (float)x / (resolutionAz-1);
                uvX = FssValueUtils.ScaleVal(uvX, 0f, 1f, 0.001f, 0.999f);

                float currAzDegs      = azMinDegs + (float)x * azInc;
                float bottomEdgeDelta = bottomEdge[x] * surfaceScale;

                topIds.Add( AddVertex(FssGeoConvOperations.RwToGe(surfaceRadius + bottomEdgeDelta, elMinDegs, currAzDegs)) );
                bottomIds.Add( AddVertex(FssGeoConvOperations.RwToGe(innerRadius,                  elMinDegs, currAzDegs)) );

                AddUV(new Vector2(uvX, uvY1));
                AddUV(new Vector2(uvX, uvY2));
            }

            // Loop through the points to add the resulting triangles/squares
            for (int i = 0; i < topIds.Count-1; i++)
            {
                int i1 = topIds[i];
                int i2 = topIds[i + 1];
                int i3 = bottomIds[i];
                int i4 = bottomIds[i + 1];

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

        // Left
        {
            float uvY  = 0.0f;
            float uvX1 = 0.002f;
            float uvX2 = 0.001f;

            float elInc = (elMaxDegs - elMinDegs) / (resolutionEl - 1);

            // Create lists for the top and bottom points
            List<int> topIds    = new List<int>();
            List<int> bottomIds = new List<int>();
            for (int y = 0; y < resolutionEl; y++)
            {
                uvY = (float)y / (resolutionEl-1);
                uvY = FssValueUtils.ScaleVal(uvY, 0f, 1f, 0.001f, 0.999f);

                float currElDegs = elMaxDegs - (float)y * elInc; // El max to min, to match UV min to max
                float edgeDelta  = leftEdge[y] * surfaceScale;

                topIds.Add( AddVertex(FssGeoConvOperations.RwToGe(surfaceRadius + edgeDelta, currElDegs, azMinDegs)) );
                bottomIds.Add( AddVertex(FssGeoConvOperations.RwToGe(innerRadius,            currElDegs, azMinDegs)) );

                AddUV(new Vector2(uvX1, uvY));
                AddUV(new Vector2(uvX2, uvY));
            }

            // Loop through the points to add the resulting triangles/squares
            for (int i = 0; i < topIds.Count-1; i++)
            {
                int i1 = topIds[i];
                int i2 = topIds[i + 1];
                int i3 = bottomIds[i];
                int i4 = bottomIds[i + 1];

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

        // Right
        {
            float uvY  = 0.0f;
            float uvX1 = 0.998f;
            float uvX2 = 0.999f;

            float elInc = (elMaxDegs - elMinDegs) / (resolutionEl - 1);

            // Create lists for the top and bottom points
            List<int> topIds    = new List<int>();
            List<int> bottomIds = new List<int>();
            for (int y = 0; y < resolutionEl; y++)
            {
                uvY = (float)y / (resolutionEl-1);
                uvY = FssValueUtils.ScaleVal(uvY, 0f, 1f, 0.001f, 0.999f);

                float currElDegs = elMaxDegs - (float)y * elInc; // El max to min, to match UV min to max
                float edgeDelta  = rightEdge[y] * surfaceScale;

                topIds.Add( AddVertex(FssGeoConvOperations.RwToGe(surfaceRadius + edgeDelta, currElDegs, azMaxDegs)) );
                bottomIds.Add( AddVertex(FssGeoConvOperations.RwToGe(innerRadius,            currElDegs, azMaxDegs)) );

                AddUV(new Vector2(uvX1, uvY));
                AddUV(new Vector2(uvX2, uvY));
            }

            // Loop through the points to add the resulting triangles/squares
            for (int i = 0; i < topIds.Count-1; i++)
            {
                int i1 = topIds[i];
                int i2 = topIds[i + 1];
                int i3 = bottomIds[i];
                int i4 = bottomIds[i + 1];

                if (flipTriangles)
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
                else
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
            }
        }

    }

    // --------------------------------------------------------------------------------------------

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

                Vector3 insidePoint  = FssGeoConvOperations.RwToGe(radiusMin, currAzDegs, currElDegs);
                Vector3 outsidePoint = FssGeoConvOperations.RwToGe(radiusMax + outsidePointDelta, currAzDegs, currElDegs);

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

