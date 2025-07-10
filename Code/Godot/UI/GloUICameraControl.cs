using System.Text;
using Godot;

#nullable enable

public partial class GloUICameraControl : Control
{
    Godot.PanelContainer?  CamModePanel;
    Label?  CameraModeLabel;
    Button? CameraModeWorldButton;
    Button? CameraModeChaseCamButton;
    Button? CameraModeAlignCamButton;

    Godot.PanelContainer?  CameraMemoryPanel;
    Label?  CameraMemoryLabel;
    Button? CameraMemoryStoreButton;
    Button? CameraMemoryRecallButton;

    Godot.PanelContainer?  CameraNearTargetPanel;
    Label?  NearTargetPanelLabel;
    Button? NearTargetNextButton;
    Label?  NearTargetLabel;
    Button? NearTargetPrevButton;
    Button? SpinChaseCamButton;

    Godot.PanelContainer?  CameraFarTargetPanel;
    Label?  FarTargetPanelLabel;
    Button? FarTargetNextButton;
    Label?  FarTargetLabel;
    Button? FarTargetPrevButton;

    Godot.PanelContainer?  CameraSlidePanel;
    Label?  CameraSlideLabel;
    Button? CameraSlideStoreP1Button;
    Button? CameraSlideStoreP2Button;
    Button? CameraSlideGoButton;

    private GloWorldCamPos CamP1Pos;
    private GloWorldCamPos CamP2Pos;
    private bool SlideRunning = false;
    private int  StepTotal     = 1000;
    private int  StepCurrent   = 0;
    private GloFloat1DArray EaseArray;




    float  TimerUIUpdate = 0.0f;
    bool OneShot = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("GloUICameraControl._Ready()");

        ResourcePointers();
        if (CamModePanel == null) { GD.PrintErr("CamModePanel is null"); return; }

        CameraModeWorldButton?.Connect("pressed", new Callable(this, "OnCameraModeWorldButtonPressed"));
        CameraModeChaseCamButton?.Connect("pressed", new Callable(this, "OnCameraModeChaseCamButtonPressed"));
        CameraModeAlignCamButton?.Connect("pressed", new Callable(this, "OnCameraModeAlignCamButtonPressed"));

        CameraMemoryStoreButton?.Connect("pressed", new Callable(this, "OnCameraMemoryStoreButtonPressed"));
        CameraMemoryRecallButton?.Connect("pressed", new Callable(this, "OnCameraMemoryRecallButtonPressed"));

        NearTargetNextButton?.Connect("pressed", new Callable(this, "OnNearTargetNextButtonPressed"));
        NearTargetPrevButton?.Connect("pressed", new Callable(this, "OnNearTargetPrevButtonPressed"));
        SpinChaseCamButton?.Connect("pressed", new Callable(this, "OnSpinChaseCamButtonPressed"));

        FarTargetNextButton?.Connect("pressed", new Callable(this, "OnFarTargetNextButtonPressed"));
        FarTargetPrevButton?.Connect("pressed", new Callable(this, "OnFarTargetPrevButtonPressed"));

        CameraSlideStoreP1Button?.Connect("pressed", new Callable(this, "OnCameraSlideStoreP1ButtonPressed"));
        CameraSlideStoreP2Button?.Connect("pressed", new Callable(this, "OnCameraSlideStoreP2ButtonPressed"));
        CameraSlideGoButton?.Connect("pressed", new Callable(this, "OnCameraSlideGoButtonPressed"));

        //ReadConfig();

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerUIUpdate < GloCentralTime.RuntimeSecs)
        {

            if (OneShot == false)
            {
                OneShot = true;
                RecallCamPos();
            }

            TimerUIUpdate = GloCentralTime.RuntimeSecs + 1f;
            UpdateUIText();
            UpdatePlatformLabels();
            UpdateCameraModeButtonStates();
            UpdateCameraPanelVisibility();

            StoreCamPos();
        }

        if (SlideRunning)
        {
            StepCurrent++;
            if (StepCurrent > EaseArray.Length - 1)
            {
                SlideRunning = false;
                StepCurrent  = 0;
            }
            else
            {
                float fraction = EaseArray[StepCurrent];

                GloWorldCamPos camPos = GloWorldCamPos.Lerp(CamP1Pos, CamP2Pos, fraction);
                KoreGodotFactory.Instance.WorldCamNode.SetWorldCamPos(camPos);

                if (StepCurrent >= StepTotal)
                {
                    SlideRunning = false;
                }
            }


        }

    }

    // --------------------------------------------------------------------------------------------

    private void ResourcePointers()
    {
        CamModePanel             = FindChildManually("PanelContainer-CamMode")   as Godot.PanelContainer;
        CameraModeLabel          = FindChildManually("CameraModeLabel")          as Godot.Label;
        CameraModeWorldButton    = FindChildManually("CameraModeWorldButton")    as Button;
        CameraModeChaseCamButton = FindChildManually("CameraModeChaseCamButton") as Button;
        CameraModeAlignCamButton = FindChildManually("CameraModeAlignCamButton") as Button;

        CameraMemoryPanel        = FindChildManually("PanelContainer-CamMemory") as Godot.PanelContainer;
        CameraMemoryLabel        = FindChildManually("CameraMemoryLabel") as Label;
        CameraMemoryStoreButton  = FindChildManually("CameraMemoryStoreButton") as Button;
        CameraMemoryRecallButton = FindChildManually("CameraMemoryRecallButton") as Button;

        CameraNearTargetPanel    = FindChildManually("PanelContainer-NearCamTarget") as Godot.PanelContainer;
        NearTargetPanelLabel     = FindChildManually("NearTargetPanelLabel") as Label;
        NearTargetNextButton     = FindChildManually("NearTargetNextButton") as Button;
        NearTargetLabel          = FindChildManually("NearTargetLabel") as Label;
        NearTargetPrevButton     = FindChildManually("NearTargetPrevButton") as Button;
        SpinChaseCamButton       = FindChildManually("SpinChaseCamButton") as Button;

        CameraFarTargetPanel     = FindChildManually("PanelContainer-FarCamTarget") as Godot.PanelContainer;
        FarTargetPanelLabel      = FindChildManually("FarTargetPanelLabel") as Label;
        FarTargetNextButton      = FindChildManually("FarTargetNextButton") as Button;
        FarTargetLabel           = FindChildManually("FarTargetLabel") as Label;
        FarTargetPrevButton      = FindChildManually("FarTargetPrevButton") as Button;

        CameraSlidePanel         = FindChildManually("PanelContainer-CamSlide") as Godot.PanelContainer;
        CameraSlideLabel         = FindChildManually("CameraSlideLabel") as Label;
        CameraSlideStoreP1Button = FindChildManually("CameraSlideStoreP1Button") as Button;
        CameraSlideStoreP2Button = FindChildManually("CameraSlideStoreP2Button") as Button;
        CameraSlideGoButton      = FindChildManually("CameraSlideGoButton") as Button;


        bool failedLoad = false;
        if (CamModePanel == null)             { GD.PrintErr("READY CamModePanel is null");             failedLoad = true; }
        if (CameraModeLabel == null)          { GD.PrintErr("READY CameraModeLabel is null");          failedLoad = true; }
        if (CameraModeWorldButton == null)    { GD.PrintErr("READY CameraModeWorldButton is null");    failedLoad = true; }
        if (CameraModeChaseCamButton == null) { GD.PrintErr("READY CameraModeChaseCamButton is null"); failedLoad = true; }
        if (CameraModeAlignCamButton == null) { GD.PrintErr("READY CameraModeAlignCamButton is null"); failedLoad = true; }
        if (CameraMemoryPanel == null)        { GD.PrintErr("READY CameraMemoryPanel is null");        failedLoad = true; }
        if (CameraMemoryLabel == null)        { GD.PrintErr("READY CameraMemoryLabel is null");        failedLoad = true; }
        if (CameraMemoryStoreButton == null)  { GD.PrintErr("READY CameraMemoryStoreButton is null");  failedLoad = true; }
        if (CameraMemoryRecallButton == null) { GD.PrintErr("READY CameraMemoryRecallButton is null"); failedLoad = true; }
        if (CameraNearTargetPanel == null)    { GD.PrintErr("READY CameraNearTargetPanel is null");    failedLoad = true; }
        if (NearTargetPanelLabel == null)     { GD.PrintErr("READY NearTargetPanelLabel is null");     failedLoad = true; }
        if (NearTargetNextButton == null)     { GD.PrintErr("READY NearTargetNextButton is null");     failedLoad = true; }
        if (NearTargetLabel == null)          { GD.PrintErr("READY NearTargetLabel is null");          failedLoad = true; }
        if (NearTargetPrevButton == null)     { GD.PrintErr("READY NearTargetPrevButton is null");     failedLoad = true; }
        if (SpinChaseCamButton == null)       { GD.PrintErr("READY SpinChaseCamButton is null");       failedLoad = true; }
        if (CameraFarTargetPanel == null)     { GD.PrintErr("READY CameraFarTargetPanel is null");     failedLoad = true; }
        if (FarTargetPanelLabel == null)      { GD.PrintErr("READY FarTargetPanelLabel is null");      failedLoad = true; }
        if (FarTargetNextButton == null)      { GD.PrintErr("READY FarTargetNextButton is null");      failedLoad = true; }
        if (FarTargetLabel == null)           { GD.PrintErr("READY FarTargetLabel is null");           failedLoad = true; }
        if (FarTargetPrevButton == null)      { GD.PrintErr("READY FarTargetPrevButton is null");      failedLoad = true; }

        if (CameraSlidePanel == null)         { GD.PrintErr("READY CameraSlidePanel is null");         failedLoad = true; }
        if (CameraSlideLabel == null)         { GD.PrintErr("READY CameraSlideLabel is null");         failedLoad = true; }
        if (CameraSlideStoreP1Button == null) { GD.PrintErr("READY CameraSlideStoreP1Button is null"); failedLoad = true; }
        if (CameraSlideStoreP2Button == null) { GD.PrintErr("READY CameraSlideStoreP2Button is null"); failedLoad = true; }
        if (CameraSlideGoButton == null)      { GD.PrintErr("READY CameraSlideGoButton is null");      failedLoad = true; }

        if (failedLoad)
        {
            GD.PrintErr("READY GloUICameraControl: Failed to load one or more UI elements");
            return;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        // if (CamModePanel == null) { ResourcePointers(); }

        // GD.Print($"CameraMode: {GloLanguageStrings.Instance.GetParam("CameraMemory")}");
        // if (CameraMemoryLabel == null) GD.Print($"NULL! // {ChildNodeList()}");
        // CameraModeLabel          = (Label)FindChild("CameraModeLabel");

        CameraModeLabel!.Text      = GloLanguageStrings.Instance.GetParam("CameraMode");
        CameraMemoryLabel!.Text    = GloLanguageStrings.Instance.GetParam("CameraMemory");
        NearTargetPanelLabel!.Text = GloLanguageStrings.Instance.GetParam("NearTargetPanel");
        FarTargetPanelLabel!.Text  = GloLanguageStrings.Instance.GetParam("FarTargetPanel");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Config
    // --------------------------------------------------------------------------------------------

    private void ReadConfig()
    {
        var config = GloCentralConfig.Instance;
        string camPosString = config.GetParam<string>("WorldCamPos");

        GD.Print($"ReadConfig: {camPosString}");

        //if (!string.IsNullOrEmpty(camPosString))
        //{
            KoreGodotFactory.Instance.UIState.CameraMemory = camPosString;
            KoreGodotFactory.Instance.WorldCamNode.CamPosFromString(camPosString);
        //}
    }

    private void WriteConfig()
    {
        var config = GloCentralConfig.Instance;

        if (!string.IsNullOrEmpty(KoreGodotFactory.Instance.UIState.CameraMemory))
        {
            config.SetParam("WorldCamPos", KoreGodotFactory.Instance.UIState.CameraMemory);
            GloCentralConfig.Instance.WriteToFile();
        }
    }

    private void StoreCamPos()
    {
        if (KoreGodotFactory.Instance.UIState.CameraMode == GloCamMode.WorldCam)
        {
            string camPosString = KoreGodotFactory.Instance.WorldCamNode.CamPosToString();

            if (!string.IsNullOrEmpty(camPosString))
            {
                var config = GloCentralConfig.Instance;
                config.SetParam("WorldCamPos-Live", camPosString);
                GloCentralConfig.Instance.WriteToFile();
            }
        }
    }

    private void RecallCamPos()
    {
        if (KoreGodotFactory.Instance.UIState.CameraMode == GloCamMode.WorldCam)
        {
            var config = GloCentralConfig.Instance;
            string camPosString = config.GetParam<string>("WorldCamPos-Live");

            if (!string.IsNullOrEmpty(camPosString))
            {
                //KoreGodotFactory.Instance.UIState.CameraMemory = camPosString;
                KoreGodotFactory.Instance.WorldCamNode.CamPosFromString(camPosString);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Updates
    // --------------------------------------------------------------------------------------------

    private void UpdatePlatformLabels()
    {
        NearTargetLabel.Text = GloAppFactory.Instance.EventDriver.NearPlatformName();
        FarTargetLabel.Text  = GloAppFactory.Instance.EventDriver.FarPlatformName();

        bool nearEnabled = GloAppFactory.Instance.EventDriver.NearPlatformValid();
        NearTargetNextButton.Disabled = !nearEnabled;
        NearTargetPrevButton.Disabled = !nearEnabled;

        bool farEnabled = GloAppFactory.Instance.EventDriver.FarPlatformValid();
        FarTargetNextButton.Disabled = !farEnabled;
        FarTargetPrevButton.Disabled = !farEnabled;
    }

    private void UpdateCameraModeButtonStates()
    {
        bool worldEnabled = true;
        bool chaseEnabled = (GloAppFactory.Instance.EventDriver.NumPlatforms() >= 1);
        bool alignEnabled = (GloAppFactory.Instance.EventDriver.NumPlatforms() >= 2);

        CameraModeWorldButton.Disabled    = !worldEnabled;
        CameraModeChaseCamButton.Disabled = !chaseEnabled;
        CameraModeAlignCamButton.Disabled = true; //!alignEnabled;

        CameraModeAlignCamButton.Visible = false;

        CameraMemoryStoreButton.Disabled  = !worldEnabled;
        CameraMemoryRecallButton.Disabled = !worldEnabled;

        CameraModeWorldButton.SetPressedNoSignal(KoreGodotFactory.Instance.UIState.IsCamModeWorld());
        CameraModeChaseCamButton.SetPressedNoSignal(KoreGodotFactory.Instance.UIState.IsCamModeChaseCam());
        CameraModeAlignCamButton.SetPressedNoSignal(KoreGodotFactory.Instance.UIState.IsCamModeAlignCam());

        SpinChaseCamButton.SetPressedNoSignal(KoreGodotFactory.Instance.UIState.SpinChaseCam);
    }

    private void UpdateCameraPanelVisibility()
    {
        bool worldVisible = KoreGodotFactory.Instance.UIState.IsCamModeWorld();
        bool chaseVisible = KoreGodotFactory.Instance.UIState.IsCamModeChaseCam();
        bool alignVisible = KoreGodotFactory.Instance.UIState.IsCamModeAlignCam();

        CamModePanel.Visible          = true;
        CameraMemoryPanel.Visible     = worldVisible;
        CameraNearTargetPanel.Visible = chaseVisible;
        CameraFarTargetPanel.Visible  = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Modes
    // --------------------------------------------------------------------------------------------

    private void OnCameraModeWorldButtonPressed()
    {
        GD.Print("OnCameraModeWorldButtonPressed");

        KoreGodotFactory.Instance.UIState.CameraMode = GloCamMode.WorldCam;

        KoreGodotFactory.Instance.WorldCamNode.CamNode.Current = true;

        UpdateCameraPanelVisibility();
    }

    private void OnCameraModeChaseCamButtonPressed()
    {
        GD.Print("OnCameraModeChaseCamButtonPressed");

        KoreGodotFactory.Instance.UIState.CameraMode = GloCamMode.ChaseCam;
        KoreGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();

        UpdateCameraPanelVisibility();
    }

    private void OnCameraModeAlignCamButtonPressed()
    {
        GD.Print("OnCameraModeAlignCamButtonPressed");

        KoreGodotFactory.Instance.UIState.CameraMode = GloCamMode.AlignCam;

        UpdateCameraPanelVisibility();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Memory
    // --------------------------------------------------------------------------------------------

    private void OnCameraMemoryStoreButtonPressed()
    {
        GD.Print("OnCameraMemoryStoreButtonPressed");

        string camPosString = KoreGodotFactory.Instance.WorldCamNode.CamPosToString();
        GD.Print($"OnCameraMemoryStoreButtonPressed: {camPosString}");

        KoreGodotFactory.Instance.UIState.CameraMemory = camPosString;
        WriteConfig();
    }

    private void OnCameraMemoryRecallButtonPressed()
    {
        if (!string.IsNullOrEmpty(KoreGodotFactory.Instance.UIState.CameraMemory))
        {
            KoreGodotFactory.Instance.WorldCamNode.CamPosFromString(KoreGodotFactory.Instance.UIState.CameraMemory);
            GD.Print($"OnCameraMemoryRecallButtonPressed: {KoreGodotFactory.Instance.UIState.CameraMemory}");
        }
        else
        {
            GD.Print("OnCameraMemoryRecallButtonPressed: No memory stored");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Slide
    // --------------------------------------------------------------------------------------------

    private void OnCameraSlideStoreP1ButtonPressed()
    {
        GD.Print("OnCameraSlideStoreP1ButtonPressed");
        CamP1Pos = KoreGodotFactory.Instance.WorldCamNode.GetWorldCamPos();
    }

    private void OnCameraSlideStoreP2ButtonPressed()
    {
        GD.Print("OnCameraSlideStoreP2ButtonPressed");
        CamP2Pos = KoreGodotFactory.Instance.WorldCamNode.GetWorldCamPos();
    }

    private void OnCameraSlideGoButtonPressed()
    {
        GD.Print("OnCameraSlideGoButtonPressed");

        if (!SlideRunning)
        {
            SlideRunning = true;
            EaseArray = GloFloat1DArrayOperations.EaseInOut(0.0f, 1.0f, StepTotal);
        }

    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Near / Far Targets
    // --------------------------------------------------------------------------------------------

    private void OnNearTargetNextButtonPressed()
    {
        GD.Print("OnNearTargetNextButtonPressed");

        GloAppFactory.Instance.EventDriver.NearPlatformNext();
        KoreGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
        UpdatePlatformLabels();
    }

    private void OnNearTargetPrevButtonPressed()
    {
        GD.Print("OnNearTargetPrevButtonPressed");

        GloAppFactory.Instance.EventDriver.NearPlatformPrev();
        KoreGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
        UpdatePlatformLabels();
    }

    private void OnFarTargetNextButtonPressed()
    {
        GD.Print("OnFarTargetNextButtonPressed");

        GloAppFactory.Instance.EventDriver.FarPlatformNext();
        UpdatePlatformLabels();
    }

    private void OnFarTargetPrevButtonPressed()
    {
        GD.Print("OnFarTargetPrevButtonPressed");

        GloAppFactory.Instance.EventDriver.FarPlatformPrev();
        UpdatePlatformLabels();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Spin
    // --------------------------------------------------------------------------------------------

    private void OnSpinChaseCamButtonPressed()
    {
        KoreGodotFactory.Instance.UIState.SpinChaseCam = !KoreGodotFactory.Instance.UIState.SpinChaseCam;

        GD.Print($"OnSpinChaseCamButtonPressed - Value now {KoreGodotFactory.Instance.UIState.SpinChaseCam}");

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Child Nodes
    // --------------------------------------------------------------------------------------------

    private string ChildNodeList()
    {
        StringBuilder sb = new StringBuilder();
        BuildChildList(this, sb, 0);
        return sb.ToString();
    }

    private void BuildChildList(Node node, StringBuilder sb, int depth)
    {
        string indent = new string(' ', depth * 2);
        sb.AppendLine($"{indent}- {node.Name}");

        foreach (Node child in node.GetChildren())
        {
            BuildChildList(child, sb, depth + 1);

            if (child.Name =="CameraMemoryLabel")
                CameraMemoryLabel = (Label)child;
        }
    }

    private Node? FindChildManually(string name, Node? current = null)
    {
        current ??= this;

        if (current.Name == name)
            return current;

        foreach (Node child in current.GetChildren())
        {
            Node? found = FindChildManually(name, child);
            if (found != null)
                return found;
        }

        return null;
    }


}
