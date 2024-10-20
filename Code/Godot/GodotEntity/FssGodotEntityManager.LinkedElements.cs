using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class FssGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Linked Element Management
    // --------------------------------------------------------------------------------------------
    // Linked elements are those that hang off of the platform, such as radars.
    // They are named children of the entity node, and are managed through dedicated routines.

    public bool LinkedElementExists(string entityName, string elementName)
    {
        FssGodotEntity? ent = GetEntity(entityName);

        if (ent == null)
            return false;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return true;
        }

        return false;
    }

    public void AddLinkedElement(string entityName, FssGodotPlatformElement element)
    {
        string elementName = element.Name;
        if (!LinkedElementExists(entityName, elementName))
        {
            FssGodotEntity? ent = GetEntity(entityName);

            if (ent == null)
                return;

            ent.AttitudeNode.AddChild(element);
        }
    }

    public void RemoveLinkedElement(string entityName, string elementName)
    {
        FssGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
        {
            if (currNode.Name == elementName)
            {
                currNode.QueueFree();
                return;
            }
        }
    }

    public Node3D? GetLinkedElement(string entityName, string elementName)
    {
        FssGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return null;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return currNode;
        }

        return null;
    }

    public List<string> LinkedElementNames(string entityName)
    {
        List<string> elementNames = new List<string>();

        FssGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return elementNames;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
            elementNames.Add(currNode.Name);

        return elementNames;
    }


}
