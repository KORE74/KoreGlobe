

using Godot;

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementAntennaPatterns : FssGodotPlatformElement
{
    // Polar offset of the port
    // public FssPolarOffset  PortPolarOffset = new FssPolarOffset();
    // public FssFloat2DArray AntennaPattern  = new FssFloat2DArray();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //CreateAP();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create
    // --------------------------------------------------------------------------------------------

    // private void CreateAP()
    // {
    //     // TBD Set Rotation


    // }


    // --------------------------------------------------------------------------------------------
    // MARK: Set Antenna Pattern
    // --------------------------------------------------------------------------------------------

    public void AddPattern(FssAntennaPattern pattern)
    {
        FssCentralLog.AddEntry($"======> FssGodotPlatformElementAntennaPatterns: AddPattern: {pattern.PortName}");

        //AntennaPattern = pattern;

        // Add a named child node
        //Node3D childNode = new Node3D() { Name = pattern.Name };

        // Create the mesh for the pattern
        Node3D? patternNode = CreateSinglePatternMesh(pattern.PortName, pattern.SphereMagPattern, pattern.PatternOffset);

        AddChild(patternNode);

    }


    public Node3D? CreateSinglePatternMesh(string name, FssFloat2DArray data, FssPolarOffset offset)
    {
        // Setup materials and colors
        var matWire        = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["White"]);
        var matVertexColor = FssMaterialFactory.VertexColorMaterial();
        FssColorRange colorRange = FssColorRange.RedYellowGreen();

        FssMeshBuilder meshBuilder  = new ();

        data = new FssFloat2DArray(36, 18);
        data.SetRandomVals(1, 2);

        // Create the geometry for the AP
        meshBuilder.AddMalleableSphere(Vector3.Zero, data, colorRange);
        ArrayMesh meshData = meshBuilder.Build2("AP", false);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new() { Name = "AP-Mesh" };
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = matVertexColor;

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new() { Name = "Wireframe" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        // Attach the meshes to the current Node3D
        Node3D patternNode = new Node3D() { Name = name };
        patternNode.AddChild(meshInstance);
        patternNode.AddChild(meshInstanceW);

        patternNode.AddChild( new Node3D() { Name = "AP-Target" } );


        // Place the AP at the offset position and pointed away from the AC
        Vector3 offsetPos = FssGodotGeometryOperations.ToVector3( offset.ToXYZ() );
        meshInstance.Position  = offsetPos;
        meshInstanceW.Position = offsetPos;

        return patternNode;
    }

}
