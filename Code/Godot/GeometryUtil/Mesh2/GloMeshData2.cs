using System.Collections.Generic;
using Godot;

public partial class GloMeshData2
{
    public string Name;

    public List<Vector3>                  Vertices;     // List of vertices
    public List<(int, int, Color, Color)> Lines;        // List of lines
    public List<(int, int, int)>          Triangles;    // List of triangles
    public List<Vector3>                  Normals;      // List of normal per vertex
    public List<Vector2>                  UVs;          // List of UV coordinates per vertex
    public List<Color>                    VertexColors; // List of vertex colors
    // public List<(int, Color, Color)>      LineColors;   // List of line ids, start and end colors
    // public List<(int, Color)>             TriangleColors; // List of triangle ids and colors

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors
    // --------------------------------------------------------------------------------------------

    // Empty constructor
    public GloMeshData2()
    {
        this.Name         = string.Empty;
        this.Vertices     = new List<Vector3>();
        this.Lines        = new List<(int, int, Color, Color)>();
        this.Triangles    = new List<(int, int, int)>();
        this.Normals      = new List<Vector3>();
        this.UVs          = new List<Vector2>();
        this.VertexColors = new List<Color>();
    }

    // Copy constructor
    public GloMeshData2(
        string                         inName,
        List<Vector3>                  vertices,
        List<(int, int, Color, Color)> lines,
        List<(int, int, int)>          triangles,
        List<Vector3>                  normals,
        List<Vector2>                  uvs,
        List<Color>                    vertexColors)
    {
        this.Name         = inName;
        this.Vertices     = vertices;
        this.Lines        = lines;
        this.Triangles    = triangles;
        this.Normals      = normals;
        this.UVs          = uvs;
        this.VertexColors = vertexColors;
    }

    // Copy constructor
    public GloMeshData2(GloMeshData2 mesh)
    {
        this.Name         = mesh.Name;
        this.Vertices     = new List<Vector3>(mesh.Vertices);
        this.Lines        = new List<(int, int, Color, Color)>(mesh.Lines);
        this.Triangles    = new List<(int, int, int)>(mesh.Triangles);
        this.Normals      = new List<Vector3>(mesh.Normals);
        this.UVs          = new List<Vector2>(mesh.UVs);
        this.VertexColors = new List<Color>(mesh.VertexColors);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Basic Data Entry Functions
    // --------------------------------------------------------------------------------------------

    // Not the exclusive way to add data, but helper routines.

    public int AddPoint(Vector3 vertex, Vector3? normal = null, Color? color = null)
    {
        int index = Vertices.Count;
        Vertices.Add(vertex);

        // Compute a default normal if one is not provided.
        if (normal.HasValue)
            Normals.Add(normal.Value);
        else
            Normals.Add(vertex == Vector3.Zero ? new Vector3(0, 1, 0) : vertex.Normalized());

        // Use provided color or default to White.
        VertexColors.Add(color ?? new Color(1, 1, 1));

        return index;
    }

    // --------------------------------------------------------------------------------------------

    public void AddLine(int idxA, int idxB, Color colStart, Color colEnd)
    {
        Lines.Add((idxA, idxB, colStart, colEnd));
    }

    public void AddLine(Vector3 start, Vector3 end, Color colStart, Color colEnd)
    {
        int idxA = AddPoint(start, null, colStart);
        int idxB = AddPoint(end, null, colEnd);

        AddLine(idxA, idxB, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------

    public void AddDottedLine(
        Vector3 start, Vector3 end,
        Color colStart, Color colEnd,
        int numDots)
    {
        Vector3 direction = (end - start).Normalized();
        float   length    = (end - start).Length();
        float   step      = length / (numDots + 1);

        for (int i = 0; i < numDots; i++)
        {
            Vector3 point = start + direction * step * (i + 1);
            AddPoint(point, null, colStart);
        }

        AddLine(start, end, colStart, colEnd);
    }

    // --------------------------------------------------------------------------------------------

    public void AddTriangle(int idxA, int idxB, int idxC)
    {
        Triangles.Add((idxA, idxB, idxC));
    }

    public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color? color = null)
    {
        int idxA = AddPoint(a);
        int idxB = AddPoint(b);
        int idxC = AddPoint(c);

        if (color.HasValue)
        {
            VertexColors[idxA] = color.Value;
            VertexColors[idxB] = color.Value;
            VertexColors[idxC] = color.Value;
        }

        Color lineColor = color ?? new Color(1, 1, 1);
        AddLine(idxA, idxB, lineColor, lineColor);
        AddLine(idxB, idxC, lineColor, lineColor);
        AddLine(idxC, idxA, lineColor, lineColor);

        // Add the triangle to the list
        Triangles.Add((idxA, idxB, idxC));
    }

    // --------------------------------------------------------------------------------------------

    public void AddNormal(Vector3 normal)
    {
        Normals.Add(normal);
    }
    public void AddUV(Vector2 uv)
    {
        UVs.Add(uv);
    }
    public void AddColor(Color color)
    {
        VertexColors.Add(color);
    }

}
