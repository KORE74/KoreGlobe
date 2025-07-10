using Godot;
using System;
using System.Collections.Generic;

public partial class GloUISettingWindow : Window
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
    private bool ProcessOneShot = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Standard Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("GloUISettingWindow _Ready");

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
            GloCentralLog.AddEntry("GloUISettingWindow: One or more controls not found");
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
        //WriteControlValues();

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
        if (!ProcessOneShot)
        {
            ProcessOneShot = true;
            WriteControlValues();
        }

        if (TimerUIUpdate < GloCentralTime.RuntimeSecs)
        {
            TimerUIUpdate = GloCentralTime.RuntimeSecs + 1f;
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
        MapPathLineEdit.Text       = GloFileOperations.StandardizePath( GloCentralConfig.Instance.GetParam<string>("MapRootPath") );
        MeshCachePathLineEdit.Text = GloFileOperations.StandardizePath( GloCentralConfig.Instance.GetParam<string>("MeshCachePath") );
        CapturePathLineEdit.Text   = GloFileOperations.StandardizePath( GloCentralConfig.Instance.GetParam<string>("CapturePath") );
        LogPathLineEdit.Text       = GloFileOperations.StandardizePath( GloCentralConfig.Instance.GetParam<string>("LogPath") );
        DLCPathLineEdit.Text       = GloFileOperations.StandardizePath( GloCentralConfig.Instance.GetParam<string>("DlcPath") );

        ActiveLanguageLabel.Text   = GloLanguageStrings.Instance.CurrActiveLanguage();

        MaxMapLvlValueLabel.Text   = KoreZeroNodeMapManager.CurrMaxMapLvl.ToString();
        MaxMapLvlSlider.Value      = KoreZeroNodeMapManager.CurrMaxMapLvl;

        ToggleTileDetailsButton.SetPressed(KoreGodotFactory.Instance.UIState.ShowTileInfo);

        ToggleLogButton.SetPressed(GloCentralLog.LoggingActive);

        UpdateDlcList();
    }

    private void SaveControlValues()
    {
        // Extract the path from the control and correct any backslash characters
        string fixedMapRootPath   = GloFileOperations.StandardizePath(MapPathLineEdit.Text);
        string fixedMeshCachePath = GloFileOperations.StandardizePath(MeshCachePathLineEdit.Text);
        string fixedCapturePath   = GloFileOperations.StandardizePath(CapturePathLineEdit.Text);
        string fixedLogPath       = GloFileOperations.StandardizePath(LogPathLineEdit.Text);
        string fixedDLCPath       = GloFileOperations.StandardizePath(DLCPathLineEdit.Text);

        GloCentralConfig.Instance.SetParam("MapRootPath",   fixedMapRootPath);
        GloCentralConfig.Instance.SetParam("MeshCachePath", fixedMeshCachePath);
        GloCentralConfig.Instance.SetParam("CapturePath",   fixedCapturePath);
        GloCentralConfig.Instance.SetParam("LogPath",       fixedLogPath);
        GloCentralConfig.Instance.SetParam("DlcPath",       fixedDLCPath);

        // Assign (and write to config) the new max map level
        KoreZeroNodeMapManager.SetMaxMapLvl((int)MaxMapLvlSlider.Value);

        // Set the active language in GloLanguageStrings, it will pass this on to the config
        GloCentralConfig.Instance.SetParam("ActiveLanguage", GloLanguageStrings.Instance.CurrActiveLanguage());

        // Toggle the log window
        GloCentralLog.LoggingActive = ToggleLogButton.IsPressed();
        GloCentralConfig.Instance.SetParam("LoggingActive", GloCentralLog.LoggingActive);

        GloCentralConfig.Instance.WriteToFile();
    }

    private void UpdateDlcList()
    {
        // Clear the list
        DLCLoadedList.Clear();

        // // Get the list of files in the DLC path
        List<string> loadedDLCTitlesList = GloDlcOperations.ListLoadedDlcTitles();

        // Populate the list control
        foreach(string dlcName in loadedDLCTitlesList)
            DLCLoadedList.AddItem(dlcName);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        Title                        = GloLanguageStrings.Instance.GetParam("Settings");
        MapPathLabel.Text            = GloLanguageStrings.Instance.GetParam("MapPath");
        MeshCachePathLabel.Text      = GloLanguageStrings.Instance.GetParam("MeshCachePath");
        CapturePathLabel.Text        = GloLanguageStrings.Instance.GetParam("CapturePath");
        LogPathLabel.Text            = GloLanguageStrings.Instance.GetParam("LogPath");
        MaxMapLvlLabel.Text          = GloLanguageStrings.Instance.GetParam("MaxMapLvl");
        ToggleTileDetailsButton.Text = GloLanguageStrings.Instance.GetParam("TileInfo");
        LanguageLabel.Text           = GloLanguageStrings.Instance.GetParam("Language");
        DLCPathLabel.Text            = GloLanguageStrings.Instance.GetParam("DlcPath");
        DLCLoadedLabel.Text          = GloLanguageStrings.Instance.GetParam("DlcLoaded");

        OkButton.Text                = GloLanguageStrings.Instance.GetParam("Ok");
        CancelButton.Text            = GloLanguageStrings.Instance.GetParam("Cancel");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Dialog Buttons
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        GloCentralLog.AddEntry("GloUISettingWindow.OnOkButtonPressed");


        SaveControlValues();
        Visible = false;
    }

    public void OnCancelButtonPressed()
    {
        GloCentralLog.AddEntry("GloUISettingWindow.OnCancelButtonPressed");

        WriteControlValues();
        Visible = false;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Language
    // --------------------------------------------------------------------------------------------

    public void OnNextLanguageButtonPressed()
    {
        GloCentralLog.AddEntry("GloUISettingWindow.OnNextLanguageButtonPressed");

        GloLanguageStrings.Instance.NextActiveLanguage();
        ActiveLanguageLabel.Text = GloLanguageStrings.Instance.CurrActiveLanguage();
    }

    public void OnPrevLanguageButtonPressed()
    {
        GloCentralLog.AddEntry("GloUISettingWindow.OnPrevLanguageButtonPressed");

        GloLanguageStrings.Instance.PrevActiveLanguage();
        ActiveLanguageLabel.Text = GloLanguageStrings.Instance.CurrActiveLanguage();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions - Map Info
    // --------------------------------------------------------------------------------------------

    public void OnMaxMapLvlSliderValueChanged(float value)
    {
        GloCentralLog.AddEntry($"GloUISettingWindow.OnMaxMapLvlSliderValueChanged: {value}");

        // Update the label
        MaxMapLvlValueLabel.Text = value.ToString();
    }

    public void OnToggleTileDetailsButtonPressed()
    {
        bool flagState = ToggleTileDetailsButton.IsPressed();
        GloCentralLog.AddEntry($"GloUISettingWindow.OnToggleTileDetailsButtonPressed: {flagState}");

        // Save the debug flag to config
        // GloMapManager.SetDebug(flagState);
        KoreGodotFactory.Instance.UIState.UpdateTileInfo(flagState);
    }

    public void OnToggleLogButtonPressed()
    {
        GloCentralLog.AddEntry("GloUISettingWindow.OnToggleLogButtonPressed");
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
