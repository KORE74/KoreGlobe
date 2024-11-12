
using Godot;
using System;
using System.Collections.Generic;

// Create a contrail of points for a platform.
// Node rooted off of the zero point, not moving with the platform.

public partial class FssElementContrail : Node3D
{
    List<FssLLAPoint> TrailPoints   = new List<FssLLAPoint>();
    List<MeshInstance3D> TrailNodes = new List<MeshInstance3D>();
    int MaxTrailPoints = 60;

    string ModelName;
    bool UseModel = false;
    float TimerModelTrail = 0.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        AddTrailFromModel();
        UpdateTrail();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    public void InitElement(string platformName)
    {
        // *this* is a node added to the ZeroPoint
        Name = $"{platformName}-ContrailRoot";
    }

    public void SetModel(string modelName)
    {
        ModelName = modelName;
        UseModel = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    public void AddTrailPoint(FssLLAPoint point)
    {
        // delete the first node if we have too many - FIFO
        if (TrailPoints.Count > MaxTrailPoints)
            TrailPoints.RemoveAt(0);

        TrailPoints.Add(point);
    }

    public void AddTrailFromModel()
    {
        // Add a point from the model
        if (UseModel)
        {

            if (TimerModelTrail < FssCentralTime.RuntimeSecs)
            {
                TimerModelTrail = FssCentralTime.RuntimeSecs + 1f;

                FssLLAPoint? PlatformPos = FssEventDriver.EntityCurrLLA(ModelName);

                if (PlatformPos != null)
                    AddTrailPoint((FssLLAPoint)PlatformPos);
            }
        }
    }


    // Update called to keep the trail in place with the zero point.
    // Iterate through the points, creating new segments as needed and moving old ones to the latest position.
    public void UpdateTrail()
    {
        int numPoints = TrailPoints.Count;
        int numNodes  = TrailNodes.Count;

        // manage the number of points: If there are more points than nodes, create new nodes
        if (numPoints > numNodes)
        {
            for (int i = numNodes; i < numPoints; i++)
            {
                MeshInstance3D meshInstance = FssPrimitiveFactory.CreateGodotSphere(Vector3.Zero, 0.025f, new Color(0.8f, 0.8f, 0.8f, 1.0f));
                TrailNodes.Add(meshInstance);
                AddChild(meshInstance);
            }
        }

        // Now we have the balance, loop through the points and update the nodes
        for (int i = 0; i < numPoints; i++)
        {
            // Update the node position
            TrailNodes[i].Position = FssZeroOffset.GeZeroPointOffset(TrailPoints[i].ToXYZ());
        }
    }

}
