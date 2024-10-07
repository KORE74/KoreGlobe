using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#nullable enable

namespace FssNetworking
{
    class FssTcpServerClientConnection : FssCommonConnection
    {
        public bool running;

        public NetworkStream? stream;
        public TcpClient? client;
        public DateTime lastUpdateTime;
        private int NumMsgsHandled;

        private Thread? sendThread;
        private Thread? receiveThread;

        public BlockingCollection<string> sendMsgQueue;

        // Splits messages based on a sentinel character between messages
        private FssMsgSplitter IncomingMsgSplitter;
        private FssMsgSplitter2 IncomingMsgSplitter2;

        private byte[] ReadBuffer = new byte[250000];

        // ----------------------------------------------------------------------------------------

        public FssTcpServerClientConnection()
        {
            running = false;

            sendMsgQueue = new BlockingCollection<string>();

            NumMsgsHandled = 0;

            char sentinelCharacter = '\u0003';
            IncomingMsgSplitter  = new FssMsgSplitter(sentinelCharacter);
            IncomingMsgSplitter2 = new FssMsgSplitter2(sentinelCharacter);

            client = null;
            stream = null;
        }

        public void setConnectionDetails(TcpClient newTcpClient, NetworkStream newStream)
        {
            client = newTcpClient;
            stream = newStream;
        }

        // ========================================================================================
        // override methods
        // ========================================================================================

        public override string type()
        {
            return "TcpServerClient";
        }

        // ----------------------------------------------------------------------------------------

        public override string connectionDetailsString()
        {
            return $"type:TcpServerClient // name:{Name} // count:{NumMsgsHandled}";
        }

        // ----------------------------------------------------------------------------------------

        public override void startConnection()
        {
            running = true;

            // Process the client connection in a new thread.
            sendThread = new Thread(new ThreadStart(sendThreadFunc));
            sendThread?.Start();

            receiveThread = new Thread(new ThreadStart(receiveThreadFunc));
            receiveThread?.Start();
        }

        // ----------------------------------------------------------------------------------------

        public override void stopConnection()
        {
            // Set the running flag to false.
            running = false;

            client?.Close();

            if (sendThread != null)
                sendThread.Join();

            if (receiveThread != null)
                receiveThread.Join();
        }

        // ----------------------------------------------------------------------------------------

        // The call to send a message adds it to a queue. The send operation may be blocking, so the
        // send thread will unblock on the collection getting a new element and send it on its own
        // timeline.
        public override void sendMessage(string msgData)
        {
            sendMsgQueue.Add(msgData);
        }

        // ========================================================================================

        void sendThreadFunc()
        {
            FssCentralLog.AddEntry($"Start ServerClient Send Thread {connectionDetailsString()}");

            // Enter an infinite loop to process client connections.
            while (running)
            {
                string nextMsg;

                // Blocking call on the new message collection, with a 1sec timeout to allow the running flag to be checked.
                if (sendMsgQueue.TryTake(out nextMsg, 1000))
                {
                    byte[] msgBuffer = Encoding.ASCII.GetBytes(nextMsg);
                    if (stream != null)
                        stream.Write(msgBuffer, 0, msgBuffer.Length);
                    if (NumMsgsHandled<10000) NumMsgsHandled++;
                }
            }
        }

        // ----------------------------------------------------------------------------------------

        void receiveThreadFunc()
        {
            FssCentralLog.AddEntry($"Start ServerClient Receive Thread {connectionDetailsString()}");

            // Enter a loop to continuously process client requests.
            while (running)
            {
                // Check if the client is still connected, of break out of the infinite loop, to the removal and exit.
                if (client != null)
                {
                    if (!client.Connected)
                    {
                        FssCentralLog.AddEntry($"ServerClient Disconnected {connectionDetailsString()}");
                        client?.Close();
                        break;
                    }
                }

                // Timeout a client if not sent a message recently
                DateTime currentTime = DateTime.UtcNow;
                if (currentTime.Subtract(lastUpdateTime).TotalSeconds > 10)
                {
                    FssCentralLog.AddEntry($"ServerClient Inactive {connectionDetailsString()} (10secs)");
                    client?.Close();
                    break;
                }

                // Check if data is available on the client stream.
                if (stream != null)
                {
                    if (stream.DataAvailable)
                    {
                        // Update the timer if we have anything.
                        lastUpdateTime = DateTime.Now;

                        // Read data from the client stream.
                        //byte[] buffer = new byte[50000];
                        int bytesRead = stream.Read(ReadBuffer, 0, ReadBuffer.Length);

                        if (bytesRead == 0)
                        {
                            // listener was closed or broken, exit loop
                            running = false;
                            continue; // to top of loop to exit
                        }
                        else
                        {
                            // Read the raw incoming data
                            string data = Encoding.ASCII.GetString(ReadBuffer, 0, bytesRead);

                            IncomingMsgSplitter2.AddRawMessage(data);

                            // FssMessageText rawMsg = new FssMessageText() { connectionName = Name, msgData = data};

                            // // Add the incoming data to a raw buffer, outputting messages based on a splitter character
                            // IncomingMsgSplitter.AddRawMessage(rawMsg);
                        }
                    }
                }

                // When messages arrive close together, there can be multiple-per-input, so we need to "while"
                // to ensure they all get sent onto the main application.
                while (IncomingMsgSplitter2.HasMessage())
                {
                    // create a new message object to add to the incoming message queue.
                    QueueIncomingMessage(IncomingMsgSplitter2.NextMsg());
                    if (NumMsgsHandled<10000) NumMsgsHandled++;
                }

                // Put the thread on a timer, so it doesn't 100% busy-wait
                var now = DateTime.Now;
                var waitTime = 100 - (now.Millisecond % 100); // Calculate the time to wait until the next 0.1 second interval
                Thread.Sleep(waitTime);
            }
        }

    } // class
} // namespace
