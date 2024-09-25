using Godot;
using System;

public partial class FssCameraMoverWorld : Node3D
{
    public FssLLAPoint CamPos    = new FssLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 5000 };
    public FssCourse   CamCourse = new FssCourse()   { HeadingDegs = 180, SpeedKph = 0 };

    public float camPitch = 0;

    private float TimerCamReport = 0;

    public Camera3D CamNode;


    private Fss1DMappedRange camSpeedForAlt = new Fss1DMappedRange();
    private Fss1DMappedRange camVertSpeedForAlt = new Fss1DMappedRange();

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
        UpdateInputs();

        if (TimerCamReport < FssCoreTime.RuntimeSecs)
        {
            TimerCamReport = FssCoreTime.RuntimeSecs + 3f;

            // GD.Print($"CamPos:{CamPos}");

        }

        // Turn the position CamPos into a GE positino and place the camera
        UpdateCameraPosition();

        // if (direction.Length() > 0)
        // {
        //     // Normalize the direction vector to prevent faster diagonal movement
        //     direction = direction.Normalized();

        //     // Transform the direction from local space to global space
        //     Vector3 forward = -Transform.Basis.Z;
        //     Vector3 right   =  Transform.Basis.X;
        //     Vector3 up      =  Transform.Basis.Y;
        //     Vector3 globalDirection = (forward * direction.Z) + (right * direction.X) + (up * direction.Y);

        //     // Move the object
        //     Position += globalDirection * MoveSpeed * (float)(delta);
        // }
        // if (rotation.Length() > 0)
        // {
        //     Rotation += rotation * RotateSpeed * (float)(delta);
        // }
    }

// ------------------------------------------------------------------------------------------------

    private void UpdateInputs()
    {

        // exit if the camera is not current
        if (CamNode == null) return;
        if (!CamNode.IsCurrent()) return;

        double translateFwdM  = 0;
        double translateLeftM = 0;
        double translateUpM   = 0;
        double rotateUpDegs   = 0;
        double rotateLeftDegs = 0;
        CamCourse.SpeedKph = 0;

        double translateSpeed   = 2500;
        double rotateSpeed      = 2;
        double translateUpSpeed = 100;

        // double MoveSpeed = (float)(FssValueUtils.LimitToRange(CamPos.AltMslKm / 0.03, 2500, 5000000));
        // translateSpeed = MoveSpeed;

        double MoveSpeed = camSpeedForAlt.GetValue(CamPos.AltMslM);
        translateSpeed = MoveSpeed;
        double VertMoveSpeed = camVertSpeedForAlt.GetValue(CamPos.AltMslM);


        if (Input.IsMouseButtonPressed(Godot.MouseButton.Left))
            GD.Print("Mouse Button Left");
        if (Input.IsMouseButtonPressed(Godot.MouseButton.Right))
            GD.Print("Mouse Button Right");
        if (Input.IsMouseButtonPressed(Godot.MouseButton.Middle))
            GD.Print("Mouse Button Middle");

        if (Input.IsMouseButtonPressed(Godot.MouseButton.WheelUp))
            GD.Print("Mouse Button WheelUp");
        if (Input.IsMouseButtonPressed(Godot.MouseButton.WheelDown))
            GD.Print("Mouse Button WheelDown");





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
        CamCourse.SpeedKph    += translateFwdM  * MoveSpeed;//translateSpeed;

        // GD.Print($"MoveSpeed:{MoveSpeed} // VertMoveSpeed:{VertMoveSpeed}");


        if (CamPos.AltMslM < 500) CamPos.AltMslM = 500;

        if (!FssValueUtils.IsZero(CamCourse.SpeedKph))
        {
            CamPos = CamPos.PlusRangeBearing(CamCourse.OffsetForTime(1));
        }

        // Translation means setting up a temp course 90 degree off, and adding that.
        if (translateLeftM != 0)
        {
            FssCourse tempCourse = new FssCourse(translateLeftM * translateSpeed, CamCourse.HeadingDegs + 90);
            CamPos = CamPos.PlusRangeBearing(tempCourse.OffsetForTime(1));
        }

        //FssMapManager.LoadRefLLA = CamPos;

        rotateUpDegs *= 2;
        //rotateUpDegs = FssValueUtils.LimitToRange(rotateUpDegs, -50, 50);
        camPitch += (float)rotateUpDegs;
        camPitch  = FssValueUtils.LimitToRange(camPitch, -80, 0);

        //CamNode.Translation = new Vector3(0, 0, 0);

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

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    GD.Print($"Left button was clicked at {mouseEvent.Position}");
                    break;
                case MouseButton.WheelUp:
                    GD.Print("Wheel up");
                    break;
            }
        }
    }

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
}
