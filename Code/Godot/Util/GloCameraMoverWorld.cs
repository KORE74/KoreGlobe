using System;
using System.Text;

using Godot;

public struct GloWorldCamPos
{
    public GloLLAPoint CamPos    = new GloLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 5000 };
    public GloCourse   CamCourse = new GloCourse()   { HeadingDegs = 180, SpeedKph = 0 };
    public float       camPitch = 0;

    public GloWorldCamPos()
    {
        CamPos = new GloLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 5000 };
        CamCourse = new GloCourse() { HeadingDegs = 180, SpeedKph = 0 };
        camPitch = 0;
    }

    // static functino to move the camera between two positions, smoothing through angles and positions
    // fractionp2 means the move goes from 0->1.
    public static GloWorldCamPos Lerp(GloWorldCamPos p1, GloWorldCamPos p2, float fractionP2)
    {
        GloLLAPoint newPos    = GloLLAPointOperations.RhumbLineInterpolation(p1.CamPos, p2.CamPos, fractionP2);
        GloCourse   newCourse = GloCourse.Lerp(p1.CamCourse, p2.CamCourse, fractionP2);
        double      newPitch  = GloValueUtils.Interpolate(p1.camPitch, p2.camPitch, fractionP2);
        return new GloWorldCamPos { CamPos = newPos, CamCourse = newCourse, camPitch = (float)newPitch };
    }

}

// ------------------------------------------------------------------------------------------------


public partial class GloCameraMoverWorld : Node3D
{
    public GloLLAPoint CamPos    = new GloLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 5000 };
    public GloCourse   CamCourse = new GloCourse()   { HeadingDegs = 180, SpeedKph = 0 };
    public float       camPitch  = 0;

    private float TimerCamReport = 0;

    public Camera3D CamNode;


    private Glo1DMappedRange camSpeedForAlt     = new Glo1DMappedRange();
    private Glo1DMappedRange camVertSpeedForAlt = new Glo1DMappedRange();

    // mouse drag state
    private bool    MouseDraggingLeft   = false;
    private bool    MouseDraggingRight  = false;
    private Vector2 MouseDragStartLeft  = new Vector2();
    private Vector2 MouseDragStartRight = new Vector2();


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
        if (TimerCamReport < GloCentralTime.RuntimeSecs)
        {
            TimerCamReport = GloCentralTime.RuntimeSecs + 1f;

            if (CamNode.Current)
            {
                GloZeroNode.SetZeroNodePosition(CamPos.LatDegs, CamPos.LonDegs);
                // GD.Print($"ZERO NODE UPDATE: WorldCam CurrentPosition:{CamPos}");
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
        double effectiveAlt   = GloValueUtils.LimitToRange(CamPos.AltMslM, 10000, 5000000);

        double MoveSpeed      = camSpeedForAlt.GetValue(effectiveAlt) * 2;
        translateSpeed        = MoveSpeed;
        double VertMoveSpeed  = camVertSpeedForAlt.GetValue(effectiveAlt);

        CamCourse.SpeedKph = 0;



        // Mouse Wheel - - - - -

        bool applyPolar = false;


        //float distanceDelta           = CamOffsetDist * GeDeltaTime * 0.8f;
        float distanceDeltaMouseWheel = (float)(CamPos.AltMslM * 0.3f);
        //float angDeltaDegsPerFrame    = 90f * GeDeltaTime;

        if (inputEvent is InputEventMouseButton mouseWheelEvent)
        {
            if (Input.IsActionPressed("ui_shift")) distanceDeltaMouseWheel *= 5f;
            if (Input.IsActionPressed("ui_ctrl"))  distanceDeltaMouseWheel /= 5f;

            switch (mouseWheelEvent.ButtonIndex)
            {
                case MouseButton.WheelUp: // Zoom in
                    //GD.Print($"Wheel up"); // {CamOffsetDist} // {distanceDelta} // {GeDeltaTime}");
                    applyPolar = true;
                    distanceDeltaMouseWheel *= -1;
                    break;
                case MouseButton.WheelDown: // Zoom out
                    //GD.Print("Wheel down");
                    applyPolar = true;
                    //distanceDeltaMouseWheel += distanceDeltaMouseWheel;
                    break;
            }
        }
        GloAzElRange polar = new () { AzDegs = CamCourse.HeadingDegs, ElDegs = camPitch, RangeM = distanceDeltaMouseWheel };





        // Mouse Drag // Right Rotate - - - - -

        if (inputEvent is InputEventMouseButton dragEventRight && dragEventRight.ButtonIndex == MouseButton.Right)
        {
            if (!MouseDraggingRight &&  dragEventRight.Pressed) { MouseDraggingRight = true; MouseDragStartLeft = dragEventRight.Position; }
            if (                       !dragEventRight.Pressed) { MouseDraggingRight = false; }

            //GD.Print($"Right Mouse Button: {dragEventRight.Pressed} / {dragEventRight.Position}");
        }
        else if (inputEvent is InputEventMouseMotion motionEventRight && MouseDraggingRight)
        {
            //GD.Print($"Right Mouse Drag: {motionEventRight.Position} / {motionEventRight.Relative}");

            Vector2 dragPosition = motionEventRight.Position;
            Vector2 dragMovement = dragPosition - MouseDragStartLeft;

            MouseDragStartLeft = dragPosition;

            float rotateScale = 0.02f;

            rotateUpDegs   += dragMovement.Y * rotateScale;
            rotateLeftDegs -= dragMovement.X * rotateScale;
        }





        // Mouse Drag // Left Pan - - - - -

        if (inputEvent is InputEventMouseButton dragEvent && dragEvent.ButtonIndex == MouseButton.Left)
        {
            if (!MouseDraggingLeft &&  dragEvent.Pressed) { MouseDraggingLeft = true; MouseDragStartLeft = dragEvent.Position; }
            if (                  !dragEvent.Pressed) { MouseDraggingLeft = false; }
        }
        else
        {
            if (inputEvent is InputEventMouseMotion motionEvent && MouseDraggingLeft)
            {
                Vector2 dragPosition = motionEvent.Position;
                Vector2 dragMovement = dragPosition - MouseDragStartLeft;

                float drawMovementScale = 150f;
                if (Input.IsActionPressed("ui_shift")) drawMovementScale /= 3f;
                if (Input.IsActionPressed("ui_ctrl"))  drawMovementScale *= 2f;

                if (Input.IsActionPressed("ui_alt"))
                {
                    translateUpM   += dragMovement.Y / drawMovementScale;
                }
                else
                {
                    translateFwdM  -= dragMovement.Y / drawMovementScale;
                    translateLeftM += dragMovement.X / drawMovementScale;
                }


                // Reset the drag start position: Not doing this makes the offset act more as a veolicty than a position.
                MouseDragStartLeft = dragPosition;
            }
        }



        // Keyboard - - - - -

        // Turn keypresses into events
        bool upPressed    = Input.IsActionPressed("ui_up")    || Input.IsActionPressed("ui_w");
        bool downPressed  = Input.IsActionPressed("ui_down")  || Input.IsActionPressed("ui_s");
        bool leftPressed  = Input.IsActionPressed("ui_left")  || Input.IsActionPressed("ui_a");
        bool rightPressed = Input.IsActionPressed("ui_right") || Input.IsActionPressed("ui_d");
        bool altPressed   = Input.IsActionPressed("ui_alt");
        bool shiftPressed = Input.IsActionPressed("ui_shift");

        bool translateUpPressed    = (altPressed && shiftPressed && upPressed)   || Input.IsActionPressed("ui_r");
        bool translateDownPressed  = (altPressed && shiftPressed && downPressed) || Input.IsActionPressed("ui_f");
        bool translateFwdPressed   = (!altPressed || !shiftPressed) && upPressed;
        bool translateBckPressed   = (!altPressed || !shiftPressed) && downPressed;
        bool translateLeftPressed  = (!altPressed || !shiftPressed) && leftPressed;
        bool translateRightPressed = (!altPressed || !shiftPressed) && rightPressed;

        bool rotateUpPressed      = altPressed && upPressed;
        bool rotateDownPressed    = altPressed && downPressed;
        bool rotateLeftPressed    = altPressed && leftPressed;
        bool rotateRightPressed   = altPressed && rightPressed;

        // Act on keystrokes
        if (translateFwdPressed)   translateFwdM  -= 1; // Z is minus in Godot
        if (translateBckPressed)   translateFwdM  += 1;
        if (translateUpPressed)    translateUpM   += 1;
        if (translateDownPressed)  translateUpM   -= 1;
        if (translateLeftPressed)  translateLeftM += 1;
        if (translateRightPressed) translateLeftM -= 1;

        if (rotateUpPressed)       rotateUpDegs   += 1f;
        if (rotateDownPressed)     rotateUpDegs   -= 1f;
        if (rotateLeftPressed)     rotateLeftDegs -= 1f;
        if (rotateRightPressed)    rotateLeftDegs += 1f;

        // Simple: Apply alt and heading chanegs
        CamPos.AltMslM        += translateUpM   * VertMoveSpeed;//translateUpSpeed;
        CamCourse.HeadingDegs += rotateLeftDegs * rotateSpeed;
        CamCourse.SpeedKph    += translateFwdM  * MoveSpeed;//translateSpeed;

        // GD.Print($"MoveSpeed:{MoveSpeed} // VertMoveSpeed:{VertMoveSpeed}");

        // Limit movements - - - - -

        //if (CamPos.AltMslM < 500) CamPos.AltMslM = 500;

        if (!GloValueUtils.IsZero(CamCourse.SpeedKph))
        {
            CamPos = CamPos.PlusRangeBearing(CamCourse.OffsetForTime(1));
        }

        // Translation means setting up a temp course 90 degree off, and adding that.
        if (translateLeftM != 0)
        {
            GloCourse tempCourse = new GloCourse(translateLeftM * translateSpeed, CamCourse.HeadingDegs + 90, 0);
            CamPos = CamPos.PlusRangeBearing(tempCourse.OffsetForTime(1));
        }


        if (applyPolar)
        {
            CamPos = CamPos.PlusPolarOffset(polar);
        }

        rotateUpDegs *= 2;
        //rotateUpDegs = GloValueUtils.LimitToRange(rotateUpDegs, -50, 50);
        camPitch += (float)rotateUpDegs;
        //camPitch  = GloValueUtils.LimitToRange(camPitch, -80, 0);

        //CamNode.Translation = new Vector3(0, 0, 0);


        // Limit the camera altitude
        if (CamPos.AltMslM > 7000000) CamPos.AltMslM = 7000000;

        //CamPosToMapRefPos();
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Update
    // ------------------------------------------------------------------------------------------------

    // The world view camera is anchored on the earth center node, and not impacted by the zero position offset.

    private void UpdateCameraPosition()
    {
        // Set the camera position
        // Vector3 GePos = KoreZeroOffset.GeZeroPointOffset(CamPos.ToXYZ());
        // Translation = GePos;

        Position = KoreZeroOffset.GeZeroPointOffset(CamPos.ToXYZ());

        // Use the heading and LLA position to update the camera rotation
        KoreEntityV3 platformV3 = KoreGeoConvOperations.RwToGeStruct(CamPos, CamCourse.HeadingDegs);

        LookAtFromPosition(
            platformV3.Pos,
            platformV3.PosAhead,
            platformV3.VecUp,
            true);

        if (CamNode.IsCurrent())
        {
            CamNode.RotationDegrees = new Vector3(camPitch, 0, 0);
        }
        CamPosToMapRefPos();
    }

    // ------------------------------------------------------------------------------------------------

    private void CamPosToMapRefPos()
    {
        // if the camera is present and current, Use the position to drive the map update.
        if (CamNode != null)
        {
            if (CamNode.IsCurrent())
            {
                CamNode.RotationDegrees = new Vector3(camPitch, 0, 0);
                KoreZeroNodeMapManager.SetLoadRefLLA(KoreGeoConvOperations.GeToRw(Position));
            }
        }
    }

    // ------------------------------------------------------------------------------------------------
    // MARK: Set Get
    // ------------------------------------------------------------------------------------------------

    // Turn the Camera position fields (one by one) into a string we can save and restore.

    public string CamPosToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"[{CamPos.LatDegs:F3},{CamPos.LonDegs:F3},{CamPos.AltMslM:F0}]");
        sb.Append($"[{CamCourse.HeadingDegs:F2},{CamCourse.SpeedKph:F0}]");
        sb.Append($"[{camPitch:F2}]");

        return sb.ToString();
    }

    public void CamPosFromString(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length < 10) return;

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
            CamCourse.SpeedKph    = double.Parse(courseParts[1]);
        }

        string[] pitchParts = parts[2].Split(',');
        if (pitchParts.Length == 1)
        {
            camPitch = float.Parse(pitchParts[0]);
        }
    }

    // --------------------------------------------------------------------------------------------

    public GloWorldCamPos GetWorldCamPos()
    {
        GloWorldCamPos camPos = new GloWorldCamPos();
        camPos.CamPos    = CamPos;
        camPos.CamCourse = CamCourse;
        camPos.camPitch  = camPitch;

        return camPos;
    }

    public void SetWorldCamPos(GloWorldCamPos camPos)
    {
        CamPos    = camPos.CamPos;
        CamCourse = camPos.CamCourse;
        camPitch  = camPos.camPitch;
    }

}
