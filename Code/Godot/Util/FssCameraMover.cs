using Godot;
using System;

public partial class FssCameraMover : Camera3D
{
    [Export]
    public float MoveSpeed   = (float)(FssZeroOffset.GeEarthRadius / 0.75); // Tunable speed parameters

    public float RotateSpeed = 1.0f;
    Vector3 CamDirection = new Vector3();
    Vector3 CamRotation  = new Vector3();

    float TimerReport = 0.0f;

    public override void _Ready()
    {
        // Initialization code if needed
    }

    public override void _Process(double delta)
    {
        UpdateInputs();

        if (CamDirection.Length() > 0)
        {
            // Normalize the CamDirection vector to prevent faster diagonal movement
            CamDirection = CamDirection.Normalized();

            // Transform the CamDirection from local space to global space
            Vector3 forward = -Transform.Basis.Z;
            Vector3 right   =  Transform.Basis.X;
            Vector3 up      =  Transform.Basis.Y;
            Vector3 globalDirection = (forward * CamDirection.Z) + (right * CamDirection.X) + (up * CamDirection.Y);

            // Move the object
            Position += globalDirection * MoveSpeed * (float)(delta);
        }
        if (CamRotation.Length() > 0)
        {
            Rotation += CamRotation * RotateSpeed * (float)(delta);
        }

        // If the node is a camera, update the FssMapManager.LoadRefLLA
        if (this is Camera3D)
        {
            Camera3D camNode = (Camera3D)this;
            if (camNode.IsCurrent())
                FssMapManager.LoadRefLLA = FssGeoConvOperations.GeToRw(Position);
        }

        MoveSpeed = (float)(FssValueUtils.LimitToRange(FssMapManager.LoadRefLLA.AltMslKm / 80, 0.02, 5000));

        if (TimerReport < FssCentralTime.RuntimeSecs)
        {
            TimerReport = FssCentralTime.RuntimeSecs + 1f;

            // GD.Print($"LoadRefLLA: {FssMapManager.LoadRefLLA} // MoveSpeed: {MoveSpeed}");
        }
    }

    private void UpdateInputs()
    {
        CamDirection = Vector3.Zero;
        CamRotation  = Vector3.Zero;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    CamDirection.Z += 1;
            if (Input.IsActionPressed("ui_down") ) CamDirection.Z -= 1;
        }
        else if (Input.IsActionPressed("ui_alt"))
        {
            if (Input.IsActionPressed("ui_up"))    CamRotation.X += 2f;
            if (Input.IsActionPressed("ui_down"))  CamRotation.X -= 2f;
            if (Input.IsActionPressed("ui_left"))  CamRotation.Y += 2f;
            if (Input.IsActionPressed("ui_right")) CamRotation.Y -= 2f;
        }
        else
        {
            if (Input.IsActionPressed("ui_left"))  CamDirection.X -= 1;
            if (Input.IsActionPressed("ui_right")) CamDirection.X += 1;
            if (Input.IsActionPressed("ui_up"))    CamDirection.Y += 1;
            if (Input.IsActionPressed("ui_down"))  CamDirection.Y -= 1;
        }
    }
}
