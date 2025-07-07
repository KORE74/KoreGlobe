using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class GloMeshBuilder
{
    public void AddCone(float axisDist, float heightDist, int baseNumPoints)
    {
        AddEllipticalCone(axisDist, axisDist, heightDist, baseNumPoints);
    }


    public void AddEllipticalCone(float majorAxis, float minorAxis, float height, int baseNumPoints)
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
        AddFan(baseCenter, basePoints, true, true); // true for wrapAround

        // Reverse the order of the base points to change the winding order
        basePoints.Reverse();

        // Create the sides of the cone
        AddFan(apex, basePoints, true, true); // true for wrapAround
    }

    // ------------------------------------------------------------------------------------------------------

    public void AddEllipticalConeOutline(float majorAxis, float minorAxis, float height, int baseNumPoints, int numCylinderSides, float baseRadius, float apexRadius)
    {
        Vector3 apex = new Vector3(0, 0, 0);
        Vector3 baseCenter = new Vector3(0, 0, height);

        List<Vector3> basePoints = new List<Vector3>();

        // Generate base vertices
        for (int i = 0; i < baseNumPoints; i++)
        {
            float angle = 2 * Mathf.Pi * i / baseNumPoints;
            float x = majorAxis * Mathf.Cos(angle);
            float y = minorAxis * Mathf.Sin(angle);
            Vector3 baseVertex = new Vector3(x, y, height);
            basePoints.Add(baseVertex);

            // Add tapered cylinder from base vertex to apex
            AddCylinder(baseVertex, apex, baseRadius, apexRadius, numCylinderSides, false); // endsClosed is false
        }

        // Create cylinders around the base ring
        for (int i = 0; i < basePoints.Count; i++)
        {
            Vector3 start = basePoints[i];
            Vector3 end = basePoints[(i + 1) % basePoints.Count];
            AddCylinder(start, end, baseRadius, baseRadius, numCylinderSides, false); // endsClosed is false
        }
    }



}
