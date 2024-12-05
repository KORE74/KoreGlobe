
// FssAppFactory is a central global class managing access to internal modelling element.
// FssGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;

#nullable enable

public class FssGodotFactory
{
    // Core Nodes
    public Node                  SceneRootNode      { get; private set; }
    public Node                  ServicesNode       { get; private set; }

    // Nodes with positional requirements
    public FssZeroNodeMapManager ZeroNodeMapManager { get; private set; }
    public FssGodotEntityManager GodotEntityManager { get; private set; }
    public FssCameraMoverWorld   CameraMoverWorld   { get; private set; }

    // Assets
    public Fss3DModelLibrary     ModelLibrary       { get; private set; }
    public FssTextureManager     TextureManager     { get; private set; }

    // UI
    public FssUIState            UIState            { get; private set; }

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


    // --------------------------------------------------------------------------------------------
    // MARK: Initialization
    // --------------------------------------------------------------------------------------------

    public void CreateObjects(Node sceneRootNode)
    {
        lock(lockObject)
        {
            if (!IsInitialised)
            {
                FssCentralLog.AddEntry("FssGodotFactory CreateObjects");

                // Record the top of the scene tree
                SceneRootNode = sceneRootNode;

                // Create key nodes
                ServicesNode = new Node() { Name = "ServicesNode" };
                SceneRootNode.AddChild(ServicesNode);

                // Create and add the nodes that have no postional requirements
                TextureManager = new FssTextureManager();
                ServicesNode.AddChild(TextureManager);

                // Create and add the nodes that have postional requirements
                GodotEntityManager = new FssGodotEntityManager();
                SceneRootNode.AddChild(GodotEntityManager);
                ZeroNodeMapManager = new FssZeroNodeMapManager();
                SceneRootNode.AddChild(ZeroNodeMapManager);
                CameraMoverWorld = new FssCameraMoverWorld();
                SceneRootNode.AddChild(CameraMoverWorld);

                // CameraMoverXYZ = new FssCameraMover();
                // SceneRootNode.AddChild(CameraMoverXYZ);
                // CameraMoverXYZ.Current = true;

                // Create the objects that are not godot nodes
                //TextureLoader = new FssTextureLoader();
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
}
