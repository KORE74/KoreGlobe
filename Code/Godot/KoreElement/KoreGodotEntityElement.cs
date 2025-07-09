
using Godot;

// A KoreGodotEntityElement corresponds to a KorePlatformElement from KoreSim, but has the Godot Node3D as a base class
// and is dedicated to display functionality in the Godot scene graph.

// "Elements" are specifically data driven, and managed to match data in the KorePlatformElement class structure.

public partial class KoreGodotEntityElement : Node3D
{
    // A virtual function for all element child classes to output a one-line report of their contents.
    public virtual string Report()
    {
        return $"Element: {Name}";
    }
}


