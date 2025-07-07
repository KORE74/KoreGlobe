using Godot;
using System.Collections.Generic;

public partial class GloLineMesh3D : Node3D
{
    private MeshInstance3D _meshInstance;
    private SurfaceTool    _surfaceTool;
    private bool           _meshNeedsUpdate = false;

    // List to store line segments along with their colors
    private List<(Vector3, Vector3, Color, Color)> _lines = new List<(Vector3, Vector3, Color, Color)>();

    // Static shader and material shared among all instances
    private static Shader         _vertexColorShader   = null;
    private static ShaderMaterial _vertexColorMaterial = null;

    public enum LineStyle { Solid, Dashed, DashedLimit };

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
        //CreateCube();
    }

    public override void _Process(double delta)
    {
        if (_meshNeedsUpdate)
        {
            UpdateMesh();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Core LineMesh3D Functions
    // --------------------------------------------------------------------------------------------

    // Function to add a line between two points with a specified color
    public void AddLine(Vector3 p1, Vector3 p2, Color linecolor)
    {
        AddLine(p1, p2, linecolor, linecolor);
    }

    public void AddLine(Vector3 p1, Vector3 p2, Color p1Color, Color p2Color)
    {
        _lines.Add((p1, p2, p1Color, p2Color));
        _meshNeedsUpdate = true;
    }

    // Function to clear all lines from the mesh

    public void Clear()
    {
        _lines.Clear();
        _meshNeedsUpdate = true;
    }

    // --------------------------------------------------------------------------------------------

    // Function to add a dotted line, for some added value beyond the colored solid lines.

    public void AddDottedLine(Vector3 p1, Vector3 p2, Color color, float segmentLength = 0.1f)
    {
        Vector3 direction = p2 - p1;
        float distance = direction.Length();
        direction = direction.Normalized();

        for (float i = 0; i < distance; i += 2 * segmentLength)
        {
            Vector3 start = p1 + direction * i;
            Vector3 end = p1 + direction * Mathf.Min(i + segmentLength, distance);
            AddLine(start, end, color);
        }
    }

    // --------------------------------------------------------------------------------------------

    // Function to rebuild the mesh when new lines are added
    private void UpdateMesh()
    {
        if (!_meshNeedsUpdate)
            return;

        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Lines);

        foreach (var line in _lines)
        {
            Vector3 p1      = line.Item1;
            Vector3 p2      = line.Item2;
            Color   p1color = line.Item3;
            Color   p2color = line.Item4;

            _surfaceTool.SetColor(p1color);
            _surfaceTool.AddVertex(p1);

            _surfaceTool.SetColor(p2color);
            _surfaceTool.AddVertex(p2);
        }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        _meshInstance.Mesh = mesh;

        _meshNeedsUpdate = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Additional Line funcs
    // --------------------------------------------------------------------------------------------

    //public void AddLine(Vector3 p1, Vector3 p2, Color color)         => AddLine(p1, p2, color);
    public void AddLine(GloXYZPoint p1, GloXYZPoint p2, Color color) => AddLine(GloGodotGeometryOperations.ToV3(p1), GloGodotGeometryOperations.ToV3(p2), color);
    public void AddLine(GloXYZLine l1, Color color)                  => AddLine(l1.P1, l1.P2, color);

    // --------------------------------------------------------------------------------------------
    // MARK: Add box - Next level complexity
    // --------------------------------------------------------------------------------------------

    public void AddBox(GloXYZBox xyzBox, Color color)
    {
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.TopFront),    color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.TopBack),     color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.TopLeft),     color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.TopRight),    color);

        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.FrontLeft),   color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.FrontRight),  color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BackLeft),    color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BackRight),   color);

        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BottomFront), color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BottomBack),  color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BottomLeft),  color);
        AddLine(xyzBox.Edge(GloXYZBox.EnumEdge.BottomRight), color);
    }

    // --------------------------------------------------------------------------------------------

    // Add a line on the front-top edge of the box, so we can check orientation

    public void AddBoxWithLeadingEdge(GloXYZBox xyzBox, Color color)
    {
        // Add all the edges of the box using AddBox
        AddBox(xyzBox, color);

        // inset the box in width and length, after figuring out a fraction of the width
        double insetFraction = 0.1;
        double insetWidth = xyzBox.Width * insetFraction;

        // Inset the box - width and length, so we have a top-front line to get and draw
        GloXYZBox insetBox = xyzBox.Inset(insetWidth, 0, insetWidth);

        // Get the top-front edge of the inset box and add it to the box
        AddLine(insetBox.Edge(GloXYZBox.EnumEdge.TopFront), color);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Add Surface Function
    // --------------------------------------------------------------------------------------------

    // Function to add lines for a given 2D surface array
    public void AddSurface(Vector3[,] points, Color color)
    {
        int rows = points.GetLength(0);
        int cols = points.GetLength(1);

        // Add lines for rows
        for (int r = 0; r < rows - 1; r++)
        {
            for (int c = 0; c < cols - 1; c++)
            {
                AddLine(points[r, c], points[r, c + 1], color); // Horizontal line
                AddLine(points[r, c], points[r + 1, c], color); // Vertical line
            }
            // Connect the last column in this row to the next row
            AddLine(points[r, cols - 1], points[r + 1, cols - 1], color);
        }

        // Add lines for the last row
        for (int c = 0; c < cols - 1; c++)
        {
            AddLine(points[rows - 1, c], points[rows - 1, c + 1], color); // Horizontal line
        }
    }

    // Function to add lines for a given 2D surface array
    // [X,Y]
    public void AddSurface(Vector3[,] points, Color color, Color color2)
    {
        int rows = points.GetLength(1);
        int cols = points.GetLength(0);

        Color currHorizColor = color;
        Color currVertColor  = color2;

        // Add lines for rows
        for (int ry = 0; ry < rows - 1; ry++)
        {
            currHorizColor = ((ry == 0) || (ry == rows - 1)) ? color2 : color;
            for (int cx = 0; cx < cols - 1; cx++)
            {
                currVertColor = ((cx == 0) || (cx == cols - 1)) ? color2 : color;

                AddLine(points[cx, ry], points[cx + 1, ry], currHorizColor); // Horizontal line
                AddLine(points[cx, ry], points[cx, ry + 1], currVertColor); // Vertical line
            }
            // Connect the last column in this row to the next row
            AddLine(points[cols - 1, ry], points[cols - 1, ry + 1], color2);
        }

        // Add lines for the last row
        for (int cx = 0; cx < cols - 1; cx++)
        {
            AddLine(points[cx, rows - 1], points[cx + 1, rows - 1], color2); // Horizontal line
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Internal Test Functions
    // --------------------------------------------------------------------------------------------

    // Example usage: Create a cube with colored edges
    public void CreateTestCube(float sideLength = 1.0f)
    {
        float halfLen = sideLength / 2.0f;

        Vector3[] cubeCorners = new Vector3[]
        {
            new Vector3(-halfLen, -halfLen, -halfLen), // 0
            new Vector3( halfLen, -halfLen, -halfLen), // 1
            new Vector3( halfLen,  halfLen, -halfLen), // 2
            new Vector3(-halfLen,  halfLen, -halfLen), // 3
            new Vector3(-halfLen, -halfLen,  halfLen), // 4
            new Vector3( halfLen, -halfLen,  halfLen), // 5
            new Vector3( halfLen,  halfLen,  halfLen), // 6
            new Vector3(-halfLen,  halfLen,  halfLen)  // 7
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
                    ALPHA = COLOR.a;
                }
                ";

            // Create the material and assign the shader
            _vertexColorMaterial = new ShaderMaterial();
            _vertexColorMaterial.Shader = _vertexColorShader;
        }

        return _vertexColorMaterial;
    }
}
