using Godot;
using System;
using System.Timers;
using System.Text;
using System.Collections.Generic;

public partial class FssUICliWindow : Window
{
    // CLI tab
    private Label           CommandResponseLabel;
    private LineEdit        CommandEntryEdit;

    // Log tab
    private ScrollContainer ScrollContainer;
    private Label           LogLabel;
    private string          LogLines = "";

    // Report tab
    private CodeEdit        ReportEdit;
    private Button          AppReportButton;
    private Button          DataReportButton;
    private Button          ClipboardButton;

    private System.Timers.Timer LabelUpdateTimer;

    float TimerUIUpdate = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Get references to the Label, LineEdit, and ScrollContainer

        // CLI tab
        CommandResponseLabel = (Label)FindChild("CommandResponseLabel");
        CommandEntryEdit     = (LineEdit)FindChild("CommandEntryEdit");

        // Log tab
        ScrollContainer      = (ScrollContainer)FindChild("ScrollContainer");
        LogLabel             = (Label)FindChild("LogLabel");

        // Report tab
        ReportEdit           = (CodeEdit)FindChild("ReportEdit");
        AppReportButton      = (Button)FindChild("AppReportButton");
        DataReportButton     = (Button)FindChild("DataReportButton");
        ClipboardButton      = (Button)FindChild("ClipboardButton");

        // Connect the text_submitted signal of the LineEdit to the OnCommandSubmitted function
        CommandEntryEdit.Connect("text_submitted", new Callable(this, "OnCommandSubmitted"));

        // Connect the close_requested signal to the OnCloseRequested function
        Connect("close_requested", new Callable(this, "OnCloseRequested"));

        AppReportButton.Connect("pressed", new Callable(this, "OnAppReportButtonPressed"));
        DataReportButton.Connect("pressed", new Callable(this, "OnDataReportButtonPressed"));
        ClipboardButton.Connect("pressed", new Callable(this, "OnClipboardButtonPressed"));

        // Initialize and start the update timer
        LabelUpdateTimer = new System.Timers.Timer(1000); // 1 second interval
        LabelUpdateTimer.Elapsed += OnUpdateTimerElapsed;
        LabelUpdateTimer.Start();

        ClearLabel();
        OnCommandSubmitted("version");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerUIUpdate < FssCentralTime.RuntimeSecs)
        {
            TimerUIUpdate = FssCentralTime.RuntimeSecs + 2f; // Update the timer to the next whole second
            UpdateUIText();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Localisation
    // --------------------------------------------------------------------------------------------

    private void UpdateUIText()
    {
        Title                    = FssLanguageStrings.Instance.GetParam("CommandLineAndLogging");

        AppReportButton.Text     = FssLanguageStrings.Instance.GetParam("AppReport");
        DataReportButton.Text    = FssLanguageStrings.Instance.GetParam("DataReport");
        ClipboardButton.Text     = FssLanguageStrings.Instance.GetParam("CopyToClipboard");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Log
    // --------------------------------------------------------------------------------------------

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = (int)(ScrollContainer.GetVScrollBar().MaxValue);
    }

    // Function to update the label periodically
    private void OnUpdateTimerElapsed(object sender, ElapsedEventArgs e)
    {
        CallDeferred(nameof(UpdateConsoleLabel));

        StringBuilder logSB = new StringBuilder();

        // Update the string builder in the timer, just to take anything off the main thread
        List<string> lines = FssCentralLog.GetLatestLines();
        foreach (string line in lines)
            logSB.AppendLine(line);

        // Update the label on the main thread
        LogLines = logSB.ToString();
        CallDeferred(nameof(UpdateLogLabel));
    }

    private void UpdateLogLabel()
    {
        LogLabel.Text = LogLines;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - CLI
    // --------------------------------------------------------------------------------------------

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

    public void UpdateConsoleLabel()
    {
        while (FssAppFactory.Instance.ConsoleInterface.HasOutput())
        {
            string newContent = FssAppFactory.Instance.ConsoleInterface.GetOutput();
            //GD.Print($"CLIWindow UpdateLabelFromConsole: {newContent}");
            UpdateLabel(newContent);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Report
    // --------------------------------------------------------------------------------------------

    // Create a report for everything in the application, split into sections

    private void OnAppReportButtonPressed()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Application Report");
        sb.AppendLine("===================");

        // Add the time and version info
        sb.AppendLine($"Time: {DateTime.Now}");
        sb.AppendLine($"Version: {FssFssbals.VersionString}");

        // Add the network report
        sb.AppendLine();
        sb.AppendLine("Network Report");
        sb.AppendLine("--------------");
        sb.Append(FssEventDriver.NetworkReport());

        // Add the DLC report
        sb.AppendLine();
        sb.AppendLine("DLC Report");
        sb.AppendLine("----------");
        sb.Append(FssDlcOperations.DlcReport());

        // Add a report for the 3D models
        sb.AppendLine();
        sb.AppendLine("3D Model Report");
        sb.AppendLine("---------------");
        sb.Append(FssGodotFactory.Instance.ModelLibrary.ReportContent());

        // Texture report
        sb.AppendLine();
        sb.AppendLine("Texture Report");
        sb.AppendLine("--------------");
        sb.Append(FssTextureLoader.Instance.TextureCacheList());

        ReportEdit.Text = sb.ToString();
    }

    private void OnDataReportButtonPressed()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Data Report");
        sb.AppendLine("===========");

        // Add the time and version info
        sb.AppendLine($"Time: {DateTime.Now}");
        sb.AppendLine($"Version: {FssFssbals.VersionString}");

        // Model report
        sb.AppendLine();
        sb.AppendLine("Model Report");
        sb.AppendLine("------------");
        sb.Append(FssAppFactory.Instance.EntityManager.FullReport());

        // Godot Entity report
        //sb.AppendLine();
        //sb.AppendLine("Godot Entity Report");
        //sb.AppendLine("-------------------");
        //sb.Append(FssGodotFactory.Instance.GodotEntityManager.FullReport());

        ReportEdit.Text = sb.ToString();
    }

    private void OnClipboardButtonPressed()
    {
        DisplayServer.ClipboardSet(ReportEdit.Text);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: UI Actions - Misc
    // --------------------------------------------------------------------------------------------

    // Function to handle the close_requested signal
    private void OnCloseRequested()
    {
        // Hide the window
        Hide();
    }

    public void ToggleVisibility() => Visible = !Visible;

}
