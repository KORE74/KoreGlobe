using Godot;
using System;

public partial class TestModel : Node3D
{
    [Export]
    public string ModelPath = "res://Resources/Plane_Paper/PaperPlanes_v002.glb";

    FssLLAPoint pos = new FssLLAPoint() { LatDegs = 40, LonDegs = 10, AltMslM = 1.4f };
    private FssCourse Course = new FssCourse() { HeadingDegs = 0, SpeedKph = 100 };

    Node3D ModelNode = null;
    Node3D ModelResourceNode = null;

    Node3D NodeMarkerZero = null;
    Node3D NodeMarkerAbove = null;
    Node3D NodeMarkerAhead = null;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);


        if (importedModel != null)
        {
            // Root of the model and orientation
            ModelNode = new Node3D();
            AddChild(ModelNode);

            // Instance the model
            Node modelInstance = importedModel.Instantiate();
            ModelResourceNode = modelInstance as Node3D;
            ModelNode.AddChild(ModelResourceNode);
            ModelResourceNode.Scale = new Vector3(0.05f, 0.05f, 0.05f); // Set the model scale


            NodeMarkerZero  = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.7f, 0.1f, 0.1f, 1f)); // dark red
            NodeMarkerAbove = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.1f, 0.1f, 0.8f, 1f)); // above = blue
            NodeMarkerAhead = FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.1f, 0.8f, 0.1f, 1f)); // ahead = green

            ModelNode.AddChild(NodeMarkerZero);
            ModelNode.AddChild(NodeMarkerAbove);
            ModelNode.AddChild(NodeMarkerAhead);



            // ModelNode.AddChild( FssPrimitiveFactory.CreateSphere(Vector3.Zero,  0.005f, new Color(0.7f, 0.1f, 0.1f, 1f)) ); // dark red
            // ModelNode.AddChild( FssPrimitiveFactory.CreateSphere(diffAbove,     0.005f, new Color(0.1f, 0.1f, 0.8f, 1f)) ); // above = blue
            // ModelNode.AddChild( FssPrimitiveFactory.CreateSphere(diffAhead,     0.005f, new Color(0.1f, 0.8f, 0.1f, 1f)) ); // ahead = green

            // AddChild( FssPrimitiveFactory.CreateSphere(vecPos,   0.005f, new Color(0.7f, 0.7f, 0.1f, 1f)) ); // Yellow
            // AddChild( FssPrimitiveFactory.CreateSphere(vecAbove, 0.005f, new Color(0.7f, 0.0f, 0.7f, 1f)) ); // Yellow
            // AddChild( FssPrimitiveFactory.CreateSphere(vecAhead, 0.005f, new Color(0.0f, 0.7f, 0.7f, 1f)) ); // Yellow

            UpdateModelPosition();
        }
        else
        {
            GD.PrintErr("Failed to load model: " + ModelPath);
        }
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        double headingChangePerSec = 5;

        Course.HeadingDegs += headingChangePerSec * delta;
        Course.SpeedKph = 20000;

        FssPolarOffset offset = Course.ToPolarOffset(delta);


        // Update the position
        pos = pos.PlusPolarOffset(offset);

       //pos.LonDegs += 5f * delta;

        GD.Print($"Course: {Course} Offset: {offset} Position: {pos}");

        UpdateModelPosition();
    }


    public void UpdateModelPosition()
    {
        // --- Define positions -----------------------

        // Define the position and associated up direction for the label
        FssLLAPoint posAbove = pos;
        posAbove.AltMslM += 0.01f;

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

        Vector3 diffAbove    = (vecAbove - vecPos);//.Normalized();
        Vector3 diffAhead    = (vecAhead - vecPos);//.Normalized();
        Vector3 unitVecAhead = diffAhead.Normalized();
        Vector3 unitVecAbove = diffAbove.Normalized();

        float mag = 0.025f;
        Vector3 fixedVecRight = new Vector3(mag, 0f, 0f);
        Vector3 fixedVecAbove = new Vector3(0f, mag, 0f);
        Vector3 fixedVecAhead = new Vector3(0f, 0f, mag);

        // --- Update node -----------------------
        ModelNode.LookAt(unitVecAhead, unitVecAbove);
        ModelNode.Position = vecPos;

        NodeMarkerZero.Position  = Vector3.Zero;
        NodeMarkerAbove.Position = fixedVecAbove; //diffAbove;
        NodeMarkerAhead.Position = fixedVecAhead; //diffAhead;
}

}
