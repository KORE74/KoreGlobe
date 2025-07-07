using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace KoreCommon;

// Static class to create KoreMeshData primitives
// This class is used to create various 3D shapes and meshes in Godot

public static partial class KoreMeshDataPrimitives
{
    // Usage: var cubeMesh = KoreMeshDataPrimitives.BasicCube(1.0f, new KoreColorRGB(255, 0, 0));
    public static KoreMeshData BasicCube(float size, KoreColorRGB color)
    {
        var mesh = new KoreMeshData();

        // Define the vertices of the cube
        int v0 = mesh.AddPoint(new KoreXYZVector(-size, -size, -size), null, color);
        int v1 = mesh.AddPoint(new KoreXYZVector(size, -size, -size), null, color);
        int v2 = mesh.AddPoint(new KoreXYZVector(size, size, -size), null, color);
        int v3 = mesh.AddPoint(new KoreXYZVector(-size, size, -size), null, color);
        int v4 = mesh.AddPoint(new KoreXYZVector(-size, -size, size), null, color);
        int v5 = mesh.AddPoint(new KoreXYZVector(size, -size, size), null, color);
        int v6 = mesh.AddPoint(new KoreXYZVector(size, size, size), null, color);
        int v7 = mesh.AddPoint(new KoreXYZVector(-size, size, size), null, color);

        // Lines
        mesh.AddLine(v0, v1, color, color);
        mesh.AddLine(v1, v5, color, color);
        mesh.AddLine(v5, v4, color, color);
        mesh.AddLine(v4, v0, color, color);
        mesh.AddLine(v2, v3, color, color);
        mesh.AddLine(v3, v7, color, color);
        mesh.AddLine(v7, v6, color, color);
        mesh.AddLine(v6, v2, color, color);
        mesh.AddLine(v0, v3, color, color);
        mesh.AddLine(v1, v2, color, color);
        mesh.AddLine(v4, v7, color, color);
        mesh.AddLine(v5, v6, color, color);

        // Triangles
        mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
        mesh.AddTriangle(v4, v5, v6); mesh.AddTriangle(v4, v6, v7);
        mesh.AddTriangle(v0, v1, v5); mesh.AddTriangle(v0, v5, v4);
        mesh.AddTriangle(v1, v2, v6); mesh.AddTriangle(v1, v6, v5);
        mesh.AddTriangle(v2, v3, v7); mesh.AddTriangle(v2, v7, v6);
        mesh.AddTriangle(v3, v0, v4); mesh.AddTriangle(v3, v4, v7);

        mesh.MakeValid();
        return mesh;
    }

    // ---------------------------------------------------------------------------------------------

    public static KoreMeshData SizedBox(
        double sizeUp, double sizeDown,
        double sizeLeft, double sizeRight,
        double sizeFront, double sizeBack,
        KoreColorRGB color)
    {
        // Create a new KoreMeshData object
        var mesh = new KoreMeshData();

        // Define 8 unique vertices for the rectangular box
        // Front face vertices:
        int v0 = mesh.AddPoint(new KoreXYZVector(-sizeLeft, -sizeDown, -sizeFront), null, color); // Lower left front
        int v1 = mesh.AddPoint(new KoreXYZVector(sizeRight, -sizeDown, -sizeFront), null, color); // Lower right front
        int v2 = mesh.AddPoint(new KoreXYZVector(sizeRight, sizeUp, -sizeFront), null, color); // Upper right front
        int v3 = mesh.AddPoint(new KoreXYZVector(-sizeLeft, sizeUp, -sizeFront), null, color); // Upper left front

        // Back face vertices:
        int v4 = mesh.AddPoint(new KoreXYZVector(-sizeLeft, -sizeDown, sizeBack), null, color); // Lower left back
        int v5 = mesh.AddPoint(new KoreXYZVector(sizeRight, -sizeDown, sizeBack), null, color); // Lower right back
        int v6 = mesh.AddPoint(new KoreXYZVector(sizeRight, sizeUp, sizeBack), null, color); // Upper right back
        int v7 = mesh.AddPoint(new KoreXYZVector(-sizeLeft, sizeUp, sizeBack), null, color); // Upper left back

        // Define edges (lines)
        // Lines
        mesh.AddLine(v0, v1, color, color);
        mesh.AddLine(v1, v5, color, color);
        mesh.AddLine(v5, v4, color, color);
        mesh.AddLine(v4, v0, color, color);
        mesh.AddLine(v2, v3, color, color);
        mesh.AddLine(v3, v7, color, color);
        mesh.AddLine(v7, v6, color, color);
        mesh.AddLine(v6, v2, color, color);
        mesh.AddLine(v0, v3, color, color);
        mesh.AddLine(v1, v2, color, color);
        mesh.AddLine(v4, v7, color, color);
        mesh.AddLine(v5, v6, color, color);

        // Triangles
        mesh.AddTriangle(v0, v1, v2); mesh.AddTriangle(v0, v2, v3);
        mesh.AddTriangle(v4, v5, v6); mesh.AddTriangle(v4, v6, v7);
        mesh.AddTriangle(v0, v1, v5); mesh.AddTriangle(v0, v5, v4);
        mesh.AddTriangle(v1, v2, v6); mesh.AddTriangle(v1, v6, v5);
        mesh.AddTriangle(v2, v3, v7); mesh.AddTriangle(v2, v7, v6);
        mesh.AddTriangle(v3, v0, v4); mesh.AddTriangle(v3, v4, v7);

        return mesh;
    }

    // ---------------------------------------------------------------------------------------------

    public static KoreMeshData SizedBox(
        KoreXYZBox box,
        KoreColorRGB? linecolor = null)
    {
        return SizedBox(
            box.OffsetUp, box.OffsetDown,
            box.OffsetLeft, box.OffsetRight,
            box.OffsetForwards, box.OffsetBackwards,
            linecolor ?? KoreColorRGB.White);
    }

}
