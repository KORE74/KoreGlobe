
// FssUIState: A collection of states in the UI

public enum FssCamMode
{
    WorldCam,
    ChaseCam,
    AlignCam
}

public class FssUIState
{
    public FssCamMode CameraMode          { get; set; } = FssCamMode.WorldCam;
    public bool       IsRwScale           { get; set; } = true;
    public float      InfographicScale    { get; set; } = 1;

    public bool       ShowRoutes          { get; set; } = true;
    public bool       ShowEmitters        { get; set; } = true;
    public bool       ShowAntennaPatterns { get; set; } = true;

    public bool       ShowRx              { get; set; } = true;
    public bool       ShowTx              { get; set; } = true;

    public FssUIState()
    {
        CameraMode = FssCamMode.WorldCam;
    }

    public bool IsCamModeWorld()    => CameraMode == FssCamMode.WorldCam;
    public bool IsCamModeChaseCam() => CameraMode == FssCamMode.ChaseCam;
    public bool IsCamModeAlignCam() => CameraMode == FssCamMode.AlignCam;

    // Usage: FssGodotFactory.Instance.UIState.UpdateDisplayedChaseCam()

    public void UpdateDisplayedChaseCam()
    {
        if (IsCamModeChaseCam() && FssAppFactory.Instance.EventDriver.NearPlatformValid())
        {
            FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(FssAppFactory.Instance.EventDriver.NearPlatformName());
        }
    }

}