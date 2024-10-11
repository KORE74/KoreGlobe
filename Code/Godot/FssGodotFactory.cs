
// FssAppFactory is a central global class managing access to internal modelling element.
// FssGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public class FssGodotFactory
{
    public Node3D                SceneRootNode      { get; private set; }
    public FssZeroNode           ZeroNode           { get; private set; }
    public FssMapManager         EarthCoreNode      { get; private set; }
    //public Node3D                EntityRootNode     { get; private set; }

    public FssGodotEntityManager GodotEntityManager { get; private set; }
    public FssTextureLoader      TextureLoader      { get; private set; }
    public Fss3DModelLibrary     ModelLibrary       { get; private set; }
    public FssUIState            UIState            { get; private set; }

    // Singleton pattern
    private static readonly object  lockObject        = new object();
    private static FssGodotFactory? SingletonInstance = null;
    private static bool             IsInitialised     = false;
    private static bool             IsCreating        = false;

    //public FssMapManager MapManager => EarthCoreNode;

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
