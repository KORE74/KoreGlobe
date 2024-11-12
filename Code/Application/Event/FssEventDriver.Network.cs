
using System;

using FssNetworking;

#nullable enable

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class FssEventDriver
{
    public static void StartNetworking()
    {

    }

    public static void StopNetworking()
    {

    }

    // Usage: FssEventDriver.NetworkConnect("TcpClient", "TcpClient", "127.0.0.1", 12345);
    public static void NetworkConnect(string connName, string connType, string ipAddrStr, int port) => FssAppFactory.Instance.NetworkHub.createConnection(connName, connType, ipAddrStr, port);

    public static void NetworkDisconnect(string connName) => FssAppFactory.Instance.NetworkHub.endConnection(connName);

    public static string ReportLocalIP() => FssAppFactory.Instance.NetworkHub.localIPAddrStr();

    // Usage: FssEventDriver.NetworkReport
    public static string NetworkReport() => FssAppFactory.Instance.NetworkHub.Report();

    public static void NetworkInjectIncoming(string message) => FssAppFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public static bool HasIncomingMessage() => FssAppFactory.Instance.NetworkHub.HasIncomingMessage();

    public static FssMessageText? GetIncomingMessage() => FssAppFactory.Instance.NetworkHub.GetIncomingMessage();

}