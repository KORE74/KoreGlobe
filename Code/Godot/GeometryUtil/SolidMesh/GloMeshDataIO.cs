using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Godot;

public static class GloMeshDataIO
{
    // --------------------------------------------------------------------------------------------
    // MARK: Binary IO
    // --------------------------------------------------------------------------------------------

    // Usage: GloMeshDataIO.WriteMeshToFile(meshData, path);
    public static void WriteMeshToFile(GloMeshData meshData, string path)
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

    // Usage: GloMeshData meshData = GloMeshDataIO.ReadMeshFromFile(path);
    public static GloMeshData ReadMeshFromFile(string path)
    {
        GloMeshData meshData = new GloMeshData();

        if (!File.Exists(path))
        {
            meshData = GloMeshData.DefaultCube();
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

    // Usage: GloMeshDataIO.WriteMeshToJSON(meshData, path);
    public static void WriteMeshToJSON(GloMeshData meshData, string path)
    {
        string jsonString = JsonSerializer.Serialize(meshData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, jsonString);
    }

    // Usage: GloMeshData meshData = GloMeshDataIO.ReadMeshFromJSON(path);

    public static GloMeshData ReadMeshFromJSON(string path)
    {
        GloMeshData meshData = new GloMeshData();

        if (File.Exists(path))
        {
            string jsonString = File.ReadAllText(path);
            meshData = JsonSerializer.Deserialize<GloMeshData>(jsonString);
        }
        else
        {
            meshData.Name = $"DefaultCube: Failed To Load {path}";
        }

        return meshData;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Bytes
    // --------------------------------------------------------------------------------------------


    // Convert the data structure to a byte array, looping through the data and handling everything in binary
    public static Byte[] MeshDataToBytes2(GloMeshData meshData)
    {
        // Write the number of vertices, triangles, normals, and uvs
        GloByteArrayWriter bwriter = new GloByteArrayWriter();

        // write out the number of list items
        bwriter.WriteInt(meshData.Vertices.Count);
        bwriter.WriteInt(meshData.Triangles.Count);
        bwriter.WriteInt(meshData.Normals.Count);
        bwriter.WriteInt(meshData.UVs.Count);

        foreach (var vertex in meshData.Vertices)
        {
            bwriter.WriteFloat(vertex.X);
            bwriter.WriteFloat(vertex.Y);
            bwriter.WriteFloat(vertex.Z);

        }
        foreach (var triangle in meshData.Triangles)
        {
            bwriter.WriteInt(triangle);
        }
        foreach (var normal in meshData.Normals)
        {
            bwriter.WriteFloat(normal.X);
            bwriter.WriteFloat(normal.Y);
            bwriter.WriteFloat(normal.Z);
        }
        foreach (var uv in meshData.UVs)
        {
            bwriter.WriteFloat(uv.X);
            bwriter.WriteFloat(uv.Y);
        }

        return bwriter.ToArray();
    }

    public static GloMeshData BytesToMeshData2(Byte[] bytes)
    {
        GloByteArrayReader breader = new GloByteArrayReader(bytes);

        GloMeshData meshData = new GloMeshData();

        // Read the number of vertices, triangles, normals, and uvs
        int numVertices  = breader.ReadInt();
        int numTriangles = breader.ReadInt();
        int numNormals   = breader.ReadInt();
        int numUVs       = breader.ReadInt();

        for (int iv = 0; iv < numVertices; iv++)
        {
            float x = breader.ReadFloat();
            float y = breader.ReadFloat();
            float z = breader.ReadFloat();
            meshData.Vertices.Add(new Vector3(x, y, z));
        }
        for (int it = 0; it < numTriangles; it++)
            meshData.Triangles.Add(breader.ReadInt());

        for (int i = 0; i < numNormals; i++)
        {
            float x = breader.ReadFloat();
            float y = breader.ReadFloat();
            float z = breader.ReadFloat();
            meshData.Normals.Add(new Vector3(x, y, z));
        }
        for (int iu = 0; iu < numUVs; iu++)
        {
            float x = breader.ReadFloat();
            float y = breader.ReadFloat();
            meshData.UVs.Add(new Vector2(x, y));
        }

        return meshData;
    }



}
