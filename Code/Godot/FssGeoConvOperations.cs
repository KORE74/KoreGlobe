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



    public static Vector3 RealWorldToGodot(float radius, float azimuth, float elevation)
    {
        float azRad = Mathf.DegToRad(azimuth);
        float elRad = Mathf.DegToRad(elevation);

        float x = radius * Mathf.Cos(elRad) * Mathf.Cos(azRad);
        float y = radius * Mathf.Cos(elRad) * Mathf.Sin(azRad);
        float z = radius * Mathf.Sin(elRad);

        return new Vector3(x, y, z);
    }

    public static Vector3 RealWorldToGodot(FssLLAPoint llap)
    {
        // convert to XYZ
        FssXYZPoint xyz = llap.ToXYZ();

        // scale M to Km
        float x = (float)(xyz.X / 1000);
        float y = (float)(xyz.Y / 1000);
        float z = (float)(xyz.Z / 1000);

        // invert Z
        z = -z;

        return new Vector3(x, y, z);
    }
}