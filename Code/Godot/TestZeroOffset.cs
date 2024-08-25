using Godot;
using System;
using System.Collections.Generic;

public partial class TestZeroOffset : Node3D
{
    // The two control nodes
    Node3D EarthCoreNode;
    Node3D ZeroNode;
    Node3D EntityRootNode;

    Node3D PlaformBaseNode;
    Node3D ModelResourceNode;

    Node3D TestNode;
    Node3D TestNodeAhead;
    Node3D TestNodeAbove;

    FssMapManager      MapManager;
    FssElementContrail ElementContrail;
    FssElementRoute    ElementRoute;
    FssGodotEntityManager   EntityManager;
    FssCameraMoverWorld WorldCamNode;

    Material matColorRed;
    Material matColorBlue;
    Material matWire;

    private float MarkerSize  = 0.2f;
    private bool  WithWire = true;

    private float AnimAzDegs  = 0f;
    private float AnimElDegs  = 0f;
    private float AnimAzDelta = 1.55f;
    private float AnimElDelta = 1.05f;

    private float UIPollTimer = 0.0f;
    private float UIPollTimer2 = 0.0f;
    private float PollTimerTrailNode = 0.0f;



    private FssLLAPoint    PlatformPos;
    private FssCourse      PlatformCourse;
    private FssCourseDelta PlatformCourseDelta;


    private float TimerContrail = 0.0f;

    // private FssCyclicIdGenerator IdGen = new FssCyclicIdGenerator(250);
    // private string randomString = FssRandomStringGenerator.GenerateRandomString(5);

    // --------------------------------------------------------------------------------------------
    // MARK: Node _Ready and _Process
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Randomize the zero point in each run so we don't bake-in assumptions.
        double randomLat = FssValueUtils.RandomInRange(50, 52);
        double randomLon = FssValueUtils.RandomInRange(-4, -2);

        // Init the real world zero pos
        FssLLAPoint zeroPos = new FssLLAPoint() {
            LatDegs = randomLat,
            LonDegs = randomLon,
            RadiusM = FssPosConsts.EarthRadiusM };
        FssZeroOffset.SetLLA(zeroPos);

        // Init materials
        matColorRed  = FssMaterialFactory.SimpleColoredMaterial(new Color(0.9f, 0.3f, 0.3f, 1f));
        matColorBlue = FssMaterialFactory.SimpleColoredMaterial(new Color(0.3f, 0.3f, 0.9f, 1f));
        matWire      = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);

        FssZeroOffset.ReportConsts();

        // Create Nodes
        CreateCoreNode();

        CreatePlatform();
        CreatePlatformNodes();

        //Fss3DModelLibrary.TestLoadModel(ZeroNode);

        bool DLCTesting = true;

        if (DLCTesting)
        {
            //FssDlcOperations.CreateDlc();

            FssDlcOperations.LoadDlc();

            // List<string> fileList = FssGodotFileOperations.ListFiles("res://DLCPrep");
            // foreach (string file in fileList)
            // {
            //     GD.Print($"=> File: {file}");
            // }

            // List<string> dlcList = FssGodotFileOperations.ListLoadedDLCs();
            // foreach (string dlc in dlcList)
            // {
            //     GD.Print($"=> DLC: {dlc}");

            //     List<string> dlcFiles = FssGodotFileOperations.ListFiles(dlc);
            //     foreach (string file in dlcFiles)
            //     {
            //         GD.Print($"=> DLC {dlc} // DLCFile: {file}");

            //         // if the file is a JSON, load it into the library
            //         if (file.EndsWith(".json"))
            //         {
            //             GD.Print($"Processing JSON file: {file}");

            //             // Use the godot virtual file system to open and rad te file
            //             using FileAccess filea = FileAccess.Open(file, FileAccess.ModeFlags.Read);
            //             string content = filea.GetAsText();



            //             // deserialize the JSON string into a list of Fss3DModelInfo objects
            //             //<List<Fss3DModelInfo> modelList = JsonSerializer.Deserialize<List<Fss3DModelInfo>>(jsonString);

            //             Fss3DModelLibrary.DeserializeJSONConfig(content);
            //         }

            //     }

            // FssDlcOperations.DlcInv();

            // }

        }
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (UIPollTimer < FssCoreTime.RuntimeSecs)
        {
            UIPollTimer = FssCoreTime.RuntimeSecs + 1f; // Update the timer to the next whole second

            // GD.Print($"ZeroOffset: Lat:{FssZeroOffset.RwZeroPointLLA.LatDegs:0.00} Lon:{FssZeroOffset.RwZeroPointLLA.LonDegs:0.00} RadiusM:{FssZeroOffset.RwZeroPointLLA.RadiusM:0.00}");
            // GD.Print($"ZeroOffset: X:{FssZeroOffset.RwZeroPointXYZ.X:0.00} Y:{FssZeroOffset.RwZeroPointXYZ.Y:0.00} Z:{FssZeroOffset.RwZeroPointXYZ.Z:0.00}");

            // GD.Print($"Platform: Lat:{PlatformPos.LatDegs:0.00} Lon:{PlatformPos.LonDegs:0.00} Alt:{PlatformPos.AltMslM:0.00} RadiusM:{PlatformPos.RadiusM:0.00}");
            // GD.Print($"Platform: Hdg:{PlatformCourse.HeadingDegs:0.00} Spd:{PlatformCourse.SpeedMps:0.00}");

            // GD.Print($"PlatformNodePos: X:{PlaformBaseNode.Position.X:0.00} Y:{PlaformBaseNode.Position.Y:0.00} Z:{PlaformBaseNode.Position.Z:0.00}");

            // GD.Print("\n");
        }

        // float scale = 0.7f;

        // AnimAzDegs += AnimAzDelta * ((float)delta * scale);
        // AnimElDegs += AnimElDelta * ((float)delta * scale);

        // if (AnimElDegs >  25f) AnimElDelta = -1.05f;
        // if (AnimElDegs <   0f) AnimElDelta =  1.05f;
        // if (AnimAzDegs >  25f) AnimAzDelta = -1.55f;
        // if (AnimAzDegs <   0f) AnimAzDelta =  1.55f;

        // FssEarthCore.RwFocusLLA = new FssLLAPoint() {
        //     LatDegs = AnimElDegs,
        //     LonDegs = AnimAzDegs,
        //     RadiusM = FssEarthCore.EarthRadiusM };

        // // Update the gloabel Focus Position, LLA and Vector3
        // FssEarthCore.UpdatePositions();

        UpdateCoreNodePositions();

        UpdatePlatform(delta);
        UpdatePlatformNodes2();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateCoreNode()
    {
        // Create the two marker objects
        EarthCoreNode = FssPrimitiveFactory.CreateSphereNode("EarthCoreNode", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffBlue"], WithWire);
        AddChild(EarthCoreNode);
        ZeroNode = FssPrimitiveFactory.CreateSphereNode("ZeroNode", new Vector3(0f, 0f, 0f), MarkerSize, FssColorUtil.Colors["OffRed"], WithWire);
        AddChild(ZeroNode);

        // Add axis markers to the core and zero nodes
        //FssPrimitiveFactory.AddAxisMarkers(EarthCoreNode, MarkerSize, MarkerSize/4);
        //FssPrimitiveFactory.AddAxisMarkers(ZeroNode,      MarkerSize, MarkerSize/4);

        EarthCoreNode.AddChild( FssPrimitiveFactory.AxisMarkerNode(MarkerSize, MarkerSize/4) );
        ZeroNode.AddChild(      FssPrimitiveFactory.AxisMarkerNode(MarkerSize, MarkerSize/4) );

        // Add the wedges
        //EarthCoreNode.AddChild(new TestEarthCore((float)FssZeroOffset.GeEarthRadius));

        //WorldCamNode = new FssCameraMoverWorld();
        //EarthCoreNode.AddChild(WorldCamNode);

        // Add the LL Labels
        //EarthCoreNode.AddChild(new TestLabelMaker());

        MapManager = new FssMapManager();
        EarthCoreNode.AddChild(MapManager);

        // Add the platform root node, onto the zero node.
        EntityRootNode = new Node3D() { Name = "EntityRootNode" };  // This is the root node for all entities
        ZeroNode.AddChild(EntityRootNode);

        FssZeroOffset.ZeroNode = ZeroNode;
    }

    // --------------------------------------------------------------------------------------------

    private void CreatePlatform()
    {
        PlatformPos = new FssLLAPoint() {
            LatDegs = 10,
            LonDegs = 10,
            AltMslM = 1000,};

        PlatformCourse = new FssCourse() {
            HeadingDegs = 95f,
            SpeedMps    = 20000.15f };

        PlatformCourseDelta = new FssCourseDelta() {
            HeadingChangeClockwiseDegsSec = -2f,
            SpeedChangeMpMps              = 0f };
    }

    // --------------------------------------------------------------------------------------------

    private void CreatePlatformNodes()
    {
        // Create the core for the platform, to place and orient
        PlaformBaseNode = new Node3D() { Name = "PlaformBaseNode" };
        EntityRootNode.AddChild(PlaformBaseNode);

        FssPrimitiveFactory.AddAxisMarkers(PlaformBaseNode, 0.2f, 0.05f);

        //string ModelPath = "res://Resources/Models/Plane/Plane_Paper/PaperPlanes_v002.glb";
        string ModelPath = "res://Resources/DLCPrep/CivillianVehicles/Plane/PlanePaper/PaperPlanes_v002.glb";

        PackedScene importedModel = (PackedScene)ResourceLoader.Load(ModelPath);
        if (importedModel != null)
        {
            // Instance the model
            Node modelInstance     = importedModel.Instantiate();
            ModelResourceNode      = modelInstance as Node3D;
            ModelResourceNode.Name = "ModelResourceNode";

            PlaformBaseNode.AddChild(ModelResourceNode);
            ModelResourceNode.Scale    = new Vector3(0.25f, 0.25f, 0.25f); // Set the model scale
            ModelResourceNode.Position = new Vector3(0f, 0f, 0f); // Set the model position
            ModelResourceNode.LookAt(Vector3.Forward, Vector3.Up);
        }

        FssAzElBox wedgeBox = FssAzElBox.BoxFromDimensions(30, 10); // Az, El
        FssElementForwardWedge wedge = new FssElementForwardWedge() {
            AzElBox    = wedgeBox,
            DistanceM  = 1.0f };
        PlaformBaseNode.AddChild(wedge);


        ElementContrail = new FssElementContrail();
        ElementContrail.InitElement("TestPlatform");
        ZeroNode.AddChild(ElementContrail);

        ElementRoute = new FssElementRoute();
        ElementRoute.InitElement("TestPlatform");
        ZeroNode.AddChild(ElementRoute);

        List<FssLLAPoint> route = new List<FssLLAPoint>();
        route.Add(new FssLLAPoint() { LatDegs = 40f, LonDegs = 10f, AltMslM = 11300f });
        route.Add(new FssLLAPoint() { LatDegs = 42f, LonDegs = 12f, AltMslM = 11100f });
        route.Add(new FssLLAPoint() { LatDegs = 41f, LonDegs = 13f, AltMslM = 12100f });
        route.Add(new FssLLAPoint() { LatDegs = 44f, LonDegs = 16f, AltMslM = 11000f });
        ElementRoute.SetRoutePoints(route);

        EntityManager = new FssGodotEntityManager();
        ZeroNode.AddChild(EntityManager);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Update
    // --------------------------------------------------------------------------------------------

    private void UpdateCoreNodePositions()
    {
        FssLLAPoint p = FssZeroOffset.RwZeroPointLLA;
        //p.LonDegs += 0.001f;


        //p.LatDegs = FssMapManager.LoadRefLLA.LatDegs;
        //p.LonDegs = FssMapManager.LoadRefLLA.LonDegs;

        FssZeroOffset.SetLLA(p); // The Set() updates the XYZ as well.


        ZeroNode.Position      = FssZeroOffset.GeZeroPoint();
        EarthCoreNode.Position = FssZeroOffset.GeCorePoint();

        // Ensure the core and focus point rotations are zero, we'll calculate those separately.
        EarthCoreNode.Rotation = new Vector3(0f, 0f, 0f);
        ZeroNode.Rotation      = new Vector3(0f, 0f, 0f);
    }

    // --------------------------------------------------------------------------------------------

    private void UpdatePlatform(double delta)
    {
        // Update real world position
        PlatformCourse = PlatformCourse.PlusDeltaForTime(PlatformCourseDelta, delta);
        PlatformPos    = PlatformPos.PlusDeltaForTime(PlatformCourse, delta);

        if (TimerContrail < FssCoreTime.RuntimeSecs)
        {
            TimerContrail = FssCoreTime.RuntimeSecs + 0.5f;
            ElementContrail.AddTrailPoint(PlatformPos);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void UpdatePlatformNodes()
    {
        PlaformBaseNode.Position = FssZeroOffset.GeZeroPointOffset(PlatformPos.ToXYZ());
    }

    private void UpdatePlatformNodes2()
    {
        //FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(PlatformPos, PlatformCourse);
        FssEntityV3 platformV3 = FssGeoConvOperations.RwToGeStruct(PlatformPos, PlatformCourse.HeadingDegs);

        // FssGeoConvOperations.DebugV3(
        //     platformV3.Pos,
        //     platformV3.PosAhead,
        //     platformV3.PosAbove);

        PlaformBaseNode.LookAtFromPosition(
            platformV3.Pos,
            platformV3.PosAhead,
            platformV3.VecUp,
            true);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Debug
    // --------------------------------------------------------------------------------------------


}
