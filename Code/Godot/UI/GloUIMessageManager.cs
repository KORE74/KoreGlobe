

// GloUIMessageManager: A node class that processes messages on the main thread.

using Godot;

using GloJSON;

public partial class GloUIMessageManager : Node
{
    // --------------------------------------------------------------------------------------------
    // MARK: Node Overrides
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        GloCentralLog.AddEntry("GloUIMessageManager._Ready");
        Name = "GloUIMessageManager";
    }

    public override void _Process(double delta)
    {
        // Process any messages in the queue
        ProcessMessages();
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessages()
    {
        JSONMessage newMsg;
        bool gotMsg = KoreGodotFactory.Instance.UIMsgQueue.TryDequeueMessage(out newMsg);

        if (gotMsg)
        {
            if (newMsg is PlatFocus platFocusMsg) { ProcessMessage_PlatFocus(platFocusMsg); }
        }
    }

    // --------------------------------------------------------------------------------------------

    private void ProcessMessage_PlatFocus(PlatFocus msg)
    {
        GloCentralLog.AddEntry($"GloMessageManager.ProcessMessage_PlatFocus: {msg.PlatName}");

        // Check if the entity (the godot object) exists
        if (KoreGodotFactory.Instance.GodotEntityManager.EntityExists(msg.PlatName))
        {
            GloCentralLog.AddEntry($"GloUIMessageManager.ProcessMessage_PlatFocus: GOT ENTITY: {msg.PlatName}");
            GloAppFactory.Instance.EventDriver.SetCameraModeChaseCam();


            int entityCount = GloAppFactory.Instance.EventDriver.NumPlatforms();
            while(GloAppFactory.Instance.EventDriver.NearPlatformName() != msg.PlatName)
            {
                GloAppFactory.Instance.EventDriver.NearPlatformNext();
                entityCount--;
                if (entityCount <= 0)
                {
                    GloCentralLog.AddEntry("GloUIMessageManager.ProcessMessage_PlatFocus: Could not find entity.");
                    return;
                }
            }

            KoreGodotFactory.Instance.UIState.UpdateDisplayedChaseCam();
        }
    }
}

