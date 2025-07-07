using System;

public struct GloAzEl
{
    public double AzRads { get; set; }
    public double ElRads { get; set; }

    // --------------------------------------------------------------------------------------------

    public double AzDegs
    {
        get { return AzRads * GloConsts.RadsToDegsMultiplier; }
        set { AzRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double ElDegs
    {
        get { return ElRads * GloConsts.RadsToDegsMultiplier; }
        set { ElRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    // Constructor accepts radians
    public GloAzEl(double azRads, double elRads)
    {
        AzRads = azRads;
        ElRads = elRads;
    }

    public static GloAzEl Zero
    {
        get { return new GloAzEl { AzRads = 0.0,  ElRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    // Determine the angle between two azimuth/elevation directions.
    public static double AngleBetween(GloAzEl v1, GloAzEl v2)
    {
        double cosTheta = Math.Cos(v1.ElRads) * Math.Cos(v2.ElRads) * Math.Cos(v1.AzRads - v2.AzRads) +
                          Math.Sin(v1.ElRads) * Math.Sin(v2.ElRads);
        return Math.Acos(cosTheta);
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"Az:{AzDegs:F2}Degs  El:{ElDegs:F2}Degs";
    }

}
