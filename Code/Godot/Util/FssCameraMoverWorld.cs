using System;
using System.Text;

using Godot;

public partial class FssCameraMoverWorld : Node3D
{
    public FssLLAPoint CamPos    = new FssLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 5000 };
    public FssCourse   CamCourse = new FssCourse()   { HeadingDegs = 180, GroundSpeedKph = 0 };
    public float camPitch = 0;

    private float TimerCamReport = 0;

    public Camera3D CamNode;


    private Fss1DMappedRange camSpeedForAlt     = new Fss1DMappedRange();
    private Fss1DMappedRange camVertSpeedForAlt = new Fss1DMappedRange();

    // mouse drag state
    private bool MouseDragging = false;
    private Vector2 MouseDragStart = new Vector2();

    // ------------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // ------------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialization code if needed
        Name = "WorldCam";

        CamNode = new Camera3D() { Name = "WorldCam" };
        AddChild(CamNode);
        CamNode.Near = 0.001f;
        CamNode.Far  = 4000f;
        CamNode.Fov  = 40f;

        camSpeedForAlt.AddEntry(10,          250);
        camSpeedForAlt.AddEntry(1000,        500);
        camSpeedForAlt.AddEntry(5000,       2000);
        camSpeedForAlt.AddEntry(1000000,  200000);
        camSpeedForAlt.AddEntry(5000000, 2000000);

        camVertSpeedForAlt.AddEntry(10,          50);
        camVertSpeedForAlt.AddEntry(1000,       100);
        camVertSpeedForAlt.AddEntry(5000,       400);
        camVertSpeedForAlt.AddEntry(1000000,  40000);
        camVertSpeedForAlt.AddEntry(5000000, 400000);
    }

    public override void _Process(double delta)
    {
        if (TimerCamReport < FssCentralTime.RuntimeSecs)
        {
            TimerCamReport = FssCentralTime.RuntimeSecs + 1f;

            if (CamNode.Current)
            {
                FssZeroNode.SetZeroNodePosition(CamPos.LatDegs, CamPos.LonDegs);
                GD.Print($"ZERO NODE UPDATE: WorldCam CurrentPosition:{CamPos}");
            }
        }

        // Turn the position CamPos into a GE positino and place the camera
        UpdateCameraPosition();
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Input
    // ------------------------------------------------------------------------------------------------

    public override void _Input(InputEvent inputEvent)
    {
        // exit if the camera is not current
        if (CamNode == null) return;
        if (!CamNode.IsCurrent()) return;

        // Available move amounts for this call
        double translateFwdM  = 0;
        double translateLeftM = 0;
        double translateUpM   = 0;
        double rotateUpDegs   = 0;
        double rotateLeftDegs = 0;

        // scaling factors for movement
        double translateSpeed = 2500;
        double rotateSpeed    = 2;
        double MoveSpeed      = camSpeedForAlt.GetValue(CamPos.AltMslM) * 2;
        translateSpeed        = MoveSpeed;
        double VertMoveSpeed  = camVertSpeedForAlt.GetValue(CamPos.AltMslM);

        CamCourse.GroundSpeedKph = 0;


        // Mouse Wheel - - - - -

        bool applyPolar = false;


        //float distanceDelta           = CamOffsetDist * GeDeltaTime * 0.8f;
        float distanceDeltaMouseWheel = (float)(CamPos.AltMslM * 0.5f);
        //float angDeltaDegsPerFrame    = 90f * GeDeltaTime;

        if (inputEvent is InputEventMouseButton mouseWheelEvent)
        {
            if (Input.IsActionPressed("ui_shift")) distanceDeltaMouseWheel *= 5f;
            if (Input.IsActionPressed("ui_ctrl"))  distanceDeltaMouseWheel /= 5f;

            switch (mouseWheelEvent.ButtonIndex)
            {
                case MouseButton.WheelUp: // Zoom in
                    GD.Print($"Wheel up"); // {CamOffsetDist} // {distanceDelta} // {GeDeltaTime}");
                    applyPolar = true;
                    break;
                case MouseButton.WheelDown: // Zoom out
                    GD.Print("Wheel down");
                    applyPolar = true;
                    distanceDeltaMouseWheel *= -1;
                    //distanceDeltaMouseWheel += distanceDeltaMouseWheel;
                    break;
            }
        }
        FssPolarOffset polar = new FssPolarOffset() { AzDegs = CamCourse.HeadingDegs, ElDegs = camPitch, RangeM = distanceDeltaMouseWheel };

        // Mouse Drag - - - - -

        if (inputEvent is InputEventMouseButton dragEvent && dragEvent.ButtonIndex == MouseButton.Left)
        {
            if (!MouseDragging &&  dragEvent.Pressed) { MouseDragging = true; MouseDragStart = dragEvent.Position; }
            if (                  !dragEvent.Pressed) { MouseDragging = false; }
        }
        else
        {
            if (inputEvent is InputEventMouseMotion motionEvent && MouseDragging)
            {
                Vector2 dragPosition = motionEvent.Position;
                Vector2 dragMovement = dragPosition - MouseDragStart;

                // ALT = rotate
                if (Input.IsActionPressed("ui_alt"))
                {
                    // ALT-SHIFT = Elevate
                    if (Input.IsActionPressed("ui_shift"))
                    {
                        translateUpM += dragMovement.Y / 50f;
                    }
                    else
                    {
                        float rotateScale = 0.02f;

                        rotateUpDegs   += dragMovement.Y * rotateScale;
                        rotateLeftDegs -= dragMovement.X * rotateScale;
                    }
                }
                else
                {
                    float drawMovementScale = 150f;
                    if (Input.IsActionPressed("ui_shift")) drawMovementScale /= 3f;
                    if (Input.IsActionPressed("ui_ctrl"))  drawMovementScale *= 2f;

                    translateFwdM  -= dragMovement.Y / drawMovementScale;
                    translateLeftM += dragMovement.X / drawMovementScale;
                }

                // Reset the drag start position: Not doing this makes the offset act more as a veolicty than a position.
                MouseDragStart = dragPosition;
            }
        }

        // Keyboard - - - - -

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    translateUpM   += 1;
            if (Input.IsActionPressed("ui_down") ) translateUpM   -= 1;
        }
        else if (Input.IsActionPressed("ui_alt"))
        {
            if (Input.IsActionPressed("ui_up"))    rotateUpDegs   += 1f;
            if (Input.IsActionPressed("ui_down"))  rotateUpDegs   -= 1f;
            if (Input.IsActionPressed("ui_left"))  rotateLeftDegs -= 1f;
            if (Input.IsActionPressed("ui_right")) rotateLeftDegs += 1f;
        }
        else
        {
            if (Input.IsActionPressed("ui_up"))    translateFwdM  -= 1;
            if (Input.IsActionPressed("ui_down"))  translateFwdM  += 1;
            if (Input.IsActionPressed("ui_left"))  translateLeftM += 1;
            if (Input.IsActionPressed("ui_right")) translateLeftM -= 1;
        }

        // Simple: Apply alt and heading chanegs
        CamPos.AltMslM        += translateUpM   * VertMoveSpeed;//translateUpSpeed;
        CamCourse.HeadingDegs += rotateLeftDegs * rotateSpeed;
        CamCourse.GroundSpeedKph    += translateFwdM  * MoveSpeed;//translateSpeed;

        // GD.Print($"MoveSpeed:{MoveSpeed} // VertMoveSpeed:{VertMoveSpeed}");

        // Limit movements - - - - -

        if (CamPos.AltMslM < 50) CamPos.AltMslM = 50;

        if (!FssValueUtils.IsZero(CamCourse.GroundSpeedKph))
        {
            CamPos = CamPos.PlusRangeBearing(CamCourse.OffsetForTime(1));
        }

        // Translation means setting up a temp course 90 degree off, and adding that.
        if (translateLeftM != 0)
        {
            FssCourse tempCourse = new FssCourse(translateLeftM * translateSpeed, CamCourse.HeadingDegs + 90, 0);
            CamPos = CamPos.PlusRangeBearing(tempCourse.OffsetForTime(1));
        }

        //FssMapManager.LoadRefLLA = CamPos;

        if (applyPolar)
        {
            CamPos = CamPos.PlusPolarOffset(polar);
        }

        rotateUpDegs *= 2;
        //rotateUpDegs = FssValueUtils.LimitToRange(rotateUpDegs, -50, 50);
        camPitch += (float)rotateUpDegs;
        camPitch  = FssValueUtils.LimitToRange(camPitch, -80, 0);

        //CamNode.Translation = new Vector3(0, 0, 0);


        // Limit the camera altitude
        if (CamPos.AltMslM > 5000000) CamPos.AltMslM = 5000000;

        // if the camera is present and current, Use the position to drive the map update.
        if (CamNode != null)
        {
            if (CamNode.IsCurrent())
            {
                CamNode.RotationDegrees = new Vector3(camPitch, 0, 0);
                FssMapManager.LoadRefLLA = FssGeoConvOperations.GeToRw(Position);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Update
    // ------------------------------------------------------------------------------------------------

    // The world view camera is anchored on the earth center node, and not impacted by the zero position offset.

    private void UpdateCameraPosition()
    {
        // Set the camera position
        // Vector3 GePos = FssZeroOffset.GeZeroPointOffset(CamPos.ToXYZ());
        // Translation = GePos;

        Position = FssZeroOffset.GeZeroPointOffset(CamPos.ToXYZ());

        // Use the heading and LLA position to update the camera rotation
        FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(CamPos, CamCourse.HeadingDegs);

        LookAtFromPosition(
            platformV3.Pos,
            platformV3.PosAhead,
            platformV3.VecUp,
            true);
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Set Get
    // ------------------------------------------------------------------------------------------------

    // Turn the Camera position fields (one by one) into a string we can save and restore.

    public string CamPosToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"[{CamPos.LatDegs:F3},{CamPos.LonDegs:F3},{CamPos.AltMslM:F0}]");
        sb.Append($"[{CamCourse.HeadingDegs:F0},{CamCourse.GroundSpeedKph:F0}]");
        sb.Append($"[{camPitch:F2}]");

        return sb.ToString();
    }

    public void CamPosFromString(string str)
    {
        string[] parts = str.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) return;

        string[] posParts = parts[0].Split(',');
        if (posParts.Length == 3)
        {
            CamPos.LatDegs = double.Parse(posParts[0]);
            CamPos.LonDegs = double.Parse(posParts[1]);
            CamPos.AltMslM = double.Parse(posParts[2]);
        }

        string[] courseParts = parts[1].Split(',');
        if (courseParts.Length == 2)
        {
            CamCourse.HeadingDegs = double.Parse(courseParts[0]);
            CamCourse.GroundSpeedKph    = double.Parse(courseParts[1]);
        }

        string[] pitchParts = parts[2].Split(',');
        if (pitchParts.Length == 1)
        {
            camPitch = float.Parse(pitchParts[0]);
        }
    }

}
