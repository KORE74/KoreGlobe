using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// MagSphere - A sphere where the radius of each point is defined by an input array of floats.
// The radius is the distance from the center of the sphere to the point on the surface of the sphere.
// The radius is defined in spherical coordinates, with the azimuth and elevation angles defining the position of the point on the sphere.
// The color of a point is the magnitude of the radius, defined by an input color range.

public static partial class GloMeshData2Primitives
{
    public static GloMeshData2 MagSphere(Vector3 center, GloFloat2DArray radiusList, GloColorRange colorRange)
    {
        GloMeshData2 returnedMesh = new GloMeshData2() { Name = "MagSphere" };

        int vertSegments  = radiusList.Height - 1;
        int horizSegments = radiusList.Width - 1;

        float vertAngInc  = 180f / (float)vertSegments;
        float horizAngInc = 360f / (float)horizSegments;

        float maxRadius = radiusList.MaxVal();
        float minRadius = radiusList.MinVal();

        // Hold the indices of the MeshData.Vertices for this sphere
        List<int>   sphereVertexIndices = new List<int>();

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
                int index = returnedMesh.AddPoint(v);

                // Add the normal
                Vector3 n = new Vector3(x, y, z).Normalized();
                returnedMesh.AddNormal(n);

                // Add the UV
                float uvXFraction = (float)j / (float)horizSegments;
                float uvYFraction = (float)i / (float)vertSegments;
                returnedMesh.AddUV(new Vector2(uvXFraction, uvYFraction));

                // Calculate the fraction through the range, and add the color for that fraction
                float radiusFraction = (radius - minRadius) / (maxRadius - minRadius);
                returnedMesh.AddColor(colorRange.GetColor(radiusFraction));

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

                returnedMesh.AddTriangle(index1, index4, index2);
                returnedMesh.AddTriangle(index1, index3, index4);

                returnedMesh.AddLine(index1, index2, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index2]);
                returnedMesh.AddLine(index1, index3, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index3]);

                // returnedMesh.AddLine(index1, index4, returnedMesh.VertexColors[index1], returnedMesh.VertexColors[index4]);
                // returnedMesh.AddLine(index2, index3, returnedMesh.VertexColors[index2], returnedMesh.VertexColors[index3]);
                // returnedMesh.AddLine(index2, index4, returnedMesh.VertexColors[index2], returnedMesh.VertexColors[index4]);
                // returnedMesh.AddLine(index3, index4, returnedMesh.VertexColors[index3], returnedMesh.VertexColors[index4]);
            }
        }

        return returnedMesh;
    }
}
