using System;
using System.Collections.Generic;

// GloLLALocation: A class that encapsulates the idea of a location with latitude, longitude, and altitude, but without
// a fixed radius or altitude type.

public struct GloLLALocation
{
    // SI Units and a pure radius value so the trig functions are simple.
    // Accomodate units and MSL during accessor functions

    public double LatRads { get; set; }
    public double LonRads { get; set; }
    public double HeightM { get; set; } // Alt above EarthCentre

    public enum HeightType { MSL, AGL, AGLMSL, AGLMSLAVG };
    public HeightType HeightTypeValue { get; set; } = HeightType.MSL;


    // --------------------------------------------------------------------------------------------
    // Additional simple accessors - adding units
    // --------------------------------------------------------------------------------------------

    public double LatDegs
    {
        get { return LatRads * GloConsts.RadsToDegsMultiplier; }
        set { LatRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double LonDegs
    {
        get { return LonRads * GloConsts.RadsToDegsMultiplier; }
        set { LonRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    // public double AltMslKm // Alt above MSL
    // {
    //     get { return (RadiusM - GloWorldConsts.EarthRadiusM) * GloWorldConsts.MetresToKmMultiplier; }
    //     set { RadiusM = (value + GloWorldConsts.EarthRadiusKm) * GloWorldConsts.KmToMetresMultiplier; }
    // }
    // public double AltMslM // Alt above M
    // {
    //     get { return (RadiusM - GloWorldConsts.EarthRadiusM); }
    //     set { RadiusM = value + GloWorldConsts.EarthRadiusM; }
    // }
    // public double RadiusKm // Alt above EarthRadius
    // {
    //     get { return (RadiusM * GloWorldConsts.MetresToKmMultiplier); }
    //     set { RadiusM = (value * GloWorldConsts.KmToMetresMultiplier); }
    // }

    public override string ToString()
    {
        return string.Format($"({LatDegs:F2}, {LonDegs:F2}, {HeightM:F2})");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Constructors - different options and units
    // --------------------------------------------------------------------------------------------

    // Note that fields can be set:
    //   GloLLAPoint pos = new GloLLAPoint() { latDegs = X, LonDegs = Y, AltMslM = Z };

    public GloLLALocation(double laRads, double loRads, double altM)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = altM;
    }

    public GloLLALocation(double laRads, double loRads)
    {
        this.LatRads = laRads;
        this.LonRads = loRads;
        this.HeightM = 0;
    }

    public static GloLLALocation Zero
    {
        get { return new GloLLALocation { LatRads = 0.0, LonRads = 0.0, HeightM = 0.0 }; }
    }

    // Function to return an LLA position on the ground, either using the default MSL value, or an optional elevation above MSL in metres.

    public GloLLAPoint ToLLA()
    {
        double newRadiusM = 0;

        switch(HeightTypeValue)
        {
            case HeightType.MSL:
                newRadiusM = GloWorldConsts.EarthRadiusM + HeightM;
                break;
            case HeightType.AGL:
                newRadiusM = GloWorldConsts.EarthRadiusM + HeightM;
                break;
            case HeightType.AGLMSL:
                newRadiusM = GloWorldConsts.EarthRadiusM + HeightM;
                break;
            default:
                newRadiusM = GloWorldConsts.EarthRadiusM + HeightM;
                break;
        }

        return new GloLLAPoint(LatRads, LonRads) { RadiusM = newRadiusM };
    }

}
