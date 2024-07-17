

public enum ScanPatternShape { Undefined, Cone, Wedge, Dome };

public class FssPlatformElementScanPattern : FssPlatformElement
{
    public string Type {set; get; } = "ScanPattern";

    // Incoming Data

    // Internally Dervied values
    public ScanPatternShape ScanShape { set; get; } = ScanPatternShape.Undefined;

    // The emitter range as a somewhat arbitrary distance, and the detection range, at which the emitter can
    // see the target in the beam. Normally a shorter distance.
    public double EmitterRangeKms { set; get; } = 0;
    public double DetectionRangeKms { set; get; } = 0;

    public FssAzElBox AzElBox{ set; get; } = FssAzElBox.Zero;

}