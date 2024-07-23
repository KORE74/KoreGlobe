using Godot;
using System;

public partial class FssNetworkWindow : Window
{
    Button OkButton;
    Button CancelButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        OkButton     = (Button)FindChild("OkButton");
        CancelButton = (Button)FindChild("CancelButton");

        OkButton.Connect("pressed", new Callable(this, "OnOkButtonPressed"));
        CancelButton.Connect("pressed", new Callable(this, "OnCancelButtonPressed"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCancelButtonPressed"));

        UpdateUIText();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        OkButton.Text = "OK-2";
        CancelButton.Text = "Cancel-2";
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Interactions
    // --------------------------------------------------------------------------------------------

    public void OnOkButtonPressed()
    {
        FssCentralLog.AddEntry("FssNetworkWindow.OnOkButtonPressed");
        Visible = false;
    }
    public void OnCancelButtonPressed()
    {
        FssCentralLog.AddEntry("FssNetworkWindow.OnCancelButtonPressed");
        Visible = false;
    }
}
