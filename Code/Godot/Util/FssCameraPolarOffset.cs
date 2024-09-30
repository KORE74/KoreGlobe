using Godot;
using System;

// Camera3D with polar offset movement, rotating around a position.

public partial class FssCameraPolarOffset : Node3D
{
    // Cam position
    private Vector3 CamRotation  = new Vector3();
    private float   CamOffsetDist = 1f;
    private float   camAzAngleDegs = 0f;
    private float   camElAngleDegs = 0f;

    // Camera node - offset by an amount in a fixed axis, atop the rotation of this node.
    public Camera3D CamNode;
    private Fss1DMappedRange CamFOVForDist = new Fss1DMappedRange();

    // UI Update
    private float TimerReport = 0.0f;
    private float GeDeltaTime = 0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D - Ready / Process
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialization code if needed
        CamNode = new Camera3D() { Name = "CamNode" };
        AddChild(CamNode);

        CamFOVForDist.AddEntry( 0.001f,  5f);
        CamFOVForDist.AddEntry( 0.100f, 50f);

        CamNode.Near = 0.001f;
        CamNode.Far  = 4000f;
        CamNode.Fov  = 40f;

        // Set the camera position
        CamOffsetDist    = (float)(100 * FssZeroOffset.RwToGeDistanceMultiplierM); // set to 100m
        CamRotation      = new Vector3(0, 0, 0);
        // Apply position and rotation
        CamNode.Position = new Vector3(0, 0, CamOffsetDist);
        this.Rotation = new Vector3(CamRotation.X, CamRotation.Y, CamRotation.Z);

        Name = "FssCameraPolarOffset";
    }

    public override void _Process(double delta)
    {
        // Keep the delta time ticking along, in case we need to update the camera. Will remove potential jumps.
        GeDeltaTime = (float)delta;

        // Only process the camera if the camera is active
        if (CamNode == null) return;
        if (!CamNode.Current) return;

        // We have the Az El and Distance. Apply these with the right sign and Vector3.
        this.RotationDegrees = new Vector3(-camElAngleDegs, -camAzAngleDegs, 0);
        CamNode.Position = new Vector3(0, 0, CamOffsetDist);
        CamNode.Fov = (float)CamFOVForDist.GetValue(CamOffsetDist);

        // output the camera position and rotation every 2 seconds
        if (TimerReport < FssCoreTime.RuntimeSecs)
        {
            TimerReport = FssCoreTime.RuntimeSecs + 1.0f;
            GD.Print($"CamOffsetDist:{CamOffsetDist:F4} // camAzAngleDegs:{camAzAngleDegs:F4} // camAzAngleDegs:{camElAngleDegs:F4} // CamNode.Position:{CamNode.Position}, CamNode.Rotation:{CamNode.Rotation}");
        }
    }

    // Process inputs
    // Update CamOffsetDist and CamRotation, to be used in _Process
    public override void _Input(InputEvent @event)
    {
        // Only update the camera if the camera is active
        if (CamNode == null) return;
        if (!CamNode.Current) return;

        float distanceDelta = (float)(600 * FssZeroOffset.RwToGeDistanceMultiplierM) * GeDeltaTime;
        float angDeltaDegsPerFrame = 90f * GeDeltaTime;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    camElAngleDegs += angDeltaDegsPerFrame;
            if (Input.IsActionPressed("ui_down") ) camElAngleDegs -= angDeltaDegsPerFrame;
        }
        else
        {
            if (Input.IsActionPressed("ui_up"))    CamOffsetDist  -= distanceDelta;
            if (Input.IsActionPressed("ui_down"))  CamOffsetDist  += distanceDelta;
            if (Input.IsActionPressed("ui_left"))  camAzAngleDegs += angDeltaDegsPerFrame;
            if (Input.IsActionPressed("ui_right")) camAzAngleDegs -= angDeltaDegsPerFrame;
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

    // private void UpdateInputs()
    // {
    //     float distanceDelta        = CamOffsetDist;
    //     float angleDeltaDegsPerSec = 1f;

    //     // Check for Alt/Ctrl to update move amounts
    //     // if (Input.IsActionPressed("ui_alt"))
    //     // {
    //     //     distanceDelta        *= 10;
    //     //     angleDeltaDegsPerSec *= 10;
    //     // }
    //     // if (Input.IsActionPressed("ui_ctrl"))
    //     // {
    //     //     distanceDelta        /= 10;
    //     //     angleDeltaDegsPerSec /= 10;
    //     // }

    //     if (Input.IsActionPressed("ui_shift"))
    //     {
    //         if (Input.IsActionPressed("ui_up"))    CamRotation.Z += angleDeltaDegsPerSec;
    //         if (Input.IsActionPressed("ui_down") ) CamRotation.Z -= angleDeltaDegsPerSec;
    //     }
    //     else
    //     {
    //         if (Input.IsActionPressed("ui_up"))    CamOffsetDist -= distanceDelta;
    //         if (Input.IsActionPressed("ui_down"))  CamOffsetDist += distanceDelta;
    //         if (Input.IsActionPressed("ui_left"))  CamRotation.Y += angleDeltaDegsPerSec;
    //         if (Input.IsActionPressed("ui_right")) CamRotation.Y -= angleDeltaDegsPerSec;
    //     }
    // }
}
