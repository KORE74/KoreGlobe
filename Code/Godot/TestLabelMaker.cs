using Godot;
using System.Collections.Generic;

public partial class TestLabelMaker : Node3D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Create a new Label from the FssLabelMaker util class
        Texture2D labelTexture = FssLabelMaker.CreateLabelTexture("Hello, World!");

    }


}