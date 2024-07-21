using Godot;
using System;
using System.Timers;

public partial class FssWindowHandler : Window
{
    private Label           CommandResponseLabel;
    private LineEdit        CommandEntryEdit;
    private ScrollContainer ScrollContainer;

    private System.Timers.Timer LabelUpdateTimer;

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

        // Initialize and start the update timer
        LabelUpdateTimer = new System.Timers.Timer(1000); // 1 second interval
        LabelUpdateTimer.Elapsed += OnUpdateTimerElapsed;
        LabelUpdateTimer.Start();

        CallDeferred(nameof(ClearLabel));
        OnCommandSubmitted("version");
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

        GD.Print($"CLIWindow OnCommandSubmitted: {newText}");
        FssCentralLog.AddEntry($"CLIWindow OnCommandSubmitted: {newText}");


        if (newText == "clear" || newText == "cls")
        {
            CallDeferred(nameof(ClearLabel));
        }
        else
        {
            // Process the command
            //FssAppFactory.Instance.ConsoleInterface.Start();
            FssAppFactory.Instance.ConsoleInterface.AddInput(newText);
        }

        // Clear the CommandEntryEdit
        CallDeferred(nameof(ClearCommandEdit));
    }

    // --------------------------------------------------------------------------------------------

    // Functions perform actions on the labels. Have to be done on the main thread, which is what the CallDeferred function does.

    public void ClearCommandEdit()
    {
        CommandEntryEdit.Text = "";
    }

    public void ClearLabel()
    {
        CommandResponseLabel.Text = "";
        ScrollToBottom();
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

    public void UpdateLabelFromConsole()
    {
        while (FssAppFactory.Instance.ConsoleInterface.HasOutput())
        {
            string newContent = FssAppFactory.Instance.ConsoleInterface.GetOutput();
            GD.Print($"CLIWindow UpdateLabelFromConsole: {newContent}");
            UpdateLabel(newContent);
        }
    }

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = (int)(ScrollContainer.GetVScrollBar().MaxValue);
    }

    // Function to update the label periodically
    private void OnUpdateTimerElapsed(object sender, ElapsedEventArgs e)
    {
        CallDeferred(nameof(UpdateLabelFromConsole));
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        GD.Print("ToggleVisible");
    }
}
