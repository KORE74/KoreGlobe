using Godot;
using System;

public partial class FssCameraMover : Node3D
{
    [Export]
    public float MoveSpeed   = 0.6f; 
    public float RotateSpeed = 1.0f;

    [Export]
    Vector3 direction = new Vector3();
    Vector3 rotation  = new Vector3();

    public override void _Ready()
    {
        // Initialization code if needed
    }

    public override void _Process(double delta)
    {
        UpdateDirection();

        if (direction.Length() > 0)
        {
            // Normalize the direction vector to prevent faster diagonal movement
            direction = direction.Normalized();

            // Move the object
            Position += direction * MoveSpeed * (float)(delta);


            // Apply the movement to the basis of the object - moving relative to the object's current rotation
            // Basis = GlobalTransform.Basis;
            // Basis = Basis.Rotated(Vector3.Up, Mathf.DegToRad(rotation.Y));
            // Basis = Basis.Rotated(Vector3.Right, Mathf.DegToRad(rotation.X));
            // Basis = Basis.Rotated(Vector3.Forward, Mathf.DegToRad(rotation.Z));
            // Position += Basis.Xform(direction) * MoveSpeed * (float)delta;


            // Transform3D t = GlobalTransform;
            // t.origin += t.basis.Xform(direction) * MoveSpeed * (float)delta;

            // Apply the movement to the object's global transform - moving relative to the world's axes



            //Vector3 movement = (GlobalTransform.Basis.Xform(direction)) * MoveSpeed * (float)delta;
            //Position += movement;

        }
        if (rotation.Length() > 0) 
        {
            Rotation += rotation * RotateSpeed * (float)(delta);
        }
    }

    private void UpdateDirection()
    {
        direction = Vector3.Zero;
        rotation  = Vector3.Zero;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    direction.Z -= 1;
            if (Input.IsActionPressed("ui_down") ) direction.Z += 1;
        }
        if (Input.IsActionPressed("ui_alt"))
        {
            if (Input.IsActionPressed("ui_up"))    rotation.X += 2f;
            if (Input.IsActionPressed("ui_down"))  rotation.X -= 2f;
            if (Input.IsActionPressed("ui_left"))  rotation.Y += 2f;
            if (Input.IsActionPressed("ui_right")) rotation.Y -= 2f;
        }
        else 
        {
            if (Input.IsActionPressed("ui_left"))  direction.X -= 1;
            if (Input.IsActionPressed("ui_right")) direction.X += 1;
            if (Input.IsActionPressed("ui_up"))    direction.Y += 1;
            if (Input.IsActionPressed("ui_down"))  direction.Y -= 1;
        }
    }

}
