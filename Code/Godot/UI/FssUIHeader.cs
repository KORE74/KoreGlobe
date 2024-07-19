using Godot;
using System;

public partial class FssUIHeader : PanelContainer
{
   // Node CliWindowNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get the child element called "Exit-IconButton" from the hierachy underneath the current node

        Button exitButton = (Button)FindChild("Exit-IconButton");
        Button cliButton  = (Button)FindChild("CLI-IconToggleButton");

        //CliWindowNode = FssAppNode.Instance.FindNode("CLIWindow");

        //Button exitButton = GetNode<Button>("Exit-IconButton");

        if (exitButton == null)
        {
            GD.Print("Exit-IconButton not found");
            return;
        }

        // Connect the button's "pressed" signal to the current node's "OnExitButtonPressed" method
        exitButton.Connect("pressed", new Callable(this, "OnExitButtonPressed"));
        //CommandEntryEdit.Connect("text_submitted", new Callable(this, "OnCommandSubmitted"));

        // if (CliWindowNode == null)
        // {
        //     GD.Print("CLI-IconToggleButton not found");
        //     return;
        // }

        //cliButton.Connect("pressed", new Callable(this, "OnCLIToggleButtonPressed"));

        //cliButton.Visible = !cliButton.Visible;
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

    // public void OnCLIToggleButtonPressed()
    // {
    //     if (CliWindowNode == null)
    //     {
    //         GD.Print("CLIWindow not found");
    //         return;
    //     }




    // }

}
