using Godot;
using System;

#nullable enable

public partial class FssUIHeader : PanelContainer
{
    private Window CliWindowNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FssCentralLog.AddEntry("FssUIHeader._Ready");
        // Get the child element called "Exit-IconButton" from the hierachy underneath the current node

        Button exitButton = (Button)FindChild("Exit-IconButton");
        Button cliToggle =  (Button)FindChild("CLI-IconToggleButton");

        CliWindowNode = GetNode<Window>("../../../../CLIWindow");

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



    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    //
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

        CliWindowNode.Visible = !CliWindowNode.Visible;
    }

}
