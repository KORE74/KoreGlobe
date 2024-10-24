using System;
using System.Threading;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public partial class FssMessageManager
{
    private bool ThreadRunning = false;

    // --------------------------------------------------------------------------------------------

    public void Start()
    {
        ThreadRunning = true;

        // Start the thread
        Thread t = new Thread(IncomingMessageMonitor);
        t.Start();
    }

    // --------------------------------------------------------------------------------------------

    // Thread function, looking for new messages
    public void IncomingMessageMonitor()
    {
        // Loop forever
        while (ThreadRunning)
        {
            // Check for new messages
            while (FssAppFactory.Instance.EventDriver.HasIncomingMessage())
            {
                // Get the message - still in text format
                FssMessageText? nullableMessage = FssAppFactory.Instance.EventDriver.GetIncomingMessage();

                if (nullableMessage != null)
                {
                    // Explicitly cast nullable type to non-nullable type
                    FssMessageText message = (FssMessageText)nullableMessage;
                    // Process the message
                    ProcessMessage(message);
                }
            }

            // Sleep for a bit
            System.Threading.Thread.Sleep(100); // milliseconds
        }
    }

    // --------------------------------------------------------------------------------------------

    public void EndThread()
    {
        ThreadRunning = false;
    }

    // --------------------------------------------------------------------------------------------

    void ProcessMessage(FssMessageText message)
    {
        if (!message.HasValidContent())
        {
            FssCentralLog.AddEntry("FssMessageManager.ProcessMessage: Invalid message content received.");
            return;
        }

        FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage: {message}");

        // Get the message type
        string msgType = IncomingMessageHandler.GetMessageTypeName(message.msgData);

        // Convert the message to a JSON object
        JSONMessage msg = IncomingMessageHandler.ProcessMessage(msgType, message.msgData);

        if      (msg is EntityAdd platAddMsg)          { ProcessMessage_PlatAdd(platAddMsg); }
        else if (msg is EntityDelete platDelMsg)       { ProcessMessage_PlatDelete(platDelMsg); }
        else if (msg is EntityPosition platPosMsg)     { ProcessMessage_PlatPosition(platPosMsg); }
        else if (msg is EntityUpdate platUpdMsg)       { ProcessMessage_PlatUpdate(platUpdMsg); }
        else if (msg is EntityWayPoints platWayPtsMsg) { ProcessMessage_PlatWayPoints(platWayPtsMsg); }
        else if (msg is ScenLoad scenLoadMsg)        { ProcessMessage_ScenLoad(scenLoadMsg); }
        else if (msg is ScenStart scenStartMsg)      { ProcessMessage_ScenStart(scenStartMsg); }
        else if (msg is ScenStop scenStopMsg)        { ProcessMessage_ScenStop(scenStopMsg); }
        else if (msg is ScenPause scenPauseMsg)      { ProcessMessage_ScenPause(scenPauseMsg); }
        else if (msg is ScenCont scenContMsg)        { ProcessMessage_ScenCont(scenContMsg); }
        else if (msg is ClockSync clockSyncMsg)      { ProcessMessage_ClockSync(clockSyncMsg); }

        else
        {
            FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage: Unknown message type: {msgType} // {message.msgData}");
        }



    }

}

