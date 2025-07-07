using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class GloGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Update - ChaseCam
    // --------------------------------------------------------------------------------------------

//     private void AddChaseCam(string platName)
//     {
//         GloGodotEntity? ent = GetEntity(platName);

//         if (ent != null)
//         {
//             GloCameraPolarOffset chaseCam = new GloCameraPolarOffset();
//             chaseCam.Name = "ChaseCam";

//             ent.AddChild(chaseCam);

//             // Setup the default view
//             chaseCam.SetCameraPosition(300, 20, 20);
//         }
//     }

    public void EnableChaseCam(string platName)
    {
        GloGodotEntity? ent = GetEntity(platName);

        if (ent != null)
        {
            ent.EnableChaseCam();


            // GloCameraPolarOffset? chaseCam = ent.GetNodeOrNull<GloCameraPolarOffset>("GloCameraPolarOffset");

            // if (chaseCam != null)
            // {
            //     chaseCam.CamNode.Current = true;
            // }
        }
        else
        {
            GloCentralLog.AddEntry($"GloGodotEntityManager.EnableChaseCam: Entity not found: {platName}");

            List<string> entnames = EntityNames();
            GloCentralLog.AddEntry($"GloGodotEntityManager.EnableChaseCam: Available entities: {string.Join(", ", entnames)}");


        }
    }
}


