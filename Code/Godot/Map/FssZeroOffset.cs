
using Godot;

// A static (non-Node3D class for globally managing the zero offset)

public static class FssZeroOffset
{
    // ZeroNode: A node always at 0,0,0 for objects (platforms) to be parented to.


    // Real World Earth Center is 0,0,0. We create an offset 0,0,0 for the purposes og focussing the
    // game engine view within the range of its floating point precision.
    public static FssLLAPoint RwZeroPointLLA = new FssLLAPoint();

    // Offset "FROM real-world Earth center TO game engine center". We use the inverse of this to place the earth center.
    public static FssXYZPoint RwZeroPointXYZ = new FssXYZPoint(0, 0, 0);

    // Game engine earth radius and conversion around it.
    public static double GeEarthRadius = 1000;
    public static double RwToGeDistanceMultiplierM = GeEarthRadius / FssPosConsts.EarthRadiusM;
    public static double GeToRwDistanceMultiplierM = 1 / RwToGeDistanceMultiplierM;

    // Define a reasonable "Up Distance" (Real World Meters) that still works when scales to the GE ranges.
    public static double UpDistRwM    = 2 * GeToRwDistanceMultiplierM;
    public static double AheadDistGeM = 2 * GeToRwDistanceMultiplierM;

    // --------------------------------------------------------------------------------------------

    // Report the constants for debugging.
    // Usage: FssZeroOffset.ReportConsts();
    public static void ReportConsts()
    {
        GD.Print($"FssZeroOffset.ReportConsts:\n- GeEarthRadius:{GeEarthRadius}\n- RwToGeDistanceMultiplierM:{RwToGeDistanceMultiplierM}\n- GeToRwDistanceMultiplierM:{GeToRwDistanceMultiplierM}\n- AheadDistGeM:{AheadDistGeM}\n- UpDistRwM:{UpDistRwM}");
    }

    // --------------------------------------------------------------------------------------------

    // Set the zero point for the game engine.
    // Usage: FssZeroOffset.SetLLA(pos);

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

    public static FssXYZPoint RwZeroPointOffset(FssLLAPoint RwLLA)
    {
        return RwZeroPointXYZ.XYZTo(RwLLA.ToXYZ());
    }

    // To convert from an RW XYZ to a GE XYZ, we need to:
    // 1 - Subtract the zero point offset to get the offset XYZ.
    // 2 - Invert the Z axis to match the Godot engine orientation.
    // 3 - Scale the XYZ by the GE distance multiplier.
    // 4 - Return the vector3.

    // Usage: Vector3 GePos = FssZeroOffset.GeZeroPointOffset(RwXYZPos);

    public static Vector3 GeZeroPointOffset(FssXYZPoint RwXYZ)
    {
        // 1 - Subtract the zero point offset to get the offset XYZ.
        FssXYZPoint rwOffsetXYZ = RwZeroPointXYZ.XYZTo(RwXYZ);

        // 2 - Invert the Z axis to match the Godot engine orientation.
        double x = rwOffsetXYZ.X;
        double y = rwOffsetXYZ.Y;
        double z = rwOffsetXYZ.Z * -1;

        // 3 - Scale the XYZ by the GE distance multiplier.
        x = x * RwToGeDistanceMultiplierM;
        y = y * RwToGeDistanceMultiplierM;
        z = z * RwToGeDistanceMultiplierM;

        // 4 - Return the vector3.
        return new Vector3((float)x, (float)y, (float)z);
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
