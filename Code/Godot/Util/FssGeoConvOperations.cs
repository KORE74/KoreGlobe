using System;

using Godot;

// FssGeoConvOperations: Functions for converting between real-world values and the Godot GameEngine presentation.
// - Real world is in mainly SI units, with other accessor operations. The Godot World in is units of kilometers, to best suit the full world presentation.
// - The Godot axis system does not match the ECEF orientatino of teh Z axis, so this will see inverting.

public static class FssGeoConvOperations
{
    // Offset to the center of the Earth in Godot coordinates
    // This offset allows the currently presented elements to vbe closer to the origin, and within the finer resolutino of a 32-bit float.
    public static Vector3 EarthCenterOffset = new Vector3(0, 0, 0);

    public static Vector3 RealWorldToGodot(float radius, float latitude, float longitude)
    {
        float latRad = Mathf.DegToRad(latitude);
        float lonRad = Mathf.DegToRad(longitude);

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad);
        float y = radius * Mathf.Sin(latRad);

        return new Vector3(x, y, z);
    }


}