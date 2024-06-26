using System.Collections.Generic;
using System.IO;

using Godot;

public class FssMeshData
{
    public string Name;

    public List<Vector3> Vertices;
    public List<int>     Triangles;
    public List<Vector3> Normals;
    public List<Vector2> UVs;

    public FssMeshData()
    {
        this.Name      = string.Empty;
        this.Vertices  = new List<Vector3>();
        this.Triangles = new List<int>();
        this.Normals   = new List<Vector3>();
        this.UVs       = new List<Vector2>();
    }

    public FssMeshData(string inName, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.Name      = inName;
        this.Vertices  = vertices;
        this.Triangles = triangles;
        this.Normals   = new List<Vector3>();
        this.UVs       = uvs;
    }


    public ArrayMesh BuildMesh(bool recalcNormals = false)
    {
        var arrayMesh = new ArrayMesh();
        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)ArrayMesh.ArrayType.Max);

        var verticesArray = new Godot.Collections.Array<Vector3>(Vertices);
        var indicesArray = new Godot.Collections.Array<int>(Triangles);
        var normalsArray = new Godot.Collections.Array<Vector3>(Normals);
        var uvsArray = new Godot.Collections.Array<Vector2>(UVs);

        arrays[(int)ArrayMesh.ArrayType.Vertex] = verticesArray;
        arrays[(int)ArrayMesh.ArrayType.Index] = indicesArray;
        arrays[(int)ArrayMesh.ArrayType.Normal] = normalsArray;
        // arrays[(int)ArrayMesh.ArrayType.TexUv] = uvsArray;

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
    public static FssMeshData DefaultCube()
    {
        return new FssMeshData
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
            }
        };
    }
}
