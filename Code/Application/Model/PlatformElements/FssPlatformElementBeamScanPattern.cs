



// A child element to a Beam

public class FssPlatformElementBeamScanPattern 
{
    // The emitter range as a somewhat arbitrary distance, and the detection range, at which the emitter can
    // see the target in the beam. Normally a shorter distance.
    public double EmitterRangeKms { set; get; } = 0;
    public double DetectionRangeKms { set; get; } = 0;

    public FssAzElBox     AzElBox{ set; get; } = FssAzElBox.Zero;
    public FssPolarOffset TrackOffset { set; get; } = FssPolarOffset.Zero;

    public enum ScanPatternShape { Undefined, Cone, Wedge, Dome };
    public ScanPatternShape ScanShape { set; get; } = ScanPatternShape.Undefined;

}