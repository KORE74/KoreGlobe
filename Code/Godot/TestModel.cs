using System;

using Godot;

public partial class TestModel : Node3D
{
    [Export]
    public string ModelPath = "res://Resources/Plane_Paper/PaperPlanes_v002.glb";

    // Define the position and course
    private FssLLAPoint pos   = new FssLLAPoint() { LatDegs = 40, LonDegs = -70, AltMslM = 1.4f };
    private FssCourse Course  = new FssCourse()   { HeadingDegs = 0, SpeedKph = 250000 };

    // Define the model node hierarchy
    // Parent
    // |- ModelNode
    //    |- ModelResourceNode
    //    |- NodeMarkerZero
    //    |- NodeMarkerAbove
    //    |- NodeMarkerAhead

    Node3D ModelNode         = null;
    Node3D ModelResourceNode = null;
    Node3D NodeMarkerZero    = null;
    Node3D NodeMarkerAbove   = null;
    Node3D NodeMarkerAhead   = null;

    float Timer1Hz = 0f;

    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);

        if (importedModel != null)
        {
            // Root of the model and orientation
            ModelNode = new Node3D() { Name = "ModelNode" };
            ModelNode.LookAt(Vector3.Forward, Vector3.Up);
            AddChild(ModelNode);

            // Instance the model
            Node modelInstance     = importedModel.Instantiate();
            ModelResourceNode      = modelInstance as Node3D;
            ModelResourceNode.Name = "ModelResourceNode";
            ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up);

            ModelNode.AddChild(ModelResourceNode);
            ModelResourceNode.Scale    = new Vector3(0.05f, 0.05f, 0.05f); // Set the model scale
            ModelResourceNode.Position = new Vector3(0f, 0f, 0f); // Set the model position

            // Create and assign the markers
            NodeMarkerZero  = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.7f, 0.1f, 0.1f, 1f)); // zero  = red
            NodeMarkerAbove = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.1f, 0.1f, 0.8f, 1f)); // above = blue
            NodeMarkerAhead = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.1f, 0.8f, 0.1f, 1f)); // ahead = green

            NodeMarkerZero.Name  = "NodeMarkerZero - Red";
            NodeMarkerAbove.Name = "NodeMarkerAbove - Blue";
            NodeMarkerAhead.Name = "NodeMarkerAhead - Green";
            // AddChild(NodeMarkerZero);
            // AddChild(NodeMarkerAbove);
            // AddChild(NodeMarkerAhead);
            ModelNode.AddChild(NodeMarkerZero);
            ModelNode.AddChild(NodeMarkerAbove);
            ModelNode.AddChild(NodeMarkerAhead);

            UpdateModelPosition();
        }
        else
        {
            GD.PrintErr("Failed to load model: " + ModelPath);
        }
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Figure out the change in position
        double headingChangePerSec = 5;
        Course.HeadingDegs += headingChangePerSec * delta;
        FssPolarOffset offset = Course.ToPolarOffset(delta);

        // Update the position with the new offset
        pos = pos.PlusPolarOffset(offset);

        // Debug print the new position values once a second
        if (Timer1Hz < FssCoreTime.RuntimeSecs)
        {
            Timer1Hz += 1f;
            GD.Print($"RuntimeSecs: {Timer1Hz:F1} Course: {Course} Offset: {offset} Position: {pos}");
        }

        UpdateModelPosition();
    }

    // --------------------------------------------------------------------------------------------

    public void UpdateModelPosition()
    {
        // --- Define positions -----------------------

        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.04f;

        // Get the position 5 seconds ahead, or just north if stationary
        FssLLAPoint posAhead = FssLLAPoint.Zero;
        if (Course.IsStationary())
        {
            posAhead = pos;
            posAhead.LatDegs += 0.001;
        }
        else
        {
            posAhead = pos.PlusPolarOffset(Course.ToPolarOffset(5));
        }

        // --- Define vectors -----------------------

        // Define the Vector3 Offsets
        Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
        Vector3 vecAbove = FssGeoConvOperations.RealWorldToGodot(posAbove);
        Vector3 vecAhead = FssGeoConvOperations.RealWorldToGodot(posAhead);

        Vector3 diffAbove    = vecPos.DirectionTo(vecAbove);//.Normalized();
        Vector3 diffAhead    = vecPos.DirectionTo(vecAhead);//.Normalized();
        Vector3 unitVecAhead = diffAhead;//.Normalized();
        Vector3 unitVecAbove = diffAbove;//.Normalized();

        float mag = 0.025f;
        Vector3 fixedVecPlusX = new Vector3(mag, 0f, 0f);
        Vector3 fixedVecPlusY = new Vector3(0f, mag, 0f);
        Vector3 fixedVecPlusZ = new Vector3(0f, 0f, mag);
        Vector3 markerAhead = unitVecAhead * mag;
        Vector3 markerAbove = unitVecAbove * mag;

        // --- Update node -----------------------
        //ModelNode.LookAt(vecAhead, vecAbove);
        ModelNode.Position = vecPos;
        ModelNode.LookAt(vecAhead, vecAbove);

        //ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up);

        NodeMarkerZero.Position  = Vector3.Zero;
        NodeMarkerAbove.Position = fixedVecPlusY; //diffAbove;
        NodeMarkerAhead.Position = fixedVecPlusZ; //diffAhead;

        // NodeMarkerZero.Position  = vecPos;
        // NodeMarkerAbove.Position = vecAbove; //diffAbove;
        // NodeMarkerAhead.Position = vecAhead; //diffAhead;
    }
}
