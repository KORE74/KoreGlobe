using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssLineMesh3D : Node3D
{
    // Add a pyramid the the top point at the origin, and a base by height and width in the positive Z direction, distance baseDist from the origin

    public void AddPyramidByPoint(float baseDist, float baseWidth, float baseHeight, Color lineColor)
    {
        Vector3 apex = new Vector3(0, 0, 0); // Apex of the pyramid

        List<Vector3> basePoints = new List<Vector3>();

        // Generate base vertices
        basePoints.Add(new Vector3(-baseWidth / 2, -baseHeight / 2, baseDist));
        basePoints.Add(new Vector3( baseWidth / 2, -baseHeight / 2, baseDist));
        basePoints.Add(new Vector3( baseWidth / 2,  baseHeight / 2, baseDist));
        basePoints.Add(new Vector3(-baseWidth / 2,  baseHeight / 2, baseDist));

        // Add the Base Points
        AddLine(basePoints[0], basePoints[1], lineColor);
        AddLine(basePoints[1], basePoints[2], lineColor);
        AddLine(basePoints[2], basePoints[3], lineColor);
        AddLine(basePoints[3], basePoints[0], lineColor);

        // Add the Apex to Base Lines
        AddLine(apex, basePoints[0], lineColor);
        AddLine(apex, basePoints[1], lineColor);
        AddLine(apex, basePoints[2], lineColor);
        AddLine(apex, basePoints[3], lineColor);
    }

    public void AddPyramidByAzElDist(FssAzElBox azElBox, float baseDist, Color lineColor)
    {
        Vector3 apex = new Vector3(0, 0, 0); // Apex of the pyramid

        // Do some basic trig with the AzEl values and the distance to get the height and width offsets
        float azOffset = (float)(baseDist * Math.Sin(azElBox.HalfArcAzRads));
        float elOffset = (float)(baseDist * Math.Sin(azElBox.HalfArcElRads));

        List<Vector3> basePoints = new List<Vector3>();

        // Elevation = y, Azimuth = x, distance = -z
        // Generate base vertices
        basePoints.Add(new Vector3(-azOffset, -elOffset, baseDist));
        basePoints.Add(new Vector3( azOffset, -elOffset, baseDist));
        basePoints.Add(new Vector3( azOffset,  elOffset, baseDist));
        basePoints.Add(new Vector3(-azOffset,  elOffset, baseDist));

        // Add the Base Points
        AddLine(basePoints[0], basePoints[1], lineColor);
        AddLine(basePoints[1], basePoints[2], lineColor);
        AddLine(basePoints[2], basePoints[3], lineColor);
        AddLine(basePoints[3], basePoints[0], lineColor);

        // Add the Apex to Base Lines
        AddLine(apex, basePoints[0], lineColor);
        AddLine(apex, basePoints[1], lineColor);
        AddLine(apex, basePoints[2], lineColor);
        AddLine(apex, basePoints[3], lineColor);
    }

    // Add a pyramid, as in AddPyramidByAzElDist(), but the top is cropped to the topDist value, making it more
    // of a deformed cube than a pyramid

    public void AddCroppedPyramidByAzElDist(FssAzElBox azElBox, float topDist, float baseDist, Color lineColor)
    {
        Vector3 apex = new Vector3(0, 0, 0); // Apex of the pyramid

        // Top Points
        float topAzOffset = (float)(topDist * Math.Sin(azElBox.HalfArcAzRads));
        float topElOffset = (float)(topDist * Math.Sin(azElBox.HalfArcElRads));

        List<Vector3> topPoints = new List<Vector3>();
        topPoints.Add(new Vector3(-topAzOffset, -topElOffset, topDist));
        topPoints.Add(new Vector3( topAzOffset, -topElOffset, topDist));
        topPoints.Add(new Vector3( topAzOffset,  topElOffset, topDist));
        topPoints.Add(new Vector3(-topAzOffset,  topElOffset, topDist));

        // Base Points
        float baseAzOffset = (float)(baseDist * Math.Sin(azElBox.HalfArcAzRads));
        float baseElOffset = (float)(baseDist * Math.Sin(azElBox.HalfArcElRads));

        List<Vector3> basePoints = new List<Vector3>();
        basePoints.Add(new Vector3(-baseAzOffset, -baseElOffset, baseDist));
        basePoints.Add(new Vector3( baseAzOffset, -baseElOffset, baseDist));
        basePoints.Add(new Vector3( baseAzOffset,  baseElOffset, baseDist));
        basePoints.Add(new Vector3(-baseAzOffset,  baseElOffset, baseDist));

        // Now we have the eight points, we need to create the faces - 8 faces = 16 triangles

        // Top Square Lines
        AddLine(topPoints[0], topPoints[1], lineColor);
        AddLine(topPoints[1], topPoints[2], lineColor);
        AddLine(topPoints[2], topPoints[3], lineColor);
        AddLine(topPoints[3], topPoints[0], lineColor);

        // Base Square Lines
        AddLine(basePoints[0], basePoints[1], lineColor);
        AddLine(basePoints[1], basePoints[2], lineColor);
        AddLine(basePoints[2], basePoints[3], lineColor);
        AddLine(basePoints[3], basePoints[0], lineColor);

        // Side Lines
        AddLine(topPoints[0], basePoints[0], lineColor);
        AddLine(topPoints[1], basePoints[1], lineColor);
        AddLine(topPoints[2], basePoints[2], lineColor);
        AddLine(topPoints[3], basePoints[3], lineColor);
    }

}
