
using System;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class FssEventDriver
{

    // ---------------------------------------------------------------------------------------------
    // MARK: Simulation Clock
    // ---------------------------------------------------------------------------------------------
    // different to runtime clock and Time Functions, this is the in-sium time that can be paused etc.

    public void SimClockStart() => FssAppFactory.Instance.ModelRun.Start();
    public void SimClockStop()  => FssAppFactory.Instance.ModelRun.Stop();
    public void SimClockReset() => FssAppFactory.Instance.ModelRun.Reset();

    public void SetSimClockSeconds(double secs)
    {
    }

    public void SetSimTimeHMS(string hms)
    {
        FssAppFactory.Instance.SimClock.SimTimeHMS = hms;
    }

    public int    SimSeconds() => (int)FssAppFactory.Instance.SimClock.SimTime;
    public string SimTimeHMS() => FssAppFactory.Instance.SimClock.SimTimeHMS;

    // ---------------------------------------------------------------------------------------------

    public void   ClockSetRate(double rate) => FssAppFactory.Instance.SimClock.SetSimRate(rate);
    public double ClockRate()               => FssAppFactory.Instance.SimClock.SimRate;
}