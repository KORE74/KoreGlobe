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

        FssAppFactory.Instance.EventDriver.NetworkInjectIncoming(message);

        return $"Message injected: {message}";
    }
}