using System;

public static class GloWorldConsts
{
    // --------------------------------------------------------------------------------------------

    // Basic Conversion Consts

    public const double KmToMetresMultiplier = 1000.0;
    public const double MetresToKmMultiplier = 1.0 / KmToMetresMultiplier;

    public const double MPStoKPHMultiplier = 3.6;
    public const double KPHtoMPSMultiplier = 1.0 / MPStoKPHMultiplier;

    public const double LookaheadDistanceM = 10000.0; // Lookahead distance in meters

    // --------------------------------------------------------------------------------------------

    // Earth Constants

    // https://en.wikipedia.org/wiki/Earth_radius
    // Metric system 6,357 to 6,378 km
    public const double EarthRadiusKm = 6365.0; // GloWorldConsts.EarthRadiusKm
    public const double EarthRadiusM  = EarthRadiusKm * 1000.0;

    public const double TileEdgeDepthKm = 25.0;
    public const double TileEdgeDepthM  = TileEdgeDepthKm * KmToMetresMultiplier;

    public const float  EarthRadiusInnerKm = (float)(EarthRadiusKm - (TileEdgeDepthKm + 1));
    public const float  EarthRadiusInnerM  = (float)(EarthRadiusM - TileEdgeDepthM);

    // WGS84 Values
    public const double EarthRadiusPolarM   = 6356752; // GloWorldConsts.EarthRadiusPolarM
    public const double EarthRadiusEquatorM = 6378137; // GloWorldConsts.EarthRadiusEquatorM
    public const double EccentricitySquared = (EarthRadiusEquatorM * EarthRadiusEquatorM - EarthRadiusPolarM * EarthRadiusPolarM) / (EarthRadiusEquatorM * EarthRadiusEquatorM);

    public const double MinCalculationRadiusM = 100000; // 100km minimum radius for calculations // GloWorldConsts.MinCalculationRadiusM

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation
    // --------------------------------------------------------------------------------------------

    // GloElevationUtils.InvalidEle
    public static float InvalidEle      = -9999f;
    public static float InvalidEleCheck = -9990f; // For checking < or > comparisons


}
