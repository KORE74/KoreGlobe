using System.Collections.Generic;
using System.Collections.Concurrent;

namespace FssNetworking
{
    public struct FssCommsMessage
    {
        public string connectionName;
        public string msgData;

        public string msgDebug()
        {
            return $"<{connectionName} => {msgData}>";
        }
    }

    // --------------------------------------------------------------------------------------------

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

        public BlockingCollection<FssCommsMessage> IncomingQueue;

        public List<FssCommsMessage> IncomingMessageLog;

        public void setupIncomingQueue(BlockingCollection<FssCommsMessage> newIncomingQueue)
        {
            IncomingQueue = newIncomingQueue;
            IncomingMessageLog = new List<FssCommsMessage>();
        }

        public void QueueIncomingMessage(string msgData)
        {
            FssCommsMessage newMsg = new FssCommsMessage();

            newMsg.connectionName = Name;
            newMsg.msgData = msgData;

            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public void QueueIncomingMessage(FssCommsMessage newMsg)
        {
            IncomingMessageLog.Add(newMsg);
            IncomingQueue.Add(newMsg);
        }

        public bool hasIncomingMessage()
        {
            return IncomingQueue.Count > 0;
        }

        public FssCommsMessage getNextIncomingMessage()
        {
            return IncomingQueue.Take();
        }

    }

}
