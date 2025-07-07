using Godot;
using System;


// Usage: AddChild (new TesColoredCube());

public partial class TesColoredCube : Node3D
{
    public override void _Ready()
    {
        ArrayMesh mesh = new ArrayMesh();
        var arrays = new Godot.Collections.Array();

        // Define the vertices of the cube
        var vertices = new Godot.Collections.Array<Vector3>
        {
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1)
        };

        // Define the colors of the vertices
        var colors = new Godot.Collections.Array<Color>
        {
            new Color(1, 0, 0), // Red
            new Color(0, 1, 0), // Green
            new Color(0, 0, 1), // Blue
            new Color(1, 1, 0), // Yellow
            new Color(1, 0, 1), // Magenta
            new Color(0, 1, 1), // Cyan
            new Color(1, 1, 1), // White
            new Color(0, 0, 0)  // Black
        };

        // Define the indices of the triangles
        var indices = new Godot.Collections.Array<int>
        {
            0, 1, 2, 0, 2, 3, // Front face
            4, 7, 6, 4, 6, 5, // Back face
            0, 4, 5, 0, 5, 1, // Bottom face
            1, 5, 6, 1, 6, 2, // Right face
            2, 6, 7, 2, 7, 3, // Top face
            3, 7, 4, 3, 4, 0  // Left face
        };

        // Assign the arrays to the mesh
        arrays.Resize((int)ArrayMesh.ArrayType.Max);
        arrays[(int)ArrayMesh.ArrayType.Vertex] = vertices;
        arrays[(int)ArrayMesh.ArrayType.Color] = colors;
        arrays[(int)ArrayMesh.ArrayType.Index] = indices;

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        // Create a MeshInstance3D and assign the mesh
        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;


        // // Load the shader material that uses vertex colors
        // ShaderMaterial vertexColorMaterial = new ShaderMaterial();
        // Shader shader = (Shader)GD.Load("res://Shaders/vertex_color_shader.tres");
        // vertexColorMaterial.Shader = shader;



        var matColor = GloMaterialFactory.VertexColorMaterial();
        var matBlue  = GloMaterialFactory.SimpleColoredMaterial(new Color(0.0f, 0.5f, 1.0f, 1.0f));

        // Assign the material to the mesh instance
        //meshInstance.MaterialOverride = vertexColorMaterial;
        meshInstance.MaterialOverride  = matBlue;

        // Add the mesh instance to the current node
        AddChild(meshInstance);
    }
}
