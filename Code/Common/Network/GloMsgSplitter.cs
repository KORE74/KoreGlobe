using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace GloNetworking
{
    /*
    public struct CommsMessage
    {
        public string connectionName;
        public string msgData;

        public string msgDebug()
        {
            return "<connectionName:" + connectionName + ", msgData:" + msgData + ">";
        }
    }
    */

    class GloMsgSplitter
    {
        private char sentinel;
        StringBuilder inBuffer = new StringBuilder();
        private List<GloMessageText> OutBuffers = new List<GloMessageText>();

        // Create a debug logfile of everythgin that passes through this function
        private string debugLogFilename = "GloMsgSplitter.log";


        public GloMsgSplitter(char sentinelCharacter)
        {
            sentinel = sentinelCharacter;
            inBuffer.Clear();
        }

        public void AddRawMessage(string inRawMsg)
        {
            // open the debug file and append the new string
            System.IO.File.AppendAllText(debugLogFilename, inRawMsg);


            // Loop through each new character
            foreach (char c in inRawMsg)
            {
                if (c == sentinel)
                {
                    // Add the message to the list
                    GloMessageText newMsg = new GloMessageText() { connectionName = "UNDEFINED", msgData = inBuffer.ToString() };
                    OutBuffers.Add(newMsg);

                    GloCentralLog.AddEntry($"=====> Message from SENTINEL character: {newMsg.msgData}");

                    // Clear the buffer
                    inBuffer.Clear();
                }
                else
                {
                    inBuffer.Append(c);
                }
            }
        }

        public bool HasMessage()
        {
            return (OutBuffers.Count > 0);
        }

        public GloMessageText NextMsg()
        {
            if (OutBuffers.Count == 0)
            {
                throw new InvalidOperationException("No messages available.");
            }

            GloMessageText n = OutBuffers[0];
            OutBuffers.RemoveAt(0); // Adding at last, reading and removing at first
            return n;
        }
    }
}
