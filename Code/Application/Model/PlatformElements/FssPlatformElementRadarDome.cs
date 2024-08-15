using System;

public enum FssRadarElementShape { Unknown, Dome, Wedge };

public class FssPlatformElementRadarDome : FssPlatformElement
{
    public string Type {set; get; } = "RadarDome";

    public bool Enabled { get; set; } = false;

    public double DetectionRange { get; set; } = 0.0;
    public double EmitterDetectionRange { get; set; } = 0.0;
}
