using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace FssNetworking
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

    class FssMsgSplitter2
    {
        private char sentinel;
        StringBuilder inBuffer = new StringBuilder();
        private List<FssMessageText> OutBuffers = new List<FssMessageText>();

        public FssMsgSplitter2(char sentinelCharacter)
        {
            sentinel = sentinelCharacter;
            inBuffer.Clear();
        }

        public void AddRawMessage(string inRawMsg)
        {
            // Loop through each new character
            foreach (char c in inRawMsg)
            {
                if (c == sentinel)
                {
                    // Add the message to the list
                    FssMessageText newMsg = new FssMessageText() { connectionName = "UNDEFINED", msgData = inBuffer.ToString() };
                    OutBuffers.Add(newMsg);

                    FssCentralLog.AddEntry($"=====> Message from SENTINEL character: {newMsg.msgData}");

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

        public FssMessageText NextMsg()
        {
            if (OutBuffers.Count == 0)
            {
                throw new InvalidOperationException("No messages available.");
            }

            FssMessageText n = OutBuffers[0];
            OutBuffers.RemoveAt(0); // Adding at last, reading and removing at first
            return n;
        }
    }
}
