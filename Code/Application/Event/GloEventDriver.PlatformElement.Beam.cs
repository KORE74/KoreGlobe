
using System;
using System.Collections.Generic;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    // ---------------------------------------------------------------------------------------------
    // MARK: Add Beam
    // ---------------------------------------------------------------------------------------------

    // Beams are the basic unit of a scan setting up an angular port, scanpatterns are added to them

    public void PlatformAddBeam(string platName, string elemName)
    {
        GloCentralLog.AddEntry($"PlatformAddBeam: {platName} / {elemName}");

        GloPlatform? platform = GloAppFactory.Instance.PlatformManager.PlatForName(platName);

        if (platform == null)
        {
            GloCentralLog.AddEntry($"EC0-0011: PlatformAddScanWedge: Platform {platName} not found.");
            return;
        }

        // Clear any pre-existing
        GloPlatformElement? element = platform.ElementForName(elemName);
        if (element != null)
            platform.DeleteElement(elemName);

        // Create the element
        GloPlatformElementBeam newBeam = new GloPlatformElementBeam() { EmitName = elemName, Name = elemName };

        platform.AddElement(newBeam);
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Set Beam Components
    // ---------------------------------------------------------------------------------------------

    public void PlatformSetBeamTargeting(string platName, string elemName, bool isTargetting, string targetName)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);

        if (beamObj != null)
        {
            beamObj.Targeted       = isTargetting;
            beamObj.TargetPlatName = targetName;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0012: PlatformSetBeamTargeting: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamRanges(string platName, string elemName, double rxRangeM, double txRangeM)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);
        if (beamObj != null)
        {
            beamObj.DetectionRangeRxM = rxRangeM;
            beamObj.DetectionRangeTxM = txRangeM;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0013: PlatformSetBeamTargeting: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamPortAngles(string platName, string elemName, GloAttitude portAttitude)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);
        if (beamObj != null)
        {
            beamObj.PortAttitude = portAttitude;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0014: PlatformSetBeamAngles: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamAngles(string platName, string elemName, GloAzElRange trackOffset, GloAzElBox azElBox)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);
        if (beamObj != null)
        {
            beamObj.TrackOffset  = trackOffset;
            beamObj.AzElBox      = azElBox;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0015: PlatformSetBeamAngles: Element {elemName} not found.");
        }
    }

    public void PlatformSetBeamScanType(string platName, string elemName, string scanType, double periodSecs)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);
        if (beamObj != null)
        {
            beamObj.SetScanPattern(scanType);
            beamObj.PeriodSecs = (float)periodSecs;
            GloCentralLog.AddEntry($"PlatformSetBeamScanType: Element {elemName} // scanType // Type:{scanType} to Shape:{beamObj.ScanShape}");
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0016: PlatformSetBeamScanType: Element {elemName} not found.");
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Beam Enable
    // ---------------------------------------------------------------------------------------------

    public void PlatformEnableBeam(string platName, string elemName, bool enable)
    {
        GloPlatformElementBeam? beamObj = GetElementBeam(platName, elemName);
        if (beamObj != null)
        {
            beamObj.Enabled = enable;
        }
        else
        {
            GloCentralLog.AddEntry($"EC0-0016: PlatformEnableBeam: Element {elemName} not found.");
        }
    }



    // ---------------------------------------------------------------------------------------------
    // MARK: Access Beam Element
    // ---------------------------------------------------------------------------------------------

    // Passing the data out of the EventDriver layer piecemeal in simple data types is currently considered
    // too repetitive, without concrete benefit.

    public GloPlatformElementBeam? GetElementBeam(string platName, string elemName)
    {
        GloPlatformElement? basicElem = GetElement(platName, elemName);
        if (basicElem != null)
        {
            if (basicElem is GloPlatformElementBeam beam)
            {
                return beam;
            }
        }

        // Report the failure.

        // 1 - list the available elements
        // 2 - list the requested element

        List<string> elemNamesList = PlatformElementNames(platName);
        string elemNames = string.Join(", ", elemNamesList);

        GloCentralLog.AddEntry($"EC0-0017: GetElementBeam: Element {elemName} not found. Available elements: {elemNames}");

        return null;
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Delete Beams
    // ---------------------------------------------------------------------------------------------

    public void DeleteElementAllBeams()
    {
        GloAppFactory.Instance.PlatformManager.DeleteAllBeamElements();
        GloCentralLog.AddEntry("DeleteElementAllBeams");
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Element Name Helpers
    // ---------------------------------------------------------------------------------------------

    public static string ElementNameForBeam(string platName, string emitName, string beamName)
    {
        return $"{platName}_{emitName}_{beamName}";
    }

}
