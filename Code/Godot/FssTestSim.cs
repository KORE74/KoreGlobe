using System;

using Godot;

// Class to add an entity from the PlatformModel.

public partial class FssTestSim : Node3D
{
    private string ModelName = "TEST-003";
    private bool   IsCreated = false;

    private Node3D ModelNode = null;

    public override void _Ready()
    {
        GD.Print("FssTestSim._Ready");
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
        }

        if (IsCreated)
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


        // GD.Print("FssTestSim._Process");
    }

}
