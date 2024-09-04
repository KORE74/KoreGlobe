using System;
using System.Collections.Generic;
using System.Text;

using Godot;

public partial class FssGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Linked Element Management
    // --------------------------------------------------------------------------------------------
    // Linked elements are those that hang off of the platform, such as radars.
    // They are named children of the entity node, and are managed through dedicated routines.

    public bool LinkedElementExists(string entityName, string elementName)
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
                FssElementRoute newRoute = new FssElementRoute();
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
