using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddSphere(Vector3 center, float radius, int numSegments)
    {
        int vertSegments  = (int)Mathf.Round((float)numSegments / 2f);
        float vertAngInc  = 180f / (float)vertSegments;
        float horizAngInc = 360f / (float)numSegments;

        // Hold the indices of the MeshData.Vertices for this sphere
        List<int> sphereVertexIndices = new List<int>();

        // Define the points on the sphere surface
        for (int i = 0; i < vertSegments + 1; i++)
        {
            float angle1 = Mathf.DegToRad(90f - (vertAngInc * i));
            float y = radius * Mathf.Sin(angle1);
            float r = radius * Mathf.Cos(angle1);

            for (int j = 0; j < numSegments; j++)
            {
                float angle2 = Mathf.DegToRad(horizAngInc * j);
                float x = r * Mathf.Cos(angle2);
                float z = r * Mathf.Sin(angle2);

                Vector3 v = new Vector3(x, y, z) + center;
                int index = AddVertex(v);
                sphereVertexIndices.Add(index);
            }
        }

        // Define the MeshData.Triangles
        for (int row = 0; row < vertSegments; row++)
        {
            int rowStart = row * numSegments;
            int nextRowStart = (row + 1) * numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                int index1 = sphereVertexIndices[rowStart + i];
                int index2 = sphereVertexIndices[rowStart + (i + 1) % numSegments];
                int index3 = sphereVertexIndices[nextRowStart + i];
                int index4 = sphereVertexIndices[nextRowStart + (i + 1) % numSegments];

                AddTriangle(index1, index2, index4);
                AddTriangle(index1, index4, index3);
            }
        }
    }



    public void AddSphereOutline(Vector3 center, float radius, int numSegments, float outlineRadius)
    {
        int vertSegments  = (int)Mathf.Round((float)numSegments / 2f);
        float vertAngInc  = 180f / (float)vertSegments;
        float horizAngInc = 360f / (float)numSegments;

        // Iterate through all the vertical segments
        for (int i = 1; i < vertSegments; i++)
        {
            float angle1 = Mathf.DegToRad(90f - (vertAngInc * i));
            float y = radius * Mathf.Sin(angle1);
            float r = radius * Mathf.Cos(angle1);

            Vector3 prevPoint = Vector3.Zero; // To remember the previous point in the loop
            Vector3 firstPoint = Vector3.Zero; // To remember the first point in the loop

            for (int j = 0; j < numSegments; j++)
            {
                float angle2 = Mathf.DegToRad(horizAngInc * j);
                float x = r * Mathf.Cos(angle2);
                float z = r * Mathf.Sin(angle2);

                Vector3 currentPoint = new Vector3(x, y, z) + center;

                // If this is the first point in the loop, store it
                if (j == 0)
                {
                    firstPoint = currentPoint;
                }

                // If we have a previous point, we can add a cylinder between it and the current point
                if (prevPoint != Vector3.Zero)
                {
                    AddCylinder(prevPoint, currentPoint, outlineRadius, outlineRadius, numSegments, true);
                }

                prevPoint = currentPoint;

                // If this is the last point in the loop, we can add a cylinder between it and the first point
                if (j == numSegments - 1)
                {
                    AddCylinder(currentPoint, firstPoint, outlineRadius, outlineRadius, numSegments, true);
                }
            }
        }
    }


}
