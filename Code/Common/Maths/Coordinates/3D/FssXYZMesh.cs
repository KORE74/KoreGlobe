using System;
using System.Collections.Generic;

// FssXYZPoint: A class to hold an XYZ position. Units are abstract.
// Class has operations to move the points as required by a 3D viewer.

public class FssXYZMesh : FssXYZ
{
    public List<FssXYZ> Vertices { get; }
    public List<int> TriangleIds { get; } // List of triangle IDs, should be 3x the number of vertices

    // --------------------------------------------------------------------------------------------
    // Constructor
    // --------------------------------------------------------------------------------------------

    public FssXYZMesh(List<FssXYZ> vertices, List<int> triangleIds)
    {
        Vertices = vertices;
        TriangleIds = triangleIds;
    }
}