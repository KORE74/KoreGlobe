using Godot;
using System;

#nullable enable

public partial class GloUIHelpWindow : Window
{
    private Label? AboutLabel;

    // --------------------------------------------------------------------------------------------
    // MARK: Standard Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Map Control Section
        AboutLabel = (Label)FindChild("AboutLabel");

        if (AboutLabel != null)
        {
            AboutLabel.Text = KoreGlobals.VersionString;
        }
    }

    // --------------------------------------------------------------------------------------------

    public override void _Process(double delta)
    {

    }

    // --------------------------------------------------------------------------------------------

}
