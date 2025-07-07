
// GloAppFactory is a central global class managing access to internal modelling element.
// GloGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;

using GloJSON;

#nullable enable

public class GloGodotFactory
{
    public Node3D                SceneRootNode      { get; private set; }
    public GloZeroNode           ZeroNode           { get; private set; }
    // public GloMapManager         EarthCoreNode      { get; private set; }  // GloGodotFactory.Instance.EarthCodeNode

    public GloCameraMoverWorld WorldCamNode         { get; private set; }


    public GloGodotEntityManager GodotEntityManager { get; private set; }
    public GloZeroNodeMapManager ZeroNodeMapManager { get; private set; }
    public GloTextureLoader      TextureLoader      { get; private set; }
    public Glo3DModelLibrary     ModelLibrary       { get; private set; } // GloGodotFactory.Instance.ModelLibrary
    public GloUIState            UIState            { get; private set; }

    // Singleton pattern
    private static readonly object  lockObject        = new object();
    private static GloGodotFactory? SingletonInstance = null;
    private static bool             IsInitialised     = false;
    private static bool             IsCreating        = false;

    // Threadsafe message queue, for messages that need to be handled on the main thread
    public JSONThreadsafeMessageFIFO UIMsgQueue       { get; private set; }
    private GloUIMessageManager      UIMessageManager { get; set; }

    // Usage:
    // - GloGodotFactory.Instance.UIMsgQueue.EnqueueMessage
    // - GloGodotFactory.Instance.UIMsgQueue.TryDequeueMessage

    // --------------------------------------------------------------------------------------------
    // MARK: Singleton Pattern
    // --------------------------------------------------------------------------------------------

    // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    public static GloGodotFactory Instance
    {
        get
        {
            //GD.Print("GloGodotFactory.Instance");
            lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
            {
                if (IsCreating)
                {
                    throw new InvalidOperationException("GloGodotFactory instance is being initialized and cannot be accessed recursively.");
                }

                if (SingletonInstance == null)
                {
                    IsCreating = true;
                    try
                    {
                        SingletonInstance = new GloGodotFactory();
                    }
                    finally
                    {
                        IsCreating = false;
                    }
                }
                return SingletonInstance;
            }
        }
    }

    // Private constructor
    private GloGodotFactory()
    {
    }

    // Function to check if the instance has been created, and if its not there yet, create it
    public static void TriggerInstance()
    {
        if (SingletonInstance == null)
        {
            GD.Print("GloGodotFactory.TriggerInstance");
            lock (lockObject)
            {
                if (SingletonInstance == null)
                {
                    SingletonInstance = new GloGodotFactory();
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Initialization
    // --------------------------------------------------------------------------------------------

    public void CreateObjects(Node3D sceneRootNode)
    {
        lock(lockObject)
        {
            if (!IsInitialised)
            {
                // Doing a chunk of object creation, wrapping in an excpetion handler
                try
                {
                    GD.Print("- GloGodotFactory CreateObjects - - - -");

                    SceneRootNode = sceneRootNode;

                    ZeroNode = new GloZeroNode();
                    SceneRootNode.AddChild(ZeroNode);

                    // EarthCoreNode = new GloMapManager();
                    // SceneRootNode.AddChild(EarthCoreNode);

                    GodotEntityManager = new GloGodotEntityManager();
                    ZeroNode.AddChild(GodotEntityManager);

                    ZeroNodeMapManager = new GloZeroNodeMapManager(ZeroNode);
                    ZeroNode.AddChild(ZeroNodeMapManager);

                    // ImageManager = new();
                    // SceneRootNode.AddChild(ImageManager);


                    TextureLoader = new GloTextureLoader();
                    ModelLibrary  = new Glo3DModelLibrary();
                    UIState       = new GloUIState();

                    WorldCamNode = new GloCameraMoverWorld() { Name = "WorldCamBase" };
                    SceneRootNode.AddChild(WorldCamNode);
                    WorldCamNode.CamNode.Current = true;
                    WorldCamNode.CamPosFromString(UIState.CameraMemory);


                    UIMsgQueue    = new JSONThreadsafeMessageFIFO();


                    UIMessageManager = new GloUIMessageManager();
                    SceneRootNode.AddChild(UIMessageManager);

                    IsInitialised = true;

                    GloCentralLog.AddEntry("GloGodotFactory CreateObjects Done");
                    GD.Print("- GloGodotFactory CreateObjects Done - - - - ");

                    // TestPoint();
                }
                catch (Exception ex)
                {
                    GloCentralLog.AddEntry($"GloGodotFactory.CreateObjects: Exception: {ex.Message}");
                }
            }
            else
            {
                throw new InvalidOperationException("GloGodotFactory.CreateObjects called twice.");
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------

    private void TestPoint()
    {
        GloNumeric2DArray<float> testPattern =
            GloNumeric2DArray<float>.LLATestPattern(360, 180,
                new GloLLBox() {MinLonDegs = -180.0, MinLatDegs = -90.0, MaxLonDegs = 180.0, MaxLatDegs = 90.0 });

        GloNumeric2DArrayIO<float>.SaveToCSVFile(testPattern, "C:/Util/Godot/Globe4-Maps/ElePrep/testPattern.csv", 2);
    }


}
