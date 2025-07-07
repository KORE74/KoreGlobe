
using KoreCommon;
using Godot;

public static class KoreConvPos
{
    // Converts KoreXYZPoint to Godot Vector3
    // Usage: Vector3 godotPos = KoreConvPos.ToGodotVector3(korePoint);
    public static Vector3 ToGodotVector3(KoreXYZVector pos)
    {
        return new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
    }

    // Converts Godot Vector3 to KoreXYZVector
    // Usage: KoreXYZVector koreVec = KoreConvPos.FromGodotVector3(godotVec);
    public static KoreXYZVector FromGodotVector3(Vector3 vec)
    {
        return new KoreXYZVector { X = vec.X, Y = vec.Y, Z = vec.Z };
    }


}