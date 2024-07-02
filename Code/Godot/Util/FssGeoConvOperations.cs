using System;

using Godot;

// FssGeoConvOperations: Functions for converting between real-world values and the Godot GameEngine presentation.
// - Real world is in mainly SI units, with other accessor operations. The Godot World in is units of kilometers, to best suit the full world presentation.
// - The Godot axis system does not match the ECEF orientatino of teh Z axis, so this will see inverting.

public static class FssGeoConvOperations
{
    public static Vector3 RealWorldToGodot(float radius, float latDegs, float lonDegs)
    {
        float latRad = Mathf.DegToRad(latDegs);
        float lonRad = Mathf.DegToRad(lonDegs);

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Sin(latRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);

        return new Vector3(x, y, z);
    }
}