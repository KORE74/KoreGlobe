
using System;
using System.Collections.Generic;
using System.Text;

// A beam defines the anchor point, and some constraints for BeamsScanPatterns to be built upon.

public class GloPlatformElementBeam : GloPlatformElement
{
    // Names
    // "Name" exists as part of the element for lookup
    public string EmitName { get; set; } = "DefaultEmitName";
    public string BeamName { get; set; } = "DefaultBeamName";

    // Port (mounting point) angles
    public GloAttitude  PortAttitude { set; get; } = GloAttitude.Zero;
    public GloAzElRange TrackOffset  { set; get; } = GloAzElRange.Zero;

    // Time related values
    public float PeriodSecs { get; set; } = 0.0f;

    // Scan area angle
    public GloAzElBox AzElBox { set; get; } = GloAzElBox.Zero;

    // Scan ranges
    public double DetectionRangeTxM { set; get; } = 0;
    public double DetectionRangeRxM { set; get; } = 0;

    // Scan shapes
    public enum ScanPatternShape { Undefined, Cone, Wedge, Dome, DomeSector };
    public ScanPatternShape ScanShape { set; get; } = ScanPatternShape.Undefined;

    // Targetted - is the scan aimed at a target in the viewer
    public bool   Targeted       { get; set; } = false;
    public string TargetPlatName { get; set; } = "DefaultTarget";

    // Lookup to a named antenna pattern
    public string AntennaPattern { get; set; } = "DefaultPattern";

    // --------------------------------------------------------------------------------------------
    // MARK: Complex Accessors
    // --------------------------------------------------------------------------------------------

    public void SetScanPattern(string scanPatternStr)
    {
        // Case insensitive compare
        ScanShape = scanPatternStr.ToLower().Trim() switch
        {
            "conical"  => ScanPatternShape.Cone,
            "raster"   => ScanPatternShape.Wedge,
            "circular" => ScanPatternShape.Dome,
            "circle"   => ScanPatternShape.Dome,
            "hsector"  => ScanPatternShape.DomeSector,
            _          => ScanPatternShape.Undefined
        };
    }

    public ScanPatternShape GetScanPatternShape()
    {
        if (Targeted) return ScanPatternShape.Cone;

        return ScanShape;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Report
    // --------------------------------------------------------------------------------------------

    public override string Report()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Beam: {Name} / EmitName: {EmitName} / BeamName: {BeamName}");
        sb.AppendLine($"  Enabled: {Enabled}");
        sb.AppendLine($"  Targeted: {Targeted} // TargetPlatName: {TargetPlatName}");
        sb.AppendLine($"  Ranges: {DetectionRangeTxM:F0}M (Tx), {DetectionRangeRxM:F0}M (Rx)");
        sb.AppendLine($"  Port Angles: {PortAttitude} / Track Offset: {TrackOffset}");
        sb.AppendLine($"  Scan AzElBox: {AzElBox}");
        sb.AppendLine($"  Scan Shape: {ScanShape}");

        return sb.ToString();
    }

}