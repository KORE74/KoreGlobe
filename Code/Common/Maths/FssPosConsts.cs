using System;

public static class FssPosConsts
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
    public const double EarthRadiusKm = 6365.0; // FssPosConsts.EarthRadiusKm
    public const double EarthRadiusM  = EarthRadiusKm * 1000.0; // FssPosConsts.EarthRadiusM

    public const double TileEdgeDepthKm = 25.0;
    public const double TileEdgeDepthM  = TileEdgeDepthKm * KmToMetresMultiplier;

    public const float  EarthRadiusInnerKm = (float)(EarthRadiusKm - (TileEdgeDepthKm + 1));
    public const float  EarthRadiusInnerM  = (float)(EarthRadiusM - TileEdgeDepthM);

    // WGS84 Values
    public const double EarthRadiusPolarM   = 6356752;
    public const double EarthRadiusEquatorM = 6378137;
    public const double EccentricitySquared = (EarthRadiusEquatorM * EarthRadiusEquatorM - EarthRadiusPolarM * EarthRadiusPolarM) / (EarthRadiusEquatorM * EarthRadiusEquatorM);
}
