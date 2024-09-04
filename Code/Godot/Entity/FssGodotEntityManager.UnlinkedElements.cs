using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Unlinked Element Management
    // --------------------------------------------------------------------------------------------
    // Unlinked elements are those that remain fixed in a global position, such as routes. They are
    // parented off of the ZeroNode (plus children) and not the moving platform, so their access is
    // through dedicated routines.
    // We also add a platform layer to the nodes, so they can be managed as a group.

    public bool UnlinkedPlatformExists(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return true;
        }

        return false;
    }

    public void AddUnlinkedPlatform(string entityName)
    {
        if (!UnlinkedPlatformExists(entityName))
        {
            Node3D entityNode = new Node3D() { Name = entityName };
            EntityRootNode.AddChild(entityNode);
        }
    }

    public void RemoveUnlinkedPlatform(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
            {
                currNode.QueueFree();
                return;
            }
        }
    }

    public Node3D GetUnlinkedPlatform(string entityName)
    {
        foreach (Node3D currNode in EntityRootNode.GetChildren())
        {
            if (currNode.Name == entityName)
                return currNode;
        }

        return null;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Unlinked Element Management - Elements
    // --------------------------------------------------------------------------------------------
    // This section is about managing the elements that are parented off of the main Unlinked nodes,
    // namely the platform elements themselves.

    public bool UnlinkedElementExists(string entityName, string elementName)
    {
        Node3D? entityNode = GetUnlinkedPlatform(entityName);
        if (entityNode == null)
            return false;

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return true;
        }

        return false;
    }

    public void AddUnlinkedElement(string entityName, string elementName, FssPlatformElement element)
    {
        if (!UnlinkedElementExists(entityName, elementName))
        {
            Node3D? entityNode = GetUnlinkedPlatform(entityName);
            if (entityNode == null)
                return;

            Node3D elementNode = new Node3D() { Name = elementName };
            entityNode.AddChild(elementNode);

            // Determine the element type
            if (element.Type == "Route")
            {
                FssGodotPlatformElementRoute newRoute = new FssGodotPlatformElementRoute();
                newRoute.Name = elementName;
                newRoute.SetRoutePoints( (element as FssPlatformElementRoute)!.Points );

                // Add the route to the entity and scene tree
                elementNode.AddChild(newRoute);
            }
        }
    }

    public void RemoveUnlinkedElement(string entityName, string elementName)
    {
        Node3D? entityNode = GetUnlinkedPlatform(entityName);
        if (entityNode == null)
            return;

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
        Node3D? entityNode = GetUnlinkedPlatform(entityName);
        if (entityNode == null)
            return null;

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

        Node3D? entityNode = GetUnlinkedPlatform(entityName);
        if (entityNode == null)
            return elementNames;

        foreach (Node3D currNode in entityNode.GetChildren())
        {
            elementNames.Add(currNode.Name);
        }

        return elementNames;
    }


}
