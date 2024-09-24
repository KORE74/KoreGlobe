
// FssAppFactory is a central global class managing access to internal modelling element.
// FssGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;

public class FssGodotFactory
{
    public Node3D                SceneRootNode      { get; private set; } = null;
    public FssZeroNode           ZeroNode           { get; private set; } = null;
    public FssMapManager         EarthCoreNode      { get; private set; } = null;
    //public Node3D                EntityRootNode     { get; private set; } = null;

    public FssGodotEntityManager GodotEntityManager { get; private set; } = null;
    public FssTextureLoader      TextureLoader      { get; private set; } = null;
    public Fss3DModelLibrary     ModelLibrary       { get; private set; } = null;
    public FssUIState            UIState            { get; private set; } = null;

    // Singleton pattern
    private static readonly object  lockObject        = new object();
    private static FssGodotFactory? SingletonInstance = null;
    private static bool             IsInitialised     = false;
    private static bool             IsCreating        = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Singleton Pattern
    // --------------------------------------------------------------------------------------------

    // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    public static FssGodotFactory Instance
    {
        get
        {
            //GD.Print("FssGodotFactory.Instance");
            lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
            {
                if (IsCreating)
                {
                    throw new InvalidOperationException("FssGodotFactory instance is being initialized and cannot be accessed recursively.");
                }

                if (SingletonInstance == null)
                {
                    IsCreating = true;
                    try
                    {
                        SingletonInstance = new FssGodotFactory();
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
    private FssGodotFactory()
    {
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
                FssCentralLog.AddEntry("FssGodotFactory CreateObjects");

                SceneRootNode = sceneRootNode;

                ZeroNode = new FssZeroNode();
                SceneRootNode.AddChild(ZeroNode);

                EarthCoreNode = new FssMapManager();
                SceneRootNode.AddChild(EarthCoreNode);

                GodotEntityManager = new FssGodotEntityManager();
                ZeroNode.AddChild(GodotEntityManager);

                TextureLoader = new FssTextureLoader();
                ModelLibrary  = new Fss3DModelLibrary();
                UIState       = new FssUIState();

                IsInitialised = true;
            }
            else
            {
                throw new InvalidOperationException("FssGodotFactory.CreateObjects called twice.");
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Helpers
    // --------------------------------------------------------------------------------------------



}
