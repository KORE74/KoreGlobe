



// A child element to a Beam

public class FssPlatformElementBeamScanPattern
{
    public FssAzElBox     AzElBox { set; get; } = FssAzElBox.Zero;
    public FssPolarOffset TrackOffset { set; get; } = FssPolarOffset.Zero;

    public enum ScanPatternShape { Undefined, Cone, Wedge, Dome };
    public ScanPatternShape ScanShape { set; get; } = ScanPatternShape.Undefined;

}