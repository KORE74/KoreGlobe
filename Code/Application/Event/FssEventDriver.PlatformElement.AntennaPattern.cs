
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

    public void PlatformSetAntennaPatternMetadata(string platName, string portName, FssAzElBox azElBox, FssPolarOffset offset)
    {
        // Get the platform, or return
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);
        if (platform == null)
        {
            FssCentralLog.AddEntry($"EC0-0029: PlatformSetAntennaPatternMetadata: Platform {platName} not found.");
            return;
        }

        // Figure out to get the element or create it
        FssPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem == null)
        {
            // Add the element to the platform
            FssCentralLog.AddEntry($"WC0-0000: PlatformSetAntennaPatternMetadata: Creating new AntennaPatterns element // Plat:{platName} // Elem:{constElemName} // Port:{portName}.");
            elem = new FssPlatformElementAntennaPatterns() { Name = constElemName };
            AddPlatformElement(platName, constElemName, elem);
        }

        // See if the port already exists in the patterns
        if (elem.PatternForPortName(portName) != null)
        {
            FssCentralLog.AddEntry($"WC0-0001: PlatformSetAntennaPatternMetadata: Pattern {portName} already exists. Replacing it.");
            elem.RemoveAntennaPatterns(portName);
        }

        // Add the pattern to the element
        FssAntennaPattern newPattern = new FssAntennaPattern() { PortName = portName, PatternOffset = offset };
        elem.AddAntennaPattern(newPattern);
    }

    // ---------------------------------------------------------------------------------------------

    // Will only work if the metadata has been set up first

    public void PlatformSetAntennaPatternData(string platName, string portName, int azPointCount, int elPointCount, List<double> pattern)
    {
        // Access the AP element
        FssPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem == null)
        {
            FssCentralLog.AddEntry($"EC0-0027: Platform:{platName} // Element:{constElemName} // PortName:{portName}.");
            return;
        }

        // Assign port on the element
        FssAntennaPattern? ap = elem.PatternForPortName(portName);
        if (ap == null)
        {
            FssCentralLog.AddEntry($"EC0-0028: PlatformSetAntennaPatternData: Pattern {portName} not found.");
            return;
        }

        // Validate the pattern data
        if (azPointCount < 10 || elPointCount < 10 || azPointCount > 1000 || elPointCount > 1000)
        {
            FssCentralLog.AddEntry($"WC0-0002: AzPointCount:{azPointCount} // ElPointCount:{elPointCount}.");
        }
        if (pattern.Count < 10)
        {
            FssCentralLog.AddEntry($"WC0-0003: Pattern.Count:{pattern.Count}.");
        }

        // Set further values on the pattern
        ap.SphereMagPattern = new FssFloat2DArray(azPointCount, elPointCount, pattern);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Access AP Element
    // ---------------------------------------------------------------------------------------------

    // Passing the data out of the EventDriver layer piecemeal in simple data types is currently considered
    // too repetitive, without concrete benefit.

    public FssPlatformElementAntennaPatterns? GetElementAntennaPattern(string platName, string elemName)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem != null)
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
