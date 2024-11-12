using Godot;

public partial class FssRootNode : Node
{
    // Static reference to the root node for easy access
    private static FssRootNode   _instance;

    // References to the lower level nodes
    public  FssZeroNode          ZeroNode        { get; private set; }
    //public  Node2D               UIRoot          { get; private set; }
    //public  FssIOManager         IOManager       { get; private set; }
    //public  FssResourceManager   ResourceManager { get; private set; }

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
            GD.PrintErr("Warning: More than one RootNode instance detected!");
            QueueFree(); // Ensure there is only one instance
            return;
        }

        // Create the level nodes
        ZeroNode        = new FssZeroNode();
       // UIRoot          = (CanvasLayer)ResourceLoader.Load("res://Scenes/ui_top.tscn").Instance();
        //IOManager       = new FssIOManager();
        //ResourceManager = new FssResourceManager();

        // Add the level nodes to the scene tree
        AddChild(ZeroNode);
        //AddChild(UIRoot);
        //AddChild(IOManager);
        //AddChild(ResourceManager);
    }

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
