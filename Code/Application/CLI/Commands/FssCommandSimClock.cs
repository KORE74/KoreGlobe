using System.Collections.Generic;

// FssCommandSimClock

public class FssCommandSimClock : FssCommand
{
    public FssCommandSimClock()
    {
        Signature.Add("sim");
        Signature.Add("clock");
    }

    public override string Execute(List<string> parameters)
    {
        return $"SimClock: {FssEventDriver.SimSeconds()}Secs";
    }
}
