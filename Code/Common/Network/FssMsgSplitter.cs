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

    class FssMsgSplitter
    {
        private char sentinel;
        private Dictionary<string, StringBuilder> InBuffers = new Dictionary<string, StringBuilder>();
        private List<FssMessageText> OutBuffers = new List<FssMessageText>();

        public FssMsgSplitter(char sentinelCharacter)
        {
            sentinel = sentinelCharacter;
        }

        public void AddRawMessage(FssMessageText inRawMsg)
        {
            string connName = inRawMsg.connectionName;

            // Retrieve or create a StringBuilder for the given connection.
            if (!InBuffers.TryGetValue(connName, out StringBuilder? Buf))
            {
                Buf = new StringBuilder();
                InBuffers[connName] = Buf;
            }

            // Append incoming message data to the buffer.
            Buf?.Append(inRawMsg.msgData); // Use null-conditional operator here

            int sentinelIndex;

            // Process complete messages until no more sentinel characters are found.
            while ((sentinelIndex = Buf?.ToString().IndexOf(sentinel) ?? -1) != -1) // Use null-coalescing operator here
            {
                // Extract and store the complete message preceding the sentinel.
                string? completeMsg = Buf?.ToString(0, sentinelIndex);
                if (completeMsg != null)
                {
                    OutBuffers.Add(new FssMessageText() { connectionName = connName, msgData = completeMsg });
                }

                // Remove the processed part from the buffer.
                Buf?.Remove(0, sentinelIndex + 1);
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
