using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class GloGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Unlinked Element Management
    // --------------------------------------------------------------------------------------------
    // Unlinked elements are those that remain fixed in a global position, such as routes. They are
    // parented off of the ZeroNode (plus children) and not the moving platform, so their access is
    // through dedicated routines.
    // We also add a platform layer to the nodes, so they can be managed as a group.

    // Get or Add the Unlinked Platform Node
    public Node3D UnlinkedPlatform(string entityName)
    {
        // Return the existing node if we find it
        foreach (Node3D currNode in UnlinkedRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return currNode;
        }

        // Create and add the node if we don't find it.
        Node3D entNode = new Node3D() { Name = entityName };
        UnlinkedRootNode.AddChild(entNode);

        return entNode;
    }

    public void RemoveUnlinkedPlatform(string entityName)
    {
        Node3D? entNode = UnlinkedPlatform(entityName);

        if (entNode != null)
            entNode.QueueFree();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Unlinked Element Management - Elements
    // --------------------------------------------------------------------------------------------
    // This section is about managing the elements that are parented off of the main Unlinked nodes,
    // namely the platform elements themselves.

    public bool UnlinkedElementExists(string entityName, string elementName)
    {
        Node3D entityNode = UnlinkedPlatform(entityName);

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return true;
        }

        return false;
    }

    public void AddUnlinkedElement(string entityName, GloGodotPlatformElement element)
    {
        Node3D entityNode = UnlinkedPlatform(entityName);
        entityNode.AddChild(element);
    }

    public void RemoveUnlinkedElement(string entityName, string elementName)
    {
        Node3D entityNode = UnlinkedPlatform(entityName);

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            if (currNode.Name == elementName)
            {
                currNode.QueueFree();
                return;
            }
        }
    }

    public Node3D? GetUnlinkedElement(string entityName, string elementName)
    {
        Node3D entityNode = UnlinkedPlatform(entityName);

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return currNode;
        }

        return null;
    }

    public List<string> UnlinkedElementNames(string entityName)
    {
        List<string> elementNames = new List<string>();

        Node3D entityNode = UnlinkedPlatform(entityName);

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            elementNames.Add(currNode.Name);
        }

        return elementNames;
    }


}
