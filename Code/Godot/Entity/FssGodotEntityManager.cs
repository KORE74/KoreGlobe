using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{
    List<FssGodotEntity> EntityList = new List<FssGodotEntity>();

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
        ElementRootNode = new Node3D() { Name = "ElementRootNode" };
        AddChild(ElementRootNode);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (TimerModelCheck < FssCoreTime.RuntimeSecs)
        {
            TimerModelCheck = FssCoreTime.RuntimeSecs + 1f;
            MatchPlatformNodesToModel();
            DeleteOrphanedEntities();
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
            AddUnlinkedPlatform(entityName);
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



    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void MatchPlatformNodesToModel()
    {
        // Get the model
        List<string> platNames = FssAppFactory.Instance.EventDriver.PlatformNames();


        // Loop through the list of platform names, and the EntityList, match them up.
        foreach (string currModelName in platNames)
        {
            bool matchFound = false;
            foreach (FssGodotEntity currEntity in EntityList)
            {
                if (currEntity.Name == currModelName)
                {
                    matchFound = true;
                    continue; // We have it in the model, and the entity list. No action.
                }
            }

            // Match not found, add it.
            if (!matchFound)
            {
                // We have it in the model, but not in the entity list. Add it.
                FssGodotEntity newEntity = new FssGodotEntity();
                newEntity.Name = currModelName;
                newEntity.EntityName = currModelName;
                EntityList.Add(newEntity);
                AddChild(newEntity);
            }


            LookForElementsToPresent(currModelName);
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Add Elements
    // --------------------------------------------------------------------------------------------

    private void LookForElementsToPresent(string platName)
    {
        // Get the list of model elements for the platform.
        List<string> modelElementNames = FssAppFactory.Instance.EventDriver.PlatformElementNames(platName);

        // List the unlinked elements for the platform.
        List<string> unlinkedElementNames = UnlinkedElementNames(platName);


        // Loop through the list of model elements, and the EntityList, match them up.
        foreach (string currModelElementName in modelElementNames)
        {
            bool matchFound = false;
            foreach (FssGodotEntity currEntity in EntityList)
            {
                if (currEntity.Name == currModelElementName)
                {
                    matchFound = true;
                    continue; // We have it in the model, and the entity list. No action.
                }
            }

            // Match not found, add it.
            if (!matchFound)
            {
                // Get the model element to represent
                FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetElement(platName, currModelElementName);

                if (element == null)
                    continue;

                // Determine the element type
                if (element!.Type == "Route")
                {
                    // We have it in the model, but not in the entity list. Add it.
                    FssElementRoute newRoute = new FssElementRoute();
                    newRoute.Name = $"{platName}-{currModelElementName}";
                    //newRoute.ElementName = currModelElementName;
                    newRoute.SetRoutePoints( (element as FssPlatformElementRoute)!.Points );

                    // Add the route to the entity and scene tree
                    //EntityList.Add(newRoute);
                    ElementRootNode.AddChild(newRoute);
                }


                // FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetPlatformElement(platName, currModelElement);

                // // We have it in the model, but not in the entity list. Add it.
                // FssGodotEntity newEntity = new FssGodotEntity();

            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Reporting
    // --------------------------------------------------------------------------------------------

    public string FullReport()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"Entity Count: {EntityList.Count}");

        foreach (FssGodotEntity currEntity in EntityList)
        {
            sb.AppendLine($"Entity: {currEntity.Name}");
        }

        return sb.ToString();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Delete
    // --------------------------------------------------------------------------------------------

    private void DeleteOrphanedEntities()
    {
        List<FssGodotEntity> entitiesToDelete = new List<FssGodotEntity>();
        foreach (FssGodotEntity currEntity in EntityList)
        {
            if (currEntity.Name == "EntityManager")
                continue;

            if (!FssAppFactory.Instance.EventDriver.DoesPlatformExist(currEntity.Name))
            {
                entitiesToDelete.Add(currEntity);
            }
        }

        foreach (FssGodotEntity currEntity in entitiesToDelete)
        {
            string platName = currEntity.Name;

            EntityList.Remove(currEntity);
            DeleteElementsForPlatform(platName);
            currEntity.QueueFree();
        }
    }

    private void DeleteElementsForPlatform(string platName)
    {
        string platNamePrefix = $"{platName}-";

        // Delete the elements for the platform.
        Godot.Collections.Array<Node> children = ElementRootNode.GetChildren();
        foreach (Node currChild in children)
        {
            string name = currChild.Name;
            if (name.Contains(platNamePrefix))
                currChild.QueueFree();
        }
    }

}
