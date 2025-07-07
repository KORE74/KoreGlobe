
using System;

using GloNetworking;

#nullable enable

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{
    public void StartNetworking()
    {

    }

    public void StopNetworking()
    {

    }

    // Usage: GloEventDriver.NetworkConnect("TcpClient", "TcpClient", "127.0.0.1", 12345);
    public void NetworkConnect(string connName, string connType, string ipAddrStr, int port) => GloAppFactory.Instance.NetworkHub.createConnection(connName, connType, ipAddrStr, port);

    public void NetworkDisconnect(string connName) => GloAppFactory.Instance.NetworkHub.endConnection(connName);

    public string ReportLocalIP() => GloAppFactory.Instance.NetworkHub.localIPAddrStr();

    // Usage: GloEventDriver.NetworkReport
    public string NetworkReport() => GloAppFactory.Instance.NetworkHub.Report();

    public void NetworkInjectIncoming(string message) => GloAppFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public bool HasIncomingMessage() => GloAppFactory.Instance.NetworkHub.HasIncomingMessage();

    public GloMessageText? GetIncomingMessage() => GloAppFactory.Instance.NetworkHub.GetIncomingMessage();

}