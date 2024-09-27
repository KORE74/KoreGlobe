using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

using Godot;

#nullable enable

// Class to run a thread for command line input.
// This class can't do anything, it must delgate all actual processing to the FssEventDriver class.
public class FssConsole
{
    // Thread control
    private Thread? consoleThread = null;
    private bool running;

    // List of command handlers (with a signature) we can select and exeute.
    private readonly List<FssCommand> commandHandlers = new List<FssCommand>();

    // Link to the EventDriver, an entity with an interface to action changes within the application.
    public FssEventDriver? EventDriver;

    // Two lists to hold input and output strings for the console.
    private FssThreadsafeStringList InputQueue  = new FssThreadsafeStringList();
    private FssThreadsafeStringList OutputQueue = new FssThreadsafeStringList();

    // Event to set on new input, to unblock the console thread to process new commands.
    private AutoResetEvent InputEvent = new AutoResetEvent(false);

    // ---------------------------------------------------------------------------------------------
    // #MARK: Constructor
    // ---------------------------------------------------------------------------------------------

    public FssConsole()
    {
        GD.Print("FssConsole: Constructor");
        running = false;

        // Register command handlers
        InitializeCommands();
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Thread control
    // ---------------------------------------------------------------------------------------------

    public void Start()
    {
        if (running == false)
        {
            GD.Print("Starting console thread...");
            FssCentralLog.AddEntry($"Starting console thread... ({commandHandlers.Count} Commands.)");
            running = true;
            consoleThread = new Thread(ConsoleLoop);
            consoleThread?.Start();
        }
    }

    public void Stop()
    {
        running = false;
        consoleThread = null;
    }

    public void WaitForExit()
    {
        FssCentralLog.AddEntry("Waiting on Join()...");
        consoleThread?.Join(); // This will block until consoleThread finishes execution
        FssCentralLog.AddEntry("Join() returned.");
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: InitializeCommands
    // ---------------------------------------------------------------------------------------------

    private void InitializeCommands()
    {
        // Register commands and their handlers here
        FssCentralLog.AddEntry("FssConsole: Initializing commands...");

        // General app control commands
        commandHandlers.Add(new FssCommandVersion());
        commandHandlers.Add(new FssCommandExit());

        // Sim control
        commandHandlers.Add(new FssCommandSimClock());
        commandHandlers.Add(new FssCommandSimStart());
        commandHandlers.Add(new FssCommandSimStop());
        commandHandlers.Add(new FssCommandSimReset());
        commandHandlers.Add(new FssCommandSimPause());
        commandHandlers.Add(new FssCommandSimResume());

        // Platform Rreport
        commandHandlers.Add(new FssCommandPlatReportElem());
        commandHandlers.Add(new FssCommandPlatReportPos());

        //commandHandlers.Add(new FssCommandModelJsonRead());
        //commandHandlers.Add(new FssCommandModelJsonWrite());

        // Platform control
        commandHandlers.Add(new FssCommandPlatTestScenario());
        commandHandlers.Add(new FssCommandPlatAdd());
        commandHandlers.Add(new FssCommandPlatDelete());

        // Platform details
        commandHandlers.Add(new FssCommandPlatPosition());
        commandHandlers.Add(new FssCommandPlatCourse());
        commandHandlers.Add(new FssCommandPlatCourseDelta());

        // Network
        commandHandlers.Add(new FssCommandNetworkReport());
        commandHandlers.Add(new FssCommandNetworkInjectIncoming());

        // MapServer
        commandHandlers.Add(new FssCommandElePrep());
        commandHandlers.Add(new FssCommandEleLoadArc());
        commandHandlers.Add(new FssCommandEleSaveTile());
        commandHandlers.Add(new FssCommandEleForPos());
        commandHandlers.Add(new FssCommandEleReport());


        // commandHandlers.Add("help", CmdHelp);
        // commandHandlers.Add("version", CmdVersion);
        // commandHandlers.Add("#", CmdComment);
        // commandHandlers.Add("runfile", CmdRunFile);

        // // MapUtils
        // commandHandlers.Add("tilecode", CmdTileCode);

        // // Map handlers
        // commandHandlers.Add("setroot", CmdSetRoot);
        // commandHandlers.Add("getroot", CmdGetRoot);
        // commandHandlers.Add("create", CmdCreate);

        // // Networking
        // commandHandlers.Add("network", CmdNetwork);
    }

    private void ConsoleLoop()
    {
        FssCentralLog.AddEntry("Console thread starting...");

        GD.Print("Console thread starting...");

        while (running)
        {
            // Wait for input trigger
            InputEvent.WaitOne();
            ProcessCommand();
        }
        FssCentralLog.AddEntry("Console thread exiting...");
    }

    private void ProcessCommand()
    {
        // Loop while we have commands to process
        while (!InputQueue.IsEmpty())
        {
            // Get the string a space-delimit the parts
            string inputLine = InputQueue.RetrieveString();
            //var inputParts = inputLine.Trim().Split(' ').ToList();

            // Split the input line into parts, removing any empty entries
            var inputParts = inputLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Check for internal commands - true if executed.
            if (RunInternalCommand(inputParts))
                continue;

            // Go through each of the registered command handlers looking for a match
            foreach (var currCmd in commandHandlers)
            {
                if (currCmd.Matches(inputParts))
                {
                    // Pass remaining parts as parameters to the command
                    string responseStr = currCmd.Execute(inputParts.Skip(currCmd.SignatureCount).ToList());
                    OutputQueue.AddString(responseStr);
                    break; // leave the current command loop - move back out to the next input line
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Input
    // ---------------------------------------------------------------------------------------------

    public void AddInput(string input)
    {
        InputQueue.AddString(input);
        InputEvent.Set();
    }

    // ---------------------------------------------------------------------------------------------
    // #MARK: Output
    // ---------------------------------------------------------------------------------------------

    public bool HasOutput()
    {
        return !OutputQueue.IsEmpty();
    }

    public string GetOutput()
    {
        StringBuilder output = new StringBuilder();

        while (!OutputQueue.IsEmpty())
            output.AppendLine(OutputQueue.RetrieveString());

        return output.ToString();
    }

    // ---------------------------------------------------------------------------------------------
    // Command functions:
    // ---------------------------------------------------------------------------------------------
    // - private void Cmd<Name>(string[] args)

    private bool RunInternalCommand(List<string> inputParts)
    {
        if (inputParts.Count == 0)
            return false;

        string command = inputParts[0];

        switch (command)
        {
            case "help":
                {
                    StringBuilder helpStr = new StringBuilder();
                    helpStr.AppendLine("Available commands:");
                    foreach (var cmd in commandHandlers)
                    {
                        helpStr.AppendLine($"- {cmd.HelpString}");
                    }
                    OutputQueue.AddString(helpStr.ToString());
                    return true;
                }

            case "runfile":
                {
                    if (inputParts.Count < 2)
                    {
                        OutputQueue.AddString("Usage: runfile <filename>");
                        return true;
                    }

                    string filename = inputParts[1];

                    if (!System.IO.File.Exists(filename))
                    {
                        OutputQueue.AddString($"File does not exist: {filename}");
                        return true;
                    }

                    OutputQueue.AddString($"Running file: {filename}");

                    string[] lines = System.IO.File.ReadAllLines(filename);

                    foreach (string line in lines)
                    {
                        // trim line to 100 characters
                        if (line.Length > 100)
                        {
                            string shortLine = line.Substring(0, 100);
                            shortLine += "...";
                            OutputQueue.AddString($"FILE>> {shortLine}");
                        }
                        else
                        {
                            OutputQueue.AddString($"FILE>> {line}");
                        }

                        InputQueue.AddString(line);
                    }
                    return true;
                }

            default:
                return false;
        }
    }
}

