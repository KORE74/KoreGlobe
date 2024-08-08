
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    public void StartNetworking()
    {

    }

    public void StopNetworking()
    {

    }

    // Usage: FssEventDriver.NetworkConnect("TcpClient", "TcpClient", "127.0.0.1", 12345);
    public void NetworkConnect(string connName, string connType, string ipAddrStr, int port) => FssAppFactory.Instance.NetworkHub.createConnection(connName, connType, ipAddrStr, port);

    public void NetworkDisconnect(string connName) => FssAppFactory.Instance.NetworkHub.endConnection(connName);

    public string ReportLocalIP() => FssAppFactory.Instance.NetworkHub.localIPAddrStr();

    // Usage: FssEventDriver.NetworkReport
    public string NetworkReport() => FssAppFactory.Instance.NetworkHub.Report();

    public void NetworkInjectIncoming(string message) => FssAppFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public bool HasIncomingMessage() => FssAppFactory.Instance.NetworkHub.HasIncomingMessage();

    public FssMessageText? GetIncomingMessage() => FssAppFactory.Instance.NetworkHub.GetIncomingMessage();

}