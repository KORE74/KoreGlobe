using System;
using System.IO;
using System.Collections.Generic;
using Godot;

public static partial class GloMeshData2IO
{
    // --------------------------------------------------------------------------------------------
    // MARK: ToBytes
    // --------------------------------------------------------------------------------------------

    public static byte[] ToBytes(GloMeshData2 mesh)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        // Write name
        bw.Write(mesh.Name ?? "");

        // Vertices
        bw.Write(mesh.Vertices.Count);
        foreach (var v in mesh.Vertices)
        {
            bw.Write(v.X);
            bw.Write(v.Y);
            bw.Write(v.Z);
        }

        // Lines
        bw.Write(mesh.Lines.Count);
        foreach (var l in mesh.Lines)
        {
            bw.Write(l.Item1);
            bw.Write(l.Item2);
            WriteColor(bw, l.Item3);
            WriteColor(bw, l.Item4);
        }

        // Triangles
        bw.Write(mesh.Triangles.Count);
        foreach (var t in mesh.Triangles)
        {
            bw.Write(t.Item1);
            bw.Write(t.Item2);
            bw.Write(t.Item3);
        }

        // Normals
        bw.Write(mesh.Normals.Count);
        foreach (var n in mesh.Normals)
        {
            bw.Write(n.X);
            bw.Write(n.Y);
            bw.Write(n.Z);
        }

        // UVs
        bw.Write(mesh.UVs.Count);
        foreach (var uv in mesh.UVs)
        {
            bw.Write(uv.X);
            bw.Write(uv.Y);
        }

        // VertexColors
        bw.Write(mesh.VertexColors.Count);
        foreach (var c in mesh.VertexColors)
            WriteColor(bw, c);

        bw.Flush();
        return ms.ToArray();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: FromBytes
    // --------------------------------------------------------------------------------------------

    public static GloMeshData2 FromBytes(byte[] data)
    {
        var mesh = new GloMeshData2();
        using var ms = new MemoryStream(data);
        using var br = new BinaryReader(ms);

        // Name
        mesh.Name = br.ReadString();

        // Vertices
        int vCount = br.ReadInt32();
        for (int i = 0; i < vCount; i++)
            mesh.Vertices.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));

        // Lines
        int lCount = br.ReadInt32();
        for (int i = 0; i < lCount; i++)
        {
            int a = br.ReadInt32();
            int b = br.ReadInt32();
            Color cA = ReadColor(br);
            Color cB = ReadColor(br);
            mesh.Lines.Add((a, b, cA, cB));
        }

        // Triangles
        int tCount = br.ReadInt32();
        for (int i = 0; i < tCount; i++)
            mesh.Triangles.Add((br.ReadInt32(), br.ReadInt32(), br.ReadInt32()));

        // Normals
        int nCount = br.ReadInt32();
        for (int i = 0; i < nCount; i++)
            mesh.Normals.Add(new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));

        // UVs
        int uvCount = br.ReadInt32();
        for (int i = 0; i < uvCount; i++)
            mesh.UVs.Add(new Vector2(br.ReadSingle(), br.ReadSingle()));

        // VertexColors
        int cCount = br.ReadInt32();
        for (int i = 0; i < cCount; i++)
            mesh.VertexColors.Add(ReadColor(br));

        return mesh;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Colour
    // --------------------------------------------------------------------------------------------

    private static void WriteColor(BinaryWriter bw, Color c)
    {
        bw.Write(c.R);
        bw.Write(c.G);
        bw.Write(c.B);
        bw.Write(c.A);
    }

    private static Color ReadColor(BinaryReader br)
    {
        return new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
    }
}