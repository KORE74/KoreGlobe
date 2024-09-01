using System;
using System.Collections.Generic;

using Godot;

public partial class FssGodotEntityManager : Node3D
{
    List<FssGodotEntity> EntityList = new List<FssGodotEntity>();

    float TimerModelCheck = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Name = "EntityManager";
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
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update - Add Elements
    // --------------------------------------------------------------------------------------------

    private void LookForElementsToPresent(string platName)
    {
        // Get the list of model elements for the platform.
        List<string> modelElementNames = FssAppFactory.Instance.EventDriver.PlatformElementNames(platName);

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
                // FssPlatformElement? element = FssAppFactory.Instance.EventDriver.GetPlatformElement(platName, currModelElement);

                // // We have it in the model, but not in the entity list. Add it.
                // FssGodotEntity newEntity = new FssGodotEntity();

            }
        }
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
        Godot.Collections.Array<Node> children = FssZeroOffset.ZeroNode.GetChildren();
        foreach (Node currChild in children)
        {
            string name = currChild.Name;
            if (name.Contains(platNamePrefix))
                currChild.QueueFree();
        }
    }

}
