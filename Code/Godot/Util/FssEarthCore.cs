using Godot;

public static class FssEarthCore
{
    public static double EarthRadiusM = 10;
    public static double backoffFraction = 0.05;

    public static FssLLAPoint FocusLLA = new FssLLAPoint() {LatDegs = 0, LonDegs = 0, RadiusM = EarthRadiusM};

    public static FssXYZPoint FocusXYZ = new FssXYZPoint(0, 0, 0);
    public static FssXYZPoint CoreXYZ  = new FssXYZPoint(0, 0, 0);

    public static Vector3 FocusPos;
    public static Vector3 CorePos;

    public static void UpdatePositions()
    {
        // Update the distances
        double FocusDist = EarthRadiusM * backoffFraction;
        double CoreDist  = EarthRadiusM - FocusDist;

        // Update the forwards and backwards angles.
        double lonRads     = FocusLLA.LonRads;
        double latRads     = FocusLLA.LatRads;
        double backLonRads = lonRads + Mathf.Pi;
        double backLatRads = latRads * -1;

        // We start with the focus point, so we just have to calculate the (backwards) core point.
        FssLLAPoint backLLA = new FssLLAPoint() {LatRads=backLatRads, LonRads=backLonRads, RadiusM=CoreDist};

        // Determine the real-world XYZ values
        FocusXYZ = FocusLLA.ToXYZ();
        CoreXYZ  = backLLA.ToXYZ();

        // Convert the XYZ values to Godot coordinates
        FocusPos = FssGeoConvOperations.RealWorldToGodotFocusPoint((float)FocusDist, (float)latRads,     (float)lonRads);
        CorePos  = FssGeoConvOperations.RealWorldToGodotFocusPoint((float)CoreDist,  (float)backLatRads, (float)backLonRads);
    }


    public static FssXYZPoint FocusOffsetForRWLLA(FssLLAPoint lla)
    {
        FssXYZPoint posXYZ = lla.ToXYZ();

        FssXYZPoint posXYZOffset = FocusXYZ.XYZTo(posXYZ);

        return posXYZOffset;
    }

    public static Vector3 FocusOffsetForLLA(FssLLAPoint lla)
    {
        FssXYZPoint posXYZ = lla.ToXYZ();

        FssXYZPoint posXYZOffset = FocusXYZ.XYZTo(posXYZ);

        posXYZOffset.Z = posXYZOffset.Z * -1;

        return new Vector3((float)posXYZOffset.X, (float)posXYZOffset.Y, (float)posXYZOffset.Z);

    }

}
