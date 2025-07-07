using System.Collections.Generic;
using Godot;

// File of specialisations for the GloMeshData2 class for Godot.

public partial class GloMeshData2
{
    // --------------------------------------------------------------------------------------------
    // MARK: Godot Mesh output
    // --------------------------------------------------------------------------------------------

    public ArrayMesh BuildMesh(bool recalcNormals = false)
    {
        // Create mesh to return
        var arrayMesh = new ArrayMesh();

        // Create an array to hold the mesh data
        var arrays    = new Godot.Collections.Array();

        // Create arrays for each of the mesh data types
        var verticesArray = new Godot.Collections.Array<Vector3>(Vertices);

        List<int> triangles = new List<int>();
        foreach (var triangle in Triangles)
        {
            triangles.Add(triangle.Item1);
            triangles.Add(triangle.Item2);
            triangles.Add(triangle.Item3);
        }
        var trianglesArray = new Godot.Collections.Array<int>(triangles);

        var normalsArray  = new Godot.Collections.Array<Vector3>(Normals);
        var uvsArray      = new Godot.Collections.Array<Vector2>(UVs);
        var colorsArray   = new Godot.Collections.Array<Color>(VertexColors);

        // Resize the arrays to hold all the data
        arrays[(int)ArrayMesh.ArrayType.Vertex] = verticesArray;
        arrays[(int)ArrayMesh.ArrayType.Index]  = trianglesArray;
        arrays[(int)ArrayMesh.ArrayType.Normal] = normalsArray;
        arrays[(int)ArrayMesh.ArrayType.Color]  = colorsArray;
        arrays[(int)ArrayMesh.ArrayType.TexUV]  = uvsArray;

        arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        if (recalcNormals)
        {
            SurfaceTool st = new SurfaceTool();
            st.CreateFrom(arrayMesh, 0);
            st.GenerateNormals();
            arrayMesh = st.Commit();
        }

        return arrayMesh;
    }

    // --------------------------------------------------------------------------------------------

    public Mesh BuildLineMesh()
    {
        // Create a new SurfaceTool for building the line mesh
        var surfaceTool = new SurfaceTool();

        // Begin the SurfaceTool with the PrimitiveType.Lines
        surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        int vertexCount = Vertices.Count; // Get the number of vertices
        int maxVertexId = vertexCount - 1;

        // Iterate through the lines in the mesh data
        foreach (var line in Lines)
        {
            int   p1Id    = line.Item1; // Index of the first vertex
            int   p2Id    = line.Item2; // Index of the second vertex
            Color p1Color = line.Item3; // Color for the start point
            Color p2Color = line.Item4; // Color for the end point

            // Basic validation to stop out of range errors
            if (p1Id < 0 || p1Id > maxVertexId || p2Id < 0 || p2Id > maxVertexId)
                continue;

            Vector3 p1 = Vertices[line.Item1]; // Start point of the line
            Vector3 p2 = Vertices[line.Item2]; // End point of the line

            // Add the first vertex with its color
            surfaceTool.SetColor(p1Color);
            surfaceTool.AddVertex(p1);

            // Add the second vertex with its color
            surfaceTool.SetColor(p2Color);
            surfaceTool.AddVertex(p2);
        }

        // Commit the surface tool to create the mesh and return it
        Mesh mesh = surfaceTool.Commit();
        return mesh;
    }
}
