using System;

public class FssPlatformElementRadarWedge : FssPlatformElement
{
    public string Type {set; get; } = "RadarWedge";

    public bool Enabled { get; set; } = false;

    public double DetectionRange { get; set; } = 0.0;
    public double EmitterDetectionRange { get; set; } = 0.0;
    public FssAzElBox emitterBox = FssAzElBox.Zero;

    // --------------------------------------------------------------------------------------------
    // #MARK Report
    // --------------------------------------------------------------------------------------------

}
