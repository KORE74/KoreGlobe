
// GloUIState: A collection of states in the UI

using Godot;

public enum GloCamMode
{
    WorldCam,
    ChaseCam,
    AlignCam
}

public class GloUIState
{
    public string     ScenarioName        { get; set; } = "DefaultScenario";

    public GloCamMode CameraMode          { get; set; } = GloCamMode.WorldCam;
    public bool       IsRwScale           { get; set; } = true;
    public float      InfographicScale    { get; set; } = 1;

    public bool       ShowRoutes          { get; set; } = true;
    public bool       ShowEmitters        { get; set; } = true;
    public bool       ShowAntennaPatterns { get; set; } = true;

    public bool       ShowRx              { get; set; } = true;
    public bool       ShowTx              { get; set; } = true;

    public string     CameraMemory        { get; set; } = "";
    public bool       SpinChaseCam        { get; set; } = false;

    // Usage: GloGodotFactory.Instance.UIState.ShowTileInfo
    public bool       ShowTileInfo        { get; set; } = false;

    public GloUIState()
    {
        GD.Print("GloUIState constructor");

        CameraMode = GloCamMode.WorldCam;

        var config = GloCentralConfig.Instance;
        CameraMemory = config.GetParam<string>("WorldCamPos");


        ShowTileInfo = config.GetParam<bool>("ShowDebug");
    }

    public bool IsCamModeWorld()    => CameraMode == GloCamMode.WorldCam;
    public bool IsCamModeChaseCam() => CameraMode == GloCamMode.ChaseCam;
    public bool IsCamModeAlignCam() => CameraMode == GloCamMode.AlignCam;

    public void SetCameraModeWorld()    => CameraMode = GloCamMode.WorldCam;
    public void SetCameraModeChaseCam() => CameraMode = GloCamMode.ChaseCam;
    public void SetCameraModeAlignCam() => CameraMode = GloCamMode.AlignCam;


    // Usage: GloGodotFactory.Instance.UIState.UpdateDisplayedChaseCam()

    public void UpdateDisplayedChaseCam()
    {
        if (GloAppFactory.Instance.EventDriver.NearPlatformValid())
        {
            if (!IsCamModeChaseCam())
                SetCameraModeChaseCam();

            GloGodotFactory.Instance.GodotEntityManager.EnableChaseCam(GloAppFactory.Instance.EventDriver.NearPlatformName());
        }
    }

    public void UpdateTileInfo(bool infoVisible)
    {
        // Save the debug flag to config
        var config = GloCentralConfig.Instance;
        config.SetParam("ShowDebug", infoVisible);
        ShowTileInfo = infoVisible;

        GloGodotFactory.Instance.ZeroNodeMapManager.UpdateInfoVisibility(infoVisible);
    }

}