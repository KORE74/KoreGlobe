
using Godot;

public static class FssZeroOffset
{
    // ZeroNode: A node always at 0,0,0 for objects (platforms) to be parented to.
    public static Node3D ZeroNode;

    // Real World Earth Center is 0,0,0. We create an offset 0,0,0 for the purposes og focussing the
    // game engine view within the range of its floating point precision.
    public static FssLLAPoint RwZeroPointLLA = new FssLLAPoint();

    // Offset "FROM real-world Earth center TO game engine center". We use the inverse of this to place the earth center.
    public static FssXYZPoint RwZeroPointXYZ = new FssXYZPoint(0, 0, 0);

    // Game engine earth radius and conversion around it.
    public static double GeEarthRadius = 65;
    public static double RwToGeDistanceMultiplierM = GeEarthRadius / FssPosConsts.EarthRadiusM;
    public static double GeToRwDistanceMultiplierM = 1 / RwToGeDistanceMultiplierM;

    // --------------------------------------------------------------------------------------------

    public static void SetLLA(FssLLAPoint rwLLA)
    {
        RwZeroPointLLA = rwLLA;
        RwZeroPointXYZ = rwLLA.ToXYZ();

        //GD.Print($"FssZeroOffset.SetLLA: RwZeroPointLLA:{RwZeroPointLLA} RwZeroPointXYZ:{RwZeroPointXYZ}");
    }

    // --------------------------------------------------------------------------------------------

    // The real-world XYZ we have from the model in A. the Earth centre offset is B, and we need the game engine
    // zero-offset C: C = A - B

    public static FssXYZPoint RwZeroPointOffset(FssXYZPoint RwXYZ)
    {
        return RwZeroPointXYZ.XYZTo(RwXYZ);
    }

    // Usage: Vector3 GePos = FssZeroOffset.GeZeroPointOffset(RwXYZPos);

    public static Vector3 GeZeroPointOffset(FssXYZPoint RwXYZ)
    {
        FssXYZPoint p = RwZeroPointXYZ.XYZTo(RwXYZ);
        return new Vector3((float)p.X, (float)p.Y, (float)-p.Z);
    }

    // Usage: FssZeroOffset.GeZeroPoint()
    public static Vector3 GeZeroPoint()
    {
        // return new Vector3((float)RwZeroPointXYZ.X, (float)RwZeroPointXYZ.Y, (float)RwZeroPointXYZ.Z);
        return new Vector3(0, 0, 0);
    }

    // RwZeroPointXYZ is the offset from the real-world core to the zero point, so we need to apply the invese of this
    // to get the now-offset core point.
    // Note that we always need to be inverting the Z axis to match the Godot engine orientation, so the -- = + here.
    // Usage: Vector3 GeCorePos = FssZeroOffset.GeCorePoint();
    public static Vector3 GeCorePoint()
    {
        // The Z axis is inverted in the Godot engine, but as we're creating an iverted vector, the inverse of this pplies (ie to X and Y)
        double x = RwZeroPointXYZ.X * RwToGeDistanceMultiplierM * -1;
        double y = RwZeroPointXYZ.Y * RwToGeDistanceMultiplierM * -1;
        double z = RwZeroPointXYZ.Z * RwToGeDistanceMultiplierM;

        return new Vector3((float)x, (float)y, (float)z);
    }
}
