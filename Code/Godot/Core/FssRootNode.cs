using Godot;


// FssRootNode: Class is the top level of teh whole application, fires of initialisation activities.
public partial class FssRootNode : Node
{
    // Static reference to the root node for easy access
    private static FssRootNode _instance;

    // Called when the node enters the scene tree for the first time
    public override void _Ready()
    {
        // Initialize the static instance
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            GD.PrintErr("ERROR: More than one RootNode instance detected!");
            QueueFree(); // Ensure there is only one instance
            return;
        }

        // Create the level nodes in the Factory (singleton for common access)
        FssGodotFactory.Instance.CreateObjects(this);
    }

    // --------------------------------------------------------------------------------------------

    // Static accessor for easy access to the RootNode instance
    public static FssRootNode Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("RootNode instance is not initialized!");
            }
            return _instance;
        }
    }
}
