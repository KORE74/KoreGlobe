using System;
using System.Collections.Generic;
using System.Text;

using FssJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public partial class FssPlatformManager
{
    private int NearPlatformIndex = 0;
    private int FarPlatformIndex  = 0;

    private string invalidPlatformName = "- - -";

    // --------------------------------------------------------------------------------------------

    public bool NearPlatformValid() => NearPlatformIndex >= 0 && NearPlatformIndex < PlatfomList.Count;
    public bool FarPlatformValid()  => FarPlatformIndex >= 0  && FarPlatformIndex < PlatfomList.Count;

    // --------------------------------------------------------------------------------------------

    public string NearPlatformName()
    {
        if (NearPlatformValid())
        {
            return PlatfomList[NearPlatformIndex].Name;
        }
        return invalidPlatformName;
    }

    public string FarPlatformName()
    {
        if (FarPlatformValid())
        {
            return PlatfomList[FarPlatformIndex].Name;
        }
        return invalidPlatformName;
    }

    // --------------------------------------------------------------------------------------------

    public void NearPlatformNext()
    {
        if (NearPlatformValid())
        {
            NearPlatformIndex = (NearPlatformIndex + 1) % PlatfomList.Count;
            ApplyNearFarExclusivity();
        }
    }

    public void NearPlatformPrev()
    {
        if (NearPlatformValid())
        {
            NearPlatformIndex = (NearPlatformIndex - 1 + PlatfomList.Count) % PlatfomList.Count;
            ApplyNearFarExclusivity();
        }
    }

    public void FarPlatformNext()
    {
        if (FarPlatformValid())
        {
            FarPlatformIndex = (FarPlatformIndex + 1) % PlatfomList.Count;
            ApplyNearFarExclusivity();
        }
    }

    public void FarPlatformPrev()
    {
        if (FarPlatformValid())
        {
            FarPlatformIndex = (FarPlatformIndex - 1 + PlatfomList.Count) % PlatfomList.Count;
            ApplyNearFarExclusivity();
        }
    }

    private void ApplyNearFarExclusivity()
    {
        // The near and far targets cannot both be valid and the same platform.
        if (NearPlatformValid() && FarPlatformValid() && NearPlatformIndex == FarPlatformIndex)
        {
            FarPlatformIndex = (FarPlatformIndex + 1) % PlatfomList.Count;
        }
    }

    // --------------------------------------------------------------------------------------------
}


