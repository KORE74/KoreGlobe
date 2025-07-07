using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// Static class to create GloMeshData2 primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshData2Primitives
{
    public static GloMeshData2 Cylinder(Vector3 p1, Vector3 p2, float p1radius, float p2radius, int sides, bool endsClosed)
    {
        var mesh = new GloMeshData2() { Name = "Cylinder" };

        // Direction and length
        Vector3 axis   = p2 - p1;
        float   height = axis.Length();
        if (height < 1e-6f || sides < 3) return mesh;

        axis = axis.Normalized();

        // Find a vector not parallel to axis for basis
        Vector3 up      = Math.Abs(axis.Dot(Vector3.Up)) < 0.99f ? Vector3.Up : Vector3.Right;
        Vector3 side    = axis.Cross(up).Normalized();
        Vector3 forward = axis.Cross(side).Normalized();

        // Generate circle points for both ends
        var   p1Circle  = new List<Vector3>();
        var   p2Circle  = new List<Vector3>();
        float angleStep = Mathf.Tau / sides;

        for (int i = 0; i < sides; i++)
        {
            float   angle  = i * angleStep;
            Vector3 offset = (Mathf.Cos(angle) * side + Mathf.Sin(angle) * forward);
            p1Circle.Add(p1 + offset * p1radius);
            p2Circle.Add(p2 + offset * p2radius);
        }

        // Side faces (quads split into triangles)
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            // First triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[i], p2Circle[next]);
            // Second triangle
            mesh.AddTriangle(p1Circle[i], p2Circle[next], p1Circle[next]);
        }

        // End caps
        if (endsClosed)
        {
            // p1 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p1, p1Circle[i], p1Circle[i + 1]);

            // p2 cap
            for (int i = 1; i < sides - 1; i++)
                mesh.AddTriangle(p2, p2Circle[i + 1], p2Circle[i]);
        }

        // --- Add lines for wireframe ---
        Color lineColor = Color.Color8(200, 200, 200, 255); // Or choose as needed

        // Longitudinal lines
        for (int i = 0; i < sides; i++)
        {
            mesh.AddLine(p1Circle[i], p2Circle[i], lineColor, lineColor);
        }

        // Rings at each end
        for (int i = 0; i < sides; i++)
        {
            int next = (i + 1) % sides;
            mesh.AddLine(p1Circle[i], p1Circle[next], lineColor, lineColor);
            mesh.AddLine(p2Circle[i], p2Circle[next], lineColor, lineColor);
        }

        return mesh;
    }
}
