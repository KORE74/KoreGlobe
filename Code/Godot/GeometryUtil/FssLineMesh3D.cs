using Godot;
using System.Collections.Generic;

public partial class FssLineMesh3D : Node3D
{
    private MeshInstance3D _meshInstance;
    private SurfaceTool    _surfaceTool;
    private bool           _meshNeedsUpdate = false;

    // List to store line segments along with their colors
    private List<(Vector3, Vector3, Color)> _lines = new List<(Vector3, Vector3, Color)>();

    // Static shader and material shared among all instances
    private static Shader         _vertexColorShader   = null;
    private static ShaderMaterial _vertexColorMaterial = null;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Create a MeshInstance3D to hold the generated line mesh
        _meshInstance = new MeshInstance3D();
        AddChild(_meshInstance);

        // Initialize the SurfaceTool
        _surfaceTool = new SurfaceTool();

        // Apply the shared Vertex Color Material to the MeshInstance3D
        _meshInstance.MaterialOverride = GetSharedVertexColorMaterial();

        // Debug Call: Create a cube with colored edges
        CreateCube();
    }

    public override void _Process(double delta)
    {
        if (_meshNeedsUpdate)
        {
            UpdateMesh();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: LineMesh3D Functions
    // --------------------------------------------------------------------------------------------

    // Function to add a line between two points with a specified color
    public void AddLine(Vector3 p1, Vector3 p2, Color color)
    {
        _lines.Add((p1, p2, color));
        _meshNeedsUpdate = true;
    }

    // Function to clear all lines from the mesh

    public void Clear()
    {
        _lines.Clear();
        _meshNeedsUpdate = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Internal Mesh Generation
    // --------------------------------------------------------------------------------------------

    // Function to rebuild the mesh when new lines are added
    private void UpdateMesh()
    {
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        foreach (var line in _lines)
        {
            Vector3 p1 = line.Item1;
            Vector3 p2 = line.Item2;
            Color color = line.Item3;

            _surfaceTool.SetColor(color);
            _surfaceTool.AddVertex(p1);

            _surfaceTool.SetColor(color);
            _surfaceTool.AddVertex(p2);
        }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        _meshInstance.Mesh = mesh;

        _meshNeedsUpdate = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Internal Test Functions
    // --------------------------------------------------------------------------------------------

    // Example usage: Create a cube with colored edges
    private void CreateCube()
    {
        Vector3[] cubeCorners = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),  // 0
            new Vector3(0.5f, -0.5f, -0.5f),   // 1
            new Vector3(0.5f, 0.5f, -0.5f),    // 2
            new Vector3(-0.5f, 0.5f, -0.5f),   // 3
            new Vector3(-0.5f, -0.5f, 0.5f),   // 4
            new Vector3(0.5f, -0.5f, 0.5f),    // 5
            new Vector3(0.5f, 0.5f, 0.5f),     // 6
            new Vector3(-0.5f, 0.5f, 0.5f)     // 7
        };

        int[,] edges = new int[,]
        {
            { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 }, // Bottom face
            { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 }, // Top face
            { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }  // Vertical edges
        };

        // Assign different colors to each face
        Color[] colors = new Color[]
        {
            new Color(1, 0, 0), // Red
            new Color(0, 1, 0), // Green
            new Color(0, 0, 1), // Blue
            new Color(1, 1, 0), // Yellow
            new Color(1, 0, 1), // Magenta
            new Color(0, 1, 1)  // Cyan
        };

        // Add lines with colors
        for (int i = 0; i < edges.GetLength(0); i++)
        {
            // Cycle through colors for demonstration
            Color color = colors[i % colors.Length];
            AddLine(cubeCorners[edges[i, 0]], cubeCorners[edges[i, 1]], color);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Material and Shader Functions
    // --------------------------------------------------------------------------------------------

    // Function to get the shared vertex color material
    private static ShaderMaterial GetSharedVertexColorMaterial()
    {
        if (_vertexColorMaterial == null)
        {
            // Create the shader only once
            _vertexColorShader = new Shader();
            _vertexColorShader.Code = @"
                shader_type spatial;

                render_mode unshaded, depth_draw_always, cull_back;

                void vertex() {
                    // Called for every vertex the material is visible on.
                }

                void fragment() {
                    // Called for every pixel the material is visible on.
                    ALBEDO = COLOR.rgb;
                    ALPHA = 1.0;
                }
                ";

            // Create the material and assign the shader
            _vertexColorMaterial = new ShaderMaterial();
            _vertexColorMaterial.Shader = _vertexColorShader;
        }

        return _vertexColorMaterial;
    }
}
