

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class GloGodotPlatformElementWedge : GloGodotPlatformElement
{
    public GloAzElBox  AzElBox         = new GloAzElBox();
    public GloAttitude ElementAttitude = new GloAttitude();
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.1f;

    GloLineMesh3D RxLineMesh   = new GloLineMesh3D();
    GloLineMesh3D TxLineMesh   = new GloLineMesh3D();
    GloLineMesh3D FullLineMesh = new GloLineMesh3D();

    MeshInstance3D RxMeshInstance;
    MeshInstance3D TxMeshInstance;

    private bool RxVisible = true;
    private bool TxVisible = true;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateWedge();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateWedge()
    {
        Color elementColorRx = GloColorUtil.StringToColor(Name + "Rx");
        Color elementColorTx = elementColorRx; // GloColorUtil.StringToColor(Name + "Tx");
        elementColorRx.A = 0.4f;
        elementColorTx.A = 0.4f;

        Color wireColorRx = new Color(elementColorRx);
        Color wireColorTx = new Color(elementColorTx);
        wireColorRx.A = 1.0f;
        wireColorTx.A = 1.0f;

        var matWedgeRx = GloMaterialFactory.TransparentColoredMaterial(elementColorRx);
        var matWedgeTx = GloMaterialFactory.TransparentColoredMaterial(elementColorTx);

        GloMeshBuilder rxMeshBuilder   = new();
        GloMeshBuilder txMeshBuilder   = new();

        // ---------------------

        // Limit the display distance we'll use to something practically viewable.
        double truncateDistM = 500000; // 500km
        bool rxTruncated = false;
        bool txTruncated = false;

        double RxDistanceMToUse = GloValueUtils.LimitToRange(RxDistanceM, 1000, truncateDistM);
        double TxDistanceMToUse = GloValueUtils.LimitToRange(TxDistanceM, 1000, truncateDistM);

        if (RxDistanceM > truncateDistM) rxTruncated = true;
        if (TxDistanceM > truncateDistM) txTruncated = true;

        // Convert diatance to GameEngine units
        float rxDist = (float)(RxDistanceMToUse * GloZeroOffset.RwToGeDistanceMultiplier);
        float txDist = (float)(TxDistanceMToUse * GloZeroOffset.RwToGeDistanceMultiplier);

        // Create the pyramid shapes in the mesh builder
        rxMeshBuilder.AddPyramidByAzElDist(AzElBox, rxDist);
        txMeshBuilder.AddPyramidByAzElDist(AzElBox, txDist);
        ArrayMesh rxMeshData = rxMeshBuilder.Build2("RxWedge", false);
        ArrayMesh txMeshData = txMeshBuilder.Build2("TxWedge", false);

        // Add the RX mesh to the current Node3D
        RxMeshInstance                  = new();
        RxMeshInstance.Mesh             = rxMeshData;
        RxMeshInstance.MaterialOverride = matWedgeRx;
        AddChild(RxMeshInstance);

        // Add the TX mesh to the current Node3D
        TxMeshInstance                  = new();
        TxMeshInstance.Mesh             = txMeshData;
        TxMeshInstance.MaterialOverride = matWedgeTx;
        AddChild(TxMeshInstance);

        // ---------------------

        GloLineMesh3D.PyramidStyle rxDrawStyle = GloLineMesh3D.PyramidStyle.Pyramid;
        GloLineMesh3D.PyramidStyle txDrawStyle = GloLineMesh3D.PyramidStyle.Pyramid;
        if (rxTruncated) rxDrawStyle = GloLineMesh3D.PyramidStyle.CroppedPyramid;
        if (txTruncated) txDrawStyle = GloLineMesh3D.PyramidStyle.CroppedPyramid;

        // Create and add the wireframe shapes in the line mesh builder
        RxLineMesh.AddPyramidByAzElDist(AzElBox, rxDist, wireColorRx, rxDrawStyle);
        TxLineMesh.AddPyramidByAzElDist(AzElBox, txDist, wireColorTx, txDrawStyle);
        AddChild(RxLineMesh);
        AddChild(TxLineMesh);

        FullLineMesh.AddPyramidByAzElDist(AzElBox, rxDist, wireColorRx, rxDrawStyle);
        FullLineMesh.AddPyramidByAzElDist(AzElBox, txDist, wireColorTx, txDrawStyle);
        AddChild(FullLineMesh);

        // ---------------------

        // if (!ElementAttitude.IsZero())
        {
            Vector3 rotationVec3 = new Vector3(
                (float)ElementAttitude.PitchUpRads,
                (float)ElementAttitude.YawClockwiseRads,
                (float)ElementAttitude.RollClockwiseRads);

            GD.Print($"ElementAttitude: {ElementAttitude}");
            GD.Print($"rotationVec3: {rotationVec3}");


            // rotate the child nodes to match the element's attitude
            RxMeshInstance.Rotation = rotationVec3;
            TxMeshInstance.Rotation = rotationVec3;
            RxLineMesh.Rotation     = rotationVec3;
            TxLineMesh.Rotation     = rotationVec3;
            FullLineMesh.Rotation   = rotationVec3;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool beamsVisible, bool rxVisible, bool txVisible)
    {
        RxVisible = rxVisible & beamsVisible;
        TxVisible = txVisible & beamsVisible;

        // Update the mesh (volume) visibility
        RxMeshInstance.Visible = RxVisible;
        TxMeshInstance.Visible = TxVisible;

        // Update the wireframe visibility
        RxLineMesh.Visible   = RxVisible && !TxVisible;
        TxLineMesh.Visible   = TxVisible && !RxVisible;
        FullLineMesh.Visible = RxVisible && TxVisible;
    }


}