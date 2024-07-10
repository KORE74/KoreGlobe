using Godot;
using System;

public partial class FssWindowHandler : Window
{
    private Label           CommandResponseLabel;
    private LineEdit        CommandEntryEdit;
    private ScrollContainer ScrollContainer;

    public override void _Ready()
    {
        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCloseRequested"));

        // Get references to the Label, LineEdit, and ScrollContainer
        CommandResponseLabel = GetNode<Label>("CLIWindowLayout/ScrollContainer/CommandResponseLabel");
        CommandEntryEdit     = GetNode<LineEdit>("CLIWindowLayout/CommandEntryEdit");
        ScrollContainer      = GetNode<ScrollContainer>("CLIWindowLayout/ScrollContainer");

        // Connect the text_submitted signal of the LineEdit to the OnCommandSubmitted function
        CommandEntryEdit.Connect("text_submitted", new Callable(this, "OnCommandSubmitted"));

        UpdateLabel("Welcome to the FSS Command Line Interface!");
    }

    // Function to handle the close_requested signal
    private void OnCloseRequested()
    {
        // Hide the window
        Hide();
    }

    // Function to handle the text_submitted signal of the LineEdit
    private void OnCommandSubmitted(string newText)
    {
        // Append the new text to the CommandResponseLabel
        UpdateLabel(newText);

        FssCentralLog.AddEntry($"CLIWindow OnCommandSubmitted: {newText}");

        // Clear the CommandEntryEdit
        CommandEntryEdit.Clear();
        CommandEntryEdit.Text = "";
    }

    // Function to programmatically update the label
    public void UpdateLabel(string newText)
    {
        // Append the new text to the CommandResponseLabel
        CommandResponseLabel.Text += "\n" + newText;

        // Scroll to the bottom of the ScrollContainer using a deferred call
        CallDeferred(nameof(ScrollToBottom));
        // ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = (int)(ScrollContainer.GetVScrollBar().MaxValue);
    }
}
