using Godot;
using System.Collections.Generic;

public partial class TestShape : Node3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Define cube vertices
        List<Vector3> vertices = new List<Vector3>()
        {
            new Vector3(-1,  1,  1), // pTLF
            new Vector3( 1,  1,  1), // pTRF
            new Vector3( 1,  1, -1), // pTRB
            new Vector3(-1,  1, -1), // pTLB

            new Vector3(-1, -1,  1), // pBLF
            new Vector3( 1, -1,  1), // pBRF
            new Vector3( 1, -1, -1), // pBRB
            new Vector3(-1, -1, -1)  // pBLB
        };

        // Define cube faces (triangles)
        List<int> triangles = new List<int>()
        {
            // Top face (CCW order)
            0, 2, 1,
            0, 3, 2,

            // Bottom face (CCW order)
            4, 5, 6,
            4, 6, 7,

            // Front face (CCW order)
            1, 5, 0,
            0, 5, 4,

            // Back face (CCW order)
            3, 6, 2,
            3, 7, 6,

            // Left face (CCW order)
            0, 3, 7,
            0, 4, 7,

            // Right face (CCW order)
            1, 2, 6,
            1, 6, 5
        };

        // Create lists for surface data
        List<Vector3> vertList  = new List<Vector3>();
        List<Vector3> normalList = new List<Vector3>();
        List<Vector2> uvList     = new List<Vector2>();
        List<int> indexList      = new List<int>();

        // Populate surface lists with cube data
        for (int i = 0; i < vertices.Count; i++)
        {
            vertList.Add(vertices[i]);
            normalList.Add(vertices[i].Normalized()); // Use vertex position as a placeholder for normals
            uvList.Add(new Vector2()); // Dummy UVs
        }

        // Add triangles to index list
        indexList.AddRange(triangles);

        // Convert lists to arrays for Godot's surfaceArray
        Godot.Collections.Array surfaceArray = new Godot.Collections.Array();
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = vertList.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = normalList.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV]  = uvList.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index]  = indexList.ToArray();

        // Create the ArrayMesh and add the cube surface
        ArrayMesh cubeMesh = new ArrayMesh();
        cubeMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        // Create a MeshInstance3D and set its properties
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = cubeMesh;
        meshInstance.Scale = new Vector3(0.5f, 0.5f, 0.5f);

        // Assign wireframe material
        meshInstance.MaterialOverride = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);

        // Add the MeshInstance3D to the scene
        AddChild(meshInstance);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Optionally add update logic here
    }
}
