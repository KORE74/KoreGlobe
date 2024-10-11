using Godot;
using System;
using System.Collections.Generic;

public partial class FssUISettingWindow : Window
{
    // Map Control Section
    Label    MapPathLabel;
    LineEdit MapPathLineEdit;

    Label    MeshCachePathLabel;
    LineEdit MeshCachePathLineEdit;

    Label    MaxMapLvlLabel;
    Label    MaxMapLvlValueLabel;
    HSlider  MaxMapLvlSlider;
    Button   ToggleTileDetailsButton;

    // Capture & Log Section
    Label    CapturePathLabel;
    LineEdit CapturePathLineEdit;

    Label    LogPathLabel;
    LineEdit LogPathLineEdit;
    Button   ToggleLogButton;

    // Language Section
    Label    LanguageLabel;
    Button   LanguageNextButton;
    Label    ActiveLanguageLabel;
    Button   LanguagePrevButton;

    // DLC Section
    Label    DLCPathLabel;
    LineEdit DLCPathLineEdit;
    Label    DLCLoadedLabel;
    ItemList DLCLoadedList;
    Button   DLCRefreshButton;

    // Dialog Buttons
    Button   OkButton;
    Button   CancelButton;

    float    TimerUIUpdate = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Standard Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Map Control Section
        MapPathLabel            = (Label)FindChild("MapPathLabel");
        MapPathLineEdit         = (LineEdit)FindChild("MapPathLineEdit");

        MeshCachePathLabel      = (Label)FindChild("MeshCachePathLabel");
        MeshCachePathLineEdit   = (LineEdit)FindChild("MeshCachePathLineEdit");

        MaxMapLvlLabel          = (Label)FindChild("MaxMapLvlLabel");
        MaxMapLvlValueLabel     = (Label)FindChild("MaxMapLvlValueLabel");
        MaxMapLvlSlider         = (HSlider)FindChild("MaxMapLvlSlider");
        ToggleTileDetailsButton = (Button)FindChild("ToggleTileDetailsButton");

        // Capture & Log Section
        CapturePathLabel        = (Label)FindChild("CapturePathLabel");
        CapturePathLineEdit     = (LineEdit)FindChild("CapturePathLineEdit");

        LogPathLabel            = (Label)FindChild("LogPathLabel");
        LogPathLineEdit         = (LineEdit)FindChild("LogPathLineEdit");
        ToggleLogButton         = (Button)FindChild("ToggleLogButton");

        // Language Section
        LanguageLabel           = (Label)FindChild("LanguageLabel");
        LanguageNextButton      = (Button)FindChild("LanguageNextButton");
        ActiveLanguageLabel     = (Label)FindChild("ActiveLanguageLabel");
        LanguagePrevButton      = (Button)FindChild("LanguagePrevButton");

        // DLC Section
        DLCPathLabel            = (Label)FindChild("DLCPathLabel");
        DLCPathLineEdit         = (LineEdit)FindChild("DLCPathLineEdit");
        DLCLoadedLabel          = (Label)FindChild("DLCLoadedLabel");
        DLCLoadedList           = (ItemList)FindChild("DLCLoadedList");
        DLCRefreshButton        = (Button)FindChild("DLCRefreshButton");

        // Dialog Buttons
        OkButton                = (Button)FindChild("OkButton");
        CancelButton            = (Button)FindChild("CancelButton");

        // If any of the controls are null, we have a code-vs-UI mismatch, so report this.
        if (MapPathLabel == null || MapPathLineEdit == null || CapturePathLabel == null || CapturePathLineEdit == null || OkButton == null || CancelButton == null)
        {
            FssCentralLog.AddEntry("FssUISettingWindow: One or more controls not found");
            return;
        }

        // Wire up button pressed
        LanguageNextButton.Connect(     "pressed", new Callable(this, "OnNextLanguageButtonPressed"));
        LanguagePrevButton.Connect(     "pressed", new Callable(this, "OnPrevLanguageButtonPressed"));
        ToggleTileDetailsButton.Connect("pressed", new Callable(this, "OnToggleTileDetailsButtonPressed"));
        ToggleLogButton.Connect(        "pressed", new Callable(this, "OnToggleLogButtonPressed"));
        OkButton.Connect(               "pressed", new Callable(this, "OnOkButtonPressed"));
        CancelButton.Connect(           "pressed", new Callable(this, "OnCancelButtonPressed"));
        DLCRefreshButton.Connect(       "pressed", new Callable(this, "OnDLCRefreshButtonPressed"));

        MaxMapLvlSlider.Connect("value_changed", new Callable(this, "OnMaxMapLvlSliderValueChanged"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCancelButtonPressed"));

        UpdateUIText();
        WriteControlValues();

        // NOTE: Doesn't work. Kept as a reminder to try again later
        // Get the viewport and set the MSAA
        // GD.Print("Setting MSAA");
        // Viewport viewport = GetViewport();
        // viewport.SetMsaa3D(Viewport.Msaa.Msaa8X);
    }

    // --------------------------------------------------------------------------------------------

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
    // MARK: Data Read and Write
    // --------------------------------------------------------------------------------------------

    public void RefreshContent()
    {
        WriteControlValues();
    }

    private void WriteControlValues()
    {
        // Read the path values and standardize them before writing into the controls
        MapPathLineEdit.Text       = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("MapRootPath") );
        MeshCachePathLineEdit.Text = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("MeshCachePath") );
        CapturePathLineEdit.Text   = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("CapturePath") );
        LogPathLineEdit.Text       = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("LogPath") );
        DLCPathLineEdit.Text       = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("DlcPath") );

        ActiveLanguageLabel.Text   = FssLanguageStrings.Instance.CurrActiveLanguage();

        MaxMapLvlValueLabel.Text   = FssMapManager.CurrMaxMapLvl.ToString();
        MaxMapLvlSlider.Value      = FssMapManager.CurrMaxMapLvl;

        ToggleTileDetailsButton.SetPressed(FssMapManager.ShowDebug);

        ToggleLogButton.SetPressed(FssCentralLog.LoggingActive);

        UpdateDlcList();
    }

    private void SaveControlValues()
    {
        // Extract the path from the control and correct any backslash characters
        string fixedMapRootPath   = FssFileOperations.StandardizePath(MapPathLineEdit.Text);
        string fixedMeshCachePath = FssFileOperations.StandardizePath(MeshCachePathLineEdit.Text);
        string fixedCapturePath   = FssFileOperations.StandardizePath(CapturePathLineEdit.Text);
        string fixedLogPath       = FssFileOperations.StandardizePath(LogPathLineEdit.Text);
        string fixedDLCPath       = FssFileOperations.StandardizePath(DLCPathLineEdit.Text);

        FssCentralConfig.Instance.SetParam("MapRootPath",   fixedMapRootPath);
        FssCentralConfig.Instance.SetParam("MeshCachePath", fixedMeshCachePath);
        FssCentralConfig.Instance.SetParam("CapturePath",   fixedCapturePath);
        FssCentralConfig.Instance.SetParam("LogPath",       fixedLogPath);
        FssCentralConfig.Instance.SetParam("DlcPath",       fixedDLCPath);

        // Assign (and write to config) the new max map level
        FssMapManager.SetMaxMapLvl((int)MaxMapLvlSlider.Value);

        // Set the active language in FssLanguageStrings, it will pass this on to the config
        FssCentralConfig.Instance.SetParam("ActiveLanguage", FssLanguageStrings.Instance.CurrActiveLanguage());

        // Toggle the log window
        FssCentralLog.LoggingActive = ToggleLogButton.IsPressed();
        FssCentralConfig.Instance.SetParam("LoggingActive", FssCentralLog.LoggingActive);

        FssCentralConfig.Instance.WriteToFile();
    }

    private void UpdateDlcList()
    {
        // Clear the list
        DLCLoadedList.Clear();

        // // Get the list of files in the DLC path
        List<string> loadedDLCTitlesList = FssDlcOperations.ListLoadedDlcTitles();

        // Populate the list control
        foreach(string dlcName in loadedDLCTitlesList)
            DLCLoadedList.AddItem(dlcName);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        Title                        = FssLanguageStrings.Instance.GetParam("Settings");
        MapPathLabel.Text            = FssLanguageStrings.Instance.GetParam("MapPath");
        MeshCachePathLabel.Text      = FssLanguageStrings.Instance.GetParam("MeshCachePath");
        CapturePathLabel.Text        = FssLanguageStrings.Instance.GetParam("CapturePath");
        LogPathLabel.Text            = FssLanguageStrings.Instance.GetParam("LogPath");
        MaxMapLvlLabel.Text          = FssLanguageStrings.Instance.GetParam("MaxMapLvl");
        ToggleTileDetailsButton.Text = FssLanguageStrings.Instance.GetParam("TileInfo");
        LanguageLabel.Text           = FssLanguageStrings.Instance.GetParam("Language");
        DLCPathLabel.Text            = FssLanguageStrings.Instance.GetParam("DlcPath");
        DLCLoadedLabel.Text          = FssLanguageStrings.Instance.GetParam("DlcLoaded");

        OkButton.Text                = FssLanguageStrings.Instance.GetParam("Ok");
        CancelButton.Text            = FssLanguageStrings.Instance.GetParam("Cancel");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Dialog Buttons
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        FssCentralLog.AddEntry("FssUISettingWindow.OnOkButtonPressed");
        SaveControlValues();
        Visible = false;
    }

    public void OnCancelButtonPressed()
    {
        FssCentralLog.AddEntry("FssUISettingWindow.OnCancelButtonPressed");

        WriteControlValues();
        Visible = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Language
    // --------------------------------------------------------------------------------------------

    public void OnNextLanguageButtonPressed()
    {
        FssCentralLog.AddEntry("FssUISettingWindow.OnNextLanguageButtonPressed");

        FssLanguageStrings.Instance.NextActiveLanguage();
        ActiveLanguageLabel.Text = FssLanguageStrings.Instance.CurrActiveLanguage();
    }

    public void OnPrevLanguageButtonPressed()
    {
        FssCentralLog.AddEntry("FssUISettingWindow.OnPrevLanguageButtonPressed");

        FssLanguageStrings.Instance.PrevActiveLanguage();
        ActiveLanguageLabel.Text = FssLanguageStrings.Instance.CurrActiveLanguage();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Map Info
    // --------------------------------------------------------------------------------------------

    public void OnMaxMapLvlSliderValueChanged(float value)
    {
        FssCentralLog.AddEntry($"FssUISettingWindow.OnMaxMapLvlSliderValueChanged: {value}");

        // Update the label
        MaxMapLvlValueLabel.Text = value.ToString();
    }

    public void OnToggleTileDetailsButtonPressed()
    {
        FssCentralLog.AddEntry($"FssUISettingWindow.OnToggleTileDetailsButtonPressed: {ToggleTileDetailsButton.IsPressed()}");

        // Save the debug flag to config
        FssMapManager.SetDebug(ToggleTileDetailsButton.IsPressed());
    }

    public void OnToggleLogButtonPressed()
    {
        FssCentralLog.AddEntry("FssUISettingWindow.OnToggleLogButtonPressed");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - DLC
    // --------------------------------------------------------------------------------------------

    public void OnDLCRefreshButtonPressed()
    {
        // Unload existing DLC

        // Refresh DLC List
        UpdateDlcList();

    }
}
