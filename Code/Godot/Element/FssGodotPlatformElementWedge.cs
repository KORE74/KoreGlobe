

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssGodotPlatformElementWedge : FssGodotPlatformElement
{
    public FssAzElBox AzElBox = new FssAzElBox();
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.0f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateWedge();
        //CreateTestWedge();
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
        var matTransRed  = FssMaterialFactory.TransparentColoredMaterial(new Color(0.7f, 0.2f, 0.2f, 0.4f));
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));
        var matWire      = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);

        FssMeshBuilder meshBuilder  = new ();

        // ---------------------
        AzElBox = new FssAzElBox() { MinAzDegs = -20.0, MaxAzDegs = 20.0, MinElDegs = -4.0, MaxElDegs = 4.0 };
        //DistanceM = (float)(100 * 1000 * FssZeroOffset.RwToGeDistanceMultiplierM);

        // RxDistanceM = 110 * 1000;
        // TxDistanceM = 100 * 1000;

        // ---------------------

        {
            float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddPyramidByAzElDist(AzElBox, rxDist);

            ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstance   = new();
            rxMeshInstance.Mesh             = meshData;
            rxMeshInstance.MaterialOverride = matTransRed;

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstanceW   = new();
            rxMeshInstanceW.Mesh             = meshData;
            rxMeshInstanceW.MaterialOverride = matWire;

            AddChild(rxMeshInstance);
            AddChild(rxMeshInstanceW);
        }

        {
            float txDist = (float)(TxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddPyramidByAzElDist(AzElBox, txDist);

            ArrayMesh meshData = meshBuilder.Build2("WedgeTx", false);

            // Add the mesh to the current Node3D
            MeshInstance3D txMeshInstance   = new();
            txMeshInstance.Mesh             = meshData;
            txMeshInstance.MaterialOverride = matTransBlue;

            // Add the mesh to the current Node3D
            MeshInstance3D txMeshInstanceW   = new();
            txMeshInstanceW.Mesh             = meshData;
            txMeshInstanceW.MaterialOverride = matWire;

            AddChild(txMeshInstance);
            AddChild(txMeshInstanceW);
        }


    }
}