using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Godot;

public partial class FssMeshBuilder
{
    public void AddBox(Vector3 pTLF, Vector3 pTRF, Vector3 pTLB, Vector3 pTRB, Vector3 pBLF, Vector3 pBRF, Vector3 pBLB, Vector3 pBRB)
    {
        AddSquare(pTLF, pTRF, pTRB, pTLB); // Top face
        AddSquare(pBLF, pBLB, pBRB, pBRF); // Bottom face
        AddSquare(pBLF, pBRF, pTRF, pTLF); // Front face
        AddSquare(pBLB, pTLB, pTRB, pBRB); // Back face
        AddSquare(pBLF, pTLF, pTLB, pBLB); // Left face
        AddSquare(pBRF, pBRB, pTRB, pTRF); // Right face
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
