

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementDome : FssGodotPlatformElement
{
    public float RxDistanceM = 1.0f;
    public float TxDistanceM = 1.0f;

    FssLineMesh3D LineMesh;

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
        Color domeColor = FssColorUtil.StringToColor(Name);
        domeColor.A = 0.4f;
        var matDome = FssMaterialFactory.TransparentColoredMaterial(domeColor);

        Color wireColor = FssColorUtil.StringToColor(Name);
        wireColor.A = 1.0f;

        FssMeshBuilder meshBuilder  = new ();

        int numSegments = 20;

        if (true)
        {
            float rxDist = (float)(RxDistanceM * FssZeroOffset.RwToGeDistanceMultiplierM);

            meshBuilder.AddHemisphere(Vector3.Zero, rxDist, numSegments);

            ArrayMesh meshData = meshBuilder.Build2("Dome", false);

            // Add the mesh to the current Node3D
            MeshInstance3D rxMeshInstance   = new() { Name = $"{Name}Dome" };
            rxMeshInstance.Mesh             = meshData;
            rxMeshInstance.MaterialOverride = matDome;
            AddChild(rxMeshInstance);

            LineMesh = new FssLineMesh3D() { Name = $"{Name}Wire" };
            LineMesh.AddHemisphere(Vector3.Zero, rxDist, numSegments, wireColor);
            AddChild(LineMesh);
        }

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool isVisible)
    {
        foreach (Node3D child in GetChildren())
        {
            child.Visible = isVisible;
        }
    }
}