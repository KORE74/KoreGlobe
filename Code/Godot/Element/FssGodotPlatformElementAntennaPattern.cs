

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementAntennaPattern : FssGodotPlatformElement
{
    // Polar offset of the port
    public FssPolarOffset  PortPolarOffset = new FssPolarOffset();
    public FssFloat2DArray AntennaPattern  = new FssFloat2DArray();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CreateAP();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    private void CreateAP()
    {
        var matWire        = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);
        var matVertexColor = FssMaterialFactory.VertexColorMaterial();

        FssMeshBuilder meshBuilder  = new ();

        // Define the colors for the AP
        FssColorRange colorRange = FssColorRange.RedYellowGreen();

        // Create the geometry for the AP
        meshBuilder.AddMalleableSphere(Vector3.Zero, AntennaPattern, colorRange);
        ArrayMesh meshData = meshBuilder.Build2("AP", false);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = matVertexColor;

        // Add the mesh to the current Node3D
        MeshInstance3D MeshInstanceW   = new();
        MeshInstanceW.Mesh             = meshData;
        MeshInstanceW.MaterialOverride = matWire;

        // Attach the meshes to the current Node3D
        AddChild(meshInstance);
        AddChild(MeshInstanceW);

        // Place the AP at the offset position and pointed away from the AC
        Vector3 offsetPos = FssGodotGeometryOperations.ToVector3( PortPolarOffset.ToXYZ() );
        meshInstance.Position  = offsetPos;
        MeshInstanceW.Position = offsetPos;

        // TBD Set Rotation


    }
}