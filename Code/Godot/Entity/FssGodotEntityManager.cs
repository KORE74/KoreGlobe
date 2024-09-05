using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{
    // List<FssGodotEntity> EntityList = new List<FssGodotEntity>();

    public Node3D EntityRootNode;
    public Node3D ElementRootNode;

    float TimerModelCheck = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "EntityManager";

        // Setup the Entity Root Node.
        EntityRootNode = new Node3D() { Name = "EntityRootNode" };
        AddChild(EntityRootNode);

        // Setup the Element Root Node.
        ElementRootNode = new Node3D() { Name = "UnlinkedRootNode" };
        AddChild(ElementRootNode);
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
        {
            FssGodotEntity newEntity = new FssGodotEntity();
            newEntity.Name = entityName;
            EntityRootNode.AddChild(newEntity);
        }
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
        List<string> platNames = FssAppFactory.Instance.EventDriver.PlatformNames();
        List<string> godotEntityNames = EntityNames();

        // Compare the two lists, to find the new, the deleted and the consistent
        List<string> omittedInPresentation = FssStringListOperations.ListOmittedInSecond(platNames, godotEntityNames);
        List<string> noLongerInModel       = FssStringListOperations.ListOmittedInSecond(godotEntityNames, platNames);
        List<string> maintainedEnitites    = FssStringListOperations.ListInBoth(platNames, godotEntityNames);

        // Loop through the list of platform names, and the EntityList, match them up.
        foreach (string currModelName in omittedInPresentation)
            AddEntity(currModelName);

        foreach (string currModelName in noLongerInModel)
            RemoveEntity(currModelName);

        foreach (string currModelName in maintainedEnitites)
            MatchModelPlatformElements(currModelName);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Add Elements
    // --------------------------------------------------------------------------------------------

    private void MatchModelPlatformElements(string platName)
    {
        List<string> modelElementNames = FssAppFactory.Instance.EventDriver.PlatformElementNames(platName);

        // List the unlinked elements for the platform.
        List<string> unlinkedElementNames = UnlinkedElementNames(platName);
        List<string> linkedElementNames   = LinkedElementNames(platName);

        // Join the two lists
        List<string> allElementNames = new List<string>();
        allElementNames.AddRange(unlinkedElementNames);
        allElementNames.AddRange(linkedElementNames);

        // Loop through the list of model elements, and the EntityList, match them up.
        List<string> omittedInPresentation = FssStringListOperations.ListOmittedInSecond(modelElementNames, allElementNames);
        List<string> noLongerInModel       = FssStringListOperations.ListOmittedInSecond(allElementNames, modelElementNames);
        List<string> maintainedEnitites    = FssStringListOperations.ListInBoth(allElementNames, modelElementNames);

        foreach (string currElemName in omittedInPresentation)
        {
            FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetElement(platName, currElemName);
            if (element != null)
            {
                if (element is FssPlatformElementRoute)
                {
                    FssPlatformElementRoute r = element as FssPlatformElementRoute;

                    FssGodotPlatformElementRoute newRoute = new FssGodotPlatformElementRoute();
                    newRoute.Name = currElemName;
                    newRoute.SetRoutePoints(r.RoutePoints);

                    // Add the route to the entity and scene tree
                    AddUnlinkedElement(platName, newRoute);

                    FssCentralLog.AddEntry($"Added route element {currElemName} to {platName}");
                }
                else
                {
                    FssCentralLog.AddEntry($"Did not add element {currElemName} to {platName}");
                }
            }
        }
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
