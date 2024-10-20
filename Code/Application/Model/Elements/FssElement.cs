using System;

// FssElement: An item added to a platform that has some amount of additinoal functionality to be cmodelled and/or displayed.

public class FssElement
{
    public string Name { set; get; } = "Unnamed";

    // Overridable type for the element
    public virtual string Type { set; get; } = "Unknown";

    public bool Enabled { get; set; } = false;

    // A virtual functino for all element child classes to output a one-line report of their contents.
    public virtual string Report()
    {
        return $"Element: {Name} ({Type})";
    }
}
