using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// Static class to create GloMeshData2 primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshData2Primitives
{
    public enum ConeStyle { Cone, CroppedCone }

    public static GloMeshData2 HorizCone(
        float     length,         // Distance from apex to base center.
        float     majorAxis,      // Radius along Y.
        float     minorAxis,      // Radius along Z.
        int       baseNumPoints,
        Color     color,
        ConeStyle style = ConeStyle.Cone,
        int       numDots = 5)      // Number of dots for dotted lines.
    {
        GloMeshData2 mesh = new GloMeshData2() { Name = "HorizCone" };

        // Define key points.
        // Apex at origin.
        int idxApex = mesh.AddPoint(new Vector3(0, 0, 0), null, color);
        // Base center along +X.
        int idxBaseCenter = mesh.AddPoint(new Vector3(length, 0, 0), null, color);

        // Generate base points on an ellipse in the YZ plane (centered at baseCenter).
        List<int> basePointIndices = new List<int>();
        for (int i = 0; i < baseNumPoints; i++)
        {
            float angle   = 2 * Mathf.Pi * i / baseNumPoints;
            float offsetY = majorAxis * Mathf.Cos(angle);
            float offsetZ = minorAxis * Mathf.Sin(angle);

            // Base point is offset from the base center.
            Vector3 basePoint = new Vector3(length, offsetY, offsetZ);
            int idx = mesh.AddPoint(basePoint, null, color);
            basePointIndices.Add(idx);
        }

        // Create the cone's base by connecting each base point to the base center
        // and linking consecutive points.
        for (int i = 0; i < basePointIndices.Count; i++)
        {
            int currentIdx = basePointIndices[i];
            int nextIdx    = basePointIndices[(i + 1) % basePointIndices.Count];

            if (style == ConeStyle.Cone)
            {
                // Draw solid lines.
                mesh.AddLine(mesh.Vertices[idxBaseCenter], mesh.Vertices[currentIdx], color, color);
                mesh.AddLine(mesh.Vertices[currentIdx], mesh.Vertices[nextIdx], color, color);
            }
            else // CroppedCone: use dotted lines.
            {
                mesh.AddDottedLine(mesh.Vertices[idxBaseCenter], mesh.Vertices[currentIdx], color, color, numDots);
                mesh.AddDottedLine(mesh.Vertices[currentIdx], mesh.Vertices[nextIdx], color, color, numDots);
            }
        }

        // Create the lateral sides: connect the apex with every base point.
        foreach (int idx in basePointIndices)
        {
            mesh.AddLine(mesh.Vertices[idxApex], mesh.Vertices[idx], color, color);
        }

        return mesh;
    }
}

