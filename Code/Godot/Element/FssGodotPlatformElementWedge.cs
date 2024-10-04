

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssGodotPlatformElementWedge : FssGodotPlatformElement
{
    public FssAzElBox AzElBox = new FssAzElBox();
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.1f;

    FssLineMesh3D RxLineMesh = new FssLineMesh3D();
    FssLineMesh3D TxLineMesh = new FssLineMesh3D();

    MeshInstance3D RxMeshInstance;
    MeshInstance3D TxMeshInstance;

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
        Color elementColorTx = FssColorUtil.StringToColor(Name + "Tx");
        elementColorRx.A = 0.4f;
        elementColorTx.A = 0.4f;

        Color wireColorRx = new Color(elementColorRx);
        Color wireColorTx = new Color(elementColorTx);
        wireColorRx.A = 1.0f;
        wireColorTx.A = 1.0f;

        var matWedgeRx = FssMaterialFactory.TransparentColoredMaterial(elementColorRx);
        var matWedgeTx = FssMaterialFactory.TransparentColoredMaterial(elementColorTx);

        FssMeshBuilder rxMeshBuilder  = new ();
        FssMeshBuilder txMeshBuilder  = new ();

        // ---------------------

        FssAzElBox rxAzElBox = AzElBox;
        FssAzElBox txAzElBox = AzElBox;

        float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);
        float txDist = (float)(TxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

        rxMeshBuilder.AddPyramidByAzElDist(rxAzElBox, rxDist);
        txMeshBuilder.AddPyramidByAzElDist(txAzElBox, txDist);

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

        RxLineMesh.AddPyramidByAzElDist(rxAzElBox, rxDist, wireColorRx);
        TxLineMesh.AddPyramidByAzElDist(txAzElBox, txDist, wireColorTx);

        AddChild(RxLineMesh);
        AddChild(TxLineMesh);
    }
}