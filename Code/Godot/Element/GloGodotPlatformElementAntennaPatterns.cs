using System.Text;
using System.Collections.Generic;

using Godot;

#nullable enable

// Create a horizontally oriented dome with a given AzEl range.

public partial class GloGodotPlatformElementAntennaPatterns : GloGodotPlatformElement
{
    // Polar offset of the port
    // public GloAzElRange  PortPolarOffset = new GloAzElRange();
    // public GloFloat2DArray AntennaPattern  = new GloFloat2DArray();

    private float GeOffsetDistance   = 100f * (float)GloZeroOffset.RwToGeDistanceMultiplier;
    private float GePatternMagnitude =  80f * (float)GloZeroOffset.RwToGeDistanceMultiplier;

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
    // MARK: Query Added Patterns
    // --------------------------------------------------------------------------------------------

    // Get the list of child node names

    public List<string> PatternsList()
    {
        List<string> names = new List<string>();

        foreach (Node3D child in GetChildren())
        {
            names.Add(child.Name);
        }
        return names;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Set Antenna Pattern
    // --------------------------------------------------------------------------------------------

    public void SetSizeAndDistance(float sizeM, float distanceM)
    {
        GePatternMagnitude = sizeM; //     * (float)GloZeroOffset.RwToGeDistanceMultiplier;
        GeOffsetDistance   = distanceM; // * (float)GloZeroOffset.RwToGeDistanceMultiplier;
    }

    public void AddPattern(GloAntennaPattern pattern)
    {
        GloCentralLog.AddEntry($"======> GloGodotPlatformElementAntennaPatterns: AddPattern:{pattern}");

        //AntennaPattern = pattern;

        // Add a named child node
        //Node3D childNode = new Node3D() { Name = pattern.Name };

        // Create the mesh for the pattern
        Node3D? patternNode = CreateSinglePatternMesh(pattern.PortName, pattern.SphereMagPattern, pattern.PatternOffset);

        AddChild(patternNode);

        // Get the orientation out of the pattern.
        double apAzDegs = pattern.PatternOffset.AzDegs;
        double apElDegs = pattern.PatternOffset.ElDegs;

        // The pattern needs to me massaged a bit to orient the element ocrrectly.
        float xRotationRads = (float)GloValueUtils.DegsToRads( apElDegs + 90f );
        float yRotationRads = (float)GloValueUtils.DegsToRads( apAzDegs * -1f );
        float zRotationRads = (float)GloValueUtils.DegsToRads( 0f );

        // float rotateUpToHorizontalDegs = (float)GloValueUtils.DegsToRads(90);
        // float zrot = (float)GloValueUtils.DegsToRads(pattern.PatternOffset.AzDegs);

        Vector3 rotationElements = new Vector3(xRotationRads, yRotationRads, zRotationRads);

        // rotateUpDegs *= 2;
        // //rotateUpDegs = GloValueUtils.LimitToRange(rotateUpDegs, -50, 50);
        // camPitch += (float)rotateUpDegs;
        // camPitch  = GloValueUtils.LimitToRange(camPitch, -80, 0);

        // // Rotate the pattern, first to the "ahead" for the platform node, then to the offset
        // patternNode.RotateX((float)GloValueUtils.DegsToRads(90));
        // //patternNode.RotateZ((float)GloValueUtils.DegsToRads(pattern.PatternOffset.AzDegs));
        // patternNode.RotateZ((float)GloValueUtils.DegsToRads(zrot));

        patternNode.Rotation = rotationElements;
    }

    public Node3D? CreateSinglePatternMesh(string name, GloFloat2DArray data, GloAzElRange offset)
    {
        //GloColorRange colorRange = GloColorRange.RedYellowGreen();
        GloColorRange colorRange = GloColorRange.BlueGreenYellowOrangeRed();

        // GloFloat2DArray radiusList = new GloFloat2DArray(32, 16);
        // radiusList = GloFloat2DArray.AntennaPattern_001(36, 18);
        // GloFloat2DArray radiusList2 = radiusList.ScaleToRange(0, 1f);
        // GloFloat2DArrayIO.SaveToCSVFile(radiusList2, $"c:/util/ap{name}.csv", 2);

        GD.Print($"CreateSinglePatternMesh: Offset:{GeOffsetDistance} // Scale:{GePatternMagnitude}");

        GloFloat2DArray scaledRadiusList = data.ScaleToRange(0, GePatternMagnitude);

        GloMeshBuilder meshBuilder = new();
        meshBuilder.AddMalleableSphere(new Vector3(0, 0, 0), scaledRadiusList, colorRange);

        var matWire        = GloMaterialFactory.WireframeMaterial(GloColorUtil.Colors["Black"]);
        var matVertexColor = GloMaterialFactory.VertexColorMaterial();

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

        meshInstance.Rotation  = new Vector3(0, 0, (float)GloValueUtils.DegsToRads(-90f));
        meshInstanceW.Rotation = new Vector3(0, 0, (float)GloValueUtils.DegsToRads(-90f));

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
