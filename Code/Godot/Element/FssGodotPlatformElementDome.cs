

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementDome : FssGodotPlatformElement
{
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateDome();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateDome()
    {
        var matTransRed  = FssMaterialFactory.TransparentColoredMaterial(new Color(0.7f, 0.2f, 0.2f, 0.4f));
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));
        var matWire      = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);

        FssMeshBuilder meshBuilder  = new ();

        int numSegments = 32;

        if (true)
        {
            float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddHemisphere(Vector3.Zero, rxDist, numSegments);

            ArrayMesh meshData = meshBuilder.Build2("Dome", false);

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




    }
}