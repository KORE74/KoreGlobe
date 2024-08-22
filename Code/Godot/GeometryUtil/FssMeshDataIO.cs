using System.IO;
using System.Text.Json;

using Godot;

public static class FssMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Binary IO
    // --------------------------------------------------------------------------------------------

    // Usage: FssMeshDataIO.WriteMeshToFile(meshData, path);
    public static void WriteMeshToFile(FssMeshData meshData, string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(meshData.Name);

            // Write Vertices count and Vertices
            writer.Write(meshData.Vertices.Count);
            foreach (var vertex in meshData.Vertices)
            {
                writer.Write(vertex.X);
                writer.Write(vertex.Y);
                writer.Write(vertex.Z);
            }

            // Write Triangles count and Triangles
            writer.Write(meshData.Triangles.Count);
            foreach (var triangle in meshData.Triangles)
            {
                writer.Write(triangle);
            }

            // Write Normals count and Normals
            writer.Write(meshData.Normals.Count);
            foreach (var normal in meshData.Normals)
            {
                writer.Write(normal.X);
                writer.Write(normal.Y);
                writer.Write(normal.Z);
            }

            // Write UVs count and UVs
            writer.Write(meshData.UVs.Count);
            foreach (var uv in meshData.UVs)
            {
                writer.Write(uv.X);
                writer.Write(uv.Y);
            }
        }
    }

    // Usage: FssMeshData meshData = FssMeshDataIO.ReadMeshFromFile(path);
    public static FssMeshData ReadMeshFromFile(string path)
    {
        FssMeshData meshData = new FssMeshData();

        if (!File.Exists(path))
        {
            meshData = FssMeshData.DefaultCube();
            meshData.Name = $"DefaultCube: Failed To Load {path}";
            return meshData;
        }

        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            meshData.Name = reader.ReadString();

            // Read Vertices
            int verticesCount = reader.ReadInt32();
            for (int i = 0; i < verticesCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                Vector3 vertex = new Vector3(x, y, z);
                meshData.Vertices.Add(vertex);
            }

            // Read Triangles
            int trianglesCount = reader.ReadInt32();
            for (int i = 0; i < trianglesCount; i++)
            {
                int triangle = reader.ReadInt32();
                meshData.Triangles.Add(triangle);
            }

            // Read Normals
            int normalsCount = reader.ReadInt32();
            for (int i = 0; i < normalsCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                Vector3 norm = new Vector3(x, y, z);
                meshData.Normals.Add(norm);
            }

            // Read UVs
            int uvsCount = reader.ReadInt32();
            for (int i = 0; i < uvsCount; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                Vector2 uv = new Vector2(x, y);
                meshData.UVs.Add(uv);
            }
        }

        return meshData;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: JSON IO
    // --------------------------------------------------------------------------------------------

    // Usage: FssMeshDataIO.WriteMeshToJSON(meshData, path);
    public static void WriteMeshToJSON(FssMeshData meshData, string path)
    {
        string jsonString = JsonSerializer.Serialize(meshData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, jsonString);
    }

    // Usage: FssMeshData meshData = FssMeshDataIO.ReadMeshFromJSON(path);

    public static FssMeshData ReadMeshFromJSON(string path)
    {
        FssMeshData meshData = new FssMeshData();

        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            meshData = JsonSerializer.Deserialize<FssMeshData>(jsonString);
        }
        else
        {
            meshData.Name = $"DefaultCube: Failed To Load {path}";
        }

        return meshData;
    }
}
