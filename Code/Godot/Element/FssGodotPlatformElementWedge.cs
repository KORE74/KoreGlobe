

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssGodotPlatformElementWedge : FssGodotPlatformElement
{
    public FssAzElBox AzElBox = new FssAzElBox();
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.1f;

    FssLineMesh3D RxLineMesh   = new FssLineMesh3D();
    FssLineMesh3D TxLineMesh   = new FssLineMesh3D();
    FssLineMesh3D FullLineMesh = new FssLineMesh3D();

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
        Color elementColorRx = FssColorUtil.StringToColor(Name + "Rx");
        Color elementColorTx = elementColorRx; // FssColorUtil.StringToColor(Name + "Tx");
        elementColorRx.A = 0.4f;
        elementColorTx.A = 0.4f;

        Color wireColorRx = new Color(elementColorRx);
        Color wireColorTx = new Color(elementColorTx);
        wireColorRx.A = 1.0f;
        wireColorTx.A = 1.0f;

        var matWedgeRx = FssMaterialFactory.TransparentColoredMaterial(elementColorRx);
        var matWedgeTx = FssMaterialFactory.TransparentColoredMaterial(elementColorTx);

        FssMeshBuilder rxMeshBuilder   = new ();
        FssMeshBuilder txMeshBuilder   = new ();

        // ---------------------

        // Limit the display distance we'll use to something practically viewable.
        double truncateDistM = 500000; // 500km
        bool rxTruncated = false;
        bool txTruncated = false;

        double RxDistanceMToUse = FssValueUtils.LimitToRange(RxDistanceM, 1000, truncateDistM);
        double TxDistanceMToUse = FssValueUtils.LimitToRange(TxDistanceM, 1000, truncateDistM);

        if (RxDistanceM > truncateDistM) rxTruncated = true;
        if (TxDistanceM > truncateDistM) txTruncated = true;

        // Convert diatance to GameEngine units
        float rxDist = (float)(RxDistanceMToUse * FssZeroOffset.RwToGeDistanceMultiplierM);
        float txDist = (float)(TxDistanceMToUse * FssZeroOffset.RwToGeDistanceMultiplierM);

        // Create the pyramid shapes in the mesh builder
        rxMeshBuilder.AddPyramidByAzElDist(AzElBox, rxDist);
        txMeshBuilder.AddPyramidByAzElDist(AzElBox, txDist);
        ArrayMesh rxMeshData = rxMeshBuilder.Build2("RxWedge", false);
        ArrayMesh txMeshData = txMeshBuilder.Build2("TxWedge", false);

        // Add the RX mesh to the current Node3D
        RxMeshInstance = new();
        RxMeshInstance.Mesh             = rxMeshData;
        RxMeshInstance.MaterialOverride = matWedgeRx;
        AddChild(RxMeshInstance);

        // Add the TX mesh to the current Node3D
        TxMeshInstance = new();
        TxMeshInstance.Mesh             = txMeshData;
        TxMeshInstance.MaterialOverride = matWedgeTx;
        AddChild(TxMeshInstance);

        // ---------------------

        FssLineMesh3D.PyramidStyle rxDrawStyle = FssLineMesh3D.PyramidStyle.Pyramid;
        FssLineMesh3D.PyramidStyle txDrawStyle = FssLineMesh3D.PyramidStyle.Pyramid;
        if (rxTruncated) rxDrawStyle = FssLineMesh3D.PyramidStyle.CroppedPyramid;
        if (txTruncated) txDrawStyle = FssLineMesh3D.PyramidStyle.CroppedPyramid;

        // Create and add the wireframe shapes in the line mesh builder
        RxLineMesh.AddPyramidByAzElDist(AzElBox, rxDist, wireColorRx, rxDrawStyle);
        TxLineMesh.AddPyramidByAzElDist(AzElBox, txDist, wireColorTx, txDrawStyle);
        AddChild(RxLineMesh);
        AddChild(TxLineMesh);

        FullLineMesh.AddPyramidByAzElDist(AzElBox, rxDist, wireColorRx, rxDrawStyle);
        FullLineMesh.AddPyramidByAzElDist(AzElBox, txDist, wireColorTx, txDrawStyle);
        AddChild(FullLineMesh);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool rxVisible, bool txVisible)
    {
        RxVisible = rxVisible;
        TxVisible = txVisible;

        // Update the mesh (volume) visibility
        RxMeshInstance.Visible = rxVisible;
        TxMeshInstance.Visible = txVisible;

        // Update the wireframe visibility
        RxLineMesh.Visible   = RxVisible && !TxVisible;
        TxLineMesh.Visible   = TxVisible && !RxVisible;
        FullLineMesh.Visible = RxVisible && TxVisible;
    }


}