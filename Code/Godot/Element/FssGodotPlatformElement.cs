
using Godot; 

// A FssGodotPlatformElement corresponds to a FssPlatformElement, but has the Godot Node3D as a base class
// and is dedicated to display functionality in the Godot scene graph.

public partial class FssGodotPlatformElement : Node3D
{
    // Overridable type for the element
    public virtual string ElemType {set; get; } = "Unknown";

    // A virtual functino for all element child classes to output a one-line report of their contents.
    public virtual string Report()
    {
        return $"Element: {Name} ({ElemType})";
    }
}