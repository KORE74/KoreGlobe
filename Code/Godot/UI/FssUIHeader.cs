using Godot;
using System;

#nullable enable

public partial class FssUIHeader : PanelContainer
{
    private Button? CliButton;
    private Button? SettingButton;
    private Button? NetworkButton;
    private Button? HelpButton;
    private Button? ExitButton;

    private Window? CliWindowNode;
    private Window? SettingWindowNode;
    private Window? NetworkWindowNode;
    private Window? HelpWindowNode;
    private Window? ClearDataNode;

    private Label? ScenarioTimeLabel;
    private Label? ScenarioNameLabel;

    float UIPollTimer = 0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddEntry("FssUIHeader._Ready");

        // Get the button objects
        CliButton         = (Button)FindChild("CLI-IconButton");
        SettingButton     = (Button)FindChild("Setting-IconButton");
        NetworkButton     = (Button)FindChild("Network-IconButton");
        HelpButton        = (Button)FindChild("HelpButton");
        ExitButton        = (Button)FindChild("Exit-IconButton");
        ScenarioTimeLabel = (Label)FindChild("ScenarioTimeLabel");
        ScenarioNameLabel = (Label)FindChild("ScenarioNameLabel");

        CliWindowNode     = GetNode<Window>("../../../../CLIWindow");
        SettingWindowNode = GetNode<Window>("../../../../SettingWindow");
        NetworkWindowNode = GetNode<Window>("../../../../NetworkWindow");
        HelpWindowNode    = GetNode<Window>("../../../../HelpWindow");

        // Check if any of the read items are null
        if (CliButton == null || SettingButton == null || NetworkButton == null || ExitButton == null || HelpButton == null)
        {
            FssCentralLog.AddEntry("One or more buttons not found");
            return;
        }
        if (CliWindowNode == null || SettingWindowNode == null || NetworkWindowNode == null || HelpWindowNode == null)
        {
            FssCentralLog.AddEntry("One or more windows not found");
            return;
        }

        // -------------------
        // Find the button and window controls

        CliButton.Connect("pressed", new Callable(this, "OnCLIButtonPressed"));
        SettingButton.Connect("pressed", new Callable(this, "OnSettingButtonPressed"));
        NetworkButton.Connect("pressed", new Callable(this, "OnNetworkButtonPressed"));
        HelpButton.Connect("pressed", new Callable(this, "OnHelpButtonPressed"));
        ExitButton.Connect("pressed", new Callable(this, "OnExitButtonPressed"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (UIPollTimer < FssCoreTime.RuntimeSecs)
        {
            UIPollTimer = FssCoreTime.RuntimeSecs + 0.2f; // Update the timer to the next whole second

            // Update each window visibility to the button state - in case we have alternative ways to close the window
            NetworkButton!.ButtonPressed = NetworkWindowNode!.Visible;
            SettingButton!.ButtonPressed = SettingWindowNode!.Visible;
            CliButton!.ButtonPressed     = CliWindowNode!.Visible;

            string strScenarioTime = FssLanguageStrings.Instance.GetParam("ScenarioTime");
            //string strSeconds      = FssLanguageStrings.Instance.GetParam("Seconds");
            string strScenarioName = FssLanguageStrings.Instance.GetParam("ScenarioName");
            string strNotDefined   = FssLanguageStrings.Instance.GetParam("NotDefined");

            string simclockHMS = FssAppFactory.Instance.EventDriver.SimTimeHMS();

            string displayScenarioName = strNotDefined;
            if (!string.IsNullOrEmpty(FssGodotFactory.Instance.UIState.ScenarioName))
                displayScenarioName = FssGodotFactory.Instance.UIState.ScenarioName;

            // If the length of the scenario name is greater than 30 characters, truncate the middle and add "..."
            if (displayScenarioName.Length > 30)
            {
                string frontSubStr = displayScenarioName.Substring(0, 15);
                string backSubStr  = displayScenarioName.Substring(displayScenarioName.Length - 15, 15);
                displayScenarioName = $"{frontSubStr}...{backSubStr}";
            }

            ScenarioTimeLabel!.Text = $"{strScenarioTime}:\n{simclockHMS}";
            ScenarioNameLabel!.Text = $"{strScenarioName}:\n{displayScenarioName}";
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    // Called when the "Exit-IconButton" button is pressed
    public void OnExitButtonPressed()
    {
        // Get the current scene
        SceneTree sceneTree = GetTree();

        // Quit the game
        sceneTree.Quit();
    }

    public void OnCLIButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnCLIToggleButtonPressed");
        CliWindowNode!.Visible = CliButton!.ButtonPressed;
    }

    public void OnSettingButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnSettingButtonPressed");
        if (SettingButton!.ButtonPressed)
        {
            FssUISettingWindow settingWindow = (FssUISettingWindow)SettingWindowNode!;
            settingWindow.RefreshContent();

            SettingWindowNode!.Visible = true;
        }
        else
        {
            SettingWindowNode!.Visible = false;
        }
    }

    public void OnHelpButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnHelpButtonPressed");
        HelpWindowNode!.Visible = HelpButton!.ButtonPressed;
    }

    public void OnNetworkButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnNetworkButtonPressed");
        NetworkWindowNode!.Visible = NetworkButton!.ButtonPressed;
    }

}
