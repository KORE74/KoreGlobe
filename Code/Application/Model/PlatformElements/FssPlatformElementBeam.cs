
using System;
using System.Collections.Generic;

// A beam defines the anchor point, and some constraints for BeamsScanPatterns to be built upon.

public class FssPlatformElementBeam : FssPlatformElement
{
    public List<FssPlatformElementBeamScanPattern> ScanPatternList = new List<FssPlatformElementBeamScanPattern>();

    public string EmitName { get; set; } = "DefaultEmitName";
    public string BeamName { get; set; } = "DefaultBeamName";

    public bool Targeted { get; set; } = false;
    public string TargetPlatName { get; set; } = "DefaultTarget";

    public string AntennaPattern { get; set; } = "DefaultPattern";

    public FssAttitude PortAttitude { set; get; } = FssAttitude.Zero;
    public FssPolarOffset TrackOffset { set; get; } = FssPolarOffset.Zero;
    public double DetectionRangeTxKms { set; get; } = 0;
    public double DetectionRangeRxKms { set; get; } = 0;

}