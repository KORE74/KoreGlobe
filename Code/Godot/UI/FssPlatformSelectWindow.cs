using System;
using System.Collections.Generic;

using Godot;

// A popup window containing a list of the currently loaded platforms. The selection action records the selection,
// with an action for the cller that opened the popup to read the selected item.

public partial class FssPlatformSelectWindow : Window
{
    ItemList PlatformList;
    Button   SelectButton;
    Button   CancelButton;

    public int SelectedPlatform = -1;

    // --------------------------------------------------------------------------------------------
    // MARK: Standard Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Setup access to the controls
        PlatformList = (ItemList)FindChild("PlatformList");
        SelectButton = (Button)FindChild("SelectButton");
        CancelButton = (Button)FindChild("CancelButton");

        // Setup button events
        SelectButton.Connect("pressed", new Callable(this, "OnSelectButtonPressed"));
        CancelButton.Connect("pressed", new Callable(this, "OnCancelButtonPressed"));

        // Write the control values
        WriteControlValues();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Data Read and Write
    // --------------------------------------------------------------------------------------------
    private void WriteControlValues()
    {
        // Read the internal platform list and write the names into the PlatformList control
        PlatformList.Clear();

        // Add the platform names
        List<string> platformNames = FssAppFactory.Instance.EventDriver.PlatformNames();

        // Setup a debug list of entries
        platformNames.Add("Platform 1");
        platformNames.Add("Platform 2");
        platformNames.Add("Platform 3");

        foreach (string platformName in platformNames)
        {
            PlatformList.AddItem(platformName);
        }
    }

    private void SaveControlValues()
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        Title             = FssLanguageStrings.Instance.GetParam("SelectPlatform");
        SelectButton.Text = FssLanguageStrings.Instance.GetParam("Select");
        CancelButton.Text = FssLanguageStrings.Instance.GetParam("Cancel");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    private void OnSelectButtonPressed()
    {
        // Record the selected platform
        //SelectedPlatform = PlatformList.GetSelectedId();

        // Close the window
        Hide();
    }

    private void OnCancelButtonPressed()
    {
        // Close the window
        Hide();
    }
}
