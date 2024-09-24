using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{


    // --------------------------------------------------------------------------------------------
    // MARK: Update - ChaseCam
    // --------------------------------------------------------------------------------------------

    private void AddChaseCam(string platName)
    {
        FssGodotEntity? ent = GetEntity(platName);

        if (ent != null)
        {
            FssCameraPolarOffset chaseCam = new FssCameraPolarOffset();
            chaseCam.Name = "ChaseCam";

            ent.AddChild(chaseCam);
        }
    }

    public void EnableChaseCam(string platName)
    {
        FssGodotEntity? ent = GetEntity(platName);

        if (ent != null)
        {
            FssCameraPolarOffset? chaseCam = ent.GetNodeOrNull<FssCameraPolarOffset>("FssCameraPolarOffset");

            if (chaseCam != null)
            {
                chaseCam.CamNode.Current = true;
            }
        }

    }

}


