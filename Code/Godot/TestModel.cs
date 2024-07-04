using Godot;
using System;

public partial class TestModel : Node3D
{
    [Export]
    public string ModelPath = "res://Resources/Plane_Paper/PaperPlanes_v002.glb";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);

        float distance = 1.221f;
        int lat = 40;
        int lon = 10;


        FssLLAPoint pos = new FssLLAPoint() { LatDegs = 40, LonDegs = 10, AltMslM = 1.4f };

        if (importedModel != null)
        {
            // Define the position and associated up direction for the label
            FssLLAPoint posAbove = pos;
            posAbove.AltMslM += 0.01f;
            FssLLAPoint posAhead = pos;
            posAhead.LonDegs += 0.25f;

            Vector3 posDebug = FssGeoConvOperations.RealWorldToGodot(distance, 40, 0);

            // Define the Vector3 Offsets
            Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
            Vector3 vecAbove = FssGeoConvOperations.RealWorldToGodot(posAbove);
            Vector3 vecAhead = FssGeoConvOperations.RealWorldToGodot(posAhead);

            Vector3 diffAbove = (vecAbove - vecPos).Normalized();
            Vector3 diffAhead = (vecAhead - vecPos).Normalized();

            // Instance the model
            Node modelInstance = importedModel.Instantiate();
            Node3D model3D = modelInstance as Node3D;

            // Add the model instance as a child of the current node
            AddChild(model3D);

            // Set the model scale
            //model3D.Scale = new Vector3(0.1f, 0.1f, 0.1f);

            // use LookAt to setup the rotation
            model3D.LookAt(vecAhead, diffAbove);
            model3D.Position = vecPos;
            model3D.Scale = new Vector3(0.1f, 0.1f, 0.1f);


            // model3D.AddChild( FssPrimitiveFactory.CreateSphere(Vector3.Zero, 0.1f,  new Color(0.7f, 0.1f, 0.1f, 1f)) ); // dark red
            // model3D.AddChild( FssPrimitiveFactory.CreateSphere(vecAbove,     0.02f, new Color(0.1f, 0.1f, 0.8f, 1f)) ); // above = blue
            // model3D.AddChild( FssPrimitiveFactory.CreateSphere(vecAhead,     0.02f, new Color(0.1f, 0.8f, 0.1f, 1f)) ); // ahead = green

            AddChild( FssPrimitiveFactory.CreateSphere(vecPos,   0.005f, new Color(0.7f, 0.7f, 0.1f, 1f)) ); // Yellow
            AddChild( FssPrimitiveFactory.CreateSphere(vecAbove, 0.005f, new Color(0.7f, 0.0f, 0.7f, 1f)) ); // Yellow
            AddChild( FssPrimitiveFactory.CreateSphere(vecAhead, 0.005f, new Color(0.0f, 0.7f, 0.7f, 1f)) ); // Yellow
        }
        else
        {
            GD.PrintErr("Failed to load model: " + ModelPath);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
