using Godot;
using System;

public partial class TestMeshCreation : Node3D
{
    public override void _Ready()
    {
        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);

        Vector3[] vertices = new Vector3[]
        {
            // Front face
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),

            // Back face
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1)
        };

        int[][] triangles = new int[][]
        {
            // Front face
            new int[] {0, 1, 2}, new int[] {0, 2, 3},
            // Back face
            new int[] {4, 5, 6}, new int[] {4, 6, 7},
            // Bottom face
            new int[] {0, 1, 5}, new int[] {0, 5, 4},
            // Top face
            new int[] {3, 2, 6}, new int[] {3, 6, 7},
            // Left face
            new int[] {0, 3, 7}, new int[] {0, 7, 4},
            // Right face
            new int[] {1, 2, 6}, new int[] {1, 6, 5}
        };

        Color[] colors = new Color[]
        {
            Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow,
            Colors.Cyan, Colors.Magenta, Colors.Orange, Colors.Purple
        };

        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
        };

        for (int i = 0; i < triangles.Length; i++)
        {
            st.SetColor(colors[i % colors.Length]);
            for (int j = 0; j < 3; j++)
            {
                st.SetUV(uvs[triangles[i][j]]);
                st.AddVertex(vertices[triangles[i][j]]);
            }
        }

        st.Index();
        ArrayMesh mesh = st.Commit();

        MeshInstance3D meshInstance = new MeshInstance3D();
        meshInstance.Mesh = mesh;

        AddChild(meshInstance);
    }

    public override void _Process(double delta)
    {
    }
}
