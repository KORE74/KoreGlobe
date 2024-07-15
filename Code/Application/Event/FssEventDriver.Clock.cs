
using System;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{
    public void ClockStart()
    {
        FssAppFactory.Instance.ModelRun.Start();
    }

    public void ClockStop()
    {
        FssAppFactory.Instance.ModelRun.Stop();
    }

    public void ClockReset()
    {

    }

    public void SetClockSeonds(double secs)
    {

    }

    public int ClockSeconds()
    {
        return (int)FssAppFactory.Instance.SimClock.CurrentTime;
    }

    // ---------------------------------------------------------------------------------------------

    public void ClockSetRate(double rate)
    {
        FssAppFactory.Instance.SimClock.SetSimRate(rate);
    }

    public double ClockRate()
    {
        return FssAppFactory.Instance.SimClock.SimRate;
    }


}