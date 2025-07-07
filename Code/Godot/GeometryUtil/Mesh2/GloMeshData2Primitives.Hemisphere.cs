using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// Static class to create GloMeshData2 primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshData2Primitives
{
    public static GloMeshData2 BasicHemisphere(float radius, Color color, int numLatSegments)
    {
        // For a hemisphere, we cover latitude angles from 0 (north pole) to PI/2 (equator)
        int latSegments = numLatSegments;
        int lonSegments = numLatSegments * 2;

        var mesh = new GloMeshData2() { Name = "Hemisphere" };

        // Vertex generation: for each latitude (0 to PI/2) and a full 360° in longitude.
        for (int lat = 0; lat <= latSegments; lat++)
        {
            // Angle from the north pole (0) to the equator (PI/2)
            float a1   = (float)(Math.PI / 2.0) * lat / latSegments;
            float sin1 = (float)Math.Sin(a1);
            float cos1 = (float)Math.Cos(a1);

            for (int lon = 0; lon <= lonSegments; lon++)
            {
                float a2   = 2 * (float)Math.PI * lon / lonSegments;
                float sin2 = (float)Math.Sin(a2);
                float cos2 = (float)Math.Cos(a2);

                // Compute vertex using spherical coordinates (Y is up)
                float x = radius * sin1 * cos2;
                float y = radius * cos1;
                float z = radius * sin1 * sin2;
                Vector3 vertex = new Vector3(x, y, z);
                mesh.Vertices.Add(vertex);

                // Compute normal (if radius isn't zero); otherwise use an arbitrary default.
                Vector3 normal = (radius != 0) ? vertex / radius : new Vector3(0, 1, 0);
                mesh.Normals.Add(normal);

                // Assign the provided color to the vertex.
                mesh.VertexColors.Add(color);
            }
        }

        // Create triangles by connecting adjacent latitude/longitude vertices.
        // The grid has (lonSegments+1) vertices per latitude row.
        for (int lat = 0; lat < latSegments; lat++)
        {
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int current = lat * (lonSegments + 1) + lon;
                int next    = current + lonSegments + 1;

                // Two triangles form each quad (ensuring proper winding order).
                mesh.Triangles.Add((current, next, current + 1));
                mesh.Triangles.Add((current + 1, next, next + 1));
            }
        }

        // Generate wireframe lines.
        // Horizontal lines (constant latitude).
        for (int lat = 0; lat <= latSegments; lat++)
        {
            int rowStart = lat * (lonSegments + 1);
            for (int lon = 0; lon < lonSegments; lon++)
            {
                int current = rowStart + lon;
                int next    = current + 1;
                mesh.Lines.Add((current, next, color, color));
            }
        }

        // Vertical lines (constant longitude).
        for (int lon = 0; lon <= lonSegments; lon++)
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
