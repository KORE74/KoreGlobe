using System.Collections.Generic;

// GloCommandSimClock

public class GloCommandSimClock : GloCommand
{
    public GloCommandSimClock()
    {
        Signature.Add("sim");
        Signature.Add("clock");
    }

    public override string Execute(List<string> parameters)
    {
        return $"SimClock: {GloAppFactory.Instance.EventDriver.SimSeconds()}Secs";
    }
}
