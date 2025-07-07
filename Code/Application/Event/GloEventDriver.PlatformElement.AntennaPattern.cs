
using System;
using System.Collections.Generic;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add Antenna Pattern
    // ---------------------------------------------------------------------------------------------

    // Beams are the basic unit of a scan setting up an angular port, scanpatterns are added to them

    // The elem name passed in here is the portname from the message, we ue that to create a new antenna pattern element within the wider
    // AntennaPattern*s* element.

    private static string constElemName = "ElemAntennaPatterns";

    public void PlatformSetAntennaPatternMetadata(string platName, string portName, GloAzElBox azElBox, GloAzElRange offset)
    {
        // Get the platform, or return
        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);
        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0029: PlatformSetAntennaPatternMetadata: Platform {platName} not found.");
            return;
        }

        // Figure out to get the element or create it
        GloPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem == null)
        {
            // Add the element to the platform
            GloCentralLog.AddEntry($"WC0-0000: PlatformSetAntennaPatternMetadata: Creating new AntennaPatterns element // Plat:{platName} // Elem:{constElemName} // Port:{portName}.");
            elem = new GloPlatformElementAntennaPatterns() { Name = constElemName };
            AddPlatformElement(platName, constElemName, elem);
        }

        // See if the port already exists in the patterns
        if (elem.PatternForPortName(portName) != null)
        {
            GloCentralLog.AddEntry($"WC0-0001: PlatformSetAntennaPatternMetadata: Pattern {portName} already exists. Replacing it.");
            elem.RemoveAntennaPatterns(portName);
        }

        // Add the pattern to the element
        GloAntennaPattern newPattern = new GloAntennaPattern() { PortName = portName, PatternOffset = offset };
        elem.AddAntennaPattern(newPattern);
    }

    // ---------------------------------------------------------------------------------------------

    // Will only work if the metadata has been set up first

    public void PlatformSetAntennaPatternData(string platName, string portName, int azPointCount, int elPointCount, List<double> pattern)
    {
        // Access the AP element
        GloPlatformElementAntennaPatterns? elem = GetElementAntennaPattern(platName, constElemName);
        if (elem == null)
        {
            GloCentralLog.AddEntry($"EC0-0027: Platform:{platName} // Element:{constElemName} // PortName:{portName}.");
            return;
        }

        // Assign port on the element
        GloAntennaPattern? ap = elem.PatternForPortName(portName);
        if (ap == null)
        {
            GloCentralLog.AddEntry($"EC0-0028: PlatformSetAntennaPatternData: Pattern {portName} not found.");
            return;
        }

        // Validate the pattern data
        if (azPointCount < 10 || elPointCount < 10 || azPointCount > 1000 || elPointCount > 1000)
        {
            GloCentralLog.AddEntry($"WC0-0002: AzPointCount:{azPointCount} // ElPointCount:{elPointCount}.");
        }
        if (pattern.Count < 10)
        {
            GloCentralLog.AddEntry($"WC0-0003: Pattern.Count:{pattern.Count}.");
        }

        // Set further values on the pattern
        ap.SphereMagPattern = new GloFloat2DArray(azPointCount, elPointCount, pattern);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Access AP Element
    // ---------------------------------------------------------------------------------------------

    // Passing the data out of the EventDriver layer piecemeal in simple data types is currently considered
    // too repetitive, without concrete benefit.

    public GloPlatformElementAntennaPatterns? GetElementAntennaPattern(string platName, string elemName)
    {
        GloPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem != null)
        {
            if (basicElem is GloPlatformElementAntennaPatterns elemAP)
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
    //public GloPlatformElementBeam? GetBeamElement(string platName, string elemName)
    //{
        //GloPlatformElement? basicElem = GetElement(platName, elemName);
        //if (basicElem == null)
        //{
            //if (basicElem is GloPlatformElementBeam beam)
            //{
                //return beam;
            //}
        //}
        //return null;
    //}

}
