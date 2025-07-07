using System;

public struct GloRangeBearing
{
    public double RangeM      { get; set; }
    public double BearingRads { get; set; }

    // --------------------------------------------------------------------------------------------

    public double RangeKm // Alt above EarthRadius
    {
        get { return (RangeM * GloWorldConsts.MetresToKmMultiplier); }
        set { RangeM = (value * GloWorldConsts.KmToMetresMultiplier); }
    }

    public double BearingDegs
    {
        get { return BearingRads * GloConsts.RadsToDegsMultiplier; }
        set { BearingRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public GloRangeBearing(double inRangeM, double inBearingRads)
    {
        this.RangeM      = inRangeM;
        this.BearingRads = inBearingRads;
    }

    public double BearingDifferenceDegs(GloRangeBearing compareRB)
    {
        return BearingDegs - compareRB.BearingDegs;
    }

}
