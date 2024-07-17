using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    // Add a pyramid the the top point at the origin, and a base by height and width in the positive Z direction, distance baseDist from the origin

    public void AddPyramidByPoint(float baseDist, float baseWidth, float baseHeight)
    {
        Vector3 apex = new Vector3(0, 0, 0); // Apex of the pyramid
        Vector3 baseCenter = new Vector3(0, 0, baseDist); // Base center at the origin

        List<Vector3> basePoints = new List<Vector3>();

        // Generate base vertices
        basePoints.Add(new Vector3(-baseWidth / 2, -baseHeight / 2, baseDist));
        basePoints.Add(new Vector3( baseWidth / 2, -baseHeight / 2, baseDist));
        basePoints.Add(new Vector3( baseWidth / 2,  baseHeight / 2, baseDist));
        basePoints.Add(new Vector3(-baseWidth / 2,  baseHeight / 2, baseDist));

        // Create the sized of the pyramid
        AddFan(apex, basePoints, true); // true for wrapAround

        // Create the base of the pyramid
        AddTriangle(basePoints[0], basePoints[2], basePoints[1]);
        AddTriangle(basePoints[0], basePoints[3], basePoints[2]);
    }



}
