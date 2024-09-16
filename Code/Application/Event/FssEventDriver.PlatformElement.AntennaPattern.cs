
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

    // The elem name passed in here is the portname from the message, we ue that to create a new antenna pattern element within the wider
    // AntennaPattern*s* element.

    private static string constElemName = "ElemAntennaPatterns";

    public void PlatformSetAntennaPatternMetadata(string platName, string elemName, FssAzElBox azElBox)
    {
        FssPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem != null)
        {
            FssCentralLog.AddEntry($"EC0-0030: PlatformSetAntennaPatternMetadata: Element {elemName} already exists.");
        }

        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0029: PlatformSetAntennaPatternMetadata: Platform {platName} not found.");
            return;
        }

        // Add the element to the platform
        FssPlatformElementAntennaPatterns newElem = new FssPlatformElementAntennaPatterns() { Name = constElemName };
        platform.AddElement(newElem);

        // Add the pattern to the element
        FssAntennaPattern newPattern = new FssAntennaPattern() { PortName = elemName };
        newElem.AddAntennaPattern(newPattern);
    }

    // ---------------------------------------------------------------------------------------------

    public void PlatformSetAntennaPatternData(string platName, string elemName, int azPointsCount, int elPointsCount, List<double> pattern)
    {
        // Access the AP element
        FssPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem == null)
        {
            FssCentralLog.AddEntry($"EC0-0027: PlatformSetAntennaPatternData: Element {elemName} not found.");
            return;
        }

        // Assign port on the element
        FssAntennaPattern? ap = elem.PatternForPortName(elemName);
        if (ap == null)
        {
            FssCentralLog.AddEntry($"EC0-0028: PlatformSetAntennaPatternData: Pattern {elemName} not found.");
            return;
        }

        // Set further values on the pattern
        ap.SphereMagPattern = new FssFloat2DArray(azPointsCount, elPointsCount, pattern);
    }

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
