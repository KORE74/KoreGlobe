using System;

public struct FssRangeBearing
{
    public double RangeM { get; set; }
    public double BearingRads { get; set; }

    // --------------------------------------------------------------------------------------------

    public double RangeKm // Alt above EarthRadius
    {
        get { return (RangeM * FssPosConsts.MetresToKmMultiplier); }
        set { RangeM = (value * FssPosConsts.KmToMetresMultiplier); }
    }

    public double BearingDegs
    {
        get { return BearingRads * FssConsts.RadsToDegsMultiplier; }
        set { BearingRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public FssRangeBearing(double inRangeM, double inBearingRads)
    {
        this.RangeM = inRangeM;
        this.BearingRads = inBearingRads;
    }

    public double BearingDifferenceDegs(FssRangeBearing compareRB)
    {
        return BearingDegs - compareRB.BearingDegs;
    }

}
