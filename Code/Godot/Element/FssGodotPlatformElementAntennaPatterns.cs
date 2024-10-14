using System.Text;

using Godot;

#nullable enable

// Create a horizontally oriented dome with a given AzEl range.

public partial class FssGodotPlatformElementAntennaPatterns : FssGodotPlatformElement
{
    // Polar offset of the port
    // public FssPolarOffset  PortPolarOffset = new FssPolarOffset();
    // public FssFloat2DArray AntennaPattern  = new FssFloat2DArray();

    private float GeOffsetDistance   = 100f * (float)FssZeroOffset.RwToGeDistanceMultiplierM;
    private float GePatternMagnitude =  20f * (float)FssZeroOffset.RwToGeDistanceMultiplierM;

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

    public void SetSizeAndDistance(float sizeM, float distanceM)
    {
        GePatternMagnitude = sizeM; //     * (float)FssZeroOffset.RwToGeDistanceMultiplierM;
        GeOffsetDistance   = distanceM; // * (float)FssZeroOffset.RwToGeDistanceMultiplierM;
    }

    public void AddPattern(FssAntennaPattern pattern)
    {
        FssCentralLog.AddEntry($"======> FssGodotPlatformElementAntennaPatterns: AddPattern:{pattern}");

        //AntennaPattern = pattern;

        // Add a named child node
        //Node3D childNode = new Node3D() { Name = pattern.Name };

        // Create the mesh for the pattern
        Node3D? patternNode = CreateSinglePatternMesh(pattern.PortName, pattern.SphereMagPattern, pattern.PatternOffset);

        AddChild(patternNode);

        float rotateUpToHorizontalDegs = (float)FssValueUtils.DegsToRads(90);
        float zrot = (float)FssValueUtils.DegsToRads(pattern.PatternOffset.AzDegs);

        Vector3 rotationElements = new Vector3(rotateUpToHorizontalDegs, 0, zrot);

        // rotateUpDegs *= 2;
        // //rotateUpDegs = FssValueUtils.LimitToRange(rotateUpDegs, -50, 50);
        // camPitch += (float)rotateUpDegs;
        // camPitch  = FssValueUtils.LimitToRange(camPitch, -80, 0);

        // // Rotate the pattern, first to the "ahead" for the platform node, then to the offset
        // patternNode.RotateX((float)FssValueUtils.DegsToRads(90));
        // //patternNode.RotateZ((float)FssValueUtils.DegsToRads(pattern.PatternOffset.AzDegs));
        // patternNode.RotateZ((float)FssValueUtils.DegsToRads(zrot));

        patternNode.Rotation = rotationElements;
    }

    public Node3D? CreateSinglePatternMesh(string name, FssFloat2DArray data, FssPolarOffset offset)
    {
        //FssColorRange colorRange = FssColorRange.RedYellowGreen();
        FssColorRange colorRange = FssColorRange.BlueGreenYellowOrangeRed();

        // FssFloat2DArray radiusList = new FssFloat2DArray(32, 16);
        // radiusList = FssFloat2DArray.AntennaPattern_001(36, 18);
        // FssFloat2DArray radiusList2 = radiusList.ScaleToRange(0, 1f);
        // FssFloat2DArrayIO.SaveToCSVFile(radiusList2, $"c:/util/ap{name}.csv", 2);

        GD.Print($"CreateSinglePatternMesh: Offset:{GeOffsetDistance} // Scale:{GePatternMagnitude}");

        FssFloat2DArray scaledRadiusList = data.ScaleToRange(0, GePatternMagnitude);

        FssMeshBuilder meshBuilder = new();
        meshBuilder.AddMalleableSphere(new Vector3(0, 0, 0), scaledRadiusList, colorRange);

        var matWire        = FssMaterialFactory.WireframeMaterial(FssColorUtil.Colors["Black"]);
        var matVertexColor = FssMaterialFactory.VertexColorMaterial();

        ArrayMesh meshData = meshBuilder.Build("AP", false);

        // Colored - Add the mesh to the current Node3D
        MeshInstance3D meshInstance    = new() { Name = "AP-Mesh" };
        meshInstance.Mesh              = meshData;
        meshInstance.MaterialOverride  = matVertexColor;

        // Wirefrme - Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new() { Name = "AP-Wireframe" };
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        Node3D patternNode = new Node3D() { Name = name };
        patternNode.AddChild(meshInstance);
        patternNode.AddChild(meshInstanceW);

        // Offset the mesh against the pattern in the -y axis. This pans-out when it is rotated to the offset.
        meshInstance.Translate(new Vector3(0, -GeOffsetDistance, 0));
        meshInstanceW.Translate(new Vector3(0, -GeOffsetDistance, 0));

        return patternNode;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    public void SetVisibility(bool visible)
    {
        Visible = visible;
    }


}
