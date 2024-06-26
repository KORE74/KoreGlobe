using Godot;
using System;

public partial class FssCameraMover : Node3D
{
    [Export]
    public float Speed = 0.200f; // Speed of the player

    [Export]
    Vector3 direction = new Vector3();

    public override void _Ready()
    {
        // Initialization code if needed
    }

    public override void _Process(double delta)
    {

        if (direction.Length() > 0)
        {
            // Normalize the direction vector to prevent faster diagonal movement
            direction = direction.Normalized();

            // Move the object
            Position += direction * Speed * (float)(delta);
        }
    }


    public override void _Input(InputEvent @event)
    {
        direction.Z = 0;
        direction.X = 0;
        direction.Y = 0;

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            // if the shift key is pressed:
            if (keyEvent.ShiftPressed)
            {
                GD.Print("Shift key is pressed");
                Speed = 0.400f;
            }
            else
            {
                Speed = 0.200f;
            }


            switch (keyEvent.Keycode)
            {
                case Key.W:
                    GD.Print(keyEvent.ShiftPressed ? "Shift+W was pressed" : "W was pressed");
                    direction.Z -= 1;
                    break;
                case Key.S:
                    GD.Print(keyEvent.ShiftPressed ? "Shift+S was pressed" : "S was pressed");
                    direction.Z += 1;
                    break;
                case Key.A:
                    GD.Print(keyEvent.ShiftPressed ? "Shift+A was pressed" : "A was pressed");
                    direction.X -= 1;
                    break;
                case Key.D:
                    GD.Print(keyEvent.ShiftPressed ? "Shift+D was pressed" : "D was pressed");
                    direction.X += 1;
                    break;
                case Key.R: // Y up
                    GD.Print(keyEvent.ShiftPressed ? "Shift+R was pressed" : "R was pressed");
                    direction.Y += 1;
                    break;
                case Key.F: // Y down
                    GD.Print(keyEvent.ShiftPressed ? "Shift+F was pressed" : "F was pressed");
                    direction.Y -= 1;
                    break;

            }
        }
    }

}
