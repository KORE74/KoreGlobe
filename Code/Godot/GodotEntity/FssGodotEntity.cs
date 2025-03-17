using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class FssGodotEntity : Node3D
{
    public string EntityName { get; set; }

    // Setup default model info, so we always have something to work with.
    public Fss3DModelInfo ModelInfo { get; set; } = Fss3DModelInfo.Default();

    public Node3D AttitudeNode = new Node3D() { Name = "Attitude" };

    private FssElementContrail ElementContrail;

    private FssAttitude          CurrentAttitude = new FssAttitude();
    private FssLLAPoint          CurrentPosition = new FssLLAPoint();
    private FssCameraPolarOffset ChaseCam        = new FssCameraPolarOffset();

    private float Timer1Hz = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateEntity();

        AddChild(AttitudeNode);

        // ElementContrail = new FssElementContrail();
        // ElementContrail.InitElement(EntityName);
        // ElementContrail.SetModel(EntityName);
        // FssGodotFactory.Instance.GodotEntityManager.ElementRootNode.AddChild(ElementContrail);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        UpdateEntityPosition();

        if (Timer1Hz < FssCentralTime.RuntimeSecs)
        {
            Timer1Hz = FssCentralTime.RuntimeSecs + 1.0f;
            UpdateZeroNode();

            if (ChaseCam.IsCurrent())
                FssMapManager.LoadRefLLA = CurrentPosition;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void CreateEntity()
    {
        // Set this node name. - This gets the position and earth orinetation set on it.
        Name = EntityName;

        // Create a marker for the entity
        Node3D marker = new Node3D() { Name = "AxisMarker" };
        AddChild(marker);
        //FssPrimitiveFactory.AddAxisMarkers(marker, 0.003f, 0.001f);

        // Setup the chase camera and default position
        ChaseCam.Name = "ChaseCam";
        AddChild(ChaseCam);
        ChaseCam.SetCameraPosition(300, 20, 20); // 300m, 20 degs up, 20 degs right
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Chase Cam
    // --------------------------------------------------------------------------------------------

    public void EnableChaseCam()
    {
        ChaseCam.CamNode.Current = true;
    }

    // Report the current state of the camer

    public void UpdateZeroNode()
    {
        GD.Print("EntityName:{EntityName}");

        // Only drive the zero node to match the entity position if the chase cam is the current camera
        if (ChaseCam.IsCurrent())
        {
            //FssZeroNode.SetZeroNodePosition(CurrentPosition);
            //GD.Print($"ZERO NODE UPDATE: EntityName:{EntityName} CurrentPosition:{CurrentPosition}");
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Position and Attitude
    // --------------------------------------------------------------------------------------------

    // Note that position here is a polar offset for the rotating chase cam

    public void UpdateEntityPosition()
    {
        // Update the position and orientation of the entity.
        // This is done by the parent node.

        FssLLAPoint? pos    = FssEventDriver.EntityCurrLLA(EntityName);
        FssCourse?   course = FssEventDriver.EntityCurrCourse(EntityName);

        if (pos != null)
            CurrentPosition = (FssLLAPoint)pos;

        if (pos == null || course == null)
        {
            GD.Print($"EC0-0025: Platform {EntityName} not found.");
            return;
        }

        FssEntityV3 entityVecs = FssZeroOffsetOperations.RwToGeStruct((FssLLAPoint)pos, (FssCourse)course);

        //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

        Position = entityVecs.Pos;
        //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);

        LookAt(entityVecs.PosAhead, entityVecs.VecUp);

        FssAttitude? att = FssEventDriver.EntityCurrAttitude(EntityName);
        if (att != null)
            UpdateAttitude((FssAttitude)att);
    }

    public void UpdateAttitude(FssAttitude attitude)
    {
        CurrentAttitude.PitchUpDegs       = FssValueUtils.AdjustWithinBounds(CurrentAttitude.PitchUpDegs,       attitude.PitchUpDegs, 1);
        CurrentAttitude.RollClockwiseDegs = FssValueUtils.AdjustWithinBounds(CurrentAttitude.RollClockwiseDegs, attitude.RollClockwiseDegs, 1);
        CurrentAttitude.YawClockwiseDegs  = FssValueUtils.AdjustWithinBounds(CurrentAttitude.YawClockwiseDegs,  attitude.YawClockwiseDegs, 1);

        double pitchUpRads       = CurrentAttitude.PitchUpRads;
        double rollClockwiseRads = CurrentAttitude.RollClockwiseRads;
        double yawClockwiseRads  = CurrentAttitude.YawClockwiseRads;

        float gePitchRads = (float)pitchUpRads;
        float geRollRads  = (float)rollClockwiseRads;
        float geYawRads   = (float)yawClockwiseRads;

        AttitudeNode.Rotation = new Vector3(gePitchRads, geYawRads, geRollRads);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Elements: Route
    // --------------------------------------------------------------------------------------------

    // Function to check if the entity has a route, and update the game-engine route to match.

    public void UpdateRoute()
    {
        // Get the route
        //List<FssLLAPoint> routePoints = FssEventDriver.PlatformGetRoutePoints(EntityName);

        // if (routePoints.Count > 0)
        // {
        //     return;
        // }

        // Update the route
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Elements: Contrail
    // --------------------------------------------------------------------------------------------

    public void UpdateContrail()
    {
        //ElementContrail.UpdateElement();
    }

}


