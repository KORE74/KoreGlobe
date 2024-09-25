using Godot;
using System;

// Camera3D with polar offset movement, rotating around a position.

public partial class FssCameraPolarOffset : Node3D
{
    // Cam position
    private Vector3 CamRotation  = new Vector3();
    private float   CamOffsetDist = 1f;

    // Camera node - offset by an amount in a fixed axis, atop the rotation of this node.
    public Camera3D CamNode;

    // UI Update
    private float   TimerReport = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D - Ready / Process
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialization code if needed
        CamNode = new Camera3D() { Name = "CamNode" };
        AddChild(CamNode);

        CamNode.Near = 0.001f;
        CamNode.Far  = 4000f;
        CamNode.Fov  = 40f;

        Name = "FssCameraPolarOffset";
    }

    public override void _Process(double delta)
    {
        UpdateInputs();

        // Update the camera position
        CamNode.Position = new Vector3(0, CamOffsetDist, 0);
    }

    // Process inputs
    public override void _Input(InputEvent @event)
    {
        float distanceDelta        = CamOffsetDist;
        float angleDeltaDegsPerSec = 1f;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    CamRotation.Z += angleDeltaDegsPerSec;
            if (Input.IsActionPressed("ui_down") ) CamRotation.Z -= angleDeltaDegsPerSec;
        }
        else
        {
            if (Input.IsActionPressed("ui_up"))    CamOffsetDist -= distanceDelta;
            if (Input.IsActionPressed("ui_down"))  CamOffsetDist += distanceDelta;
            if (Input.IsActionPressed("ui_left"))  CamRotation.Y += angleDeltaDegsPerSec;
            if (Input.IsActionPressed("ui_right")) CamRotation.Y -= angleDeltaDegsPerSec;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - ChaseCam
    // --------------------------------------------------------------------------------------------

    // Update the inputs for the camera
    // - Left / right:      Update the azimuth
    // - Up / down:         Update the offset distance (radius)
    // - Shift + up / down: Update the elevation
    // - Alt:               Amplify any movement
    // - Ctrl:              Slow down any movement

    // Additionally radius distance changes proportionally, so large movements can be made at distance

    private void UpdateInputs()
    {
        float distanceDelta        = CamOffsetDist;
        float angleDeltaDegsPerSec = 1f;

        // Check for Alt/Ctrl to update move amounts
        // if (Input.IsActionPressed("ui_alt"))
        // {
        //     distanceDelta        *= 10;
        //     angleDeltaDegsPerSec *= 10;
        // }
        // if (Input.IsActionPressed("ui_ctrl"))
        // {
        //     distanceDelta        /= 10;
        //     angleDeltaDegsPerSec /= 10;
        // }

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    CamRotation.Z += angleDeltaDegsPerSec;
            if (Input.IsActionPressed("ui_down") ) CamRotation.Z -= angleDeltaDegsPerSec;
        }
        else
        {
            if (Input.IsActionPressed("ui_up"))    CamOffsetDist -= distanceDelta;
            if (Input.IsActionPressed("ui_down"))  CamOffsetDist += distanceDelta;
            if (Input.IsActionPressed("ui_left"))  CamRotation.Y += angleDeltaDegsPerSec;
            if (Input.IsActionPressed("ui_right")) CamRotation.Y -= angleDeltaDegsPerSec;
        }
    }
}
