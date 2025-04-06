
// FssUIState: A collection of states in the UI

public enum FssCamMode
{
    WorldCam,
    ChaseCam,
    AlignCam
}



public class FssUIState
{
    public string     ScenarioName        { get; set; } = "DefaultScenario";

    public FssCamMode CameraMode          { get; set; } = FssCamMode.WorldCam;
    public bool       IsRwScale           { get; set; } = true;
    public float      InfographicScale    { get; set; } = 1;

    public bool       ShowRoutes          { get; set; } = true;

    public FssUILayoutParams LayoutParams { get; set; } = new FssUILayoutParams();

    public FssUIState()
    {
        CameraMode = FssCamMode.WorldCam;

        LayoutParams.ReadFromFile();
    }

    public bool IsCamModeWorld()    => CameraMode == FssCamMode.WorldCam;
    public bool IsCamModeChaseCam() => CameraMode == FssCamMode.ChaseCam;
    public bool IsCamModeAlignCam() => CameraMode == FssCamMode.AlignCam;

    // Usage: FssGodotFactory.Instance.UIState.UpdateDisplayedChaseCam()

    // public void UpdateDisplayedChaseCam()
    // {
    //     if (IsCamModeChaseCam() && FssEventDriver.NearPlatformValid())
    //     {
    //         FssGodotFactory.Instance.GodotEntityManager.EnableChaseCam(FssEventDriver.NearPlatformName());
    //     }
    // }

}