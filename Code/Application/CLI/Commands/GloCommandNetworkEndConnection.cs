using System.Collections.Generic;
using System.Text;

// GloCommandNetworkReport

public class GloCommandNetworkEndConnection : GloCommand
{
    public GloCommandNetworkEndConnection()
    {
        Signature.Add("network");
        Signature.Add("endconn");
    }

    public override string HelpString => $"{SignatureString} <connection_name>";

    public override string Execute(List<string> parameters)
    {
        GloCentralLog.AddEntry("GloCommandNetworkReport.Execute");

        if (parameters.Count != 1)
            return "GloCommandEleSaveTile.Execute -> parameter count error";

        string connectionName = parameters[0];

        StringBuilder sb = new StringBuilder();

        sb.Append($"Ending connection: {connectionName}\n");

        GloAppFactory.Instance.EventDriver.NetworkDisconnect(connectionName);

        sb.Append("Done.");

        return sb.ToString();
    }
}