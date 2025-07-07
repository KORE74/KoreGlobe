using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// Static class to create GloMeshData2 primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshData2Primitives
{
    public static GloMeshData2 BasicSphere(float radius, Color color, int numLatSegments)
    {
        // Use the provided segments for latitude and double for longitude.
        int latSegments = numLatSegments;
        int lonSegments = numLatSegments * 2;

        var mesh = new GloMeshData2() { Name = "Sphere" };

        // Generate vertices, computing normals and colors in the same nested loop.
        // (lonSegments+1) vertices per latitude ensure that the seam wraps around.
        for (int lat = 0; lat <= latSegments; lat++)
        {
            float a1   = (float)Math.PI * lat / latSegments; // 0 (north pole) to PI (south pole)
            float sin1 = (float)Math.Sin(a1);
            float cos1 = (float)Math.Cos(a1);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float a2   = 2f * (float)Math.PI * lon / lonSegments; // 0 to 2PI
                float sin2 = (float)Math.Sin(a2);
                float cos2 = (float)Math.Cos(a2);

                // Compute vertex using spherical coordinates (Y is up)
                float   x      = radius * sin1 * cos2;
                float   y      = radius * cos1;
                float   z      = radius * sin1 * sin2;
                Vector3 vertex = new Vector3(x, y, z);
                mesh.Vertices.Add(vertex);

                // Compute normal; use vertex/radius if radius is non-zero.
                Vector3 normal = (radius != 0) ? vertex / radius : new Vector3(0, 1, 0);
                mesh.Normals.Add(normal);

                // Assign color to vertex.
                mesh.VertexColors.Add(color);
            }
        }

        // Create triangles connecting vertices on adjacent latitude lines.
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                // Each row has (lonSegments+1) vertices.
                int current = lat * (lonSegments + 1) + lon;
                int next    = current + lonSegments + 1;

                // Two triangles per quad
                mesh.Triangles.Add((current, next, current + 1));
                mesh.Triangles.Add((current + 1, next, next + 1));
            }
        }

        // Generate wireframe lines.
        for (int lat = 0; lat <= latSegments; lat++) // Horizontal lines along the same latitude.
        {
            int rowStart = lat * (lonSegments + 1);
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int current = rowStart + lon;
                int next    = current + 1;
                mesh.Lines.Add((current, next, color, color));
            }
        }
        for (int lon = 0; lon <= lonSegments; lon++) // Vertical lines along the same longitude.
        {
            for (int lat = 0; lat < latSegments; lat++)
            {
                int current = lat * (lonSegments + 1) + lon;
                int next    = (lat + 1) * (lonSegments + 1) + lon;
                mesh.Lines.Add((current, next, color, color));
            }
        }

        return mesh;
    }
}
