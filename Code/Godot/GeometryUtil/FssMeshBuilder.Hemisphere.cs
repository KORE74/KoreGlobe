using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddHemisphere(Vector3 center, float radius, int numSegments)
    {
        int vertSegments = (int)Mathf.Round((float)numSegments / 4f);
        float vertAngInc = 90f / (float)vertSegments;
        float horizAngInc = 360f / (float)numSegments;

        // Hold the indices of the MeshData.Vertices for this hemisphere
        List<int> hemisphereVertexIndices = new List<int>();

        // Define the points on the hemisphere surface
        for (int i = 0; i < vertSegments + 1; i++)
        {
            float angle1 = Mathf.DegToRad(90f - (vertAngInc * i));
            float y = radius * Mathf.Sin(angle1);
            float r = radius * Mathf.Cos(angle1);

            for (int j = 0; j < numSegments; j++)
            {
                float angle2 =  Mathf.DegToRad(horizAngInc * j);
                float x = r * Mathf.Cos(angle2);
                float z = r * Mathf.Sin(angle2);

                Vector3 v = new Vector3(x, y, z) + center;
                int index = AddVertex(v);
                hemisphereVertexIndices.Add(index);
            }
        }

        // Define the MeshData.Triangles
        for (int row = 0; row < vertSegments; row++)
        {
            int rowStart = row * numSegments;
            int nextRowStart = (row + 1) * numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                int index1 = hemisphereVertexIndices[rowStart + i];
                int index2 = hemisphereVertexIndices[rowStart + (i + 1) % numSegments];
                int index3 = hemisphereVertexIndices[nextRowStart + i];
                int index4 = hemisphereVertexIndices[nextRowStart + (i + 1) % numSegments];

                AddTriangle(index1, index2, index4);
                AddTriangle(index1, index4, index3);
            }
        }
    }


}
