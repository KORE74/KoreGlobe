using System.Collections.Generic;

// FssCommandNetworkReport

public class FssCommandNetworkInjectIncoming : FssCommand
{
    public FssCommandNetworkInjectIncoming()
    {
        Signature.Add("network");
        Signature.Add("ii");
    }

    public override string Execute(List<string> parameters)
    {
        FssCentralLog.AddEntry("FssCommandNetworkInjectIncoming.Execute");

        // fail if there are no further parameters
        if (parameters.Count < 1)
        {
            return "Error: No parameters provided";
        }

        // concatenate the parameters into a single string with spaces
        string message = string.Join(" ", parameters);

        FssEventDriver.NetworkInjectIncoming(message);

        // trim the message to 100 characters
        if (message.Length > 100)
        {
            message = message.Substring(0, 100);
            message += "...";
        }

        return $"Message injected: {message}";
    }
}