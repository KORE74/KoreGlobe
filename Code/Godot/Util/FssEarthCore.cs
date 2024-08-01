using Godot;

public static class FssEarthCore
{
    // FssEarthCore.EarthRadiusM
    public static double EarthRadiusM    = 10;
    public static double backoffFraction = 0.05;

    // LLA and then XYZ Real World locations from Earth Centre 0,0,0.
    public static FssLLAPoint RwFocusLLA = new FssLLAPoint() {LatDegs = 0, LonDegs = 0, RadiusM = EarthRadiusM};
    public static FssXYZPoint RwFocusXYZ = new FssXYZPoint(0, 0, 0);

    // Offset values from the 0,0,0 GE origin
    public static FssLLAPoint RwFocusOffsetLLA = new FssLLAPoint() {LatDegs = 0, LonDegs = 0, RadiusM = EarthRadiusM};
    public static FssLLAPoint RwCoreOffsetLLA  = new FssLLAPoint() {LatDegs = 0, LonDegs = 0, RadiusM = EarthRadiusM};
    public static FssXYZPoint RwFocusOffsetXYZ = new FssXYZPoint(0, 0, 0);
    public static FssXYZPoint RwCoreOffsetXYZ  = new FssXYZPoint(0, 0, 0);

    // Vector3 GameEngine positions
    public static Vector3 FocusPos;
    public static Vector3 CorePos;

    public static void UpdatePositions()
    {
        // The Real World Focus point is a striaght up XYZ from the LLA, no offsets.
        RwFocusXYZ = RwFocusLLA.ToXYZ();

        // Update the offset distances from the 0,0,0 origin.
        double FocusOffsetDist = EarthRadiusM * backoffFraction;
        double CoreOffsetDist  = EarthRadiusM - FocusOffsetDist;

        // Update the forwards and backwards angles.
        double lonRads     = RwFocusLLA.LonRads;
        double latRads     = RwFocusLLA.LatRads;
        double backLonRads = lonRads + Mathf.Pi;
        double backLatRads = latRads * -1;

        RwFocusOffsetLLA = new FssLLAPoint() {LatRads=latRads,     LonRads=lonRads,     RadiusM=FocusOffsetDist};
        RwCoreOffsetLLA  = new FssLLAPoint() {LatRads=backLatRads, LonRads=backLonRads, RadiusM=CoreOffsetDist};
        RwFocusOffsetXYZ = RwFocusOffsetLLA.ToXYZ();
        RwCoreOffsetXYZ  = RwCoreOffsetLLA.ToXYZ();

        // ---- Now GE Values ----

        // Convert the XYZ values to Godot coordinates
        FocusPos = FssGeoConvOperations.RealWorldToGodotRads((float)FocusOffsetDist, (float)latRads,     (float)lonRads);
        CorePos  = FssGeoConvOperations.RealWorldToGodotRads((float)CoreOffsetDist,  (float)backLatRads, (float)backLonRads);
    }


    public static FssXYZPoint FocusOffsetForRWLLA(FssLLAPoint lla)
    {
        FssXYZPoint posXYZ = lla.ToXYZ();

        FssXYZPoint posXYZOffset = RwFocusXYZ.XYZTo(posXYZ);

        return posXYZOffset;
    }

    public static Vector3 FocusOffsetForLLA(FssLLAPoint lla)
    {
        FssXYZPoint posXYZ = lla.ToXYZ();

        FssXYZPoint posXYZOffset = RwFocusXYZ.XYZTo(posXYZ);

        posXYZOffset.Z = posXYZOffset.Z * -1;

        return new Vector3((float)posXYZOffset.X, (float)posXYZOffset.Y, (float)posXYZOffset.Z);

    }

}
