using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FssNetworking
{
    public class FssNetworkHub
    {
        // List of all the connections (from a CommonConnection parent class).
        public List<FssCommonConnection> connections;

        // List of all incoming messages, containing the message data and a connection identifier.
        public BlockingCollection<FssMessageText> IncomingQueue;

        private CancellationTokenSource _cts;
        //bool ClientConnectWatchdog;

        private int WatchDogActivityCount; // Simple counter to show watchdog is running.

        // ----------------------------------------------------------------------------------------
        // MARK: Constructor / Destructor
        // ----------------------------------------------------------------------------------------

        public FssNetworkHub()
        {
            connections = new List<FssCommonConnection>();
            IncomingQueue = new BlockingCollection<FssMessageText>();

            //ClientConnectWatchdog = true;
            _cts = new CancellationTokenSource();
            WatchDogActivityCount = 0;
            Task.Run(async () => await ConnectionWatchdog(_cts.Token));
        }

        // Destructor stops each of the connections (with their threads and blocking calls) and clears down the connection collection.
        ~FssNetworkHub()
        {
            // Stop all threads
            foreach (FssCommonConnection connection in connections)
            {
                connection.stopConnection();
            }
            connections.Clear();

            _cts.Cancel();
            //ClientConnectWatchdog = false;
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Connections
        // ----------------------------------------------------------------------------------------

        public void createConnection(string connName, string connType, string ipAddrStr, int port)
        {
            // End any pre-existing connection with the name we would look to use.
            endConnection(connName);

            if (connType == "UdpSender")
            {
                // Create connection
                FssUdpSender UdpSender = new FssUdpSender()
                {
                    Name = connName,
                    IncomingQueue = IncomingQueue,
                    IncomingMessageLog = new List<FssMessageText>() // Assuming IncomingMessageLog is a List<CommsMessage>.
                };

                connections.Add(UdpSender);
                UdpSender.setConnectionDetails(ipAddrStr, port);
                UdpSender.startConnection();
            }
            else if (connType == "UdpReceiver")
            {
                // Create connection
                FssUdpReceiver UdpReceiver = new FssUdpReceiver()
                {
                    Name = connName,
                    IncomingQueue = IncomingQueue,
                    IncomingMessageLog = new List<FssMessageText>()
                };

                connections.Add(UdpReceiver);
                UdpReceiver.setConnectionDetails(port);
                UdpReceiver.startConnection();
            }
            else if (connType == "TcpClient")
            {
                // Create connection
                FssTcpClientConnection clientConnection = new FssTcpClientConnection()
                {
                    Name = connName,
                    IncomingQueue = IncomingQueue,
                    IncomingMessageLog = new List<FssMessageText>()
                };

                connections.Add(clientConnection);
                clientConnection.setConnectionDetails(ipAddrStr, port);
                // clientConnection.startConnection(); Let the watchdog do it.
            }
            else if (connType == "TcpServer")
            {
                // Create connection
                FssTcpServerConnection serverConnection = new FssTcpServerConnection()
                {
                    Name = connName,
                    IncomingQueue = IncomingQueue,
                    IncomingMessageLog = new List<FssMessageText>()
                };

                serverConnection.commsHub = this;

                connections.Add(serverConnection);
                serverConnection.setConnectionDetails(ipAddrStr, port);
                serverConnection.startConnection();
            }
        }

        // -----------------------------------------------------------------------------------

        public void endConnection(string connName)
        {
            foreach (FssCommonConnection conn in connections)
            {
                if (conn.Name == connName)
                {
                    conn.stopConnection();
                    connections.Remove(conn);
                    break;
                }
            }
        }

        // -----------------------------------------------------------------------------------

        public void endAllConnections()
        {
            while (connections.Count > 0)
            {
                FssCommonConnection currConn = connections.First();
                currConn.stopConnection();
                connections.Remove(currConn);
            }
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Send/output messages
        // ----------------------------------------------------------------------------------------

        public void sendMessage(string connName, string msgData)
        {
            bool messageSent = false;
            foreach (FssCommonConnection conn in connections)
            {
                if (conn.Name == connName)
                {
                    conn.sendMessage(msgData);
                    messageSent = true;
                }
            }

            if (!messageSent)
            {
                Console.WriteLine("SEND FAIL:" + connName + " // " + msgData);
            }
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Receive/Incoming messages
        // ----------------------------------------------------------------------------------------

        public bool HasIncomingMessage()
        {
            return (IncomingQueue.Count > 0);
        }

        // -----------------------------------------------------------------------------------

        public bool GetIncomingMessage(out FssMessageText msg)
        {
            return IncomingQueue.TryTake(out msg, 100);
        }

        public FssMessageText? GetIncomingMessage()
        {
            if (IncomingQueue.Count == 0)
            {
                return null;
            }
            return IncomingQueue.Take();
        }

        // ----------------------------------------------------------------------------------------
        // MARK: Misc Functions
        // ----------------------------------------------------------------------------------------

        public string localIPAddrStr()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addresses = hostEntry.AddressList;
            IPAddress[] ipv4Addresses = addresses.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();

            foreach (IPAddress address in ipv4Addresses)
            {
                return address.ToString();
            }
            return "<noaddr>";
        }

        // -----------------------------------------------------------------------------------

        public string Report()
        {
            string outTxt = "NetworkHub Report:\n";

            outTxt += $"- LocalIP {localIPAddrStr()}\n";
            outTxt += $"- WatchDogActivityCount {WatchDogActivityCount}\n";

            foreach (FssCommonConnection conn in connections)
            {
                outTxt += "- " + conn.connectionDetailsString() + "\n";
            }

            return outTxt;
        }

        // -----------------------------------------------------------------------------------

        public async Task ConnectionWatchdog(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                WatchDogActivityCount = (WatchDogActivityCount + 1) % 1000;

                foreach (FssCommonConnection currConn in connections)
                {
                    if (currConn is FssTcpClientConnection)
                    {
                        FssTcpClientConnection currConnTcp = currConn as FssTcpClientConnection;

                        if (currConnTcp != null && !currConnTcp.connected)
                        {
                            Console.WriteLine("Watchdog starting: " + currConnTcp.connectionDetailsString());
                            currConnTcp.startConnection();
                        }
                    }
                }

                //Task.Delay(1000);
                await Task.Delay(5000, token);
            }
            FssCentralLog.AddEntry($"Network Comms Hub: Watchdog Ended");
        }

        // -----------------------------------------------------------------------------------

        // Debug function, to allow the application to inject incoming messages

        public void InjectIncomingMessage(string msgText)
        {
            FssMessageText injectMsg = new FssMessageText() { connectionName = "INJECTED", msgData = msgText };
            IncomingQueue.Add(injectMsg);

            FssCentralLog.AddEntry($"Network Comms Hub: Injected message: {msgText}");
        }

    }
}
