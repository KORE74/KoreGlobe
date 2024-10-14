using Godot;

public static class FssGodotGeometryOperations
{
    // Usage: FssGodotGeometryOperations.ToVector3(point);

    public static Vector3 ToVector3(FssXYZPoint point)
    {
        return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
    }

    // Usage: FssGodotGeometryOperations.Vector3FromPolar(Vector3.Zero, azimuth, elevation, distance);

    public static Vector3 Vector3FromPolar(Vector3 origin, float azRads, float elRads, float distance)
    {
        float x = origin.X + distance * Mathf.Cos(azRads) * Mathf.Cos(elRads);
        float y = origin.Y + distance * Mathf.Sin(elRads);
        float z = origin.Z + distance * Mathf.Sin(azRads) * Mathf.Cos(elRads);

        return new Vector3(x, y, z);
    }
}