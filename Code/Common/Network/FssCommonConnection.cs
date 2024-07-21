using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FssNetworking
{
    public abstract class FssCommonConnection
    {
        public string Name { set; get; }

        public abstract string type();
        public abstract string connectionDetailsString();

        public abstract void sendMessage(string msgData);

        public abstract void startConnection();
        public abstract void stopConnection();

        // ========================================================================================
        // Incoming message queue
        // ========================================================================================

        public BlockingCollection<FssMessageText> IncomingQueue;

        public List<FssMessageText> IncomingMessageLog;

        public void setupIncomingQueue(BlockingCollection<FssMessageText> newIncomingQueue)
        {
            IncomingQueue = newIncomingQueue;
            IncomingMessageLog = new List<FssMessageText>();
        }

        public void QueueIncomingMessage(string msgData)
        {
            FssMessageText newMsg = new FssMessageText();

            newMsg.connectionName = Name;
            newMsg.msgData = msgData;

            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public void QueueIncomingMessage(FssMessageText newMsg)
        {
            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public bool hasIncomingMessage()
        {
            return IncomingQueue.Count > 0;
        }

        public FssMessageText getNextIncomingMessage()
        {
            return IncomingQueue.Take();
        }

    }

}
