using System;

using FssNetworking;

using Godot;

// Class to construct the main classes in the application and link them together, in lieu
// of a globals file that would publicly expose the main classes.

#nullable enable

public class FssAppFactory
{
    // --------------------------------------------------------------------------------------------

    // IO
    public FssNetworkHub       NetworkHub       { get; private set; }
    public FssConsole          ConsoleInterface { get; private set; }
    public FssMessageManager   MessageManager   { get; private set; }

    // Model . Simulation
    public FssModelRun         ModelRun         { get; private set; } // Overall Run Status
    public FssSimTime          SimClock         { get; private set; } // Clock
    public FssEntityManager    EntityManager    { get; private set; } // platform data

    // Utilities
    public FssElevationManager EleManager       { get; private set; }

    // --------------------------------------------------------------------------------------------

    // Instance control, with a lock, hidden constructor, and a singleton instance.

    private static readonly object lockObject = new object();

    private static FssAppFactory? SingletonInstance = null;
    private static bool IsInitializing = false;

    // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    public static FssAppFactory Instance
    {
        get
        {
            //GD.Print("FssAppFactory.Instance");
            lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
            {
                if (IsInitializing)
                {
                    throw new InvalidOperationException("FssAppFactory instance is being initialized and cannot be accessed recursively.");
                }

                if (SingletonInstance == null)
                {
                    IsInitializing = true;
                    try
                    {
                        SingletonInstance = new FssAppFactory();
                    }
                    finally
                    {
                        IsInitializing = false;
                    }
                }

                return SingletonInstance;
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Private constructor

    private FssAppFactory()
    {
        // Create the objects
        FssCentralLog.AddEntry("Creating FssAppFactory objects");
        GD.Print("FssAppFactory");

        NetworkHub       = new FssNetworkHub();
        ConsoleInterface = new FssConsole();
        MessageManager   = new FssMessageManager();

        ModelRun         = new FssModelRun();
        SimClock         = new FssSimTime();
        EntityManager    = new FssEntityManager();

        EleManager        = new FssElevationManager();

        CallStart();
    }

    // point to start the services, called after the main constructors
    public void CallStart()
    {
        ConsoleInterface.Start();
        MessageManager.Start();

        // co-ordinate some config (avoid directly coupling the classes)
        FssCentralLog.LoggingActive = FssCentralConfig.Instance.GetParam<bool>("LoggingActive");
        FssCentralLog.UpdatePath(FssCentralConfig.Instance.GetParam<string>("LogPath"));
    }
}
