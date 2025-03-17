using System.Collections.Generic;
using System.Text;

// FssCommandNetworkReport

public class FssCommandNetworkEndConnection : FssCommand
{
    public FssCommandNetworkEndConnection()
    {
        Signature.Add("network");
        Signature.Add("endconn");
    }

    public override string HelpString => $"{SignatureString} <connection_name>";

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandNetworkReport.Execute");

        if (parameters.Count != 1)
            return "FssCommandEleSaveTile.Execute -> parameter count error";

        string connectionName = parameters[0];

        StringBuilder sb = new StringBuilder();

        sb.Append($"Ending connection: {connectionName}\n");

        FssEventDriver.NetworkDisconnect(connectionName);

        sb.Append("Done.");

        return sb.ToString();
    }
}
