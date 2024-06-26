using System.Collections.Generic;

// FssCommandVersion

public class FssCommandVersion : FssCommand
{
    public FssCommandVersion()
    {
        Signature.Add("version");
    }

    public override void Execute(List<string> parameters)
    {
        System.Console.WriteLine("FSS Version 1.0");
    }

}
