
using System;
using System.Collections.Generic;
using System.Text;

// A beam defines the anchor point, and some constraints for BeamsScanPatterns to be built upon.

public class FssPlatformElementBeam : FssPlatformElement
{
    // Names
    // "Name" exists as part of the element for lookup
    public string EmitName { get; set; } = "DefaultEmitName";
    public string BeamName { get; set; } = "DefaultBeamName";

    // Is the pattern on display
    public bool Enabled { get; set; } = false;

    // Port (mounting point) angles
    public FssAttitude    PortAttitude { set; get; } = FssAttitude.Zero;
    public FssPolarOffset TrackOffset  { set; get; } = FssPolarOffset.Zero;

    // Scan area angle
    public FssAzElBox AzElBox { set; get; } = FssAzElBox.Zero;

    // Scan ranges
    public double DetectionRangeTxM { set; get; } = 0;
    public double DetectionRangeRxM { set; get; } = 0;

    // Scan shapes
    public enum ScanPatternShape { Undefined, Cone, Wedge, Dome };
    public ScanPatternShape ScanShape { set; get; } = ScanPatternShape.Undefined;

    // Targetted - is the scan aimed at a target in teh viewer
    public bool   Targeted       { get; set; } = false;
    public string TargetPlatName { get; set; } = "DefaultTarget";

    // Lookup to a named antenna pattern
    public string AntennaPattern { get; set; } = "DefaultPattern";

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