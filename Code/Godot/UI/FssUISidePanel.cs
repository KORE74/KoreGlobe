using Godot;
using System;
using System.Collections.Generic;

#nullable enable

public partial class FssUISidePanel : HBoxContainer
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

    // RxTx Controls
    private Button? ShowTxButton;
    private Button? ShowRxButton;

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

        // get the RxTx controls
        ShowTxButton = (Button)FindChild("ShowTxButton");
        ShowRxButton = (Button)FindChild("ShowRxButton");

        DebugCheck();

        // -------------------
        // Find the button and window controls

        // Top level (Panel display toggle)
        PlatformScaleButton.Connect("pressed", new Callable(this, "OnPlatformScaleButtonPressed"));
        ShowElementsButton.Connect( "pressed", new Callable(this, "OnShowElementsButtonPressed"));
        RxTxButton.Connect("pressed", new Callable(this, "OnRxTxButtonPressed"));

        // Scale Controls
        RWScaleToggleButton.Connect(   "pressed",       new Callable(this, "OnRWScaleToggleButtonPressed"));
        InfographicScaleButton.Connect("pressed",       new Callable(this, "OnInfographicScaleButtonPressed"));
        InfographicScaleSlider.Connect("value_changed", new Callable(this, "OnInfographicScaleSliderValueChanged"));

        // Show Elements Controls
        ShowRoutesButton.Connect(          "pressed", new Callable(this, "OnShowRoutesButtonPressed"));

        // RxTx Controls
        ShowTxButton.Connect("pressed", new Callable(this, "OnShowTxButtonPressed"));
        ShowRxButton.Connect("pressed", new Callable(this, "OnShowRxButtonPressed"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update the performance label
        currentSecondCallCount++;
        float averageCallsPerSecond = UpdateCallCount();

        if (UIPollTimer < FssCoreTime.RuntimeSecs)
        {
            UIPollTimer = FssCoreTime.RuntimeSecs + 1f; // Update the timer to the next whole second

            // Update each window visibility to the button state - in case we have alternative ways to close the window
            PlatformScaleButton!.ButtonPressed = PanelPlatformScale!.Visible;
            ShowElementsButton!.ButtonPressed  = PanelShowElements!.Visible;
            RxTxButton!.ButtonPressed          = PanelBeamRxTx!.Visible;

            //GD.Print($"FssGodotFactory.Instance.UIState.InfographicScale:{FssGodotFactory.Instance.UIState.InfographicScale}");

            // Manage the toggle buttons
            RWScaleToggleButton!.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
            InfographicScaleButton!.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;

            // Fix the number to the range and update the slider and label
            FssGodotFactory.Instance.UIState.InfographicScale = FssValueUtils.Clamp(FssGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);
            InfographicScaleSlider!.Value = FssGodotFactory.Instance.UIState.InfographicScale;
            InfographicScaleLabel!.Text   = $"{FssGodotFactory.Instance.UIState.InfographicScale:F0}";

            // Manage the Show Elements buttons
            ShowRoutesButton!.ButtonPressed          = FssGodotFactory.Instance.UIState.ShowRoutes;

            PerformanceLabel!.Text = $"{averageCallsPerSecond:F0}\nUPS";
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

        if (RWScaleToggleButton == null)    GD.PrintErr("RWScaleToggleButton null");
        if (InfographicScaleButton == null) GD.PrintErr("InfographicScaleButton null");
        if (InfographicScaleSlider == null) GD.PrintErr("InfographicScaleSlider null");
        if (InfographicScaleLabel == null)  GD.PrintErr("InfographicScaleLabel null");

        if (ShowRoutesButton == null)          GD.PrintErr("ShowRoutesButton null");

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

    public void OnShowElementsButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnShowElementsButtonPressed");
        PanelShowElements!.Visible = ShowElementsButton!.ButtonPressed;
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

        RWScaleToggleButton!.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton!.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;
    }

    // Called when the "InfographicScaleButton" button is pressed
    public void OnInfographicScaleButtonPressed()
    {
        GD.Print("FssUIHeader.OnInfographicScaleButtonPressed");

        if (FssGodotFactory.Instance.UIState.IsRwScale)
            FssGodotFactory.Instance.UIState.IsRwScale = false;

        RWScaleToggleButton!.ButtonPressed    =  FssGodotFactory.Instance.UIState.IsRwScale;
        InfographicScaleButton!.ButtonPressed = !FssGodotFactory.Instance.UIState.IsRwScale;
    }

    // Called when the "InfographicScaleSlider" value is changed
    public void OnInfographicScaleSliderValueChanged(float value)
    {
        FssCentralLog.AddEntry("FssUIHeader.OnInfographicScaleSliderValueChanged");

        FssGodotFactory.Instance.UIState.InfographicScale = value;
        InfographicScaleLabel!.Text = $"{FssGodotFactory.Instance.UIState.InfographicScale:F0}";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI - Show Elements
    // --------------------------------------------------------------------------------------------

    // Called when the "ShowRoutesButton" button is pressed
    public void OnShowRoutesButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnShowRoutesButtonPressed");

        FssGodotFactory.Instance.UIState.ShowRoutes = ShowRoutesButton!.ButtonPressed;
    }

}
