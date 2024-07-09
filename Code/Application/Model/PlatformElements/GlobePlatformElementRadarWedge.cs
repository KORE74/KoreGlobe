using System;

public class GlobePlatformElementRadarWedge : GlobePlatformElement
{
    public double DetectionRange { get; set; } = 0.0;
    public double EmitterDetectionRange { get; set; } = 0.0;
    public GlobeAzElBox emitterBox = GlobeAzElBox.Zero;

}
