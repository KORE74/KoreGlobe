using System;
using System.Collections.Generic;

using FssJSON;

#nullable enable

// Class to provide the top level management of platforms in the system.
public class FssMessageManagerElement
{
    // Pool of incoming messages to queue up for processing
    private JSONThreadsafeMessageFIFO PendingMessageList = new();

    // --------------------------------------------------------------------------------------------

    public void QueueMessage(JSONMessage msg)
    {
        PendingMessageList.EnqueueMessage(msg);
    }

    public void ApplyMessages()
    {
        while (PendingMessageList.TryDequeueMessage(out JSONMessage currMsg))

        // foreach (JSONMessage currMsg in PendingMessageList)
        {
            if (currMsg == null) continue;

            // Data
            if      (currMsg is BeamLoad loadMsg)    { ProcessMessage_BeamLoad(loadMsg); } // Load. Enable shows it later.
            else if (currMsg is BeamDelete deleteMsg)  { ProcessMessage_BeamDelete(deleteMsg); }
            else if (currMsg is RxAntenna antennaMsg)   { ProcessMessage_RxAntenna(antennaMsg); }

            // Presentation
            else if (currMsg is BeamEnable enableMsg)  { ProcessMessage_BeamEnable(enableMsg); }
            else if (currMsg is BeamDisable disableMsg) { ProcessMessage_BeamDisable(disableMsg); }
        }
    }

    // ========================================================================================
    // Emitter Messages
    // ========================================================================================

    // Data based Beam messages

    private void ProcessMessage_BeamLoad(BeamLoad msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Derive the names to look everything up.
        string platName = msg.PlatName;
        string elemName = BeamElementName(msg.PlatName, msg.EmitName, msg.BeamName);

        // delete any pre-existing beam under the name we want to use
        platMgr.DeleteElement(platName, elemName);

        // Create and add the new emitter
        FssPlatformElementScanPattern newEmitter = new FssPlatformElementScanPattern();
        newEmitter.ScanShape         = ScanPatternShape.Wedge;
        newEmitter.EmitterRangeKms   = msg.DetectionRangeKms;
        newEmitter.DetectionRangeKms = msg.DetectionRangeRxMtrs;

        newEmitter.AzElBox = new FssAzElBox() {
            MinAzDegs = msg.AzMinDegs,
            MaxAzDegs = msg.AzMaxDegs,
            MinElDegs = msg.ElMinDegs,
            MaxElDegs = msg.ElMaxDegs
        };

        if (newEmitter != null)
            platMgr.AddElement(platName, newEmitter);
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_BeamDelete(BeamDelete msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Derive the names to look everything up.
        string platName = msg.PlatName;
        string elemName = BeamElementName(msg.PlatName, msg.EmitName, msg.BeamName);

        // Get the element (or null if not found).
        FssPlatformElement? element = platMgr.ElementForName(platName, elemName);

        if (element == null)
        {
            //element.SetVisibility(true);
            FssCentralLog.AddEntry($"BeamEnable: Beam Enabled: platName:{platName} elemName:{elemName}");
        }
        else
        {
            FssCentralLog.AddEntry($"BeamEnable: Entity not found: platName:{platName}");
        }
    }

    // --------------------------------------------------------------------------------
    // Presentation only Beam messages
    // --------------------------------------------------------------------------------

    private void ProcessMessage_BeamEnable(BeamEnable msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Derive the names to look everything up.
        string platName = msg.PlatName;
        string elemName = BeamElementName(msg.PlatName, msg.EmitName, msg.BeamName);

        // Get the element (or null if not found).
        FssPlatformElement? element = platMgr.ElementForName(platName, elemName);

        if (element == null)
        {
            //element.SetVisibility(true);
            FssCentralLog.AddEntry($"BeamEnable: Beam Enabled: platName:{platName} elemName:{elemName}");
        }
        else
        {
            FssCentralLog.AddEntry($"BeamEnable: Entity not found: platName:{platName}");
        }
    }

    private void ProcessMessage_BeamDisable(BeamDisable msg)
    {
        // Get the access to the platform objects
        FssPlatformManager? platMgr = PlatMgr();
        if (platMgr == null) return;

        // Derive the names to look everything up.
        string platName = msg.PlatName;
        string elemName = BeamElementName(msg.PlatName, msg.EmitName, msg.BeamName);

        // Get the element (or null if not found).
        FssPlatformElement? element = platMgr.ElementForName(platName, elemName);

        if (element == null)
        {
            //element.SetVisibility(false);
            FssCentralLog.AddEntry($"BeamEnable: Beam Enabled: platName:{platName} elemName:{elemName}");
        }
        else
        {
            FssCentralLog.AddEntry($"BeamEnable: Entity not found: platName:{platName}");
        }
    }

    // --------------------------------------------------------------------------------

    private void ProcessMessage_RxAntenna(RxAntenna msg)
    {
        // string platName  = msg.PlatName;
        // string portName = msg.PortName;
        // FssCentralLog.AddEntry($"ProcessMessage_RxAntenna: Entity not found: platName:{platName} portName:{portName}");

        // FssEntity ent = EntityManager.GetEntityByName(platName);
        // if (ent != null)
        // {

        // }
        // else
        // {
        //     FssCentralLog.AddEntry($"RxAntenna: Entity not found: platName:{platName}");
        // }
    }

    // ========================================================================================
    // Utils
    // ========================================================================================

    // Isolate the external architectural access to the platform manager in a utility function.
    private FssPlatform? PlatformForName(string name)
    {
        return FssAppFactory.Instance.PlatformManager.PlatForName(name);
    }

    private FssPlatformManager? PlatMgr()
    {
        return FssAppFactory.Instance.PlatformManager;
    }

    // ========================================================================================

    private string BeamElementName(string platname, string emitname, string beamname)
    {
        return $"{platname}-{emitname}-{beamname}";
    }

    private string PortElementName(string platname, string portName)
    {
        return $"{platname}-{portName}";
    }

}


