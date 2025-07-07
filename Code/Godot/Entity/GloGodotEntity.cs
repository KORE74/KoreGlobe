using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class GloGodotEntity : Node3D
{
    public string EntityName { get; set; }

    // Setup default model info, so we always have something to work with.
    public Glo3DModelInfo ModelInfo { get; set; } = Glo3DModelInfo.Default();

    public Node3D AttitudeNode = new Node3D() { Name = "Attitude" };

    private GloElementContrail ElementContrail;

    private GloAttitude          CurrentAttitude = new GloAttitude();
    private GloLLAPoint          CurrentPosition = new GloLLAPoint();
    private GloCameraPolarOffset ChaseCam        = new GloCameraPolarOffset();

    private float Timer1Hz = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Node Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateEntity();

        AddChild(AttitudeNode);

        // ElementContrail = new GloElementContrail();
        // ElementContrail.InitElement(EntityName);
        // ElementContrail.SetModel(EntityName);
        // GloGodotFactory.Instance.GodotEntityManager.ElementRootNode.AddChild(ElementContrail);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        UpdateEntityPosition();

        if (Timer1Hz < GloCentralTime.RuntimeSecs)
        {
            Timer1Hz = GloCentralTime.RuntimeSecs + 1.0f;
            UpdateZeroNode();

            if (ChaseCam.IsCurrent())
            {
                // Get the Camera Polar Offset - flip the azimuth so we create the LLA correctly.
                GloAzElRange camPO = ChaseCam.RwCamOffset;

                // Get the platform heading and add the camera offset to get the chase cam LLA
                //GloLLAPoint? pos    = GloAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
                GloCourse? course = GloAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

                if (course != null)
                    camPO.AzDegs += course?.HeadingDegs ?? 0.0;

                GloLLAPoint chaseCamLLA  = CurrentPosition.PlusPolarOffset(camPO);

                GloZeroNodeMapManager.SetLoadRefLLA(chaseCamLLA);

                string strCamLLA = chaseCamLLA.ToString();
                GD.Print($"Camera LLA: Lat:{chaseCamLLA.LatDegs:F6} Lon:{chaseCamLLA.LonDegs:F6} Alt:{chaseCamLLA.AltMslM:F2}");
            }
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
        //GloPrimitiveFactory.AddAxisMarkers(marker, 0.003f, 0.001f);

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
        // GD.Print("EntityName:{EntityName}");

        // Only drive the zero node to match the entity position if the chase cam is the current camera
        if (ChaseCam.IsCurrent())
        {
            GloZeroNode.SetZeroNodePosition(CurrentPosition);
            // GD.Print($"ZERO NODE UPDATE: EntityName:{EntityName} CurrentPosition:{CurrentPosition}");
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

        GloLLAPoint? pos    = GloAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
        GloCourse?   course = GloAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);
        String?      platCat = GloAppFactory.Instance.EventDriver.PlatformCategory(EntityName);

        if (pos != null)
            CurrentPosition = (GloLLAPoint)pos;

        if (pos == null || course == null)
        {
            GD.Print($"EC0-0025: Platform {EntityName} not found.");
            return;
        }

        GloEntityV3 entityVecs = GloGeoConvOperations.RwToGeStruct((GloLLAPoint)pos, (GloCourse)course);

        //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

        Position = entityVecs.Pos;
        //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);

        LookAt(entityVecs.PosAhead, entityVecs.VecUp);

        GloAttitude? att = GloAppFactory.Instance.EventDriver.GetPlatformAttitude(EntityName);
        if (att != null)
            UpdateAttitude((GloAttitude)att);


        if (!String.IsNullOrEmpty(platCat) && (platCat == "GroundClamped"))
        {
            GloLLPoint llPos = new GloLLPoint() { LatDegs = pos?.LatDegs ?? 0.0, LonDegs = pos?.LonDegs ?? 0.0 };
            float mapEle = GloGodotFactory.Instance.ZeroNodeMapManager.GetElevation(llPos);

            GloLLAPoint platLLA = new GloLLAPoint() { LatDegs = llPos.LatDegs, LonDegs = llPos.LonDegs, AltMslM = mapEle };
            GloAppFactory.Instance.EventDriver.SetPlatformPosition(EntityName, platLLA);

            GD.Print($"GroundClamped: {EntityName} {platLLA}");
        }
    }

    public void UpdateAttitude(GloAttitude attitude)
    {
        CurrentAttitude.PitchUpDegs       = GloValueUtils.AdjustWithinBounds(CurrentAttitude.PitchUpDegs,       attitude.PitchUpDegs, 1);
        CurrentAttitude.RollClockwiseDegs = GloValueUtils.AdjustWithinBounds(CurrentAttitude.RollClockwiseDegs, attitude.RollClockwiseDegs, 1);
        CurrentAttitude.YawClockwiseDegs  = GloValueUtils.AdjustWithinBounds(CurrentAttitude.YawClockwiseDegs,  attitude.YawClockwiseDegs, 1);

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
        List<GloLLAPoint> routePoints = GloAppFactory.Instance.EventDriver.PlatformGetRoutePoints(EntityName);

        if (routePoints.Count > 0)
        {
            return;
        }

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


