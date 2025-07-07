using System;
using System.Collections.Generic;
using System.Text;

using GloJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public partial class GloPlatformManager
{
    public void DeleteAllBeamElements()
    {
        // Loop through all of the platforms
        List<string> nameList = PlatNameList();

        foreach (GloPlatform currPlat in PlatfomList)
        {
            List<GloPlatformElement> elemList = currPlat.ElementsList;

            // Loop through the elements list in reverse
            for (int i = elemList.Count - 1; i >= 0; i--)
            {
                GloPlatformElement elem = elemList[i];

                // If the element type is a beam, delete it
                if (elem is GloPlatformElementBeam)
                {
                    currPlat.DeleteElement(elem);
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------


}


