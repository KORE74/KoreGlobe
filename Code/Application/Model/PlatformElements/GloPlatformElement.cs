using System;

// GloElement: An item added to a platform that has some amount of additinoal functionality to be cmodelled and/or displayed.

public class GloPlatformElement
{
    public string Name { set; get; } = "Unnamed";

    // Overridable type for the element
    public virtual string Type { set; get; } = "Unknown";

    public bool Enabled { get; set; } = false;

    public GloStringDictionary Properties { get; set; } = new GloStringDictionary();

    // A virtual functino for all element child classes to output a one-line report of their contents.
    public virtual string Report()
    {
        return $"Element: {Name} ({Type})";
    }
}
