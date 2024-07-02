using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssMeshBuilder
{
    public FssMeshData meshData;

    public FssMeshBuilder()
    {
        Init();
    }

    public void Init()
    {
        meshData = new FssMeshData();
    }

    // --------------------------------------------------------------------------------------------
    // #MARK: Mesh Building
    // --------------------------------------------------------------------------------------------

    public ArrayMesh Build(string name, bool recalcNormals = false)
    {
        var newMesh = new ArrayMesh();
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        foreach (var vertex in meshData.Vertices)
        {
            surfaceTool.AddVertex(vertex);
        }

        foreach (var index in meshData.Triangles)
        {
            surfaceTool.AddIndex(index);
        }

        if (recalcNormals)
        {
            surfaceTool.GenerateNormals();
        }

        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = meshData.Vertices.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV] = meshData.UVs.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = meshData.Triangles.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = meshData.Normals.ToArray();

        newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        surfaceTool.GenerateTangents();

        newMesh = surfaceTool.Commit();
        //newMesh.Name = name;

        return newMesh;
    }

    // -----------------------------------------------------------------------------------------------
    // #MARK: Simple Queries
    // -----------------------------------------------------------------------------------------------

    public int NumPoints()
    {
        return meshData.Vertices.Count;
    }

    public int NumTriangles()
    {
        return meshData.Triangles.Count / 3;
    }

    public string MeshReport()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Vertices:\n");
        foreach (var vertex in meshData.Vertices)
            sb.AppendLine($"{vertex}");

        sb.Append("\nTriangles:\n");
        for (int i = 0; i < meshData.Triangles.Count; i += 3)
            sb.AppendLine($"{meshData.Triangles[i]} {meshData.Triangles[i + 1]} {meshData.Triangles[i + 2]}\n");

        sb.Append("\nUVs:\n");
        foreach (var normal in meshData.Normals)
            sb.AppendLine($"{normal}");

        // If the number of triangles isn't a multiple of 3, add a warning
        if (meshData.Triangles.Count % 3 != 0)
            sb.AppendLine("Warning: Number of triangles is not a multiple of 3");
        if (meshData.Vertices.Count != meshData.Normals.Count && meshData.Normals.Count > 0)
            sb.AppendLine("Warning: Number of vertices does not match number of normals");
        if (meshData.Vertices.Count != meshData.UVs.Count && meshData.UVs.Count > 0)
            sb.AppendLine("Warning: Number of vertices does not match number of UVs");

        return sb.ToString();
    }

    // -----------------------------------------------------------------------------------------------
    // #MARK: Simple Primitives
    // -----------------------------------------------------------------------------------------------

    private int AddVertex(Vector3 vertex)
    {
        meshData.Vertices.Add(vertex);
        return meshData.Vertices.Count - 1;
    }

    public void AddTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int i1 = AddVertex(p1);
        int i2 = AddVertex(p2);
        int i3 = AddVertex(p3);

        meshData.Triangles.Add(i1);
        meshData.Triangles.Add(i2);
        meshData.Triangles.Add(i3);
    }

    public void AddTriangle(int i1, int i2, int i3) // Construct a triangle from previously added points
    {
        meshData.Triangles.Add(i1);
        meshData.Triangles.Add(i2);
        meshData.Triangles.Add(i3);
    }

    public void AddSquare(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        int i1 = AddVertex(p1);
        int i2 = AddVertex(p2);
        int i3 = AddVertex(p3);
        int i4 = AddVertex(p4);

        AddTriangle(i1, i3, i2);
        AddTriangle(i3, i1, i4);
    }

    // -----------------------------------------------------------------------------------------------

    public void AddNormal(Vector3 normal)
    {
        meshData.Normals.Add(normal);
    }

    public void AddUV(Vector2 uv)
    {
        meshData.UVs.Add(uv);
    }

    // -----------------------------------------------------------------------------------------------
    // #MARK: Next Level Primitives
    // -----------------------------------------------------------------------------------------------

    public void AddFan(Vector3 center, List<Vector3> edgePoints, bool wrapAround)
    {
        int centerIndex = AddVertex(center);

        for (int i = 0; i < edgePoints.Count - 1; i++)
        {
            int i1 = AddVertex(edgePoints[i]);
            int i2 = AddVertex(edgePoints[i + 1]);

            // Create a triangle using the center point and two consecutive edge points
            AddTriangle(centerIndex, i1, i2);
        }

        if (wrapAround)
        {
            // Create a triangle using the center point, last edge point, and first edge point
            int i1 = AddVertex(edgePoints[edgePoints.Count - 1]);
            int i2 = AddVertex(edgePoints[0]);
            AddTriangle(centerIndex, i1, i2);
        }
    }

    // ----------------------------------------------------------------------------------

    public void AddRibbon(List<Vector3> pntList1, List<Vector3> pntList2, bool wrapAround = false)
    {
        if (pntList1.Count != pntList2.Count || pntList1.Count < 2)
        {
            // Handle error if the lists are not of the same length or have less than 2 points
            return;
        }

        int i1 = AddVertex(pntList1[0]);
        AddNormal(pntList1[0].Normalized());

        int i2 = AddVertex(pntList2[0]);
        AddNormal(pntList2[0].Normalized());

        int firstI1 = i1;
        int firstI2 = i2;

        // Loop to one short of the list, as we add an artificial +1 in looking ahead to the next point.
        for (int i = 0; i < pntList1.Count - 1; i++)
        {
            int i3 = AddVertex(pntList2[i + 1]);
            AddNormal(pntList2[i + 1].Normalized());

            int i4 = AddVertex(pntList1[i + 1]);
            AddNormal(pntList1[i + 1].Normalized());

            AddTriangle(i1, i2, i3);
            AddTriangle(i1, i3, i4);

            i1 = i4; // Update i1 and i2 for the next iteration
            i2 = i3;
        }

        if (wrapAround)
        {
            AddTriangle(i1, i2, firstI2);
            AddTriangle(i1, firstI2, firstI1);
        }
    }

    // ----------------------------------------------------------------------------------

    public void AddSurface(int resolutionX, int resolutionY, List<Vector3> points, bool flipTriangles = false)
    {
        // Check if the points list has the expected length
        if (points.Count != (resolutionX + 1) * (resolutionY + 1))
        {
            FssCentralLog.AddEntry("The points list does not match the expected dimensions.");
            return;
        }

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                // Get the MeshData.Vertices for the current quad
                Vector3 p1 = points[y * (resolutionX + 1) + x];
                Vector3 p2 = points[y * (resolutionX + 1) + x + 1];
                Vector3 p3 = points[(y + 1) * (resolutionX + 1) + x];
                Vector3 p4 = points[(y + 1) * (resolutionX + 1) + x + 1];

                // Add points to MeshData.Vertices list and record the index of each point
                int i1 = AddVertex(p1);
                AddNormal(p1.Normalized());

                int i2 = AddVertex(p2);
                AddNormal(p2.Normalized());

                int i3 = AddVertex(p3);
                AddNormal(p3.Normalized());

                int i4 = AddVertex(p4);
                AddNormal(p4.Normalized());

                // Create two MeshData.Triangles using the four MeshData.Vertices just added
                if (flipTriangles)
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
                else
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
            }
        }
    }

    // ----------------------------------------------------------------------------------

    // Add surface, from an input 2d array of Vector3 points
    public void AddSurface(Vector3[,] points, bool flipTriangles = false)
    {
        int resolutionX = points.GetLength(0);
        int resolutionY = points.GetLength(1);

        for (int y = 0; y < resolutionY - 1; y++)
        {
            for (int x = 0; x < resolutionX - 1; x++)
            {
                // Get the MeshData.Vertices for the current quad
                Vector3 p1 = points[x, y];
                Vector3 p2 = points[x + 1, y];
                Vector3 p3 = points[x, y + 1];
                Vector3 p4 = points[x + 1, y + 1];

                // Add points to MeshData.Vertices list and record the index of each point
                int i1 = AddVertex(p1);
                int i2 = AddVertex(p2);
                int i3 = AddVertex(p3);
                int i4 = AddVertex(p4);

                // Create two MeshData.Triangles using the four MeshData.Vertices just added
                if (flipTriangles)
                {
                    AddTriangle(i3, i2, i1);
                    AddTriangle(i3, i4, i2);
                }
                else
                {
                    AddTriangle(i1, i2, i3);
                    AddTriangle(i2, i4, i3);
                }
            }
        }
    }

}
