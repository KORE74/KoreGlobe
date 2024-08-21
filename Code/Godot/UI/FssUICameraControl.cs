using Godot;
using System;

public partial class FssUICameraControl : Control
{
    Label  CameraModeLabel;
    Button CameraModeWorldButton;
    Button CameraModeChaseCamButton;
    Button CameraModeAlignCamButton;

    Label  NearTargetPanelLabel;
    Button NearTargetNextButton;
    Label  NearTargetLabel;
    Button NearTargetPrevButton;

    Label  FarTargetPanelLabel;
    Button FarTargetNextButton;
    Label  FarTargetLabel;
    Button FarTargetPrevButton;

    float    TimerUIUpdate = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CameraModeLabel          = (Label)FindChild("CameraModeLabel");
        CameraModeWorldButton    = (Button)FindChild("CameraModeWorldButton");
        CameraModeChaseCamButton = (Button)FindChild("CameraModeChaseCamButton");
        CameraModeAlignCamButton = (Button)FindChild("CameraModeAlignCamButton");

        NearTargetPanelLabel     = (Label)FindChild("NearTargetPanelLabel");
        NearTargetNextButton     = (Button)FindChild("NearTargetNextButton");
        NearTargetLabel          = (Label)FindChild("NearTargetLabel");
        NearTargetPrevButton     = (Button)FindChild("NearTargetPrevButton");

        FarTargetPanelLabel      = (Label)FindChild("FarTargetPanelLabel");
        FarTargetNextButton      = (Button)FindChild("FarTargetNextButton");
        FarTargetLabel           = (Label)FindChild("FarTargetLabel");
        FarTargetPrevButton      = (Button)FindChild("FarTargetPrevButton");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerUIUpdate < FssCoreTime.RuntimeSecs)
        {
            TimerUIUpdate = FssCoreTime.RuntimeSecs + 1f;
            UpdateUIText();
        }
    }

        // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        CameraModeLabel.Text      = FssLanguageStrings.Instance.GetParam("CameraModeLabel");
        NearTargetPanelLabel.Text = FssLanguageStrings.Instance.GetParam("NearTargetPanelLabel");
        FarTargetPanelLabel.Text  = FssLanguageStrings.Instance.GetParam("FarTargetPanelLabel");
    }
}
