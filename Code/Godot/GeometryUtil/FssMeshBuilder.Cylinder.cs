using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddCylinder(Vector3 p1, Vector3 p2, float p1radius, float p2radius, int sides, bool endsClosed)
    {
        // //Quaternion direction = Quaternion.LookRotation(p2 - p1, Vector3.up);
        // float angleStep = 360f / sides;

        // List<Vector3> p1CirclePoints = new List<Vector3>();
        // List<Vector3> p2CirclePoints = new List<Vector3>();

        // // --- create MeshData.Vertices for the circles at p1 and p2 ---

        // for (int i = 0; i < sides; i++)
        // {
        //     float angle = (float)(i * angleStep) * Mathf.Deg2Rad;
        //     float p1x = p1radius * Mathf.Cos(angle);
        //     float p1y = p1radius * Mathf.Sin(angle);
        //     float p1z = 0f;

        //     //p1CirclePoints.Add(direction * new Vector3(p1x, p1y, p1z) + p1);

        //     float p2x = p2radius * Mathf.Cos(angle);
        //     float p2y = p2radius * Mathf.Sin(angle);
        //     float p2z = 0f;

        //     //p2CirclePoints.Add(direction * new Vector3(p2x, p2y, p2z) + p2);
        // }

        // // --- create the sides of the cylinder ---

        // for (int i = 0; i < sides; i++)
        // {
        //     int i1 = AddVertex(p1CirclePoints[i]);
        //     int i2 = AddVertex(p2CirclePoints[i]);
        //     int i3 = AddVertex(p1CirclePoints[(i + 1) % sides]);
        //     int i4 = AddVertex(p2CirclePoints[(i + 1) % sides]);

        //     AddTriangle(i3, i2, i1);
        //     AddTriangle(i3, i4, i2);
        // }

        // // --- optionally close the ends ---

        // if (endsClosed)
        // {
        //     AddFan(p1, p1CirclePoints, true);
        //     AddFan(p2, p2CirclePoints, true);
        // }
    }

    // ----------------------------------------------------------------------------------

    private void AddCylinders(List<Vector3> points, float p1radius, float p2radius, int sides, bool endsClosed)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            AddCylinder(points[i], points[i + 1], p1radius, p2radius, sides, endsClosed);
        }
    }
}
