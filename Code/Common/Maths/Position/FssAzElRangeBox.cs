using System;

public struct FssAzElRangeBox
{
    public double MinAzRads { get; set; }
    public double MaxAzRads { get; set; }
    public double MinElRads { get; set; }
    public double MaxElRads { get; set; }
    public double MinRangeM { get; set; }
    public double MaxRangeM { get; set; }

    // ------------------------------------------------------------------------

    public double MinAzDegs
    {
        get { return MinAzRads * FssConsts.RadsToDegsMultiplier; }
        set { MinAzRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double MaxAzDegs
    {
        get { return MaxAzRads * FssConsts.RadsToDegsMultiplier; }
        set { MaxAzRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double MinElDegs
    {
        get { return MinElRads * FssConsts.RadsToDegsMultiplier; }
        set { MinElRads = value * FssConsts.DegsToRadsMultiplier; }
    }
    public double MaxElDegs
    {
        get { return MaxElRads * FssConsts.RadsToDegsMultiplier; }
        set { MaxElRads = value * FssConsts.DegsToRadsMultiplier; }
    }

    public double MinRangeKm
    {
        get { return MinRangeM * FssPosConsts.MetresToKmMultiplier; }
        set { MinRangeM = value * FssPosConsts.KmToMetresMultiplier; }
    }

    public double MaxRangeKm
    {
        get { return MaxRangeM * FssPosConsts.MetresToKmMultiplier; }
        set { MaxRangeM = value * FssPosConsts.KmToMetresMultiplier; }
    }
    // ------------------------------------------------------------------------

    public double ArcAzRads { get { return (MaxAzRads - MinAzRads); } }
    public double ArcAzDegs { get { return (MaxAzRads - MinAzRads) * FssConsts.RadsToDegsMultiplier; } }
    public double ArcElRads { get { return (MaxElRads - MinElRads); } }
    public double ArcElDegs { get { return (MaxElRads - MinElRads) * FssConsts.RadsToDegsMultiplier; } }

    // ------------------------------------------------------------------------

    public double HalfArcAzRads { get { return ArcAzRads / 2.0; } }
    public double HalfArcAzDegs { get { return ArcAzDegs / 2.0; } }
    public double HalfArcElRads { get { return ArcElRads / 2.0; } }
    public double HalfArcElDegs { get { return ArcElDegs / 2.0; } }

    // ------------------------------------------------------------------------

    public double MidAzRads { get { return MinAzRads + HalfArcAzRads; } }
    public double MidAzDegs { get { return MinAzDegs + HalfArcAzDegs; } }
    public double MidElRads { get { return MinElRads + HalfArcElRads; } }
    public double MidElDegs { get { return MinElDegs + HalfArcElDegs;  } }

    // ------------------------------------------------------------------------

    public double DepthM { get { return (MaxRangeM - MinRangeM); } }
    public double DepthKm { get { return (MaxRangeM - MinRangeM) * FssPosConsts.MetresToKmMultiplier; } }

    public double HalfDepthM { get { return (MaxRangeM - MinRangeM) / 2.0; } }
    public double HalfDepthKm { get { return (MaxRangeKm - MinRangeKm) / 2.0; } }

    // ------------------------------------------------------------------------

    public bool IsOffsetInBox(FssPolarOffset offset)
    {
        bool inAz    = (offset.AzRads >= MinAzRads && offset.AzRads <= MaxAzRads);
        bool inEl    = (offset.ElRads >= MinElRads && offset.ElRads <= MaxElRads);
        bool inRange = (offset.RangeM >= MinRangeM && offset.RangeM <= MaxRangeM);
        return (inAz && inEl && inRange);
    }

    // ------------------------------------------------------------------------

    public static FssAzElRangeBox BoxFromOffset(FssPolarOffset offset)
    {
        return new FssAzElRangeBox
        {
            MinAzRads = offset.AzRads,
            MaxAzRads = offset.AzRads,
            MinElRads = offset.ElRads,
            MaxElRads = offset.ElRads,
            MinRangeM = offset.RangeM,
            MaxRangeM = offset.RangeM
        };
    }

    public static FssAzElRangeBox BoxFromDimensions(double azDegs, double elDegs, double depthM)
    {
        double halfAzDegs = azDegs / 2.0;
        double halfElDegs = elDegs / 2.0;
        return new FssAzElRangeBox
        {
            MinAzDegs = -halfAzDegs,
            MaxAzDegs = halfAzDegs,
            MinElDegs = -halfElDegs,
            MaxElDegs = halfElDegs,
            MinRangeM =  0.0,
            MaxRangeM = depthM,
        };
    }

    public FssAzElRangeBox ExpandBoxByOffset(FssPolarOffset offset)
    {
        return new FssAzElRangeBox
        {
            MinAzRads = Math.Min(MinAzRads, offset.AzRads),
            MaxAzRads = Math.Max(MaxAzRads, offset.AzRads),
            MinElRads = Math.Min(MinElRads, offset.ElRads),
            MaxElRads = Math.Max(MaxElRads, offset.ElRads),
            MinRangeM = Math.Min(MinRangeM, offset.RangeM),
            MaxRangeM = Math.Max(MaxRangeM, offset.RangeM)
        };
    }

    public FssAzElRangeBox ExpandBoxByBox(FssAzElRangeBox box)
    {
        return new FssAzElRangeBox
        {
            MinAzRads = Math.Min(MinAzRads, box.MinAzRads),
            MaxAzRads = Math.Max(MaxAzRads, box.MaxAzRads),
            MinElRads = Math.Min(MinElRads, box.MinElRads),
            MaxElRads = Math.Max(MaxElRads, box.MaxElRads),
            MinRangeM = Math.Min(MinRangeM, box.MinRangeM),
            MaxRangeM = Math.Max(MaxRangeM, box.MaxRangeM)
        };
    }

    public FssAzElRangeBox CentreBoxOnOffset(FssPolarOffset offset)
    {
        return new FssAzElRangeBox
        {
            MinAzRads = MinAzRads - HalfArcAzRads,
            MaxAzRads = MaxAzRads + HalfArcAzRads,
            MinElRads = MinElRads - HalfArcElRads,
            MaxElRads = MaxElRads + HalfArcElRads,
            MinRangeM = MinRangeM - HalfDepthM,
            MaxRangeM = MaxRangeM + HalfDepthM
        };
    }

    public FssAzElRangeBox ShiftBoxAzElRange(double azAdjustDegs, double elAdjustDegs, double rangeAdjustM)
    {
        return new FssAzElRangeBox
        {
            MinAzDegs = MinAzDegs + azAdjustDegs,
            MaxAzDegs = MaxAzDegs + azAdjustDegs,
            MinElDegs = MinElDegs + elAdjustDegs,
            MaxElDegs = MaxElDegs + elAdjustDegs,
            MinRangeM = MinRangeM + rangeAdjustM,
            MaxRangeM = MaxRangeM + rangeAdjustM,
        };
    }

    public FssAzElRangeBox ConsistencyCheck()
    {
        return new FssAzElRangeBox
        {
            MinAzRads = MinAzRads,
            MaxAzRads = MaxAzRads,
            MinElRads = (MinElRads < MaxElRads) ? MinElRads : MaxElRads,
            MaxElRads = (MinElRads < MaxElRads) ? MaxElRads : MinElRads,
            MinRangeM = (MinRangeM < MaxRangeM) ? MinRangeM : MaxRangeM,
            MaxRangeM = (MinRangeM < MaxRangeM) ? MaxRangeM : MinRangeM
        };
    }

    // ------------------------------------------------------------------------

    public override string ToString()
    {
        return $"Az:{MinAzDegs:F2}degs to {MaxAzDegs:F2}degs, El:{MinElDegs:F2}degs to {MaxElDegs:F2}degs, Range: {MinRangeKm:F2}Km to {MaxRangeKm:F2}Km";
    }

}
