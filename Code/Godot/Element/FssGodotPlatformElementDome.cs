

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementDome : FssGodotPlatformElement
{
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.0f;

    FssLineMesh3D LineMesh = new FssLineMesh3D();

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
        Color domeColor = new Color(0.8f, 0.2f, 0.2f, 0.4f);
        Color domeWireColor = new Color(domeColor);
        domeWireColor.A = 1.0f;

        var matDome = FssMaterialFactory.TransparentColoredMaterial(domeColor);
        var matWire = FssMaterialFactory.WireframeMaterial(domeWireColor);

        FssMeshBuilder meshBuilder  = new ();

        int numSegments = 24;

        if (true)
        {
            float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddHemisphere(Vector3.Zero, rxDist, numSegments);

            ArrayMesh meshData = meshBuilder.Build2("Dome", false);

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstance   = new();
            rxMeshInstance.Mesh             = meshData;
            rxMeshInstance.MaterialOverride = matDome;

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstanceW   = new();
            rxMeshInstanceW.Mesh             = meshData;
            rxMeshInstanceW.MaterialOverride = matWire;

            AddChild(rxMeshInstance);
            //AddChild(rxMeshInstanceW);

            LineMesh.AddHemisphere(Vector3.Zero, rxDist, numSegments);
            AddChild(LineMesh);
        }



    }
}