
using System;

// Design Decisions:
// - The GloEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public partial class GloEventDriver
{

    // ---------------------------------------------------------------------------------------------
    // MARK: Simulation Clock
    // ---------------------------------------------------------------------------------------------
    // different to runtime clock and Time Functions, this is the in-sium time that can be paused etc.

    public void SimClockStart()  => GloAppFactory.Instance.ModelRun.Start();
    public void SimClockStop()   => GloAppFactory.Instance.ModelRun.Stop();
    public void SimClockReset()  => GloAppFactory.Instance.ModelRun.Reset();
    public void SimClockResume() => GloAppFactory.Instance.ModelRun.Resume();

    public void SetSimClockSeconds(double secs)
    {
    }

    public void SetSimTimeHMS(string hms)
    {
        GloAppFactory.Instance.SimClock.SimTimeHMS = hms;
    }

    public int    SimSeconds() => (int)GloAppFactory.Instance.SimClock.SimTime;
    public string SimTimeHMS() => GloAppFactory.Instance.SimClock.SimTimeHMS;

    // ---------------------------------------------------------------------------------------------

    public void   ClockSetRate(double rate) => GloAppFactory.Instance.SimClock.SetSimRate(rate);
    public double ClockRate()               => GloAppFactory.Instance.SimClock.SimRate;
}