
using System;
using System.Collections.Generic;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add Beam
    // ---------------------------------------------------------------------------------------------

    // Beams are the basic unit of a scan setting up an angular port, scanpatterns are added to them

    public void PlatformAddBeam(string platName, string elemName)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddScanWedge: Platform {platName} not found.");
            return;
        }

        // Clear any pre-existing
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);

        // Create the element
        FssPlatformElementBeam newBeam = new FssPlatformElementBeam() { EmitName = elemName };

        platform.AddElement(newBeam);
    }

    // ---------------------------------------------------------------------------------------------

    public void PlatformSetBeamTargeting(string platName, string elemName, bool isTargetting, string targetName)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementBeam beam)
            {
                beam.Targeted = isTargetting;
                beam.TargetPlatName = targetName;
            }
            else
            {
                FssCentralLog.AddEntry($"E00003: PlatformSetBeamTargeting: Element {elemName} is not a beam.");
            }
        }
        else
        {
            FssCentralLog.AddEntry($"E00003: PlatformSetBeamTargeting: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamRanges(string platName, string elemName, double rxRangeKms, double txRangeKms)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementBeam beam)
            {
                beam.DetectionRangeRxKms = rxRangeKms;
                beam.DetectionRangeTxKms = txRangeKms;
            }
            else
            {
                FssCentralLog.AddEntry($"E00003: PlatformSetBeamTargeting: Element {elemName} is not a beam.");
            }
        }
        else
        {
            FssCentralLog.AddEntry($"E00003: PlatformSetBeamTargeting: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamAngles(string platName, string elemName, FssAttitude portAttitude, FssPolarOffset trackOffset)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementBeam beam)
            {
                beam.PortAttitude = portAttitude;
                beam.TrackOffset  = trackOffset;
            }
            else
            {
                FssCentralLog.AddEntry($"E00003: PlatformSetBeamAngles: Element {elemName} is not a beam.");
            }
        }
        else
        {
            FssCentralLog.AddEntry($"E00003: PlatformSetBeamAngles: Element {elemName} not found.");
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Add Scan Patterns
    // ---------------------------------------------------------------------------------------------

    public void PlatformAddScanPattern(string platName, string elemName, string patternType)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementBeam beam)
            {
                FssPlatformElementBeamScanPattern newScanPattern = new FssPlatformElementBeamScanPattern();
                beam.ScanPattern = newScanPattern;
            }
            else
            {
                FssCentralLog.AddEntry($"E00003: PlatformAddScanPattern: Element {elemName} is not a beam.");
            }
        }
    }

    public void PlatformSetScanPatternAngles(string platName, string elemName, FssAzElBox azElBox)
    {
        FssPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem == null)
        {
            if (basicElem is FssPlatformElementBeam beam)
            {
                if (beam.ScanPattern != null)
                {
                    beam.ScanPattern.AzElBox = azElBox;
                }
                else
                {
                    FssCentralLog.AddEntry($"E00003: PlatformSetScanPatternAngles: No scan pattern found for {elemName}.");
                }
            }
            else
            {
                FssCentralLog.AddEntry($"E00003: PlatformSetScanPatternAngles: Element {elemName} is not a beam.");
            }
        }
        else
        {
            FssCentralLog.AddEntry($"E00003: PlatformSetScanPatternAngles: Element {elemName} not found.");
        }
    }

    // ---------------------------------------------------------------------------------------------

    public void PlatformAddScanHemisphere(string platName, string elemName, double DetectionRangeKms)
    {

    }


    public void PlatformAddScanWedge(string platName, string elemName, double DetectionRangeKms, double DetectionRangeRxMtrs, FssAzElBox azElBox)
    {

        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddScanWedge: Platform {platName} not found.");
            return;
        }

        // Get the element
        FssPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);

        // Create the element
        FssPlatformElementOperations.CreatePlatformElement(platName, elemName, "ScanWedge");


    }


    public void PlatformAddScanConical(string platName, string elemName, double DetectionRangeKms, string targetName)
    {

    }


    public void PlatformAddAntennaPattern(string platName, string elemName, FssPolarOffset offset, FssAzElBox azElBox)
    {
        FssPlatform? platform = FssAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            FssCentralLog.AddEntry($"E00003: PlatformAddAntennaPattern: Platform {platName} not found.");
            return;
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public static string ElementNameForBeam(string platName, string emitName, string beamName)
    {
        return $"{platName}_{emitName}_{beamName}";
    }

    // ---------------------------------------------------------------------------------------------
}
