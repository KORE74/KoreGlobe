using Godot;
using System;
using System.Collections.Generic;

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
    // MARK: Update
    // --------------------------------------------------------------------------------------------


}
