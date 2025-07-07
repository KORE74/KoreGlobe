using System;

using GloNetworking;

using Godot;

// Class to construct the main classes in the application and link them together, in lieu
// of a globals file that would publicly expose the main classes.

#nullable enable

public class GloAppFactory
{
    // Singleton instance created as soon as the class is loaded.
    public static readonly GloAppFactory Instance = new GloAppFactory();

    private static readonly object lockObject = new object();

    // --------------------------------------------------------------------------------------------

    public GloConsole          ConsoleInterface { get; private set; }
    public GloEventDriver      EventDriver      { get; private set; }
    public GloPlatformManager  PlatformManager  { get; private set; }
    public GloNetworkHub       NetworkHub       { get; private set; }
    public GloSimTime          SimClock         { get; private set; }
    public GloModelRun         ModelRun         { get; private set; }
    public GloMessageManager   MessageManager   { get; private set; }
    public GloElevationManager EleManager       { get; private set; }

    // --------------------------------------------------------------------------------------------

    // Instance control, with a lock, hidden constructor, and a singleton instance.

    // private static readonly object lockObject = new object();

    // private static GloAppFactory? SingletonInstance = null;
    // private static bool           IsInitializing    = false;

    // // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    // public static GloAppFactory Instance
    // {
    //     get
    //     {
    //         //GD.Print("GloAppFactory.Instance");
    //         lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
    //         {
    //             if (IsInitializing)
    //             {
    //                 throw new InvalidOperationException("GloAppFactory instance is being initialized and cannot be accessed recursively.");
    //             }

    //             if (SingletonInstance == null)
    //             {
    //                 SingletonInstance = new GloAppFactory();
    //             }
    //             IsInitializing = false;

    //             return SingletonInstance;
    //         }
    //     }
    // }

    // --------------------------------------------------------------------------------------------

    // Private constructor

    private GloAppFactory()
    {
        // Create the objects
        GloCentralLog.AddEntry("Creating GloAppFactory objects");
        GD.Print("GloAppFactory Construction - Start");

        ConsoleInterface = new GloConsole();
        EventDriver      = new GloEventDriver();
        PlatformManager  = new GloPlatformManager();
        NetworkHub       = new GloNetworkHub();
        SimClock         = new GloSimTime();
        ModelRun         = new GloModelRun();
        MessageManager   = new GloMessageManager();
        EleManager       = new GloElevationManager();

        // Link the objects
        //ConsoleInterface.EventDriver = EventDriver;
        //EventDriver.ConsoleInterface = ConsoleInterface;

        // CallStart();

        GloCentralConfig.Instance.ReadFromFile();

        GloCentralLog.LoggingActive = GloCentralConfig.Instance.GetParam<bool>("LoggingActive");

        // Read the logfile path from the config and update the centralised logger with it.
        string logPath = GloCentralConfig.Instance.GetParam<string>("LogPath");
        if (!String.IsNullOrWhiteSpace(logPath))
            GloCentralLog.UpdatePath(logPath);


        ConsoleInterface.Start();
        MessageManager.Start();

        GD.Print("GloAppFactory Construction - Done");

        GloTestCenter.RunAdHocTests();
    }
    // --------------------------------------------------------------------------------------------

    // // point to start the services, called after the main constructors
    // public void CallStart()
    // {


    //     //ConsoleInterface.Start();
    //     //MessageManager.Start();

    //     // co-ordinate some config (avoid directly coupling the classes)
    //     GloCentralLog.LoggingActive = GloCentralConfig.Instance.GetParam<bool>("LoggingActive");
    //     //GloCentralLog.UpdatePath(GloCentralConfig.Instance.GetParam<string>("LogPath"));

    //     string logPath = GloCentralConfig.Instance.GetParam<string>("LogPath");
    //     if (!String.IsNullOrWhiteSpace(logPath))
    //     {
    //         GloCentralLog.UpdatePath(logPath);
    //     }
    // }
}
