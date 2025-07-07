using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

// Static class to create GloMeshData2 primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class GloMeshData2Primitives
{
    public static GloMeshData2 BasicCube(float size, Color color)
    {
        // Create a new GloMeshData2 object
        var mesh = new GloMeshData2() { Name = "Cube" };

        // Define the vertices of the cube
        mesh.Vertices.Add( new Vector3(-size, -size, -size));
        mesh.Vertices.Add( new Vector3( size, -size, -size));
        mesh.Vertices.Add( new Vector3( size,  size, -size));
        mesh.Vertices.Add( new Vector3(-size,  size, -size));
        mesh.Vertices.Add( new Vector3(-size, -size,  size));
        mesh.Vertices.Add( new Vector3( size, -size,  size));
        mesh.Vertices.Add( new Vector3( size,  size,  size));
        mesh.Vertices.Add( new Vector3(-size,  size,  size));

        // Define each edge as a line
        mesh.Lines.Add((0, 1, color, color)); // bottom face (-ve Y)
        mesh.Lines.Add((1, 4, color, color));
        mesh.Lines.Add((4, 5, color, color));
        mesh.Lines.Add((5, 0, color, color));
        mesh.Lines.Add((2, 3, color, color)); // top face (+ve Y)
        mesh.Lines.Add((3, 6, color, color));
        mesh.Lines.Add((6, 7, color, color));
        mesh.Lines.Add((7, 2, color, color));
        mesh.Lines.Add((0, 3, color, color)); // Vertical edges (-ve y to +ve y)
        mesh.Lines.Add((1, 2, color, color));
        mesh.Lines.Add((4, 7, color, color));
        mesh.Lines.Add((5, 6, color, color));

        mesh.Triangles.Add((0, 1, 2)); // bottom face (-ve Y)
        mesh.Triangles.Add((0, 2, 3));
        mesh.Triangles.Add((4, 5, 6)); // top face (+ve Y)
        mesh.Triangles.Add((4, 6, 7));
        mesh.Triangles.Add((0, 1, 5)); // side faces
        mesh.Triangles.Add((0, 5, 4));
        mesh.Triangles.Add((1, 2, 6));
        mesh.Triangles.Add((1, 6, 5));
        mesh.Triangles.Add((2, 3, 7));
        mesh.Triangles.Add((2, 7, 6));
        mesh.Triangles.Add((3, 0, 4));
        mesh.Triangles.Add((3, 4, 7));

        // Set normals and colors for each vertex
        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            mesh.Normals.Add(mesh.Vertices[i].Normalized());
            mesh.VertexColors.Add(color);
        }

        return mesh;
    }

    // ---------------------------------------------------------------------------------------------

    public static GloMeshData2 SizedBox(
        float sizeUp, float sizeDown,
        float sizeLeft, float sizeRight,
        float sizeFront, float sizeBack,
        Color color)
    {
        // Create a new GloMeshData2 object
        var mesh = new GloMeshData2() { Name = "SizedBox" };

        // Define 8 unique vertices for the rectangular box
        // Front face vertices:
        mesh.Vertices.Add(new Vector3(-sizeLeft,  -sizeDown, -sizeFront)); // 0: Lower left front
        mesh.Vertices.Add(new Vector3( sizeRight, -sizeDown, -sizeFront)); // 1: Lower right front
        mesh.Vertices.Add(new Vector3( sizeRight,  sizeUp,   -sizeFront)); // 2: Upper right front
        mesh.Vertices.Add(new Vector3(-sizeLeft,   sizeUp,   -sizeFront)); // 3: Upper left front
        // Back face vertices:
        mesh.Vertices.Add(new Vector3(-sizeLeft,  -sizeDown, sizeBack));   // 4: Lower left back
        mesh.Vertices.Add(new Vector3( sizeRight, -sizeDown, sizeBack));   // 5: Lower right back
        mesh.Vertices.Add(new Vector3( sizeRight,  sizeUp,   sizeBack));   // 6: Upper right back
        mesh.Vertices.Add(new Vector3(-sizeLeft,   sizeUp,   sizeBack));   // 7: Upper left back

        // Define edges (lines)
        // Front face edges
        mesh.Lines.Add((0, 1, color, color));
        mesh.Lines.Add((1, 2, color, color));
        mesh.Lines.Add((2, 3, color, color));
        mesh.Lines.Add((3, 0, color, color));
        // Back face edges
        mesh.Lines.Add((4, 5, color, color));
        mesh.Lines.Add((5, 6, color, color));
        mesh.Lines.Add((6, 7, color, color));
        mesh.Lines.Add((7, 4, color, color));
        // Connecting edges between front and back faces
        mesh.Lines.Add((0, 4, color, color));
        mesh.Lines.Add((1, 5, color, color));
        mesh.Lines.Add((2, 6, color, color));
        mesh.Lines.Add((3, 7, color, color));

        // Define triangles for each face (two per face)
        // Front
        mesh.Triangles.Add((0, 1, 2));
        mesh.Triangles.Add((0, 2, 3));
        // Back (winding order reversed if needed)
        mesh.Triangles.Add((4, 6, 5));
        mesh.Triangles.Add((4, 7, 6));
        // Left
        mesh.Triangles.Add((0, 3, 7));
        mesh.Triangles.Add((0, 7, 4));
        // Right
        mesh.Triangles.Add((1, 5, 6));
        mesh.Triangles.Add((1, 6, 2));
        // Top
        mesh.Triangles.Add((3, 2, 6));
        mesh.Triangles.Add((3, 6, 7));
        // Bottom
        mesh.Triangles.Add((0, 4, 5));
        mesh.Triangles.Add((0, 5, 1));

        // Set normals and colors for each vertex
        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            mesh.Normals.Add(mesh.Vertices[i].Normalized());
            mesh.VertexColors.Add(color);
        }

        return mesh;
    }

}
