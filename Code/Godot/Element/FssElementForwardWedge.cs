

using Godot;

// Create a forward facing (unless rotated otherwise) wedge with a given AzEl range.

public partial class FssElementForwardWedge : Node3D
{
    public FssAzElBox AzElBox = new FssAzElBox();
    public float DistanceM = 1.0f;

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
        var matTransBlue = FssMaterialFactory.TransparentColoredMaterial(new Color(0.2f, 0.2f, 0.7f, 0.4f));
        var matWire      = FssMaterialFactory.WireframeWhiteMaterial();

        FssMeshBuilder meshBuilder  = new ();

        meshBuilder.AddPyramidByAzElDist(AzElBox, DistanceM);

        ArrayMesh meshData = meshBuilder.Build2("Wedge", false);

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstance   = new();
        meshInstance.Mesh             = meshData;
        meshInstance.MaterialOverride = matTransBlue;

        // Add the mesh to the current Node3D
        MeshInstance3D meshInstanceW   = new();
        meshInstanceW.Mesh             = meshData;
        meshInstanceW.MaterialOverride = matWire;

        AddChild(meshInstance);
        AddChild(meshInstanceW);

    }
}