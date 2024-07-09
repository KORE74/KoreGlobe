using System;

public enum GlobeRadarElementShape { Unknown, Dome, Wedge };

public class GlobePlatformElementRadarDome : GlobePlatformElement
{
    public double DetectionRange { get; set; } = 0.0;
    public double EmitterDetectionRange { get; set; } = 0.0;
}
