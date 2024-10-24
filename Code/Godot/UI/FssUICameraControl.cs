using Godot;
using System;

public partial class FssUICameraControl : Control
{
    // Label  CameraModeLabel;
    // Button CameraModeWorldButton;
    // Button CameraModeChaseCamButton;
    // Button CameraModeAlignCamButton;

    // Label  NearTargetPanelLabel;
    // Button NearTargetNextButton;
    // Label  NearTargetLabel;
    // Button NearTargetPrevButton;

    // Label  FarTargetPanelLabel;
    // Button FarTargetNextButton;
    // Label  FarTargetLabel;
    // Button FarTargetPrevButton;

    // float  TimerUIUpdate = 0.0f;

    // // Called when the node enters the scene tree for the first time.
    // public override void _Ready()
    // {
    //     CameraModeLabel          = (Label)FindChild("CameraModeLabel");
    //     CameraModeWorldButton    = (Button)FindChild("CameraModeWorldButton");
    //     CameraModeChaseCamButton = (Button)FindChild("CameraModeChaseCamButton");
    //     CameraModeAlignCamButton = (Button)FindChild("CameraModeAlignCamButton");

    //     NearTargetPanelLabel     = (Label)FindChild("NearTargetPanelLabel");
    //     NearTargetNextButton     = (Button)FindChild("NearTargetNextButton");
    //     NearTargetLabel          = (Label)FindChild("NearTargetLabel");
    //     NearTargetPrevButton     = (Button)FindChild("NearTargetPrevButton");

    //     FarTargetPanelLabel      = (Label)FindChild("FarTargetPanelLabel");
    //     FarTargetNextButton      = (Button)FindChild("FarTargetNextButton");
    //     FarTargetLabel           = (Label)FindChild("FarTargetLabel");
    //     FarTargetPrevButton      = (Button)FindChild("FarTargetPrevButton");

    //     CameraModeWorldButton.Connect("pressed", new Callable(this, "OnCameraModeWorldButtonPressed"));
    //     CameraModeChaseCamButton.Connect("pressed", new Callable(this, "OnCameraModeChaseCamButtonPressed"));
    //     CameraModeAlignCamButton.Connect("pressed", new Callable(this, "OnCameraModeAlignCamButtonPressed"));

    //     NearTargetNextButton.Connect("pressed", new Callable(this, "OnNearTargetNextButtonPressed"));
    //     NearTargetPrevButton.Connect("pressed", new Callable(this, "OnNearTargetPrevButtonPressed"));

    //     FarTargetNextButton.Connect("pressed", new Callable(this, "OnFarTargetNextButtonPressed"));
    //     FarTargetPrevButton.Connect("pressed", new Callable(this, "OnFarTargetPrevButtonPressed"));
    // }

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(double delta)
    // {
    //     if (TimerUIUpdate < FssCoreTime.RuntimeSecs)
    //     {
    //         TimerUIUpdate = FssCoreTime.RuntimeSecs + 1f;
    //         UpdateUIText();
    //         UpdatePlatformLabels();
    //         UpdateCameraModeButtonStates();
    //     }
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: Localisation
    // // --------------------------------------------------------------------------------------------

    // private void UpdateUIText()
    // {
    //     CameraModeLabel.Text      = FssLanguageStrings.Instance.GetParam("CameraMode");
    //     NearTargetPanelLabel.Text = FssLanguageStrings.Instance.GetParam("NearTargetPanel");
    //     FarTargetPanelLabel.Text  = FssLanguageStrings.Instance.GetParam("FarTargetPanel");
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: UI Updates
    // // --------------------------------------------------------------------------------------------

    // private void UpdatePlatformLabels()
    // {
    //     NearTargetLabel.Text = FssAppFactory.Instance.EventDriver.NearPlatformName();
    //     FarTargetLabel.Text  = FssAppFactory.Instance.EventDriver.FarPlatformName();

    //     bool nearEnabled = FssAppFactory.Instance.EventDriver.NearPlatformValid();
    //     NearTargetNextButton.Disabled = !nearEnabled;
    //     NearTargetPrevButton.Disabled = !nearEnabled;

    //     bool farEnabled = FssAppFactory.Instance.EventDriver.FarPlatformValid();
    //     FarTargetNextButton.Disabled = !farEnabled;
    //     FarTargetPrevButton.Disabled = !farEnabled;
    // }

    // private void UpdateCameraModeButtonStates()
    // {
    //     bool worldEnabled = true;
    //     bool chaseEnabled = (FssAppFactory.Instance.EventDriver.NumPlatforms() >= 1);
    //     bool alignEnabled = (FssAppFactory.Instance.EventDriver.NumPlatforms() >= 2);

    //     CameraModeWorldButton.Disabled    = !worldEnabled;
    //     CameraModeChaseCamButton.Disabled = !chaseEnabled;
    //     CameraModeAlignCamButton.Disabled = !alignEnabled;

    //     CameraModeWorldButton.SetPressedNoSignal(FssGodotFactory.Instance.UIState.IsCamModeWorld());
    //     CameraModeChaseCamButton.SetPressedNoSignal(FssGodotFactory.Instance.UIState.IsCamModeChaseCam());
    //     CameraModeAlignCamButton.SetPressedNoSignal(FssGodotFactory.Instance.UIState.IsCamModeAlignCam());
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: UI Actions - Modes
    // // --------------------------------------------------------------------------------------------

    // private void OnCameraModeWorldButtonPressed()
    // {
    //     GD.Print("OnCameraModeWorldButtonPressed");

    //     FssGodotFactory.Instance.UIState.CameraMode = FssCamMode.WorldCam;

    //     TestZeroOffset.WorldCamNode.CamNode.Current = true;
    // }

    // private void OnCameraModeChaseCamButtonPressed()
    // {
    //     GD.Print("OnCameraModeChaseCamButtonPressed");

    //     FssGodotFactory.Instance.UIState.CameraMode = FssCamMode.ChaseCam;
    //     FssGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
    // }

    // private void OnCameraModeAlignCamButtonPressed()
    // {
    //     GD.Print("OnCameraModeAlignCamButtonPressed");

    //     FssGodotFactory.Instance.UIState.CameraMode = FssCamMode.AlignCam;
    // }

    // // --------------------------------------------------------------------------------------------
    // // MARK: UI Actions - Near / Far Targets
    // // --------------------------------------------------------------------------------------------

    // private void OnNearTargetNextButtonPressed()
    // {
    //     GD.Print("OnNearTargetNextButtonPressed");

    //     FssAppFactory.Instance.EventDriver.NearPlatformNext();
    //     FssGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
    //     UpdatePlatformLabels();
    // }

    // private void OnNearTargetPrevButtonPressed()
    // {
    //     GD.Print("OnNearTargetPrevButtonPressed");

    //     FssAppFactory.Instance.EventDriver.NearPlatformPrev();
    //     FssGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
    //     UpdatePlatformLabels();
    // }

    // private void OnFarTargetNextButtonPressed()
    // {
    //     GD.Print("OnFarTargetNextButtonPressed");

    //     FssAppFactory.Instance.EventDriver.FarPlatformNext();
    //     UpdatePlatformLabels();
    // }

    // private void OnFarTargetPrevButtonPressed()
    // {
    //     GD.Print("OnFarTargetPrevButtonPressed");

    //     FssAppFactory.Instance.EventDriver.FarPlatformPrev();
    //     UpdatePlatformLabels();
    // }

}
