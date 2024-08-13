using Godot;
using System;

public partial class FssCameraMoverWorld : Camera3D
{
    public static FssLLAPoint CamPos    = new FssLLAPoint() { LatDegs = 50, LonDegs = -1, AltMslM = 1000 };
    public FssCourse   CamCourse = new FssCourse() {HeadingDegs = 30, SpeedKph = 0};

    public FssPolarOffset CamOffset = new FssPolarOffset(10, 0, 0);

    private float TimerCamReport = 0;

    // ------------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Initialization code if needed
        Name = "WorldCam";
    }

    public override void _Process(double delta)
    {
        UpdateInputs();

        if (TimerCamReport < FssCoreTime.RuntimeSecs)
        {
            TimerCamReport = FssCoreTime.RuntimeSecs + 3f;

            // GD.Print($"CamPos:{CamPos}");

        }

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
        double translateFwdM  = 0;
        double translateLeftM = 0;
        double translateUpM   = 0;
        double rotateUpDegs   = 0;
        double rotateLeftDegs = 0;

        if (Input.IsActionPressed("ui_shift"))
        {
            if (Input.IsActionPressed("ui_up"))    translateUpM += 1;
            if (Input.IsActionPressed("ui_down") ) translateUpM -= 1;
        }
        else if (Input.IsActionPressed("ui_alt"))
        {
            if (Input.IsActionPressed("ui_up"))    rotateUpDegs += 2f;
            if (Input.IsActionPressed("ui_down"))  rotateUpDegs -= 2f;
            if (Input.IsActionPressed("ui_left"))  rotateLeftDegs += 2f;
            if (Input.IsActionPressed("ui_right")) rotateLeftDegs -= 2f;
        }
        else
        {
            if (Input.IsActionPressed("ui_left"))  translateLeftM += 1;
            if (Input.IsActionPressed("ui_right")) translateLeftM -= 1;
            if (Input.IsActionPressed("ui_up"))    translateFwdM += 1;
            if (Input.IsActionPressed("ui_down"))  translateFwdM -= 1;
        }

        // Create a polar offset to apply to the camera position
        CamOffset.AzDegs += rotateLeftDegs;
        CamOffset.ElDegs += rotateUpDegs;
        CamOffset.RangeM += translateFwdM;

        // GD.Print($"CamOffset:{CamOffset}");
    }

    // ------------------------------------------------------------------------------------------------

    // The world view camera is anchored on the earth center node, and not impacted by the zero position offset.

    private void UpdateCameraPosition()
    {
        // // Set the camera position
        // FssZeroOffset.SetLLA(CamPos);
        // Vector3 GePos = FssZeroOffset.GeZeroPointOffset(CamPos.ToXYZ());
        // Translation = GePos;

        // // Set the camera rotation
        // Vector3 GeRot = new Vector3(0, 0, 0);
        // RotationDegrees = GeRot;

        // If the range value of the offset is non-zero, get the offset and apply it to the position. Then zero the range.
        if (CamOffset.RangeM > 1)
        {
            FssXYZPoint camXYZ = CamOffset.ToXYZ();
            Vector3 GePos = FssZeroOffset.GeZeroPointOffset(camXYZ);


            CamOffset.RangeM = 0;
        }


    }
}
