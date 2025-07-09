using System;
using System.Collections.Generic;
using System.Text;

using Godot;

#nullable enable

public partial class KoreGodotEntityManager : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Linked Element Management
    // --------------------------------------------------------------------------------------------
    // Linked elements are those that hang off of the platform, such as the 3D models, and geometry like radar wedges etc.
    // They are named children of the entity node, and are managed through dedicated routines.
    //
    // (Conversely unlinked elements are ones that are not attached, such as routes/waypoints. Things with a lat/long than
    //  than an offset to the entity position).

    public bool LinkedElementExists(string entityName, string elementName)
    {
        KoreGodotEntity? ent = GetEntity(entityName);

        if (ent == null)
            return false;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return true;
        }
        return false;
    }

    public void AddLinkedElement(string entityName, KoreGodotEntityElement element)
    {
        // fail if named item already exixts
        if (LinkedElementExists(entityName, element.Name))
            return;

        // Fail if the entity can't be accessed
        KoreGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return;

        // Add the element to the node to the element attitude node
        ent.AttitudeNode.AddChild(element);
    }

    public void RemoveLinkedElement(string entityName, string elementName)
    {
        // return if we can't find the entity
        KoreGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return;

        // Find the element by name and remove it
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
        KoreGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return null;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
        {
            if (currNode.Name == elementName)
                return currNode;
        }

        return null;
    }

    // Return a list of all the linked elements for an entity.
    public List<string> LinkedElementNames(string entityName)
    {
        List<string> elementNames = new List<string>();

        KoreGodotEntity? ent = GetEntity(entityName);
        if (ent == null)
            return elementNames;

        foreach (Node3D currNode in ent.AttitudeNode.GetChildren())
            elementNames.Add(currNode.Name);

        return elementNames;
    }


}
