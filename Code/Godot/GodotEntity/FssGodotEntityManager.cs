using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class FssGodotEntityManager : Node3D
{
    // List<FssGodotEntity> EntityList = new List<FssGodotEntity>();

    public Node3D EntityRootNode   = new Node3D() { Name = "EntityRootNode" };
    public Node3D UnlinkedRootNode = new Node3D() { Name = "UnlinkedRootNode" };

    float TimerModelCheck = 0.0f;

    private Fss1DMappedRange InfographicScaleRange = new Fss1DMappedRange();

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "EntityManager";

        // Setup the Root Nodes.
        AddChild(EntityRootNode);
        AddChild(UnlinkedRootNode);

        // Setup the Infographic Scale Range
        InfographicScaleRange.AddEntry(1,          1);
        InfographicScaleRange.AddEntry(4,         16);
        InfographicScaleRange.AddEntry(8,        100);
        InfographicScaleRange.AddEntry(10,       400);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerModelCheck < FssCoreTime.RuntimeSecs)
        {
            TimerModelCheck = FssCoreTime.RuntimeSecs + 1f;
            MatchModelPlatforms();

            // DeleteOrphanedEntities();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Entities
    // --------------------------------------------------------------------------------------------
    // Entities, mainly platforms, are the moving objects in the simulation. They are parented off
    // EntityRootNode. We need routines to add, delete list and edit them.

    public bool EntityExists(string entityName)
    {
        // Look in the child nodes of the EntityRootNode for the entity.
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return true;
        }

        return false;
    }

    public void AddEntity(string entityName)
    {
        if (!EntityExists(entityName))
            EntityRootNode.AddChild( new FssGodotEntity() { EntityName = entityName } );
    }

    public void RemoveEntity(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
            {
                currNode.QueueFree();
                RemoveUnlinkedPlatform(entityName);
                return;
            }
        }
    }

    public FssGodotEntity? GetEntity(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return currNode as FssGodotEntity;
        }

        return null;
    }

    public List<string> EntityNames()
    {
        List<string> names = new List<string>();

        foreach (Node3D currNode in EntityRootNode.GetChildren())
            names.Add(currNode.Name);

        return names;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    // Create and delete entities to keep uptodate with the model.

    public void MatchModelPlatforms()
    {
        // Get the model
        List<string> modelEntityNames  = FssAppFactory.Instance.EventDriver.EntityNames();
        List<string> godotEntityNames = EntityNames();

        // Compare the two lists, to find the new, the deleted and the consistent
        List<string> omittedInPresentation = FssStringListOperations.ListOmittedInSecond(modelEntityNames, godotEntityNames);
        List<string> noLongerInModel       = FssStringListOperations.ListOmittedInSecond(godotEntityNames, modelEntityNames);
        List<string> maintainedEnitites    = FssStringListOperations.ListInBoth(modelEntityNames, godotEntityNames);

        bool  addInfographicScale = FssGodotFactory.Instance.UIState.IsRwScale;
        float scaleModifier       = FssValueUtils.Clamp(FssGodotFactory.Instance.UIState.InfographicScale, 1f, 10f);


        // Loop through the list of platform names, and the EntityList, match them up.
        foreach (string currModelName in omittedInPresentation)
        {
            AddEntity(currModelName);
            // AddChaseCam(currModelName);
        }

        foreach (string currModelName in noLongerInModel)
            RemoveEntity(currModelName);

        foreach (string currModelName in maintainedEnitites)
        {
            // MatchModelPlatformElements(currModelName);
            MatchModelPlatform3DModel(currModelName);

            // Set the scale of the model
            string platformType = FssAppFactory.Instance.EventDriver.EntityAttrib(currModelName, "type") ?? "default";

            SetModelScale(currModelName, platformType, addInfographicScale, scaleModifier);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Add Elements
    // --------------------------------------------------------------------------------------------


    // --------------------------------------------------------------------------------------------
    // MARK: Routes
    // --------------------------------------------------------------------------------------------

    public void AddPlatformElementRoute(string platName, string currElemName)
    {
        // // Get the Route Details
        // FssPlatformElementRoute? route = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementRoute;

        // FssGodotPlatformElementRoute newRoute = new FssGodotPlatformElementRoute();

        // if ((route != null) && (newRoute != null))
        // {
        //     newRoute.Name = currElemName;
        //     newRoute.SetRoutePoints(route!.RoutePoints);

        //     // Update visibility based on the UI state
        //     newRoute.SetVisibility(FssGodotFactory.Instance.UIState.ShowRoutes);

        //     // Add the route to the entity and scene tree
        //     AddUnlinkedElement(platName, newRoute);

        //     FssCentralLog.AddEntry($"Added route element {currElemName} to {platName}");
        // }
    }

    public void UpdatePlatformElementRoute(string platName, string currElemName)
    {
        // // Get the Route Details
        // FssPlatformElementRoute? route = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName) as FssPlatformElementRoute;

        // // Get the godot route we'll update
        // FssGodotPlatformElementRoute? routeNode = GetUnlinkedElement(platName, currElemName) as FssGodotPlatformElementRoute;

        // if ((route != null) && (routeNode != null))
        // {
        //     // Update visibility based on the UI state
        //     routeNode!.SetVisibility(FssGodotFactory.Instance.UIState.ShowRoutes);
        //     routeNode!.SetRoutePoints(route.RoutePoints);
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reporting
    // --------------------------------------------------------------------------------------------

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();


        List<string> namesList = EntityNames();

        sb.AppendLine($"Entity Count: {namesList.Count}");
        foreach (string currName in namesList)
        {
            sb.AppendLine($"Entity: {currName}");
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Delete
    // --------------------------------------------------------------------------------------------

}


