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

    // mouse drag state
    private bool MouseDragging = false;
    private Vector2 MouseDragStart = new Vector2();

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
        if (TimerReport < FssCentralTime.RuntimeSecs)
        {
            TimerReport = FssCentralTime.RuntimeSecs + 1.0f;
            GD.Print($"CamOffsetDist:{CamOffsetDist:F4} // camAzAngleDegs:{camAzAngleDegs:F4} // camAzAngleDegs:{camElAngleDegs:F4} // CamNode.Position:{CamNode.Position}, CamNode.Rotation:{CamNode.Rotation}");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Inputs
    // --------------------------------------------------------------------------------------------

    // Process inputs
    // Update CamOffsetDist and CamRotation, to be used in _Process
    public override void _Input(InputEvent inputEvent)
    {
        // Only update the camera if the camera is active
        if (CamNode == null) return;
        if (!CamNode.Current) return;

        //float distanceDelta = (float)(600 * FssZeroOffset.RwToGeDistanceMultiplierM) * GeDeltaTime;

        float distanceDelta           = CamOffsetDist * GeDeltaTime * 0.8f;
        float distanceDeltaMouseWheel = distanceDelta * 2.5f;
        float angDeltaDegsPerFrame    = 90f * GeDeltaTime;

        // Mouse Wheel - - - - -

        if (inputEvent is InputEventMouseButton mouseWheelEvent)
        {
            if (Input.IsActionPressed("ui_shift")) distanceDeltaMouseWheel *= 5f;
            if (Input.IsActionPressed("ui_ctrl"))  distanceDeltaMouseWheel /= 5f;

            switch (mouseWheelEvent.ButtonIndex)
            {
                case MouseButton.WheelUp: // Zoom in
                    GD.Print($"Wheel up // {CamOffsetDist} // {distanceDelta} // {GeDeltaTime}");
                    CamOffsetDist -= distanceDeltaMouseWheel;
                    break;
                case MouseButton.WheelDown: // Zoom out
                    GD.Print("Wheel down");
                    CamOffsetDist += distanceDeltaMouseWheel;
                    break;
            }
        }

        // Mouse Drag - - - - -

        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (!MouseDragging &&  mouseEvent.Pressed) { MouseDragging = true; MouseDragStart = mouseEvent.Position; }
            if (                  !mouseEvent.Pressed) { MouseDragging = false; }
        }
        else
        {
            if (inputEvent is InputEventMouseMotion motionEvent && MouseDragging)
            {
                Vector2 dragPosition = motionEvent.Position;
                Vector2 dragMovement = dragPosition - MouseDragStart;

                float drawMovementScale = 0.03f;
                if (Input.IsActionPressed("ui_shift")) drawMovementScale *= 3f;
                if (Input.IsActionPressed("ui_ctrl"))  drawMovementScale /= 3f; // Multiply the scaling divisor

                camElAngleDegs += dragMovement.Y * drawMovementScale;
                camAzAngleDegs += dragMovement.X * drawMovementScale;

                // Reset the drag start position: Not doing this makes the offset act more as a veolicty than a position.
                MouseDragStart = dragPosition;
            }
        }

        // Keyboard - - - - -

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

    // Set the camera to a specific position

    public void SetCameraPosition(float rwCamDist, float rwCamAzDegs, float rwCamElDegs)
    {
        CamOffsetDist  = (float)(rwCamDist * FssZeroOffset.RwToGeDistanceMultiplierM);
        camAzAngleDegs = rwCamAzDegs;
        camElAngleDegs = rwCamElDegs;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Query State
    // --------------------------------------------------------------------------------------------

    public bool IsCurrent()
    {
        return CamNode.Current;
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
