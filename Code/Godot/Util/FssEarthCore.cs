using Godot;

public static class FssEarthCore
{
    public static double EarthRadiusM = 10;
    public static double backoffFraction = 0.2;

    public static FssLLAPoint FocusPoint = new FssLLAPoint() {LatDegs = 0, LonDegs = 0, RadiusM = EarthRadiusM};

    public static Vector3 FocusPos;
    public static Vector3 CorePos;

    public static void UpdatePositions()
    {
        float CoreDist  = (float)(EarthRadiusM - (EarthRadiusM * backoffFraction));
        float FocusDist = (float)(EarthRadiusM * backoffFraction);

        float lonRads = (float)FocusPoint.LonRads;
        float latRads = (float)FocusPoint.LatRads;

        FocusPos = FssGeoConvOperations.RealWorldToGodotFocusPoint((float)FocusDist, latRads, lonRads);

        // FocusPos = new Vector3(
        //     FocusDist * Mathf.Cos(azRads) * Mathf.Cos(elRads),
        //     FocusDist * Mathf.Sin(elRads),
        //     FocusDist * Mathf.Sin(azRads) * Mathf.Cos(elRads)
        // );

        float backLonRads = lonRads + Mathf.Pi;
        float backLatRads = latRads * -1f;

        // CorePos = new Vector3(
        //     CoreDist * Mathf.Cos(backAzRads) * Mathf.Cos(backElRads),
        //     CoreDist * Mathf.Sin(backElRads),
        //     CoreDist * Mathf.Sin(backAzRads) * Mathf.Cos(backElRads)
        // );

        CorePos = FssGeoConvOperations.RealWorldToGodotFocusPoint((float)CoreDist, backLatRads, backLonRads);

    }

}
