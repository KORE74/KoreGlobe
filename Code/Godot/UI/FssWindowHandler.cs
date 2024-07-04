
using Godot;
using System;

public partial class FssWindowHandler : Window
{
    public override void _Ready()
    {
        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCloseRequested"));
    }

    // Function to handle the close_requested signal
    private void OnCloseRequested()
    {
        // Hide the window
        Hide();
    }
}
