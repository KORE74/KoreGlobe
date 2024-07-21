using Godot;
using System;

#nullable enable

public partial class FssUIHeader : PanelContainer
{
    private FssWindowHandler? CliWindowNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddEntry("FssUIHeader._Ready");
        // Get the child element called "Exit-IconButton" from the hierachy underneath the current node

        Button exitButton = (Button)FindChild("Exit-IconButton");

        if (exitButton == null)
        {
            GD.Print("Exit-IconButton not found");
            return;
        }

        exitButton.Connect("pressed", new Callable(this, "OnExitButtonPressed"));

        // -------------------

        // Find the button and window controls
        Button cliButton  = (Button)FindChild("CLI-IconToggleButton");
        cliButton.Connect("pressed", new Callable(this, "OnCLIToggleButtonPressed"));


        CliWindowNode = (FssWindowHandler)FssAppNode.Instance.FindNode("CLIWindow");

        // Button exitButton = GetNode<Button>("Exit-IconButton");


        // Connect the button's "pressed" signal to the current node's "OnExitButtonPressed" method
        // CommandEntryEdit.Connect("text_submitted", new Callable(this, "OnCommandSubmitted"));

        if (CliWindowNode == null)
        {
            GD.Print("CLI-IconToggleButton not found");
        }


        // cliButton.Visible = !cliButton.Visible;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // Called when the "Exit-IconButton" button is pressed
    public void OnExitButtonPressed()
    {
        // Get the current scene
        SceneTree sceneTree = GetTree();

        // Quit the game
        sceneTree.Quit();
    }

    public void OnCLIToggleButtonPressed()
    {
        FssCentralLog.AddEntry("FssUIHeader.OnCLIToggleButtonPressed");
        GD.Print("OnCLIToggleButtonPressed");
    //     if (CliWindowNode == null)
    //     {
    //         GD.Print("CLIWindow not found");
    //         return;

        CliWindowNode!.ToggleVisibility();
    }




    // }

}
