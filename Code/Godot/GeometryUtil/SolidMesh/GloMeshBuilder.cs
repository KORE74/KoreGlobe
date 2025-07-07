using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class GloMeshBuilder
{
    public GloMeshData meshData;

    public GloMeshBuilder()
    {
        Init();
    }

    public void Init()
    {
        meshData = new GloMeshData();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh Building
    // --------------------------------------------------------------------------------------------

    public ArrayMesh Build(string name, bool recalcNormals = false)
    {
        var newMesh = new ArrayMesh();
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // If the color array is the same length as the vertices, add the colors, otherwise don't.
        if (meshData.Colors.Count == meshData.Vertices.Count)
        {
            for (int i = 0; i < meshData.Vertices.Count; i++)
            {
                surfaceTool.SetColor(meshData.Colors[i]);
                surfaceTool.SetUV(Vector2.Zero);
                surfaceTool.AddVertex(meshData.Vertices[i]);
            }
        }
        else
        {
            foreach (var vertex in meshData.Vertices)
            {
                surfaceTool.SetUV(Vector2.Zero);
                surfaceTool.AddVertex(vertex);
            }
        }

        foreach (var index in meshData.Triangles)
        {
            surfaceTool.AddIndex(index);
        }

        //surfaceTool.GenerateNormals();
        //surfaceTool.GenerateTangents();

        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = meshData.Vertices.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV]  = meshData.UVs.ToArray();
        arrays[(int)Mesh.ArrayType.Index]  = meshData.Triangles.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = meshData.Normals.ToArray();

        newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);


        newMesh = surfaceTool.Commit();
        //newMesh.Name = name;

        return newMesh;
    }


    public ArrayMesh Build2(string name, bool recalcNormals = false)
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

        // surfaceTool.GenerateNormals();
        // surfaceTool.GenerateTangents();

        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = meshData.Vertices.ToArray();
        arrays[(int)Mesh.ArrayType.Index]  = meshData.Triangles.ToArray();

        newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        //newMesh = surfaceTool.Commit();
        //newMesh.Name = name;

        return newMesh;
    }

    public ArrayMesh BuildWithUV(string name)
    {
        var newMesh     = new ArrayMesh();
        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        for (int i = 0; i < meshData.Vertices.Count; i++)
        {
            surfaceTool.SetUV(meshData.UVs[i]);
            surfaceTool.AddVertex(meshData.Vertices[i]);
        }

        foreach (var index in meshData.Triangles)
        {
            surfaceTool.AddIndex(index);
        }

        var arrays = new Godot.Collections.Array();

        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = meshData.Vertices.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV]  = meshData.UVs.ToArray();
        arrays[(int)Mesh.ArrayType.Index]  = meshData.Triangles.ToArray();

        newMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);

        return newMesh;
    }




    // -----------------------------------------------------------------------------------------------
    // MARK: Simple Queries
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
    // MARK: Simple Primitives
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

    public void AddColor(Color color)
    {
        meshData.Colors.Add(color);
    }

    // -----------------------------------------------------------------------------------------------
    // MARK: Next Level Primitives
    // -----------------------------------------------------------------------------------------------

    public void AddFan(Vector3 center, List<Vector3> edgePoints, bool wrapAround, bool flipTriangles = false)
    {
        int centerIndex = AddVertex(center);

        for (int i = 0; i < edgePoints.Count - 1; i++)
        {
            int i1 = AddVertex(edgePoints[i]);
            int i2 = AddVertex(edgePoints[i + 1]);

            AddUV(Vector2.Zero);
            AddUV(Vector2.Zero);
            AddNormal(Vector3.Zero);
            AddNormal(Vector3.Zero);

            // Create a triangle using the center point and two consecutive edge points
            if (flipTriangles)
                AddTriangle(centerIndex, i2, i1);
            else
                AddTriangle(centerIndex, i1, i2);
        }

        if (wrapAround)
        {
            // Create a triangle using the center point, last edge point, and first edge point
            int i1 = AddVertex(edgePoints[edgePoints.Count - 1]);
            int i2 = AddVertex(edgePoints[0]);

            AddUV(Vector2.Zero);
            AddUV(Vector2.Zero);
            AddNormal(Vector3.Zero);
            AddNormal(Vector3.Zero);

            if (flipTriangles)
                AddTriangle(centerIndex, i2, i1);
            else
                AddTriangle(centerIndex, i1, i2);

        }
    }

    // ------------------------------------------------------------------------------------------------------

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

            // AddTriangle(i1, i2, i3);
            // AddTriangle(i1, i3, i4);
            AddTriangle(i1, i3, i2);
            AddTriangle(i1, i4, i3);

            i1 = i4; // Update i1 and i2 for the next iteration
            i2 = i3;
        }

        if (wrapAround)
        {
            // AddTriangle(i1, i2, firstI2);
            // AddTriangle(i1, firstI2, firstI1);

            AddTriangle(i1, firstI2, i2);
            AddTriangle(i1, firstI1, firstI2);
        }
    }

    // ------------------------------------------------------------------------------------------------------

    public void AddSurface(int resolutionX, int resolutionY, List<Vector3> points, bool flipTriangles = false)
    {
        // Check if the points list has the expected length
        if (points.Count != (resolutionX + 1) * (resolutionY + 1))
        {
            GloCentralLog.AddEntry("The points list does not match the expected dimensions.");
            return;
        }

        // Create lists of UVs to match the surface
        GloFloat1DArray xUVs = GloFloat1DArrayOperations.ListForRange(0, 1, resolutionX);
        GloFloat1DArray yUVs = GloFloat1DArrayOperations.ListForRange(0, 1, resolutionY);

        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {
                // Get the MeshData.Vertices for the current quad
                Vector3 p1 = points[y * (resolutionX + 1) + x];
                Vector3 p2 = points[y * (resolutionX + 1) + x + 1];
                Vector3 p3 = points[(y + 1) * (resolutionX + 1) + x];
                Vector3 p4 = points[(y + 1) * (resolutionX + 1) + x + 1];

                // Vector2 uv1 = new Vector2((float)x / resolutionX, (float)y / resolutionY);
                // Vector2 uv2 = new Vector2((float)(x + 1) / resolutionX, (float)y / resolutionY);
                // Vector2 uv3 = new Vector2((float)x / resolutionX, (float)(y + 1) / resolutionY);
                // Vector2 uv4 = new Vector2((float)(x + 1) / resolutionX, (float)(y + 1) / resolutionY);

                Vector2 uv1 = new Vector2(xUVs[x]    , yUVs[y]);
                Vector2 uv2 = new Vector2(xUVs[x + 1], yUVs[y]);
                Vector2 uv3 = new Vector2(xUVs[x]    , yUVs[y + 1]);
                Vector2 uv4 = new Vector2(xUVs[x + 1], yUVs[y + 1]);

                // Add points to MeshData.Vertices list and record the index of each point
                int i1 = AddVertex(p1);
                AddNormal(p1.Normalized());
                AddUV(uv1);

                int i2 = AddVertex(p2);
                AddNormal(p2.Normalized());
                AddUV(uv2);

                int i3 = AddVertex(p3);
                AddNormal(p3.Normalized());
                AddUV(uv3);

                int i4 = AddVertex(p4);
                AddNormal(p4.Normalized());
                AddUV(uv4);

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

    // ------------------------------------------------------------------------------------------------------

    public void AddSurface(Vector3[,] points, bool flipTriangles = false)
    {
        AddSurface(points, GloFloatRange.ZeroToOne, GloFloatRange.ZeroToOne, flipTriangles);
    }

    // Add surface, from an input 2d array of Vector3 points
    public void AddSurface(Vector3[,] points, GloFloatRange uvX, GloFloatRange uvY, bool flipTriangles = false)
    {
        int resolutionX = points.GetLength(0);
        int resolutionY = points.GetLength(1);

        // Create lists of UVs to match the surface
        GloFloat1DArray xUVs = GloFloat1DArrayOperations.ListForRange(uvX, resolutionX);
        GloFloat1DArray yUVs = GloFloat1DArrayOperations.ListForRange(uvY, resolutionY);

        // xUVs = xUVs.Reverse();
        // yUVs = yUVs.Reverse();

        for (int y = 0; y < resolutionY - 1; y++)
        {
            for (int x = 0; x < resolutionX - 1; x++)
            {
                // Get the MeshData.Vertices for the current quad
                Vector3 p1 = points[x, y];
                Vector3 p2 = points[x + 1, y];
                Vector3 p3 = points[x, y + 1];
                Vector3 p4 = points[x + 1, y + 1];

                Vector2 p1UV = new Vector2(xUVs[x]    , yUVs[y]);
                Vector2 p2UV = new Vector2(xUVs[x + 1], yUVs[y]);
                Vector2 p3UV = new Vector2(xUVs[x]    , yUVs[y + 1]);
                Vector2 p4UV = new Vector2(xUVs[x + 1], yUVs[y + 1]);

                // Add points to MeshData.Vertices list and record the index of each point
                int i1 = AddVertex(p1); AddUV(p1UV);
                int i2 = AddVertex(p2); AddUV(p2UV);
                int i3 = AddVertex(p3); AddUV(p3UV);
                int i4 = AddVertex(p4); AddUV(p4UV);

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

    // Takes two 2D arrays of Vector3 points, one for the top surface and one for the bottom surface.
    // Creates sides around the four edges, using the UV ranges provided.

    public void AddBoxEdges(Vector3[,] points, Vector3[,] bottompoints, GloFloatRange uvX, GloFloatRange uvY, bool flipTriangles = false)
    {
        int resolutionX = points.GetLength(0);
        int resolutionY = points.GetLength(1);

        // Create lists of UVs to match the surface
        GloFloat1DArray xUVs = GloFloat1DArrayOperations.ListForRange(uvX, resolutionX);
        GloFloat1DArray yUVs = GloFloat1DArrayOperations.ListForRange(uvY, resolutionY);

        // xUVs = xUVs.Reverse();
        // yUVs = yUVs.Reverse();

        List<Vector3> topEdge    = new List<Vector3>();
        List<Vector3> bottomEdge = new List<Vector3>();
        List<Vector2> topUV      = new List<Vector2>();
        List<Vector2> bottomUV   = new List<Vector2>();
        List<int>     topPid     = new List<int>();
        List<int>     bottomPid  = new List<int>();

        // --- NorthEdge ---
        for (int i = 0; i < resolutionX; i++)
        {
            int iX = i;
            int iY = 0;

            // Extract the raw points
            Vector3 pTop = points[iX, iY];
            Vector3 pBottom = bottompoints[iX, iY];
            Vector2 p1UV = new Vector2(xUVs[iX], yUVs[iY]);

            // add the points to lists
            topEdge.Add(pTop);
            bottomEdge.Add(pBottom);
            topUV.Add(p1UV);
            bottomUV.Add(p1UV);

            // add the points to the meshbuilders and store the point IDs for triangle creation later.
            int p1 = AddVertex(pTop);    AddUV(p1UV);
            int p2 = AddVertex(pBottom); AddUV(p1UV);
            topPid.Add(p1);
            bottomPid.Add(p2);
        }

        // Loop through the lists, creating the triangles.
        int pCount = topEdge.Count;

        // Start from 1, so we -1 on numbers to get the previous point.
        for (int i = 1; i < pCount; i++)
        {
            // define four index values for the corners of the current square.
            int i1 = topPid[i - 1];
            int i2 = topPid[i];
            int i3 = bottomPid[i - 1];
            int i4 = bottomPid[i];

            // Create the two triangles for the current square.
            if (flipTriangles)
            {
                AddTriangle(i1, i2, i3);
                AddTriangle(i2, i4, i3);
            }
            else
            {
                AddTriangle(i1, i3, i2);
                AddTriangle(i2, i3, i4);
            }
        }

        // --- SouthEdge ---

        topEdge    = new List<Vector3>();
        bottomEdge = new List<Vector3>();
        topUV      = new List<Vector2>();
        bottomUV   = new List<Vector2>();
        topPid     = new List<int>();
        bottomPid  = new List<int>();

        for (int i = 0; i < resolutionX; i++)
        {
            int iX = (resolutionX - 1) - i;
            int iY = resolutionY - 1;

            // Extract the raw points
            Vector3 pTop = points[iX, iY];
            Vector3 pBottom = bottompoints[iX, iY];
            Vector2 p1UV = new Vector2(xUVs[iX], yUVs[iY]);

            // add the points to lists
            topEdge.Add(pTop);
            bottomEdge.Add(pBottom);
            topUV.Add(p1UV);
            bottomUV.Add(p1UV);

            // add the points to the meshbuilders and store the point IDs for triangle creation later.
            int p1 = AddVertex(pTop);    AddUV(p1UV);
            int p2 = AddVertex(pBottom); AddUV(p1UV);
            topPid.Add(p1);
            bottomPid.Add(p2);
        }

        // Loop through the lists, creating the triangles.
        pCount = topEdge.Count;

        // Start from 1, so we -1 on numbers to get the previous point.
        for (int i = 1; i < pCount; i++)
        {
            // define four index values for the corners of the current square.
            int i1 = topPid[i - 1];
            int i2 = topPid[i];
            int i3 = bottomPid[i - 1];
            int i4 = bottomPid[i];

            // Create the two triangles for the current square.
            if (flipTriangles)
            {
                AddTriangle(i1, i2, i3);
                AddTriangle(i2, i4, i3);
            }
            else
            {
                AddTriangle(i1, i3, i2);
                AddTriangle(i2, i3, i4);
            }
        }

        // --- EastEdge ---

        topEdge    = new List<Vector3>();
        bottomEdge = new List<Vector3>();
        topUV      = new List<Vector2>();
        bottomUV   = new List<Vector2>();
        topPid     = new List<int>();
        bottomPid  = new List<int>();

        for (int i = 0; i < resolutionY; i++)
        {
            int iX = resolutionX - 1;
            int iY = i;

            // Extract the raw points
            Vector3 pTop = points[iX, iY];
            Vector3 pBottom = bottompoints[iX, iY];
            Vector2 p1UV = new Vector2(xUVs[iX], yUVs[iY]);

            // add the points to lists
            topEdge.Add(pTop);
            bottomEdge.Add(pBottom);
            topUV.Add(p1UV);
            bottomUV.Add(p1UV);

            // add the points to the meshbuilders and store the point IDs for triangle creation later.
            int p1 = AddVertex(pTop);    AddUV(p1UV);
            int p2 = AddVertex(pBottom); AddUV(p1UV);
            topPid.Add(p1);
            bottomPid.Add(p2);
        }

        // Loop through the lists, creating the triangles.
        pCount = topEdge.Count;

        // Start from 1, so we -1 on numbers to get the previous point.
        for (int i = 1; i < pCount; i++)
        {
            // define four index values for the corners of the current square.
            int i1 = topPid[i - 1];
            int i2 = topPid[i];
            int i3 = bottomPid[i - 1];
            int i4 = bottomPid[i];

            // Create the two triangles for the current square.
            if (flipTriangles)
            {
                AddTriangle(i1, i2, i3);
                AddTriangle(i2, i4, i3);
            }
            else
            {
                AddTriangle(i1, i3, i2);
                AddTriangle(i2, i3, i4);
            }
        }


        // --- WestEdge ---

        topEdge    = new List<Vector3>();
        bottomEdge = new List<Vector3>();
        topUV      = new List<Vector2>();
        bottomUV   = new List<Vector2>();
        topPid     = new List<int>();
        bottomPid  = new List<int>();


        for (int i = 0; i < resolutionY; i++)
        {
            int iX = 0;
            int iY = (resolutionY - 1) - i;

            // Extract the raw points
            Vector3 pTop = points[iX, iY];
            Vector3 pBottom = bottompoints[iX, iY];
            Vector2 p1UV = new Vector2(xUVs[iX], yUVs[iY]);

            // add the points to lists
            topEdge.Add(pTop);
            bottomEdge.Add(pBottom);
            topUV.Add(p1UV);
            bottomUV.Add(p1UV);

            // add the points to the meshbuilders and store the point IDs for triangle creation later.
            int p1 = AddVertex(pTop);    AddUV(p1UV);
            int p2 = AddVertex(pBottom); AddUV(p1UV);
            topPid.Add(p1);
            bottomPid.Add(p2);
        }

        // Loop through the lists, creating the triangles.
        pCount = topEdge.Count;

        // Start from 1, so we -1 on numbers to get the previous point.
        for (int i = 1; i < pCount; i++)
        {
            // define four index values for the corners of the current square.
            int i1 = topPid[i - 1];
            int i2 = topPid[i];
            int i3 = bottomPid[i - 1];
            int i4 = bottomPid[i];

            // Create the two triangles for the current square.
            if (flipTriangles)
            {
                AddTriangle(i1, i2, i3);
                AddTriangle(i2, i4, i3);
            }
            else
            {
                AddTriangle(i1, i3, i2);
                AddTriangle(i2, i3, i4);
            }
        }

    }

}
