
// FssUIState: A collection of states in the UI

public enum FssCamMode
{
    WorldCam,
    ChaseCam,
    AlignCam
}

public class FssUIState
{
    public FssCamMode CameraMode       { get; set; }
    public bool       IsRwScale        { get; set; }
    public float      InfographicScale { get; set; }

    public FssUIState()
    {
        CameraMode = FssCamMode.WorldCam;
    }

    public bool IsCamModeWorld()    => CameraMode == FssCamMode.WorldCam;
    public bool IsCamModeChaseCam() => CameraMode == FssCamMode.ChaseCam;
    public bool IsCamModeAlignCam() => CameraMode == FssCamMode.AlignCam;

    // Usage: FssAppFactory.Instance.UIState.UpdateDisplayedChaseCam()

    public void UpdateDisplayedChaseCam()
    {
        if (IsCamModeChaseCam() && FssAppFactory.Instance.EventDriver.NearPlatformValid())
        {
            FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(FssAppFactory.Instance.EventDriver.NearPlatformName());
        }
    }

}