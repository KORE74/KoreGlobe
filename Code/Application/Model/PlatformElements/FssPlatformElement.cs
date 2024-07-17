using System;

// FssElement: An item added to a platform that has some amount of additinoal functionality to be cmodelled and/or displayed.

public class FssPlatformElement
{
    public string Name {set; get; } = "Unnamed";

    // Overridable type for the element
    public string Type {set; get; } = "Unknown";

    // Do we add the routes here?

}
