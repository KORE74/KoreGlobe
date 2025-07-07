using System.Collections.Generic;
using Godot;

public class GloMeshData
{
    public string Name;

    public List<Vector3> Vertices;
    public List<int>     Triangles;
    public List<Vector3> Normals;
    public List<Vector2> UVs;
    public List<Color>   Colors; // Add this line to include colors

    public GloMeshData()
    {
        this.Name      = string.Empty;
        this.Vertices  = new List<Vector3>();
        this.Triangles = new List<int>();
        this.Normals   = new List<Vector3>();
        this.UVs       = new List<Vector2>();
        this.Colors    = new List<Color>(); // Initialize the colors list
    }

    public GloMeshData(string inName, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, List<Color> colors)
    {
        this.Name      = inName;
        this.Vertices  = vertices;
        this.Triangles = triangles;
        this.Normals   = new List<Vector3>();
        this.UVs       = uvs;
        this.Colors    = colors; // Initialize the colors list
    }

    public ArrayMesh BuildMesh(bool recalcNormals = false)
    {
        var arrayMesh = new ArrayMesh();
        var arrays    = new Godot.Collections.Array();

        arrays.Resize((int)ArrayMesh.ArrayType.Max);

        var verticesArray = new Godot.Collections.Array<Vector3>(Vertices);
        var indicesArray  = new Godot.Collections.Array<int>(Triangles);
        var normalsArray  = new Godot.Collections.Array<Vector3>(Normals);
        var uvsArray      = new Godot.Collections.Array<Vector2>(UVs);
        var colorsArray   = new Godot.Collections.Array<Color>(Colors);

        arrays[(int)ArrayMesh.ArrayType.Vertex] = verticesArray;
        arrays[(int)ArrayMesh.ArrayType.Index]  = indicesArray;
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

    // Basic cube mesh, to be a default value.
    public static GloMeshData DefaultCube()
    {
        return new GloMeshData
        {
            Name = "DefaultCube",
            Vertices = new List<Vector3> {
                new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0),
                new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 0, 1), new Vector3(0, 0, 1),
            },
            Triangles = new List<int> {
                0, 2, 1,  0, 3, 2,  2, 3, 4,  2, 4, 5,  1, 2, 5,  1, 5, 6,  0, 7, 4,  0, 4, 3,  5, 4, 7,  5, 7, 6,  0, 6, 7,  0, 1, 6
            },
            Normals = new List<Vector3>(),
            UVs = new List<Vector2> {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0),
            },
            Colors = new List<Color> { // Add default colors if needed
                new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1), new Color(1, 1, 0),
                new Color(1, 0, 1), new Color(0, 1, 1), new Color(1, 1, 1), new Color(0, 0, 0)
            }
        };
    }
}
