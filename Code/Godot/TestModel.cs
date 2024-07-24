using System;

using Godot;

// TestModel: Class to test the 3D movement of a platform and camera.

public partial class TestModel : Node3D
{
    [Export]
    public string ModelPath = "res://Resources/Plane_Paper/PaperPlanes_v002.glb";
    //public string ModelPath = "res://Resources/FA-18F_v02.glb";


    Node? CoreNode;
    FssLLAPoint PrevPos = new FssLLAPoint() { LatDegs = 0, LonDegs = -70, AltMslM = 1.4f };

    ShaderMaterial     matWire  = FssMaterialFactory.WireframeWhiteMaterial();
    StandardMaterial3D matGrey  = FssMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0.5f, 1f));

    // Define the position and course
    private FssLLAPoint pos   = new FssLLAPoint() { LatDegs = 0, LonDegs = -70, AltMslM = 1.4f };
    private FssCourse Course  = new FssCourse()   { HeadingDegs = 0, SpeedKph = 1200000 };

    private FssPolarOffset CameraOffset = new FssPolarOffset() { RangeM = -0.7f, AzDegs = 45, ElDegs = 45 };

    // Define the model node hierarchy
    // Parent
    // |- ModelNode
    //    |- ModelResourceNode
    //    |- NodeMarkerZero
    //    |- NodeMarkerAbove
    //    |- NodeMarkerAhead
    //    |- ModelCamera

    Node3D ModelNode         = null;
    Node3D ModelResourceNode = null;
    Node3D TrailNode         = null;
    // Node3D NodeMarkerZero    = null;
    // Node3D NodeMarkerAbove   = null;
    // Node3D NodeMarkerAhead   = null;
    Camera3D ModelCamera     = null;

    FssCyclicIdGenerator IdGen = new FssCyclicIdGenerator(250);

    private string randomString = FssRandomStringGenerator.GenerateRandomString(5);

    float Timer1Hz = 0f;
    float Timer4Hz = 0f;
    private bool OneShotFlag = false;

    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Define the position to start
        double randLat = FssValueUtils.RandomInRange(-30, 30);
        double randLon = FssValueUtils.RandomInRange(-150, 150);
        double randAlt = FssValueUtils.RandomInRange(1.3, 1.5);
        pos   = new FssLLAPoint() { LatDegs = randLat, LonDegs = randLon, AltMslM = randAlt };

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
            //ModelResourceNode.Scale    = new Vector3(0.005f, 0.005f, 0.005f); // Set the model scale
            ModelResourceNode.Position = new Vector3(0f, 0f, 0f); // Set the model position

            FssPrimitiveFactory.AddAxisMarkers(ModelNode, 0.02f, 0.005f);

            // ---------------------------------------

            // Add a wedge in front
            FssMeshBuilder frontWedge = new FssMeshBuilder();
            frontWedge.AddPyramidByPoint(0.3f, 0.1f, 0.04f);

            ArrayMesh meshData = frontWedge.Build2("Sphere", false);

            var matWire      = FssMaterialFactory.WireframeWhiteMaterial();
            var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));

            MeshInstance3D meshInstance    = new() { Name = "FrontWedge" };
            meshInstance.Mesh              = meshData;
            meshInstance.MaterialOverride  = matTransBlue;

            MeshInstance3D meshInstanceW   = new() { Name = "FrontWedgeWire" };
            meshInstanceW.Mesh             = meshData;
            meshInstanceW.MaterialOverride = matWire;

            ModelNode.AddChild(meshInstance);
            ModelNode.AddChild(meshInstanceW);


            // Find a node in the global tree
            Node GlobeRoot = GetParent();

            if (GlobeRoot == null)
            {
                GD.PrintErr("Failed to find the GlobeRoot node");
                return;
            }

           // CoreNode = FssAppNode.Instance.FindNode("GlobeCore");
            TrailNode = new Node3D() { Name = FssRandomStringGenerator.GenerateRandomString(5) };
            GlobeRoot.AddChild(TrailNode);

            // -----------

            // Create the chase-camera
            ModelCamera = new Camera3D() { Name = "ModelCamera" };
            ModelCamera.Fov = 35;
            ModelNode.AddChild(ModelCamera);
            // ModelCamera.Current = true;

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

        if (OneShotFlag == false)
        {
            CreateStruct();
            OneShotFlag = true;
        }

        //CoreNode = FssAppNode.Instance.FindNode("GlobeCore");

        // Figure out the change in position
        Course.HeadingDegs += 5 * delta;
        FssPolarOffset offset = Course.ToPolarOffset(delta);

        // Pan the camera
        CameraOffset.AzDegs += 10 * delta;

        // Update the position with the new offset
        pos = pos.PlusPolarOffset(offset);

        // Debug print the new position values once a second
        if (Timer1Hz < FssCoreTime.RuntimeSecs)
        {
            // check the previous position markers at 1Hz.
            Timer1Hz = (float)(FssCoreTime.RuntimeIntSecs + 1); // Update the timer to the next whole second
            GD.Print($"RuntimeSecs: {Timer1Hz:F1} Course: {Course} Offset: {offset} Position: {pos}");

        }

        if (Timer4Hz < FssCoreTime.RuntimeSecs)
        {
            Timer4Hz = FssCoreTime.RuntimeSecs + 0.2f; // Update the timer to the next whole second

            if (TrailNode != null)
            {
                // Create a new ID
                string nextId = IdGen.NextId();

                // If a child node with the ID exists, delete it.

                TrailNode.GetNodeOrNull(nextId)?.QueueFree();
                if (TrailNode.HasNode(nextId))
                {
                    Node childNode = TrailNode.GetNode(nextId);
                    TrailNode.RemoveChild(childNode);
                    childNode.QueueFree();
                }

                // Create a new sphere at the current position, and the ID
                Node3D childSphere = FssPrimitiveFactory.CreateSphere(Vector3.Zero, 0.005f, new Color(0.9f, 0.9f, 0.9f, 1f));
                childSphere.Name = $"{randomString}{nextId}";
                TrailNode.AddChild(childSphere);
                childSphere.Name = nextId;

                // get the current position to assign to the sphere
                Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
                childSphere.Position = vecPos;

                // Create a new cylinder at the current position, and the ID
                Vector3 vecPrevPos   = FssGeoConvOperations.RealWorldToGodot(PrevPos);
                Vector3 vecDiff = vecPrevPos - vecPos;

                FssMeshBuilder meshBuilder = new FssMeshBuilder();
                meshBuilder.AddCylinder(Vector3.Zero, vecDiff, 0.005f, 0.005f, 12, true);

                ArrayMesh meshData = meshBuilder.Build2("Wedge", false);
                MeshInstance3D meshInstance = new();
                meshInstance.Mesh = meshData;
                meshInstance.MaterialOverride = matGrey;

                childSphere.AddChild(meshInstance);
                //meshInstance.Position = vecDiff / 2.0f;
                //meshInstance.LookAt(vecDiff, Vector3.Up);

                PrevPos = pos;
            }

        }


        // Update the node positions and orientations
        UpdateModelPosition();
    }

    // --------------------------------------------------------------------------------------------

    // Adding nodes in the rest of the tree, from a call withing the _Ready() function is proving
    // to be problematic. This is called as a one-off in Process.

    public void CreateStruct()
    {
        Node p = GetParent();
        if (p != null)
        {
            GD.Print($"Parent: {p.Name}");

            string trailName = FssRandomStringGenerator.GenerateRandomString(5);
            TrailNode = new Node3D() { Name = $"TrailNode-{trailName}" };
            //TrailNode = new Node3D() { Name = "TrailNode" };
            p.AddChild(TrailNode);

            GD.Print($"TrailNode: {TrailNode.Name}");
        }
    }


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
            posAhead = pos.PlusPolarOffset(Course.ToPolarOffset(-5));
        }

        // --- Define vectors -----------------------

        // Define the Vector3 Offsets
        Vector3 vecPos   = FssGeoConvOperations.RealWorldToGodot(pos);
        Vector3 vecAbove = FssGeoConvOperations.RealWorldToGodot(posAbove);
        Vector3 vecAhead = FssGeoConvOperations.RealWorldToGodot(posAhead);


        FssEntityV3 platVecs = FssGeoConvOperations.ReadWorldToStruct(pos, Course);



        // Update node position and orientation
        ModelNode.Position = platVecs.Position;// vecPos;
        ModelNode.LookAt(platVecs.PosAhead, platVecs.PosAbove);

        // Update camera position and orientation
        FssXYZPoint camOffsetXYZ = CameraOffset.ToXYZ();
       ModelCamera.Position = new Vector3((float)camOffsetXYZ.X, -(float)camOffsetXYZ.Y, -(float)camOffsetXYZ.Z);
        ModelCamera.LookAt(vecPos, vecAbove);
    }
}
