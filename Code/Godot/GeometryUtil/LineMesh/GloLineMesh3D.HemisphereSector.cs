using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class GloLineMesh3D : Node3D
{
    // A hemisphere sector is made up of a sphere surface patch, with a single point a 90 elevation, spreadsing to the azimuth range.
    // we will create lines for the left and right faces of the sector, from a center point, out to the points on the sphere surface.

    // We'll create a 2D array of points on the surface, and 1D lists of points on the edges as we go, to create the side faces more easily.

    public void AddHemisphereSector(
        Vector3 center, float radius, int numSegments,        // Overall position and radius.
        float startAzDegs, float endAzDegs,                   // Left and right Az Angles (will be 0 to 90 elevation)
        Color lineCol, LineStyle drawStyle = LineStyle.Solid) // Draw style
    {
        int vertSegments  =   9; //(int)Mathf.Round((float)numSegments / 4f);
        float vertAngInc  =  90f / (float)vertSegments;
        float horizAngInc = 360f / (float)numSegments;
        float pointSep    = radius / 40f;

        // Determine the numbers of points in the sector, and the angle working values
        int   numVertVertices  = vertSegments + 1;
        int   numHorizVertices = numSegments;
        float topElDegs        = 90.0f;
        float bottomElDegs     = 0.0f;
        float vertAngleInt     = (topElDegs - bottomElDegs) / vertSegments;
        float horizAngleInt    = (endAzDegs - startAzDegs) / numHorizVertices;

        // Setup the lists of points we'll create as the stepping stones to the final mesh
        // [X, Y]
        Vector3[,]    surfaceVertices   = new Vector3[numHorizVertices, numVertVertices];
        List<Vector3> startEdgeVertices = new List<Vector3>();
        List<Vector3> endEdgeVertices   = new List<Vector3>();

        // Loop through each axis to make the surface, and add points to the edges when the x-axis is at the start or end.
        for (int currX = 0; currX < numHorizVertices; currX++)
        {
            float currAzDegs = startAzDegs + (currX * horizAngleInt);

            for (int currY = 0; currY < numVertVertices; currY++)
            {
                // determine the angle
                float currElDegs = topElDegs - (currY * vertAngleInt);

                // Convert spherical coordinates to Cartesian coordinates
                float radianAz = Mathf.DegToRad(currAzDegs);
                float radianEl = Mathf.DegToRad(currElDegs);

                float x = radius * Mathf.Cos(radianEl) * Mathf.Cos(radianAz);
                float y = radius * Mathf.Cos(radianEl) * Mathf.Sin(radianAz);
                float z = radius * Mathf.Sin(radianEl);

                Vector3 v = new Vector3(x, z, y) + center;
                surfaceVertices[currX, currY] = v;

                // Add to edge lists if at the start or end azimuth
                if (currX == 0)
                    startEdgeVertices.Add(v);
                else if (currX == numHorizVertices - 1)
                    endEdgeVertices.Add(v);
            }
        }

        // Add lines for the surface vertices
        for (int currX = 0; currX < numHorizVertices - 1; currX++)
        {
            for (int currY = 0; currY < numVertVertices - 1; currY++)
            {
                AddLine(surfaceVertices[currX, currY], surfaceVertices[currX + 1, currY], lineCol);
                AddLine(surfaceVertices[currX, currY], surfaceVertices[currX, currY + 1], lineCol);
            }

            // Draw the last horizontal row of the surface
            AddLine(surfaceVertices[currX, numVertVertices - 1], surfaceVertices[currX + 1, numVertVertices - 1], lineCol);
        }

        // Draw the last vertical column of the surface
        for (int currY = 0; currY < numVertVertices - 1; currY++)
        {
            AddLine(surfaceVertices[numHorizVertices - 1, currY], surfaceVertices[numHorizVertices - 1, currY + 1], lineCol);
        }

        // Add lines for the start and end edges
        for (int i = 0; i < startEdgeVertices.Count; i++)
        {
            AddLine(startEdgeVertices[i], center, lineCol);
            AddLine(endEdgeVertices[i],   center,   lineCol);
        }

    }
}

