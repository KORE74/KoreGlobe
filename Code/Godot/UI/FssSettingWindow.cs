using Godot;
using System;

public partial class FssSettingWindow : Window
{
    // Controls - Top to bottom
    Label    MapPathLabel;
    LineEdit MapPathLineEdit;

    Label    MeshCachePathLabel;
    LineEdit MeshCachePathLineEdit;

    Label    CapturePathLabel;
    LineEdit CapturePathLineEdit;

    Label    LanguageLabel;
    Button   LanguageNextButton;
    Label    ActiveLanguageLabel;
    Button   LanguagePrevButton;

    Label    MaxMapLvlLabel;
    Label    MaxMapLvlValueLabel;
    HSlider  MaxMapLvlSlider;
    Button   ToggleTileDetailsButton;

    Button   OkButton;
    Button   CancelButton;

    float    TimerUIUpdate = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        MapPathLabel            = (Label)FindChild("MapPathLabel");
        MapPathLineEdit         = (LineEdit)FindChild("MapPathLineEdit");

        MeshCachePathLabel      = (Label)FindChild("MeshCachePathLabel");
        MeshCachePathLineEdit   = (LineEdit)FindChild("MeshCachePathLineEdit");

        CapturePathLabel        = (Label)FindChild("CapturePathLabel");
        CapturePathLineEdit     = (LineEdit)FindChild("CapturePathLineEdit");

        MaxMapLvlLabel          = (Label)FindChild("MaxMapLvlLabel");
        MaxMapLvlValueLabel     = (Label)FindChild("MaxMapLvlValueLabel");
        MaxMapLvlSlider         = (HSlider)FindChild("MaxMapLvlSlider");
        ToggleTileDetailsButton = (Button)FindChild("ToggleTileDetailsButton");

        LanguageLabel           = (Label)FindChild("LanguageLabel");
        LanguageNextButton      = (Button)FindChild("LanguageNextButton");
        ActiveLanguageLabel     = (Label)FindChild("ActiveLanguageLabel");
        LanguagePrevButton      = (Button)FindChild("LanguagePrevButton");

        OkButton                = (Button)FindChild("OkButton");
        CancelButton            = (Button)FindChild("CancelButton");

        // If any of the controls are null, we have a code-vs-UI mismatch, so report this.
        if (MapPathLabel == null || MapPathLineEdit == null || CapturePathLabel == null || CapturePathLineEdit == null || OkButton == null || CancelButton == null)
        {
            FssCentralLog.AddEntry("FssSettingWindow: One or more controls not found");
            return;
        }

        LanguageNextButton.Connect("pressed", new Callable(this, "OnNextLanguageButtonPressed"));
        LanguagePrevButton.Connect("pressed", new Callable(this, "OnPrevLanguageButtonPressed"));

        MaxMapLvlSlider.Connect("value_changed", new Callable(this, "OnMaxMapLvlSliderValueChanged"));
        ToggleTileDetailsButton.Connect("pressed", new Callable(this, "OnToggleTileDetailsButtonPressed"));

        OkButton.Connect("pressed", new Callable(this, "OnOkButtonPressed"));
        CancelButton.Connect("pressed", new Callable(this, "OnCancelButtonPressed"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCancelButtonPressed"));

        UpdateUIText();
        WriteControlValues();
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
    // MARK: Data Read and Write
    // --------------------------------------------------------------------------------------------

    private void WriteControlValues()
    {
        MapPathLineEdit.Text       = FssCentralConfig.Instance.GetParam<string>("MapRootPath");
        MeshCachePathLineEdit.Text = FssCentralConfig.Instance.GetParam<string>("MeshCachePath");
        CapturePathLineEdit.Text   = FssCentralConfig.Instance.GetParam<string>("CapturePath");

        ActiveLanguageLabel.Text   = FssLanguageStrings.Instance.CurrActiveLanguage();

        MaxMapLvlValueLabel.Text   = FssMapManager.CurrMaxMapLvl.ToString();
        MaxMapLvlSlider.Value      = FssMapManager.CurrMaxMapLvl;

        ToggleTileDetailsButton.SetPressed(FssMapManager.ShowDebug);
    }

    private void SaveControlValues()
    {
        FssCentralConfig.Instance.SetParam("MapRootPath",   MapPathLineEdit.Text);
        FssCentralConfig.Instance.SetParam("MeshCachePath", MeshCachePathLineEdit.Text);
        FssCentralConfig.Instance.SetParam("CapturePath",   CapturePathLineEdit.Text);

        // Set the active language in FssLanguageStrings, it will pass this on to the config
        FssCentralConfig.Instance.SetParam("ActiveLanguage", FssLanguageStrings.Instance.CurrActiveLanguage());

        FssCentralConfig.Instance.WriteToFile();
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
        MaxMapLvlLabel.Text          = FssLanguageStrings.Instance.GetParam("MaxMapLvl");
        ToggleTileDetailsButton.Text = FssLanguageStrings.Instance.GetParam("TileInfo");

        OkButton.Text           = FssLanguageStrings.Instance.GetParam("Ok");
        CancelButton.Text       = FssLanguageStrings.Instance.GetParam("Cancel");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnOkButtonPressed");
        SaveControlValues();
        Visible = false;
    }

    public void OnCancelButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnCancelButtonPressed");

        WriteControlValues();
        Visible = false;
    }

    public void OnNextLanguageButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnNextLanguageButtonPressed");

        FssLanguageStrings.Instance.NextActiveLanguage();
        ActiveLanguageLabel.Text = FssLanguageStrings.Instance.CurrActiveLanguage();
    }

    public void OnPrevLanguageButtonPressed()
    {
        FssCentralLog.AddEntry("FssSettingWindow.OnPrevLanguageButtonPressed");

        FssLanguageStrings.Instance.PrevActiveLanguage();
        ActiveLanguageLabel.Text = FssLanguageStrings.Instance.CurrActiveLanguage();
    }

    public void OnMaxMapLvlSliderValueChanged(float value)
    {
        FssCentralLog.AddEntry($"FssSettingWindow.OnMaxMapLvlSliderValueChanged: {value}");

        // Assign (and write to config) the new max map level
        FssMapManager.SetMaxMapLvl((int)value);

        // Update the label
        MaxMapLvlValueLabel.Text = value.ToString();
    }

    public void OnToggleTileDetailsButtonPressed()
    {
        FssCentralLog.AddEntry($"FssSettingWindow.OnToggleTileDetailsButtonPressed: {ToggleTileDetailsButton.IsPressed()}");

        // Save the debug flag to config
        FssMapManager.SetDebug(ToggleTileDetailsButton.IsPressed());
    }
}
