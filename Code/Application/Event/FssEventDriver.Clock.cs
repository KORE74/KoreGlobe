
using System;

// Design Decisions:
// - The FssEventDriver is the top level class that manages data. Commands and Tasks interact with the business logic through this point.

public static partial class FssEventDriver
{

    // ---------------------------------------------------------------------------------------------
    // MARK: Simulation Clock
    // ---------------------------------------------------------------------------------------------
    // different to runtime clock and Time Functions, this is the in-sium time that can be paused etc.

    public static void SimClockStart() => FssAppFactory.Instance.ModelRun.Start();
    public static void SimClockStop()  => FssAppFactory.Instance.ModelRun.Stop();
    public static void SimClockReset() => FssAppFactory.Instance.ModelRun.Reset();

    public static void SetSimClockSeconds(double secs)
    {
    }

    public static void SetSimTimeHMS(string hms)
    {
        FssAppFactory.Instance.SimClock.SimTimeHMS = hms;
    }

    public static int    SimSeconds() => (int)FssAppFactory.Instance.SimClock.SimTime;
    public static string SimTimeHMS() => FssAppFactory.Instance.SimClock.SimTimeHMS;

    // ---------------------------------------------------------------------------------------------

    public static void   ClockSetRate(double rate) => FssAppFactory.Instance.SimClock.SetSimRate(rate);
    public static double ClockRate()               => FssAppFactory.Instance.SimClock.SimRate;
}