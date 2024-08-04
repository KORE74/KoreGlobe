using System;
using System.Text;

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

public struct FssRWPlatformPositions
{
    public FssLLAPoint PosLLA;
    public FssLLAPoint PosAboveLLA;
    public FssLLAPoint PosAheadLLA;

    public FssXYZPoint PosXYZ;
    public FssXYZPoint PosAboveXYZ;
    public FssXYZPoint PosAheadXYZ;

    public Vector3 vecPos;
    public Vector3 vecPosAhead;
    public Vector3 vecPosAbove;

    public Vector3 vecOffsetAbove;
    public Vector3 vecOffsetAhead;

    // public Vector3 vecUp;
    // public Vector3 vecForward;
};

public static class FssGeoConvOperations
{
    // --------------------------------------------------------------------------------------------
    // MARK: Simple Conversion
    // --------------------------------------------------------------------------------------------

    public static Vector3 RealWorldToGodot(float radius, float latDegs, float lonDegs)
    {
        float latRad = Mathf.DegToRad(latDegs);
        float lonRad = Mathf.DegToRad(lonDegs);

        float x = radius * Mathf.Cos(latRad) * Mathf.Cos(lonRad);
        float y = radius * Mathf.Sin(latRad);
        float z = radius * Mathf.Cos(latRad) * Mathf.Sin(lonRad) * -1.0f;

        return new Vector3(x, y, z);
    }

    public static Vector3 RealWorldToGodotRads(float radius, float latRads, float lonRads)
    {
        float x = radius * Mathf.Cos(latRads) * Mathf.Cos(lonRads);
        float y = radius * Mathf.Sin(latRads);
        float z = radius * Mathf.Cos(latRads) * Mathf.Sin(lonRads) * -1.0f;

        return new Vector3(x, y, z);
    }

    public static Vector3 RealWorldToGodot(FssLLAPoint llap)
    {
        return RealWorldToGodot((float)llap.AltMslM, (float)llap.LatDegs, (float)llap.LonDegs);
    }

    // --------------------------------------------------------------------------------------------

    public static FssXYZPoint GodotToRealWorld(Vector3 godotPos)
    {
        float x = godotPos.X;
        float y = godotPos.Y;
        float z = godotPos.Z;

        z *= -1.0f;

        return new FssXYZPoint(x, y, z);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Focus Point Conversion
    // --------------------------------------------------------------------------------------------

    public static Vector3 RealWorldToGodotFocusPointRads(float radius, float latRads, float lonRads)
    {
        float x = radius * Mathf.Cos(latRads) * Mathf.Cos(lonRads);
        float y = radius * Mathf.Sin(latRads);
        float z = radius * Mathf.Cos(latRads) * Mathf.Sin(lonRads) * -1.0f;

        Vector3 RawPosition = new Vector3(x, y, z);

        Vector3 AdjustedPosition = RawPosition - FssEarthCore.FocusPos;

        return AdjustedPosition;
    }

    public static Vector3 RealWorldToGodotFocusPoint(FssLLAPoint llap)
    {
        return RealWorldToGodotFocusPointRads((float)llap.AltMslM, (float)llap.LatRads, (float)llap.LonRads);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Conversion to structures
    // --------------------------------------------------------------------------------------------

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

    public static FssRWPlatformPositions RealWorldStruct(FssLLAPoint pos, FssCourse course)
    {
        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.24f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;
        if (course.IsStationary())
        {
            posAhead = pos;
            posAhead.LatDegs += 0.001;
        }
        else
        {
            posAhead = pos.PlusPolarOffset(course.ToPolarOffset(3));
        }

        // Figure out all the real world XYZ positions as offsets from the focus pos
        FssXYZPoint focusOffsetXYZ       = FssEarthCore.FocusOffsetForRWLLA(pos);
        FssXYZPoint focusOffsetAboveXYZ  = FssEarthCore.FocusOffsetForRWLLA(posAbove);
        FssXYZPoint focusOffsetAheadXYZ  = FssEarthCore.FocusOffsetForRWLLA(posAhead);

        // Flip the Z axis here for Godot axis orientation
        focusOffsetXYZ.Z      *= -1;
        focusOffsetAboveXYZ.Z *= -1;
        focusOffsetAheadXYZ.Z *= -1;

        // Determine the relative directions forward and up
        // FssXYZPoint xyzLookAhead = focusOffsetXYZ.XYZTo(focusOffsetAheadXYZ);
        // FssXYZPoint xyzLookUp    = focusOffsetXYZ.XYZTo(focusOffsetAboveXYZ);

        // Translate the RW into Godot Types
        Vector3 v3Pos       = new Vector3((float)focusOffsetXYZ.X,      (float)focusOffsetXYZ.Y,      (float)focusOffsetXYZ.Z);
        Vector3 v3PosAbove  = new Vector3((float)focusOffsetAboveXYZ.X, (float)focusOffsetAboveXYZ.Y, (float)focusOffsetAboveXYZ.Z);
        Vector3 v3PosAhead  = new Vector3((float)focusOffsetAheadXYZ.X, (float)focusOffsetAheadXYZ.Y, (float)focusOffsetAheadXYZ.Z);

        Vector3 v3LookAhead = new Vector3((float)focusOffsetAheadXYZ.X, (float)focusOffsetAheadXYZ.Y, (float)focusOffsetAheadXYZ.Z);
        Vector3 v3LookUp    = new Vector3((float)focusOffsetAboveXYZ.X, (float)focusOffsetAboveXYZ.Y, (float)focusOffsetAboveXYZ.Z);

        // v3PosAhead += FssEarthCore.FocusPos;
        // v3PosAbove += FssEarthCore.FocusPos;

        // StringBuilder sb = new StringBuilder();
        // sb.AppendLine("RealWorldStruct:");
        // sb.AppendLine("  pos: " + pos.ToString());
        // sb.AppendLine("  posAbove: " + posAbove.ToString());
        // sb.AppendLine("  posAhead: " + posAhead.ToString());
        // sb.AppendLine("  focusOffsetXYZ: " + focusOffsetXYZ.ToString());
        // sb.AppendLine("  focusOffsetAboveXYZ: " + focusOffsetAboveXYZ.ToString());
        // sb.AppendLine("  focusOffsetAheadXYZ: " + focusOffsetAheadXYZ.ToString());
        // sb.AppendLine("  v3Pos: " + v3Pos.ToString());
        // sb.AppendLine("  v3PosAhead: " + v3PosAhead.ToString());
        // sb.AppendLine("  v3LookUp: " + v3LookUp.ToString());
        // sb.AppendLine("  v3LookAhead: " + v3LookAhead.ToString());
        // FssCentralLog.AddEntry(sb.ToString());

        return new FssRWPlatformPositions {
            PosLLA         = pos,
            PosAboveLLA    = posAbove,
            PosAheadLLA    = posAhead,

            PosXYZ         = focusOffsetXYZ,
            PosAboveXYZ    = focusOffsetAboveXYZ,
            PosAheadXYZ    = focusOffsetAheadXYZ,

            vecPos         = v3Pos,
            vecPosAhead    = v3PosAhead,
            vecPosAbove    = v3PosAbove,

            vecOffsetAbove = v3LookUp,
            vecOffsetAhead = v3LookAhead
        };
    }
}
