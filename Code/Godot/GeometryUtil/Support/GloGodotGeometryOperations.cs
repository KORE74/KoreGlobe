using Godot;

public static class GloGodotGeometryOperations
{
    // Usage: GloGodotGeometryOperations.ToVector3(point);

    public static Vector3     ToV3(GloXYZPoint point) => new Vector3((float)point.X, (float)point.Y, (float)point.Z);
    public static GloXYZPoint ToXYZ(Vector3 point)    => new GloXYZPoint(point.X, point.Y, point.Z);

    // Usage: GloGodotGeometryOperations.Vector3FromPolar(Vector3.Zero, azimuth, elevation, distance);

    public static Vector3 Vector3FromPolar(Vector3 origin, float azRads, float elRads, float distance)
    {
        float x = origin.X + distance * Mathf.Cos(azRads) * Mathf.Cos(elRads);
        float y = origin.Y + distance * Mathf.Sin(elRads);
        float z = origin.Z + distance * Mathf.Sin(azRads) * Mathf.Cos(elRads);

        return new Vector3(x, y, z);
    }
}