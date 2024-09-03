
// FssAppFactory is a central global class managing access to internal modelling element.
// FssGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;

public class FssGodotFactory
{
    public static FssZeroNode           ZeroNode { get; private set; }
    public static FssGodotEntityManager GodotEntityManager { get; private set; }
    public static FssTextureLoader      TextureLoader{ get; private set; }
    public static Fss3DModelLibrary     ModelLibrary { get; private set; }
    
    public static Node3D                SceneRootNode { get; private set; } 


    private static readonly object lockObject = new object();

    private static FssGodotFactory? SingletonInstance = null;
    private static bool IsInitializing = true;

    // The constructor could set up a lot of objects, so we add protection to ensure it doesn't get called recursively.
    public static FssGodotFactory Instance
    {
        get
        {
            //GD.Print("FssGodotFactory.Instance");
            lock (lockObject)  // Note: This locks per-thread, so can recursively call within the same thread.
            {
                if (IsInitializing)
                {
                    throw new InvalidOperationException("FssGodotFactory instance is being initialized and cannot be accessed recursively.");
                }

                if (SingletonInstance == null)
                {
                    IsInitializing = true;
                    try
                    {
                        SingletonInstance = new FssGodotFactory();
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

    private FssGodotFactory()
    {
    }

    public void CreateObjects(Node3D sceneRootNode)
    {
        lock(lockObject)
        {
            FssCentralLog.AddEntry("FssGodotFactory CreateObjects");

            SceneRootNode = sceneRootNode;

            ZeroNode = new FssZeroNode();
            SceneRootNode.AddChild(ZeroNode);

            GodotEntityManager = new FssGodotEntityManager();
            SceneRootNode.AddChild(GodotEntityManager);

            SceneRootNode.AddChild(GodotEntityManager);
            TextureLoader = new FssTextureLoader();
            ModelLibrary  = new Fss3DModelLibrary();

            IsInitializing = false;
        }
    }


}
