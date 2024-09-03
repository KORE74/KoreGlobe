
// FssAppFactory is a central global class managing access to internal modelling element.
// FssGodotFactory is a class that manages the creation of Godot entities.

using System;
using System.Collections.Generic;

using Godot;


public static class FssGodotFactory
{
    public static FssZeroNode ZeroNode = new FssZeroNode();

    public static FssGodotEntityManager GodotEntityManager = new FssGodotEntityManager();


    public static void CreateCoreObjects()
    {
        ZeroNode.Name = "ZeroNode";
        ZeroNode.AddChild(FssZeroNode.EntityRootNode);
        //ZeroNode.CreateDebugMarker();
        ZeroNode.AddChild(GodotEntityManager);
    }
}