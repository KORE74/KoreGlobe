using System;

public struct FssAzElBox
{
    public double MinAzRads { get; set; }
    public double MaxAzRads { get; set; }
    public double MinElRads { get; set; }
    public double MaxElRads { get; set; }

    // --------------------------------------------------------------------------------------------

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

    // --------------------------------------------------------------------------------------------

    public double ArcAzRads { get { return (MaxAzRads - MinAzRads); } }
    public double ArcAzDegs { get { return (MaxAzRads - MinAzRads) * FssConsts.RadsToDegsMultiplier; } }
    public double ArcElRads { get { return (MaxElRads - MinElRads); } }
    public double ArcElDegs { get { return (MaxElRads - MinElRads) * FssConsts.RadsToDegsMultiplier; } }

    // --------------------------------------------------------------------------------------------

    public double HalfArcAzRads { get { return ArcAzRads / 2.0; } }
    public double HalfArcAzDegs { get { return ArcAzDegs / 2.0; } }
    public double HalfArcElRads { get { return ArcElRads / 2.0; } }
    public double HalfArcElDegs { get { return ArcElDegs / 2.0; } }

    // --------------------------------------------------------------------------------------------

    public double MidAzRads { get { return MinAzRads + HalfArcAzRads; } }
    public double MidAzDegs { get { return MinAzDegs + HalfArcAzDegs; } }
    public double MidElRads { get { return MinElRads + HalfArcElRads; } }
    public double MidElDegs { get { return MinElDegs + HalfArcElDegs; } }

    // --------------------------------------------------------------------------------------------

    public FssPolarDirection Center { get { return new FssPolarDirection(MidAzRads, MidElRads); } }

    // --------------------------------------------------------------------------------------------

    public static FssAzElBox Zero
    {
        get { return new FssAzElBox { MinAzRads = 0.0, MaxAzRads = 0.0, MinElRads = 0.0, MaxElRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    public bool IsOffsetInBox(FssPolarOffset offset)
    {
        bool inAz = (offset.AzRads >= MinAzRads && offset.AzRads <= MaxAzRads);
        bool inEl = (offset.ElRads >= MinElRads && offset.ElRads <= MaxElRads);
        return (inAz && inEl);
    }

    // --------------------------------------------------------------------------------------------

    public static FssAzElBox BoxFromOffset(FssPolarOffset offset)
    {
        return new FssAzElBox
        {
            MinAzRads = offset.AzRads,
            MaxAzRads = offset.AzRads,
            MinElRads = offset.ElRads,
            MaxElRads = offset.ElRads,
        };
    }

    // Usage: FssAzElBox.BoxFromDimensions(10, 20);
    public static FssAzElBox BoxFromDimensions(double azDegs, double elDegs)
    {
        double halfAzDegs = azDegs / 2.0;
        double halfElDegs = elDegs / 2.0;
        return new FssAzElBox
        {
            MinAzDegs = -halfAzDegs,
            MaxAzDegs = halfAzDegs,
            MinElDegs = -halfElDegs,
            MaxElDegs = halfElDegs,
        };
    }

    // --------------------------------------------------------------------------------------------

    public FssAzElBox Inflate(double azInflateDegs, double elInflateDegs)
    {
        return new FssAzElBox
        {
            MinAzDegs = MinAzDegs - azInflateDegs,
            MaxAzDegs = MaxAzDegs + azInflateDegs,
            MinElDegs = MinElDegs - elInflateDegs,
            MaxElDegs = MaxElDegs + elInflateDegs
        };
    }

    public FssAzElBox ExpandBoxByOffset(FssPolarOffset offset)
    {
        return new FssAzElBox
        {
            MinAzRads = Math.Min(MinAzRads, offset.AzRads),
            MaxAzRads = Math.Max(MaxAzRads, offset.AzRads),
            MinElRads = Math.Min(MinElRads, offset.ElRads),
            MaxElRads = Math.Max(MaxElRads, offset.ElRads)
        };
    }

    public FssAzElBox ExpandBoxByBox(FssAzElBox box)
    {
        return new FssAzElBox
        {
            MinAzRads = Math.Min(MinAzRads, box.MinAzRads),
            MaxAzRads = Math.Max(MaxAzRads, box.MaxAzRads),
            MinElRads = Math.Min(MinElRads, box.MinElRads),
            MaxElRads = Math.Max(MaxElRads, box.MaxElRads)
        };
    }

    // --------------------------------------------------------------------------------------------

    public FssAzElBox CentreBoxOnOffset(FssPolarOffset offset)
    {
        return new FssAzElBox
        {
            MinAzRads = MinAzRads - HalfArcAzRads,
            MaxAzRads = MaxAzRads + HalfArcAzRads,
            MinElRads = MinElRads - HalfArcElRads,
            MaxElRads = MaxElRads + HalfArcElRads
        };
    }

    public FssAzElBox ShiftBoxAzElRange(double azAdjustDegs, double elAdjustDegs)
    {
        return new FssAzElBox
        {
            MinAzDegs = MinAzDegs + azAdjustDegs,
            MaxAzDegs = MaxAzDegs + azAdjustDegs,
            MinElDegs = MinElDegs + elAdjustDegs,
            MaxElDegs = MaxElDegs + elAdjustDegs
        };
    }

    // --------------------------------------------------------------------------------------------

    public FssAzElBox ConsistencyCheck()
    {
        return new FssAzElBox
        {
            MinAzRads = MinAzRads,
            MaxAzRads = MaxAzRads,
            MinElRads = (MinElRads < MaxElRads) ? MinElRads : MaxElRads,
            MaxElRads = (MinElRads < MaxElRads) ? MaxElRads : MinElRads
        };
    }

    // --------------------------------------------------------------------------------------------

    public override string ToString()
    {
        return $"MinAz:{MinAzDegs:F2}Degs MaxAz:{MaxAzDegs:F2}Degs / MinEl:{MinElDegs:F2}Degs MaxEl:{MaxElDegs:F2}Degs";
    }

}
