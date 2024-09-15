
using System;
using System.Collections.Generic;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add Antenna Pattern
    // ---------------------------------------------------------------------------------------------

    // Beams are the basic unit of a scan setting up an angular port, scanpatterns are added to them

    public void PlatformSetAntennaPatternMetadata(string platName, string elemName, FssAzElBox azElBox)
    {
        if (GetElementAntennaPattern(platName, elemName) != null)
        {
            FssCentralLog.AddEntry($"EC0-0011: PlatformSetAntennaPatternMetadata: Element {elemName} already exists.");
        }

        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0011: PlatformSetAntennaPatternMetadata: Platform {platName} not found.");
            return;
        }

        FssPlatformElementAntennaPatterns newElem = new FssPlatformElementAntennaPatterns() { Name = elemName };

        platform.AddElement(newElem);
    }

    public void PlatformSetAntennaPatternData(string platName, string elemName, int azPointsCount, int elPointsCount, List<double> pattern)
    {
        FssPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, elemName);

        if (elem == null)
        {
            FssCentralLog.AddEntry($"EC0-0011: PlatformSetAntennaPatternData: Element {elemName} not found.");
            return;
        }

        // Assign the new data to the element
        elem.SphereMagPattern = new FssFloat2DArray(azPointsCount, elPointsCount, pattern);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Set Beam Components
    // ---------------------------------------------------------------------------------------------


    // ---------------------------------------------------------------------------------------------
    // MARK: Access AP Element
    // ---------------------------------------------------------------------------------------------

    // Passing the data out of the EventDriver layer piecemeal in simple data types is currently considered
    // too repetitive, without concrete benefit.

    public FssPlatformElementAntennaPatterns? GetElementAntennaPattern(string platName, string elemName)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementAntennaPatterns elemAP)
            {
                return elemAP;
            }
        }
        return null;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    // public static string ElementNameForBeam(string platName, string emitName, string beamName)
    // {
    //     return $"{platName}_{emitName}_{beamName}";
    // }
//
    //public FssPlatformElementBeam? GetBeamElement(string platName, string elemName)
    //{
        //FssPlatformElement? basicElem = GetElement(platName, elemName);
        //if (basicElem == null)
        //{
            //if (basicElem is FssPlatformElementBeam beam)
            //{
                //return beam;
            //}
        //}
        //return null;
    //}

}
