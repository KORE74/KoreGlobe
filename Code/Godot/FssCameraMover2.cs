using Godot;
using System;

public partial class FssCameraMover2 : Camera3D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Output a message to the internal logging system
        FssCentralLog.AddStartupEntry(FssGlobals.VersionString);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
