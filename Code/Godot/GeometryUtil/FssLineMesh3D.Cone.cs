using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssLineMesh3D
{
    public void AddCone(float axisDist, float heightDist, int baseNumPoints, Color lineCol)
    {
        AddEllipticalCone(axisDist, axisDist, heightDist, baseNumPoints, lineCol);
    }


    public void AddEllipticalCone(float majorAxis, float minorAxis, float height, int baseNumPoints, Color lineCol)
    {
        Vector3 apex = new Vector3(0, 0, 0); // Apex of the cone
        Vector3 baseCenter = new Vector3(0, 0, height); // Base center at the origin

        List<Vector3> basePoints = new List<Vector3>();

        // Generate base vertices
        for (int i = 0; i < baseNumPoints; i++)
        {
            float angle = 2 * Mathf.Pi * i / baseNumPoints;
            float x = majorAxis * Mathf.Cos(angle); // X coordinate
            float y = minorAxis * Mathf.Sin(angle); // Y coordinate
            Vector3 baseVertex = new Vector3(x, y, height); // Z coordinate is 0 for base vertices
            basePoints.Add(baseVertex);
        }

        // Create the base of the cone
        for (int i = 0; i < basePoints.Count; i++)
        {
            Vector3 start = basePoints[i];
            Vector3 end = basePoints[(i + 1) % basePoints.Count];
            AddLine(baseCenter, start, lineCol); // Line from base center to base vertex
            AddLine(start, end, lineCol); // Line between base vertices
        }

        // Create the sides of the cone
        for (int i = 0; i < basePoints.Count; i++)
        {
            AddLine(apex, basePoints[i], lineCol); // Line from apex to base vertex
        }
    }

}
