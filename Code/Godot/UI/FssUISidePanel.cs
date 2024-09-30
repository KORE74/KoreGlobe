using Godot;
using System;
using System.Collections.Generic;

#nullable enable

public partial class FssUISidePanel : HBoxContainer
{
    // Top Level Elements
    private Button PlatformScaleButton;
    private Button RxTxButton;
    private Label  PerformanceLabel;

    // forms we would show/hide
    private Panel PanelPlatformScale;
    private Panel PanelBeamRxTx;

    // Scale Controls
    private Button RWScaleToggleButton;
    private Button InfographicScaleButton;
    private Slider InfographicScaleSlider;
    private Label  InfographicScaleLabel;

    // RxTx Controls
    private Button ShowTxButton;
    private Button ShowRxButton;

    float UIPollTimer = 0f;
    private List<int> callCounts = new List<int>();
    private int currentSecondCallCount = 0;
    private float lastSecondTime = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddEntry("FssUIHeader._Ready");

        // Get the top level objects
        PlatformScaleButton = (Button)FindChild("PlatformScaleButton");
        RxTxButton          = (Button)FindChild("RxTxButton");
        PerformanceLabel    = (Label)FindChild("PerformanceLabel");

        // Get the forms we would show/hide
        PanelPlatformScale  = (Panel)FindChild("PanelPlatformScale");
        PanelBeamRxTx       = (Panel)FindChild("PanelBeamRxTx");

        // get the scale controls
        RWScaleToggleButton    = (Button)FindChild("RWScaleToggleButton");
        InfographicScaleButton = (Button)FindChild("InfographicScaleButton");
        InfographicScaleSlider = (Slider)FindChild("InfographicScaleSlider");
        InfographicScaleLabel  = (Label)FindChild("InfographicScaleLabel");

        // get the RxTx controls
        ShowTxButton = (Button)FindChild("ShowTxButton");
        ShowRxButton = (Button)FindChild("ShowRxButton");

        // -------------------
        // Find the button and window controls

        // Top level (Panel display toggle)
        PlatformScaleButton.Connect("pressed", new Callable(this, "OnPlatformScaleButtonPressed"));
        RxTxButton.Connect("pressed", new Callable(this, "OnRxTxButtonPressed"));

        // Scale Controls
        RWScaleToggleButton.Connect(   "pressed",       new Callable(this, "OnRWScaleToggleButtonPressed"));
        InfographicScaleButton.Connect("pressed",       new Callable(this, "OnInfographicScaleButtonPressed"));
        InfographicScaleSlider.Connect("value_changed", new Callable(this, "OnInfographicScaleSliderValueChanged"));

        // RxTx Controls
        ShowTxButton.Connect("pressed", new Callable(this, "OnShowTxButtonPressed"));
        ShowRxButton.Connect("pressed", new Callable(this, "OnShowRxButtonPressed"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        currentSecondCallCount++;

        if (UIPollTimer < FssCoreTime.RuntimeSecs)
        {
            UIPollTimer = FssCoreTime.RuntimeSecs + 0.5f; // Update the timer to the next whole second

            // Update each window visibility to the button state - in case we have alternative ways to close the window
            PlatformScaleButton!.ButtonPressed = PanelPlatformScale!.Visible;
            RxTxButton!.ButtonPressed          = PanelBeamRxTx!.Visible;

            GD.Print($"FssGodotFactory.Instance.UIState.IsRwScale:{FssGodotFactory.Instance.UIState.IsRwScale}");

            RWScaleToggleButton.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
            InfographicScaleButton.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;
            InfographicScaleLabel.Text           = $"{FssGodotFactory.Instance.UIState.InfographicScale:F0}";

            // Update the performance label
            float averageCallsPerSecond = UpdateCallCount();
            PerformanceLabel.Text = $"{averageCallsPerSecond:F0}\nUPS";
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Perfomance Counter
    // --------------------------------------------------------------------------------------------

    private float UpdateCallCount()
    {
        // Update the performance counter every second
        if (FssCoreTime.RuntimeSecs - lastSecondTime >= 1.0f)
        {
            lastSecondTime = FssCoreTime.RuntimeSecs;

            // Add the current second's call count to the list
            callCounts.Add(currentSecondCallCount);

            // Keep only the last 5 seconds of data
            if (callCounts.Count > 5)
            {
                callCounts.RemoveAt(0);
            }

            // Calculate the average calls per second over the last 5 seconds
            int totalCalls = 0;
            foreach (int count in callCounts)
            {
                totalCalls += count;
            }
            float averageCallsPerSecond = totalCalls / (float)callCounts.Count;

            // Reset the current second's call count
            currentSecondCallCount = 0;

            return averageCallsPerSecond;
        }

        return 0.0f; // Return 0 if not enough time has passed
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - General
    // --------------------------------------------------------------------------------------------

    // Called when the "PlatformScaleButton" button is pressed
    public void OnPlatformScaleButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnPlatformScaleButtonPressed");
        PanelPlatformScale!.Visible = PlatformScaleButton!.ButtonPressed;
    }

    // Called when the "RxTxButton" button is pressed
    public void OnRxTxButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnRxTxButtonPressed");
        PanelBeamRxTx!.Visible = RxTxButton!.ButtonPressed;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - Scale
    // --------------------------------------------------------------------------------------------

    // Called when the "RWScaleToggleButton" button is pressed
    public void OnRWScaleToggleButtonPressed()
    {
        GD.Print("FssUIHeader.OnRWScaleToggleButtonPressed");

        if (!FssGodotFactory.Instance.UIState.IsRwScale)
            FssGodotFactory.Instance.UIState.IsRwScale = true;

        RWScaleToggleButton.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;
    }

    // Called when the "InfographicScaleButton" button is pressed
    public void OnInfographicScaleButtonPressed()
    {
        GD.Print("FssUIHeader.OnInfographicScaleButtonPressed");

        if (FssGodotFactory.Instance.UIState.IsRwScale)
            FssGodotFactory.Instance.UIState.IsRwScale = false;

        RWScaleToggleButton.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;

    }

    // Called when the "InfographicScaleSlider" value is changed
    public void OnInfographicScaleSliderValueChanged(float value)
    {
        FssCentralLog.AddEntry("FssUIHeader.OnInfographicScaleSliderValueChanged");
        //InfographicScaleLabel!.Text = $"{value:F0}";

        FssGodotFactory.Instance.UIState.InfographicScale = value;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - RxTx
    // --------------------------------------------------------------------------------------------

    // Called when the "ShowTxButton" button is pressed
    public void OnShowTxButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnShowTxButtonPressed");
    }

    // Called when the "ShowRxButton" button is pressed
    public void OnShowRxButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnShowRxButtonPressed");
    }

}
