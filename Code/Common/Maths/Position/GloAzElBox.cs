using System;

public struct GloAzElBox
{
    public double MinAzRads { get; set; }
    public double MaxAzRads { get; set; }
    public double MinElRads { get; set; }
    public double MaxElRads { get; set; }

    // --------------------------------------------------------------------------------------------

    public double MinAzDegs
    {
        get { return MinAzRads * GloConsts.RadsToDegsMultiplier; }
        set { MinAzRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double MaxAzDegs
    {
        get { return MaxAzRads * GloConsts.RadsToDegsMultiplier; }
        set { MaxAzRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double MinElDegs
    {
        get { return MinElRads * GloConsts.RadsToDegsMultiplier; }
        set { MinElRads = value * GloConsts.DegsToRadsMultiplier; }
    }
    public double MaxElDegs
    {
        get { return MaxElRads * GloConsts.RadsToDegsMultiplier; }
        set { MaxElRads = value * GloConsts.DegsToRadsMultiplier; }
    }

    // --------------------------------------------------------------------------------------------

    public double ArcAzRads { get { return (MaxAzRads - MinAzRads); } }
    public double ArcAzDegs { get { return (MaxAzRads - MinAzRads) * GloConsts.RadsToDegsMultiplier; } }
    public double ArcElRads { get { return (MaxElRads - MinElRads); } }
    public double ArcElDegs { get { return (MaxElRads - MinElRads) * GloConsts.RadsToDegsMultiplier; } }

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

    public GloAzEl Center { get { return new GloAzEl(MidAzRads, MidElRads); } }

    // --------------------------------------------------------------------------------------------

    public static GloAzElBox Zero
    {
        get { return new GloAzElBox { MinAzRads = 0.0, MaxAzRads = 0.0, MinElRads = 0.0, MaxElRads = 0.0 }; }
    }

    // --------------------------------------------------------------------------------------------

    public bool IsOffsetInBox(GloAzElRange offset)
    {
        bool inAz = (offset.AzRads >= MinAzRads && offset.AzRads <= MaxAzRads);
        bool inEl = (offset.ElRads >= MinElRads && offset.ElRads <= MaxElRads);
        return (inAz && inEl);
    }

    // --------------------------------------------------------------------------------------------

    public static GloAzElBox BoxFromOffset(GloAzElRange offset)
    {
        return new GloAzElBox
        {
            MinAzRads = offset.AzRads,
            MaxAzRads = offset.AzRads,
            MinElRads = offset.ElRads,
            MaxElRads = offset.ElRads,
        };
    }

    // Usage: GloAzElBox.BoxFromDimensions(10, 20);
    public static GloAzElBox BoxFromDimensions(double azDegs, double elDegs)
    {
        double halfAzDegs = azDegs / 2.0;
        double halfElDegs = elDegs / 2.0;
        return new GloAzElBox
        {
            MinAzDegs = -halfAzDegs,
            MaxAzDegs = halfAzDegs,
            MinElDegs = -halfElDegs,
            MaxElDegs = halfElDegs,
        };
    }

    // --------------------------------------------------------------------------------------------

    public GloAzElBox Inflate(double azInflateDegs, double elInflateDegs)
    {
        return new GloAzElBox
        {
            MinAzDegs = MinAzDegs - azInflateDegs,
            MaxAzDegs = MaxAzDegs + azInflateDegs,
            MinElDegs = MinElDegs - elInflateDegs,
            MaxElDegs = MaxElDegs + elInflateDegs
        };
    }

    public GloAzElBox ExpandBoxByOffset(GloAzElRange offset)
    {
        return new GloAzElBox
        {
            MinAzRads = Math.Min(MinAzRads, offset.AzRads),
            MaxAzRads = Math.Max(MaxAzRads, offset.AzRads),
            MinElRads = Math.Min(MinElRads, offset.ElRads),
            MaxElRads = Math.Max(MaxElRads, offset.ElRads)
        };
    }

    public GloAzElBox ExpandBoxByBox(GloAzElBox box)
    {
        return new GloAzElBox
        {
            MinAzRads = Math.Min(MinAzRads, box.MinAzRads),
            MaxAzRads = Math.Max(MaxAzRads, box.MaxAzRads),
            MinElRads = Math.Min(MinElRads, box.MinElRads),
            MaxElRads = Math.Max(MaxElRads, box.MaxElRads)
        };
    }

    // --------------------------------------------------------------------------------------------

    public GloAzElBox CentreBoxOnOffset(GloAzElRange offset)
    {
        return new GloAzElBox
        {
            MinAzRads = MinAzRads - HalfArcAzRads,
            MaxAzRads = MaxAzRads + HalfArcAzRads,
            MinElRads = MinElRads - HalfArcElRads,
            MaxElRads = MaxElRads + HalfArcElRads
        };
    }

    public GloAzElBox ShiftBoxAzElRange(double azAdjustDegs, double elAdjustDegs)
    {
        return new GloAzElBox
        {
            MinAzDegs = MinAzDegs + azAdjustDegs,
            MaxAzDegs = MaxAzDegs + azAdjustDegs,
            MinElDegs = MinElDegs + elAdjustDegs,
            MaxElDegs = MaxElDegs + elAdjustDegs
        };
    }

    // --------------------------------------------------------------------------------------------

    public GloAzElBox ConsistencyCheck()
    {
        return new GloAzElBox
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
