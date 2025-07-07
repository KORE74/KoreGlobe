using System;
using System.Threading;

// GloModelRun: Class to control the running of the model, starting and stopping the clock and calling the model for its cyclic updates.
// This class exists underneath the EventDriver, so it is called by the interface to perform the actual start and stop of the model.
public class GloModelRun
{
    private Thread modelThread;

    // --------------------------------------------------------------------------------------------

    public void Start()
    {
        bool running = GloAppFactory.Instance.SimClock.IsRunning;

        if (!running)
        {
            GloAppFactory.Instance.PlatformManager.Reset();
            GloAppFactory.Instance.SimClock.Start();
            GloAppFactory.Instance.SimClock.MarkTime();

            // Start the model running thread
            modelThread = new Thread(new ThreadStart(RunModel));
            modelThread.Start();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Pause()
    {
        GloAppFactory.Instance.SimClock.Stop();

        // Stop the model running thread
        if (modelThread != null && modelThread.IsAlive)
        {
            modelThread.Join();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Resume()
    {
        bool running = GloAppFactory.Instance.SimClock.IsRunning;

        if (!running)
        {
            GloAppFactory.Instance.SimClock.Start();
            GloAppFactory.Instance.SimClock.MarkTime();

            // Start the model running thread
            modelThread = new Thread(new ThreadStart(RunModel));
            modelThread.Start();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Stop()
    {
        GloAppFactory.Instance.SimClock.Stop();

        // Stop the model running thread
        if (modelThread != null && modelThread.IsAlive)
        {
            modelThread.Join();
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Reset()
    {
        Stop();
        GloAppFactory.Instance.SimClock.Reset();
    }

    // --------------------------------------------------------------------------------------------

    private void RunModel()
    {
        bool running = GloAppFactory.Instance.SimClock.IsRunning;

        while (running)
        {
            Update();
            // Assuming the update should happen at regular intervals
            // Sleep for a while to create the cyclic update
            Thread.Sleep(20); // Adjust the interval as needed - running the model at 50Hz for now.
            running = GloAppFactory.Instance.SimClock.IsRunning;
        }
    }

    // --------------------------------------------------------------------------------------------

    public void Update()
    {
        // Call the model to update
        // Assuming the model update logic is handled within this method
        GloAppFactory.Instance.PlatformManager.UpdateKinetics();
    }

    // --------------------------------------------------------------------------------------------
}
