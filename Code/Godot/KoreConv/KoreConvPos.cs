
using KoreCommon;
using Godot;

public static class KoreConvPos
{
    // ---------------------------------------------------------------------------------------------------
    // MARK: KoreCommon To Godot
    // ---------------------------------------------------------------------------------------------------

    // Converts KoreXYZPoint to Godot Vector3
    // Usage: Vector3 godotPos = KoreConvPos.ToGodotVector3(koreVect);
    public static Vector3 VecToV3(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
    }

    // Usage: Vector3 godotPos = KoreConvPos.ToGodotVector3(korePos);
    public static Vector3 PosToV3(KoreXYZPoint pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
    }

    // Convert a polar coordinate (azimuth, elevation, distance) to a Godot Vector3
    // Usage: Vector3 godotPos = KoreConvPos.PolarToV3(azimuth, elevation, distance);
    public static Vector3 PolarToV3(float azRads, float elRads, float distanceM)
    {
        // Convert the distance from Rw to Ge units
        float distance = (float)(distanceM * KoreZeroOffset.RwToGeDistanceMultiplier);

        float x = distance * Mathf.Cos(elRads) * Mathf.Sin(azRads);
        float y = distance * Mathf.Sin(elRads);
        float z = distance * Mathf.Cos(elRads) * Mathf.Cos(azRads);
        return new Vector3(x, y, z);
    }

    public static Vector3 PolarToV3(KoreAzElRange polarOffset)
    {
        return PolarToV3(
            (float)polarOffset.AzRads,
            (float)polarOffset.ElRads,
            (float)polarOffset.RangeM);
    }

    // ---------------------------------------------------------------------------------------------------
    // MARK: Godot To KoreCommon
    // ---------------------------------------------------------------------------------------------------

    // Converts Godot Vector3 to KoreXYZVector
    // Usage: KoreXYZVector koreVec = KoreConvPos.FromV3(godotVec);
    public static KoreXYZVector V3ToVec(Vector3 vec)
    {
        return new KoreXYZVector { X = vec.X, Y = vec.Y, Z = vec.Z };
    }

    // Converts Godot Vector3 to KoreXYZPoint
    // Usage: KoreXYZPoint korePos = KoreConvPos.FromV3(godotPos);
    public static KoreXYZPoint V3ToPos(Vector3 pos)
    {
        return new KoreXYZPoint { X = pos.X, Y = pos.Y, Z = pos.Z };
    }

}