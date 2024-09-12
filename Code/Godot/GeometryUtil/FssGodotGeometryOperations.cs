using Godot;

public static class FssGodotGeometryOperations
{
    // Usage: FssGodotGeometryOperations.ToVector3(point);

    public static Vector3 ToVector3(FssXYZPoint point)
    {
        return new Vector3((float)point.X, (float)point.Y, (float)point.Z);
    }
}