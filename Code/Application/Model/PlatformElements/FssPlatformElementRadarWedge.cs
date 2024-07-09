using System;

public class FssPlatformElementRadarWedge : FssPlatformElement
{
    public double DetectionRange { get; set; } = 0.0;
    public double EmitterDetectionRange { get; set; } = 0.0;
    public FssAzElBox emitterBox = FssAzElBox.Zero;

}
