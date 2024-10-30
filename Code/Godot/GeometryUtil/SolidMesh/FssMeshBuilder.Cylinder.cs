using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddCylinder(Vector3 p1, Vector3 p2, float p1radius, float p2radius, int sides, bool endsClosed)
    {
        //Quaternion direction = Quaternion.LookRotation(p2 - p1, Vector3.up);
        float angleStep = 360f / sides;

        List<Vector3> p1CirclePoints = new List<Vector3>();
        List<Vector3> p2CirclePoints = new List<Vector3>();

        // Calculate the direction vector from p1 to p2
        Vector3 direction = (p2 - p1).Normalized();

        // Find two vectors orthogonal to the direction vector
        Vector3 up = Vector3.Up;
        if (Math.Abs(direction.Dot(up)) > 0.99f) // If direction is close to 'up' vector, use a different reference
        {
            up = Vector3.Right;
        }
        Vector3 right = direction.Cross(up).Normalized();
        up = right.Cross(direction).Normalized();

        // --- create MeshData.Vertices for the circles at p1 and p2 ---

        // Create the circles at p1 and p2
        for (int i = 0; i < sides; i++)
        {
            float angle = Mathf.DegToRad(i * angleStep);

            Vector3 offset1 = right * Mathf.Cos(angle) * p1radius + up * Mathf.Sin(angle) * p1radius;
            p1CirclePoints.Add(p1 + offset1);

            float uvXFraction = (float)i / sides;
            AddUV(new Vector2(uvXFraction, 0));

            Vector3 offset2 = right * Mathf.Cos(angle) * p2radius + up * Mathf.Sin(angle) * p2radius;
            p2CirclePoints.Add(p2 + offset2);

            AddUV(new Vector2(uvXFraction, 1));
        }

        // --- create the sides of the cylinder ---

        for (int i = 0; i < sides; i++)
        {
            int i1 = AddVertex(p1CirclePoints[i]);
            int i2 = AddVertex(p2CirclePoints[i]);
            int i3 = AddVertex(p1CirclePoints[(i + 1) % sides]);
            int i4 = AddVertex(p2CirclePoints[(i + 1) % sides]);

            AddTriangle(i3, i2, i1);
            AddTriangle(i3, i4, i2);
        }

        // --- optionally close the ends ---

        if (endsClosed)
        {

            // Reverse the order of the points in the circle to the triangle are created facing the right direction
            List<Vector3> flippedPoints = new List<Vector3>(p1CirclePoints);
            flippedPoints.Reverse();

            AddFan(p1, flippedPoints, true);
            AddFan(p2, p2CirclePoints, true);
        }
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
