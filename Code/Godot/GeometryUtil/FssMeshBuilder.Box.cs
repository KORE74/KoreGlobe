using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{

    public void AddBox(Vector3 center, float height, float width, float depth)
    {
        float halfHeight = height / 2;
        float halfWidth = width / 2;
        float halfDepth = depth / 2;

        Vector3 pTLF = new Vector3(center.X - halfWidth, center.Y + halfHeight, center.Z - halfDepth);
        Vector3 pTRF = new Vector3(center.X + halfWidth, center.Y + halfHeight, center.Z - halfDepth);
        Vector3 pTLB = new Vector3(center.X - halfWidth, center.Y + halfHeight, center.Z + halfDepth);
        Vector3 pTRB = new Vector3(center.X + halfWidth, center.Y + halfHeight, center.Z + halfDepth);

        Vector3 pBLF = new Vector3(center.X - halfWidth, center.Y - halfHeight, center.Z - halfDepth);
        Vector3 pBRF = new Vector3(center.X + halfWidth, center.Y - halfHeight, center.Z - halfDepth);
        Vector3 pBLB = new Vector3(center.X - halfWidth, center.Y - halfHeight, center.Z + halfDepth);
        Vector3 pBRB = new Vector3(center.X + halfWidth, center.Y - halfHeight, center.Z + halfDepth);

        AddBox(pTLF, pTRF, pTLB, pTRB, pBLF, pBRF, pBLB, pBRB);
    }

    public void AddBox(Vector3 pTLF, Vector3 pTRF, Vector3 pTLB, Vector3 pTRB, Vector3 pBLF, Vector3 pBRF, Vector3 pBLB, Vector3 pBRB)
    {
        AddSquare(pTRF, pTLF, pTLB, pTRB); // Top face
        AddSquare(pBLB, pBLF, pBRF, pBRB); // Bottom face
        AddSquare(pBRF, pBLF, pTLF, pTRF); // Front face
        AddSquare(pTLB, pBLB, pBRB, pTRB); // Back face
        AddSquare(pTLF, pBLF, pBLB, pTLB); // Left face
        AddSquare(pBRB, pBRF, pTRF, pTRB); // Right face
    }

    // ----------------------------------------------------------------------------------

    public void AddBoxOutline(Vector3 pTLF, Vector3 pTRF, Vector3 pTLB, Vector3 pTRB, Vector3 pBLF, Vector3 pBRF, Vector3 pBLB, Vector3 pBRB, float outlineRadius, int segments)
    {
        // Spheres at the corners
        AddSphere(pTLF, outlineRadius, segments); // Top Left Front
        AddSphere(pTRF, outlineRadius, segments); // Top Right Front
        AddSphere(pTLB, outlineRadius, segments); // Top Left Back
        AddSphere(pTRB, outlineRadius, segments); // Top Right Back

        AddSphere(pBLF, outlineRadius, segments); // Bottom Left Front
        AddSphere(pBRF, outlineRadius, segments); // Bottom Right Front
        AddSphere(pBLB, outlineRadius, segments); // Bottom Left Back
        AddSphere(pBRB, outlineRadius, segments); // Bottom Right Back

        // Cylinders - bottom ring
        AddCylinder(pBLF, pBRF, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBRF, pBRB, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBRB, pBLB, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBLB, pBLF, outlineRadius, outlineRadius, segments, true);

        // Cylinders - top ring
        AddCylinder(pTLF, pTRF, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pTRF, pTRB, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pTRB, pTLB, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pTLB, pTLF, outlineRadius, outlineRadius, segments, true);

        // Cylinders - verticals
        AddCylinder(pBLF, pTLF, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBRF, pTRF, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBRB, pTRB, outlineRadius, outlineRadius, segments, true);
        AddCylinder(pBLB, pTLB, outlineRadius, outlineRadius, segments, true);
    }

}
