
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

    public string ReportNetworkingStatus()
    {
        return "pp"; // return FssAppFactory.Instance.NetworkHub.debugDump();
    }

    public string ReportLocalIP()
    {
        return "pp"; // return FssAppFactory.Instance.NetworkHub.localIPAddrStr();
    }

    public string NetworkReport() => FssAppFactory.Instance.NetworkHub.Report();

    public void NetworkInjectIncoming(string message) => FssAppFactory.Instance.NetworkHub.InjectIncomingMessage(message);

    public bool HasIncomingMessage() => FssAppFactory.Instance.NetworkHub.HasIncomingMessage();

    public FssMessageText? GetIncomingMessage() => FssAppFactory.Instance.NetworkHub.GetIncomingMessage();

}