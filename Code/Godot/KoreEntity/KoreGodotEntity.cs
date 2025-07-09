using System;
using System.Collections.Generic;

using KoreCommon;
using KoreSim;

using Godot;

#nullable enable

public partial class KoreGodotEntity : Node3D
{
    public string EntityName { get; set; }

    // Setup default model info, so we always have something to work with.
    // public 3DModelInfo ModelInfo { get; set; } = Glo3DModelInfo.Default();

    public Node3D AttitudeNode = new Node3D() { Name = "Attitude" };

    private GloElementContrail ElementContrail;

    private KoreAttitude          CurrAttitude = KoreAttitude.Zero;
    private KoreLLAPoint          CurrLLA = KoreLLAPoint.Zero;
    private KoreCourse            CurrCourse = KoreCourse.Zero;

    private KoreAttitude          SmoothedAttitude = KoreAttitude.Zero;

    private GloCameraPolarOffset ChaseCam = new GloCameraPolarOffset();

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

        if (Timer1Hz < GloCentralTime.RuntimeSecs)
        {
            Timer1Hz = GloCentralTime.RuntimeSecs + 1.0f;

            UpdateRwPosition();
            ApplySmoothAttitude();

            // if (ChaseCam.IsCurrent())
            // {
            //     // Get the Camera Polar Offset - flip the azimuth so we create the LLA correctly.
            //     GloAzElRange camPO = ChaseCam.RwCamOffset;

            //     // Get the platform heading and add the camera offset to get the chase cam LLA
            //     //GloLLAPoint? pos    = GloAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
            //     GloCourse? course = GloAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

            //     if (course != null)
            //         camPO.AzDegs += course?.HeadingDegs ?? 0.0;

            //     GloLLAPoint chaseCamLLA  = CurrentPosition.PlusPolarOffset(camPO);

            //     GloZeroNodeMapManager.SetLoadRefLLA(chaseCamLLA);

            //     string strCamLLA = chaseCamLLA.ToString();
            //     GD.Print($"Camera LLA: Lat:{chaseCamLLA.LatDegs:F6} Lon:{chaseCamLLA.LonDegs:F6} Alt:{chaseCamLLA.AltMslM:F2}");
            // }
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

    // public void EnableChaseCam()
    // {
    //     ChaseCam.CamNode.Current = true;
    // }

    // // Report the current state of the camer

    // public void UpdateZeroNode()
    // {
    //     // GD.Print("EntityName:{EntityName}");

    //     // Only drive the zero node to match the entity position if the chase cam is the current camera
    //     if (ChaseCam.IsCurrent())
    //     {
    //         GloZeroNode.SetZeroNodePosition(CurrentPosition);
    //         // GD.Print($"ZERO NODE UPDATE: EntityName:{EntityName} CurrentPosition:{CurrentPosition}");
    //     }
    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Position and Attitude
    // --------------------------------------------------------------------------------------------

    // Note that position here is a polar offset for the rotating chase cam

    public void UpdateRwPosition()
    {
        // Quick check the entity is valid
        if (!KoreEventDriver.HasEntity(EntityName))
            return;

        // Get the 6DOF data (will return a .Zero value on error, not a null object)
        CurrLLA = KoreEventDriver.GetEntityPosition(EntityName);
        CurrCourse = KoreEventDriver.GetEntityCourse(EntityName);

        // Convert the position to a game-engine structure
        KoreEntityV3 entityVecs = KoreGeoConvOperations.RwToGeStruct(CurrLLA, CurrCourse);

        //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

        // Apply the game-engine position and orientation to the node
        Position = entityVecs.Pos;
        LookAt(entityVecs.PosAhead, entityVecs.VecUp);
        //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);
    }

    public void ApplySmoothAttitude()
    {
        // Quick check the entity is valid
        if (!KoreEventDriver.HasEntity(EntityName))
            return;

        // Get the 6DOF data (will return a .Zero value on error, not a null object)
        CurrAttitude = KoreEventDriver.GetEntityAttitude(EntityName);

        // Update the smoothed attitude (that will progressively update the entity orientation), by 1 degree per update() call.
        SmoothedAttitude.PitchUpDegs       = GloValueUtils.AdjustWithinBounds(SmoothedAttitude.PitchUpDegs,       CurrAttitude.PitchUpDegs, 1);
        SmoothedAttitude.RollClockwiseDegs = GloValueUtils.AdjustWithinBounds(SmoothedAttitude.RollClockwiseDegs, CurrAttitude.RollClockwiseDegs, 1);
        SmoothedAttitude.YawClockwiseDegs  = GloValueUtils.AdjustWithinBounds(SmoothedAttitude.YawClockwiseDegs,  CurrAttitude.YawClockwiseDegs, 1);

        // Convert the the right float and rads units
        float gePitchRads = (float)SmoothedAttitude.PitchUpRads;
        float geRollRads  = (float)SmoothedAttitude.RollClockwiseRads;
        float geYawRads   = (float)SmoothedAttitude.YawClockwiseRads;

        // Apply attitude to node rotation
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


