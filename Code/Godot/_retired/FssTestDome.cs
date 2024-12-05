using System;

using Godot;

// Class to add an entity from the PlatformModel.

public partial class FssTestDome : Node3D
{
    private FssLLAPoint pos   = new FssLLAPoint() { LatDegs = 35, LonDegs = 30, AltMslM = 1.22f };
    Node3D ModelNode         = null;

    public override void _Ready()
    {
        GD.Print("FssTestSim._Ready");

            ModelNode = new Node3D() { Name = "TestDome" };
            ModelNode.LookAt(Vector3.Forward, Vector3.Up);
            AddChild(ModelNode);

            // ---------------------------------------

            // Add a Dome
            FssMeshBuilder meshBuilder = new FssMeshBuilder();
            meshBuilder.AddHemisphere(Vector3.Zero, 0.15f, 25);

            ArrayMesh meshData = meshBuilder.Build2("Dome", false);

            var matWire      = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);
            var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

            MeshInstance3D meshInstance    = new() { Name = "Dome" };
            meshInstance.Mesh              = meshData;
            meshInstance.MaterialOverride  = matTransBlue;

            MeshInstance3D meshInstanceW   = new() { Name = "DomeWire" };
            meshInstanceW.Mesh             = meshData;
            meshInstanceW.MaterialOverride = matWire;

            ModelNode.AddChild(meshInstance);
            ModelNode.AddChild(meshInstanceW);

            // ---------------------------------------

    }

    public override void _Process(double delta)
    {
        UpdateModelPosition();
    }

    public void UpdateModelPosition()
    {
        // --- Define positions -----------------------

        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;

            posAhead = pos;
            posAhead.LatDegs += 0.001;

        // --- Define vectors -----------------------

        // Define the Vector3 Offsets
        //Vector3 vecPos   = FssZeroOffsetOperations.RealWorldToGodot(pos);
        //Vector3 vecAbove = FssZeroOffsetOperations.RealWorldToGodot(posAbove);
        //Vector3 vecAhead = FssZeroOffsetOperations.RealWorldToGodot(posAhead);
//
        //FssEntityV3 platVecs = FssZeroOffsetOperations.RealWorldToStruct(pos, FssCourse.Zero);
//
        //// Update node position and orientation
        //ModelNode.Position = platVecs.Position;// vecPos;
        //ModelNode.LookAt(platVecs.PosAhead, platVecs.PosAbove);

        // Update camera position and orientation
        // FssXYZPoint camOffsetXYZ = CameraOffset.ToXYZ();
        // ModelCamera.Position = new Vector3((float)camOffsetXYZ.X, -(float)camOffsetXYZ.Y, -(float)camOffsetXYZ.Z);
        // ModelCamera.LookAt(vecPos, vecAbove);
    }
}
