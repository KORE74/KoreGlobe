using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public partial class FssGodotEntity : Node3D
{
    public string EntityName { get; set; }

    // Setup default model info, so we always have something to work with.
    public Fss3DModelInfo ModelInfo { get; set; } = Fss3DModelInfo.Default();

    private FssElementContrail ElementContrail;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateEntity();

        // ElementContrail = new FssElementContrail();
        // ElementContrail.InitElement(EntityName);
        // ElementContrail.SetModel(EntityName);
        // FssGodotFactory.Instance.GodotEntityManager.ElementRootNode.AddChild(ElementContrail);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        UpdateEntityPosition();
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
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void UpdateEntityPosition()
    {
        // Update the position and orientation of the entity.
        // This is done by the parent node.

        FssLLAPoint? pos    = FssAppFactory.Instance.EventDriver.GetPlatformPosition(EntityName);
        FssCourse?   course = FssAppFactory.Instance.EventDriver.PlatformCurrCourse(EntityName);

        if (pos == null || course == null)
        {
            GD.Print($"EC0-0025: Platform {EntityName} not found.");
            return;
        }

        FssEntityV3 entityVecs = FssGeoConvOperations.RwToGeStruct((FssLLAPoint)pos, (FssCourse)course);

        //GD.Print($"Name: {EntityName} PosLLA:{pos} Ahead:{entityVecs.PosAhead} up:{entityVecs.VecUp}");

        Position = entityVecs.Pos;
        //LookAtFromPosition(entityVecs.Pos, entityVecs.PosAhead, entityVecs.VecUp, true);

        LookAt(entityVecs.PosAhead, entityVecs.VecUp);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update Elements: Route
    // --------------------------------------------------------------------------------------------

    // Function to check if the entity has a route, and update the game-engine route to match.

    public void UpdateRoute()
    {
        // Get the route
        List<FssLLAPoint> routePoints = FssAppFactory.Instance.EventDriver.PlatformGetRoutePoints(EntityName);

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


