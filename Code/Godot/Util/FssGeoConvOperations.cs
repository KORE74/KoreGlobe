using System;

using Godot;

// FssGeoConvOperations: Functions for converting between real-world values and the Godot GameEngine presentation.
// - Real world is in mainly SI units, with other accessor operations. The Godot World in is units of kilometers, to best suit the full world presentation.
// - The Godot axis system does not match the ECEF orientatino of teh Z axis, so this will see inverting.

public struct FssEntityV3
{
    public Vector3 Position;
    public Vector3 PosAbove;
    public Vector3 PosAhead;
}

public static class FssGeoConvOperations
{
    public static Vector3 RealWorldToGodot(float radius, float latDegs, float lonDegs)
    {
        float latRad = Mathf.DegToRad(latDegs);
        float lonRad = Mathf.DegToRad(lonDegs);

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Sin(latRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad) * -1.0f;

        return new Vector3(x, y, z);
    }

    public static Vector3 RealWorldToGodot(FssLLAPoint llap)
    {
        return RealWorldToGodot((float)llap.AltMslM, (float)llap.LatDegs, (float)llap.LonDegs);
    }

    public static Vector3 RealWorldToGodotFocusPoint(float radius, float latRads, float lonRads)
    {
        float x = radius * Mathf.Cos(latRads) * Mathf.Cos(lonRads);
        float y = radius * Mathf.Sin(latRads);
        float z = radius * Mathf.Cos(latRads) * Mathf.Sin(lonRads) * -1.0f;

       // Vector3 RawPosition = new Vector3(x, y, z);

      //  Vector3 AdjustedPosition = RawPosition - FssEarthCore.FocusPos;

        return new Vector3(x, y, z);
    }


    // FssEntityV3 platformV3 = FssGeoConvOperations.ReadWorldToStruct(pos, course);

    public static FssEntityV3 ReadWorldToStruct(FssLLAPoint pos, FssCourse course)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;
        if (course.IsStationary())
        {
            posAhead = pos;
            posAhead.LatDegs += 0.001;
        }
        else
        {
            posAhead = pos.PlusPolarOffset(course.ToPolarOffset(-5));
        }

        Vector3 v3Pos   = RealWorldToGodot(pos);
        Vector3 v3Above = RealWorldToGodot(posAbove);
        Vector3 v3Ahead = RealWorldToGodot(posAhead);

        return new FssEntityV3 {
            Position = v3Pos,
            PosAbove = v3Above,
            PosAhead = v3Ahead };
    }
}