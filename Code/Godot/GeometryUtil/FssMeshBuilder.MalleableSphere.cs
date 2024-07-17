using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class FssMeshBuilder
{
    // radiusList: A 2D array of floats, for a full sphere of points, so we interpolate across 360x180 degree sphere for the list.
    // colorRange: A range of colors to interpolate between for the sphere, based on the fraction of the radius.

    public void AddMalleableSphere(Vector3 center, FssFloat2DArray radiusList, FssColorRange colorRange)
    {
        int vertSegments  = radiusList.Height - 1;
        int horizSegments = radiusList.Width - 1;

        float vertAngInc  = 180f / (float)vertSegments;
        float horizAngInc = 360f / (float)horizSegments;

        float maxRadius = radiusList.MaxVal();
        float minRadius = radiusList.MinVal();

        // Hold the indices of the MeshData.Vertices for this sphere
        List<int> sphereVertexIndices = new List<int>();
        List<Color> sphereVertexColors = new List<Color>();

        // Define the points on the sphere surface
        for (int i = 0; i < vertSegments+1; i++)
        {
            for (int j = 0; j < horizSegments; j++)
            {
                // Lookup the radius
                float radius = radiusList[j, i];

                // Calculate the angles
                float azRads = Mathf.DegToRad(90f - (vertAngInc * i));
                float elRads = Mathf.DegToRad(horizAngInc * j);

                // Calculate the x,y,z
                float y = radius * Mathf.Sin(azRads);
                float r = radius * Mathf.Cos(azRads);
                float x = r * Mathf.Cos(elRads);
                float z = r * Mathf.Sin(elRads);

                // Determine the final vertex position with the center offset
                Vector3 v = new Vector3(x, y, z) + center;

                // Add the vertex
                int index = AddVertex(v);

                // Add the normal
                Vector3 n = new Vector3(x, y, z).Normalized();
                AddNormal(n);

                // Add the UV
                float uvXFraction = (float)j / (float)horizSegments;
                float uvYFraction = (float)i / (float)vertSegments;
                AddUV(new Vector2(uvXFraction, uvYFraction));

                // Calculate and add the color
                float radiusFraction = (radius - minRadius) / (maxRadius - minRadius);
                AddColor(colorRange.GetColor(radiusFraction));

                // Add the index to the sphereVertexIndices - to calculate the triangles later
                sphereVertexIndices.Add(index);
            }
        }

        // Define the MeshData.Triangles
        for (int row = 0; row < (vertSegments); row++)
        {
            int rowStart     = row       * horizSegments;
            int nextRowStart = (row + 1) * horizSegments;

            for (int i = 0; i < (horizSegments); i++)
            {
                int index1 = sphereVertexIndices[rowStart + i];
                int index2 = sphereVertexIndices[rowStart + (i + 1) % horizSegments];
                int index3 = sphereVertexIndices[nextRowStart + i];
                int index4 = sphereVertexIndices[nextRowStart + (i + 1) % horizSegments];

                AddTriangle(index1, index4, index2);
                AddTriangle(index1, index3, index4);
            }
        }
    }
}
