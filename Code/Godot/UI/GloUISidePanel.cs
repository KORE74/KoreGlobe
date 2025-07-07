using Godot;
using System;
using System.Collections.Generic;

#nullable enable

public partial class GloUISidePanel : HBoxContainer
{
    // Top Level Elements
    private Button? PlatformScaleButton;
    private Button? ShowElementsButton;
    private Button? RxTxButton;
    private Label?  PerformanceLabel;

    // forms we would show/hide
    private Panel? PanelPlatformScale;
    private Panel? PanelShowElements;
    private Panel? PanelBeamRxTx;

    // Scale Controls
    private Button? RWScaleToggleButton;
    private Button? InfographicScaleButton;
    private Slider? InfographicScaleSlider;
    private Label?  InfographicScaleLabel;

    private Button? ShowRoutesButton;
    private Button? ShowEmittersButton;
    private Button? ShowAntennaPatternsButton;

    // RxTx Controls
    private Button? ShowTxButton;
    private Button? ShowRxButton;

    float UIPollTimer = 0f;
    private int currentSecondCallCount = 0;
    private float lastSecondTime = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GloCentralLog.AddEntry("GloUIHeader._Ready");
        GD.Print("GloUISidePanel._Ready");

        // Get the top level objects
        PlatformScaleButton = (Button)FindChild("PlatformScaleButton");
        ShowElementsButton  = (Button)FindChild("ShowElementsButton");
        RxTxButton          = (Button)FindChild("RxTxButton");
        PerformanceLabel    = (Label)FindChild("PerformanceLabel");

        // Get the forms we would show/hide
        PanelPlatformScale  = (Panel)FindChild("PanelPlatformScale");
        PanelShowElements   = (Panel)FindChild("PanelShowElements");
        PanelBeamRxTx       = (Panel)FindChild("PanelBeamRxTx");

        // get the scale controls
        RWScaleToggleButton    = (Button)FindChild("RWScaleToggleButton");
        InfographicScaleButton = (Button)FindChild("InfographicScaleButton");
        InfographicScaleSlider = (Slider)FindChild("InfographicScaleSlider");
        InfographicScaleLabel  = (Label)FindChild("InfographicScaleLabel");

        // get the show elements controls
        ShowRoutesButton          = (Button)FindChild("ShowRoutesButton");
        ShowEmittersButton        = (Button)FindChild("ShowEmittersButton");
        ShowAntennaPatternsButton = (Button)FindChild("ShowAntennaPatternsButton");

        // get the RxTx controls
        ShowTxButton = (Button)FindChild("ShowTxButton");
        ShowRxButton = (Button)FindChild("ShowRxButton");

        DebugCheck();

        // -------------------
        // Find the button and window controls

        // Top level (Panel display toggle)
        PlatformScaleButton.Connect("pressed", new Callable(this, "OnPlatformScaleButtonPressed"));
        ShowElementsButton.Connect( "pressed", new Callable(this, "OnShowElementsButtonPressed"));
        RxTxButton.Connect(         "pressed", new Callable(this, "OnRxTxButtonPressed"));

        // Scale Controls
        RWScaleToggleButton.Connect(   "pressed",       new Callable(this, "OnRWScaleToggleButtonPressed"));
        InfographicScaleButton.Connect("pressed",       new Callable(this, "OnInfographicScaleButtonPressed"));
        InfographicScaleSlider.Connect("value_changed", new Callable(this, "OnInfographicScaleSliderValueChanged"));

        // Show Elements Controls
        ShowRoutesButton.Connect(          "pressed", new Callable(this, "OnShowRoutesButtonPressed"));
        ShowEmittersButton.Connect(        "pressed", new Callable(this, "OnShowEmittersButtonPressed"));
        ShowAntennaPatternsButton.Connect( "pressed", new Callable(this, "OnShowAntennaPatternsButtonPressed"));

        // RxTx Controls
        ShowTxButton.Connect("pressed", new Callable(this, "OnShowTxButtonPressed"));
        ShowRxButton.Connect("pressed", new Callable(this, "OnShowRxButtonPressed"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update the performance label
        currentSecondCallCount++;

        if (UIPollTimer < GloCentralTime.RuntimeSecs)
        {
            UIPollTimer = GloCentralTime.RuntimeSecs + 1f; // Update the timer to the next whole second

            // Update each window visibility to the button state - in case we have alternative ways to close the window
            PlatformScaleButton!.ButtonPressed = PanelPlatformScale!.Visible;
            ShowElementsButton!.ButtonPressed  = PanelShowElements!.Visible;
            RxTxButton!.ButtonPressed          = PanelBeamRxTx!.Visible;

            //GD.Print($"GloGodotFactory.Instance.UIState.InfographicScale:{GloGodotFactory.Instance.UIState.InfographicScale}");

            // Manage the toggle buttons
            RWScaleToggleButton!.ButtonPressed    =  GloGodotFactory.Instance.UIState.IsRwScale;
            InfographicScaleButton!.ButtonPressed = !GloGodotFactory.Instance.UIState.IsRwScale;

            // Fix the number to the range and update the slider and label
            GloGodotFactory.Instance.UIState.InfographicScale = GloValueUtils.Clamp(GloGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);
            InfographicScaleSlider!.Value = GloGodotFactory.Instance.UIState.InfographicScale;
            InfographicScaleLabel!.Text   = $"{GloGodotFactory.Instance.UIState.InfographicScale:F0}";

            // Manage the Show Elements buttons
            ShowRoutesButton!.ButtonPressed          = GloGodotFactory.Instance.UIState.ShowRoutes;
            ShowEmittersButton!.ButtonPressed        = GloGodotFactory.Instance.UIState.ShowEmitters;
            ShowAntennaPatternsButton!.ButtonPressed = GloGodotFactory.Instance.UIState.ShowAntennaPatterns;

            // Manage the RxTx buttons
            ShowTxButton!.ButtonPressed = GloGodotFactory.Instance.UIState.ShowTx;
            ShowRxButton!.ButtonPressed = GloGodotFactory.Instance.UIState.ShowRx;

            PerformanceLabel!.Text = $"{currentSecondCallCount:F0}\nUPS";
            currentSecondCallCount = 0;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug Check
    // --------------------------------------------------------------------------------------------

    private void DebugCheck()
    {
        if (PlatformScaleButton == null) GD.PrintErr("PlatformScaleButton null");
        if (ShowElementsButton == null)  GD.PrintErr("ShowElementsButton null");
        if (RxTxButton == null)          GD.PrintErr("RxTxButton null");
        if (PerformanceLabel == null)    GD.PrintErr("PerformanceLabel null");

        if (PanelPlatformScale == null)  GD.PrintErr("PanelPlatformScale null");
        if (PanelShowElements == null)   GD.PrintErr("PanelShowElements null");
        if (PanelBeamRxTx == null)       GD.PrintErr("PanelBeamRxTx null");

        if (RWScaleToggleButton == null)    GD.PrintErr("RWScaleToggleButton null");
        if (InfographicScaleButton == null) GD.PrintErr("InfographicScaleButton null");
        if (InfographicScaleSlider == null) GD.PrintErr("InfographicScaleSlider null");
        if (InfographicScaleLabel == null)  GD.PrintErr("InfographicScaleLabel null");

        if (ShowRoutesButton == null)          GD.PrintErr("ShowRoutesButton null");
        if (ShowEmittersButton == null)        GD.PrintErr("ShowEmittersButton null");
        if (ShowAntennaPatternsButton == null) GD.PrintErr("ShowAntennaPatternsButton null");

        if (ShowTxButton == null) GD.PrintErr("ShowTxButton null");
        if (ShowRxButton == null) GD.PrintErr("ShowRxButton null");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - General
    // --------------------------------------------------------------------------------------------

    // Called when the "PlatformScaleButton" button is pressed
    public void OnPlatformScaleButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnPlatformScaleButtonPressed");
        PanelPlatformScale!.Visible = PlatformScaleButton!.ButtonPressed;
    }

    public void OnShowElementsButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowElementsButtonPressed");
        PanelShowElements!.Visible = ShowElementsButton!.ButtonPressed;
    }

    // Called when the "RxTxButton" button is pressed
    public void OnRxTxButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnRxTxButtonPressed");
        PanelBeamRxTx!.Visible = RxTxButton!.ButtonPressed;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - Scale
    // --------------------------------------------------------------------------------------------

    // Called when the "RWScaleToggleButton" button is pressed
    public void OnRWScaleToggleButtonPressed()
    {
        GD.Print("GloUIHeader.OnRWScaleToggleButtonPressed");

        if (!GloGodotFactory.Instance.UIState.IsRwScale)
            GloGodotFactory.Instance.UIState.IsRwScale = true;

        RWScaleToggleButton!.ButtonPressed    =  GloGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton!.ButtonPressed = !GloGodotFactory.Instance.UIState.IsRwScale;
    }

    // Called when the "InfographicScaleButton" button is pressed
    public void OnInfographicScaleButtonPressed()
    {
        GD.Print("GloUIHeader.OnInfographicScaleButtonPressed");

        if (GloGodotFactory.Instance.UIState.IsRwScale)
            GloGodotFactory.Instance.UIState.IsRwScale = false;

        RWScaleToggleButton!.ButtonPressed    =  GloGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton!.ButtonPressed = !GloGodotFactory.Instance.UIState.IsRwScale;
    }

    // Called when the "InfographicScaleSlider" value is changed
    public void OnInfographicScaleSliderValueChanged(float value)
    {
        GloCentralLog.AddEntry("GloUIHeader.OnInfographicScaleSliderValueChanged");

        GloGodotFactory.Instance.UIState.InfographicScale = value;
        InfographicScaleLabel!.Text = $"{GloGodotFactory.Instance.UIState.InfographicScale:F0}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - Show Elements
    // --------------------------------------------------------------------------------------------

    // Called when the "ShowRoutesButton" button is pressed
    public void OnShowRoutesButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowRoutesButtonPressed");

        GloGodotFactory.Instance.UIState.ShowRoutes = ShowRoutesButton!.ButtonPressed;
    }

    // Called when the "ShowEmittersButton" button is pressed
    public void OnShowEmittersButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowEmittersButtonPressed");

        GloGodotFactory.Instance.UIState.ShowEmitters = ShowEmittersButton!.ButtonPressed;
    }

    // Called when the "ShowAntennaPatternsButton" button is pressed
    public void OnShowAntennaPatternsButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowAntennaPatternsButtonPressed");

        GloGodotFactory.Instance.UIState.ShowAntennaPatterns = ShowAntennaPatternsButton!.ButtonPressed;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - RxTx
    // --------------------------------------------------------------------------------------------

    // Called when the "ShowTxButton" button is pressed
    public void OnShowTxButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowTxButtonPressed");

        GloAppFactory.Instance.EventDriver.SidebarSetBeamVisibility(ShowRxButton!.ButtonPressed, ShowTxButton!.ButtonPressed);
    }

    // Called when the "ShowRxButton" button is pressed
    public void OnShowRxButtonPressed()
    {
        GloCentralLog.AddEntry("GloUIHeader.OnShowRxButtonPressed");

        GloAppFactory.Instance.EventDriver.SidebarSetBeamVisibility(ShowRxButton!.ButtonPressed, ShowTxButton!.ButtonPressed);
    }

}
