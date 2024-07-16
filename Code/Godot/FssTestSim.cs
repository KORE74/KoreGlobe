using System;

using Godot;

// Class to add an entity from the PlatformModel.

public partial class FssTestSim : Node3D
{
    public  string ModelName = "TEST-003";
    private bool   IsCreated = false;

    private Node3D ModelNode = null;

    public override void _Ready()
    {
        GD.Print("FssTestSim._Ready");
    }

    public void SetModelName(string modelName)
    {
        ModelName = modelName;
        FssCentralLog.AddEntry($"FssTestSim: ModelName set to {ModelName}");
    }

    public override void _Process(double delta)
    {
        // Find the model
        FssPlatform? model = FssAppFactory.Instance.PlatformManager.PlatForName(ModelName);

        // If the model is not null, and we're not yet setup to look at it
        if ((model != null) && (!IsCreated))
        {
            // Create the nodes
            ModelNode = new Node3D() { Name = "ModelNode" };
            AddChild(ModelNode);
            FssPrimitiveFactory.AddAxisMarkers(ModelNode, 0.02f, 0.005f);
            IsCreated = true;

            FssCentralLog.AddEntry($"FssTestSim: Node3D {ModelName} created.");
        }

        if ((model != null) && (IsCreated))
        {
            // Get the model's start position
            FssLLAPoint pos    = model.Kinetics.CurrPosition;
            FssCourse   course = model.Kinetics.CurrCourse;

            // Adjust for current test situation
            pos.AltMslM = 1.25f;

            // Create the Vector3 positions
            FssEntityV3 platformV3 = FssGeoConvOperations.ReadWorldToStruct(pos, course);

            ModelNode.LookAt(platformV3.PosAhead, platformV3.PosAbove);
            ModelNode.Position = platformV3.Position;
        }

        // If the model is null, delete ourselves
        if ((model == null) && (IsCreated))
        {
            ModelNode.QueueFree();
            IsCreated = false;
            FssCentralLog.AddEntry($"FssTestSim: Node3D {ModelName} deleted.");
        }


        // GD.Print("FssTestSim._Process");
    }

}
