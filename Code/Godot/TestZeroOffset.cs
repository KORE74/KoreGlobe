using Godot;
using System;
using System.IO;
using System.Collections.Generic;

public partial class TestZeroOffset : Node3D
{
    Node3D PlaformBaseNode;
    Node3D ModelResourceNode;

    // FssMapManager      MapManager;
    FssElementContrail            ElementContrail;
    FssGodotPlatformElementRoute    ElementRoute;
    FssGodotEntityManager        EntityManager;


    // TestZeroOffset.WorldCamNode.CamNode

    public static FssCameraMoverWorld WorldCamNode;

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

    private float Timer5Sec = 0.0f;

    private float TimerContrail = 0.0f;

    // private FssCyclicIdGenerator IdGen = new FssCyclicIdGenerator(250);
    // private string randomString = FssRandomStringGenerator.GenerateRandomString(5);

    // --------------------------------------------------------------------------------------------
    // MARK: Node _Ready and _Process
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Setup the factory that holds a centralised reference to key objects
        FssGodotFactory.Instance.CreateObjects(this);

        // Randomize the zero point in each run so we don't bake-in assumptions.
        double randomLat = FssValueUtils.RandomInRange(45, 60);
        double randomLon = FssValueUtils.RandomInRange(-8, -0);

        // Init the real world zero pos
        FssLLAPoint randomPos = new FssLLAPoint() {
            LatDegs = randomLat,
            LonDegs = randomLon,
            RadiusM = FssPosConsts.EarthRadiusM };
        FssZeroOffset.SetLLA(randomPos);


        WorldCamNode = new FssCameraMoverWorld() { Name = "WorldCamBase" };
        AddChild(WorldCamNode);
        WorldCamNode.CamNode.Current = true;

        FssZeroOffset.ReportConsts();
        // Create Nodes

        // CreateCoreNode();

        FssGodotFactory.Instance.ZeroNode.Position      = FssZeroOffset.GeZeroPoint();
        FssGodotFactory.Instance.EarthCoreNode.Position = FssZeroOffset.GeCorePoint();

        //Fss3DModelLibrary.TestLoadModel(ZeroNode);

        // Read the manually included sets of assets
        string jsonMilitary     = FssGodotFileOperations.LoadFromFile("res://Resources/Assets/MilitaryVehicles/Inventory.json");
        string jsonCivilian     = FssGodotFileOperations.LoadFromFile("res://Resources/Assets/CivilianVehicles/Inventory.json");
        string jsonPlaceholders = FssGodotFileOperations.LoadFromFile("res://Resources/Assets/PlaceholderModels/Inventory.json");
        FssGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonMilitary);
        FssGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonCivilian);
        FssGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(jsonPlaceholders);

        // bool DLCTesting = true;

        // if (DLCTesting)
        // {
        //     //FssDlcOperations.CreateDlc();

        //     List<string> dlcList = FssDlcOperations.ListLoadableDlcPaths();

        //     foreach (string dlc in dlcList)
        //         FssDlcOperations.LoadDlc(dlc);

        //     FssCentralLog.AddEntry( FssDlcOperations.DlcReport() );

        //     // Find the JSON files in each DLC and report on them
        //     List<string> dlcTitlesList = FssDlcOperations.ListLoadedDlcTitles();
        //     foreach (string currDlcTitle in dlcTitlesList)
        //     {
        //         string invJson = FssDlcOperations.InventoryJsonForDLCTitle(currDlcTitle);
        //         FssCentralLog.AddEntry($"DLC: {currDlcTitle} JSON: {invJson}");
        //     }

        //     string invJson2 = FssDlcOperations.InventoryJsonForDLCTitle("PlaceholderModels");
        //     FssGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(invJson2);

        //     string invJson3 = FssDlcOperations.InventoryJsonForDLCTitle("MilitaryVehicles");
        //     FssGodotFactory.Instance.ModelLibrary.LoadJSONConfigFile(invJson3);

        //     FssCentralLog.AddEntry( FssGodotFactory.Instance.ModelLibrary.ReportContent() );
        // }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // trigger every 5 seconds
        if (FssCoreTime.RuntimeIntSecs > Timer5Sec)
        {
            Timer5Sec = FssCoreTime.RuntimeIntSecs + 5;

            // Randomize the zero point in each run so we don't bake-in assumptions.
            double randomLat = FssValueUtils.RandomInRange(45, 60);
            double randomLon = FssValueUtils.RandomInRange(-8, -0);

            // Init the real world zero pos
            FssLLAPoint randomPos = new FssLLAPoint() {
                LatDegs = randomLat,
                LonDegs = randomLon,
                RadiusM = FssPosConsts.EarthRadiusM };
            FssZeroOffset.SetLLA(randomPos);


            FssGodotFactory.Instance.ZeroNode.Position      = FssZeroOffset.GeZeroPoint();
            FssGodotFactory.Instance.EarthCoreNode.Position = FssZeroOffset.GeCorePoint();

        }
    }
}
