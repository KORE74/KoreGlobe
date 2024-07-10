using System;
using System.Collections.Generic;

// FssLLALocation: A class that encapsulates the idea of a location with latitude, longitude, and altitude, but without
// a fixed radius or altitude type.

public struct FssLLALocation
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }
    public double HeightM { get; set; } // Alt above EarthCentre

    public enum HeightType { MSL, AGL, AGLMSL, AGLMSLAVG };
    public HeightType HeightTypeValue { get; set; } = HeightType.MSL;

    // ------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // ------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * FssConsts.RadsToDegsMultiplier; }
        set { LatRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * FssConsts.RadsToDegsMultiplier; }
        set { LonRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    // public double AltMslKm // Alt above MSL
    // {
    //     get { return (RadiusM - FssPosConsts.EarthRadiusM) * FssPosConsts.MetresToKmMultiplier; }
    //     set { RadiusM = (value + FssPosConsts.EarthRadiusKm) * FssPosConsts.KmToMetresMultiplier; }
    // }
    // public double AltMslM // Alt above M
    // {
    //     get { return (RadiusM - FssPosConsts.EarthRadiusM); }
    //     set { RadiusM = value + FssPosConsts.EarthRadiusM; }
    // }
    // public double RadiusKm // Alt above EarthRadius
    // {
    //     get { return (RadiusM * FssPosConsts.MetresToKmMultiplier); }
    //     set { RadiusM = (value * FssPosConsts.KmToMetresMultiplier); }
    // }

    public override string ToString()
    {
        return string.Format("({0:F2}, {1:F2}, {2:F2})", LatDegs, LonDegs, HeightM);
    }

    // ------------------------------------------------------------------------
    // #MARK: Constructors - different options and units
    // ------------------------------------------------------------------------

    // Note that fields can be set:
    //   FssLLAPoint pos = new FssLLAPoint() { latDegs = X, LonDegs = Y, AltMslM = Z };

    public FssLLALocation(double laRads, double loRads, double altM)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = altM;
    }

    public FssLLALocation(double laRads, double loRads)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = 0;
    }

    public static FssLLAPoint Zero
    {
        get { return new FssLLAPoint { LatRads = 0.0, LonRads = 0.0, HeightM = 0.0 }; }
    }

    // Function to return an LLA position on the ground, either using the default MSL value, or an optional elevation above MSL in metres.

}
