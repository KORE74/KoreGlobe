using System;
using System.Threading;

using FssNetworking;
using FssJSON;

#nullable enable

// Class to translate an incoming JSON Message into calls to the Event Driver

public class FssMessageManager
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

        // Process the message
        // switch (message.MessageType)
        // {
        //     case "network":
        //         ProcessNetworkMessage(message);
        //         break;

        //     default:
        //         FssCentralLog.AddEntry($"FssMessageManager.ProcessMessage: Unknown message type: {message.MessageType}");
        //         break;
        // }
    }

}

