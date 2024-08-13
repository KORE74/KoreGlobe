using Godot;
using System;

public partial class FssCameraMover : Node3D
{
    [Export]
    public float MoveSpeed   = (float)(FssZeroOffset.GeEarthRadius / 0.75); // Tunable speed parameters

    public float RotateSpeed = 1.0f;
    Vector3 direction = new Vector3();
    Vector3 rotation  = new Vector3();

    float TimerReport = 0.0f;

    public override void _Ready()
    {
        // Initialization code if needed
    }

    public override void _Process(double delta)
    {
        UpdateInputs();

        if (direction.Length() > 0)
        {
            // Normalize the direction vector to prevent faster diagonal movement
            direction = direction.Normalized();

            // Transform the direction from local space to global space
            Vector3 forward = -Transform.Basis.Z;
            Vector3 right   =  Transform.Basis.X;
            Vector3 up      =  Transform.Basis.Y;
            Vector3 globalDirection = (forward * direction.Z) + (right * direction.X) + (up * direction.Y);

            // Move the object
            Position += globalDirection * MoveSpeed * (float)(delta);
        }
        if (rotation.Length() > 0)
        {
            Rotation += rotation * RotateSpeed * (float)(delta);
        }

        FssMapManager.LoadRefLLA = FssGeoConvOperations.GeToRw(Position);

        if (TimerReport < FssCoreTime.RuntimeSecs)
        {
            TimerReport = FssCoreTime.RuntimeSecs + 1f;

            GD.Print($"LoadRefLLA: {FssMapManager.LoadRefLLA}");
        }

    }

    private void UpdateInputs()
    {
        direction = Vector3.Zero;
        rotation  = Vector3.Zero;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    direction.Z += 1;
            if (Input.IsActionPressed("ui_down") ) direction.Z -= 1;
        }
        else if (Input.IsActionPressed("ui_alt"))
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
