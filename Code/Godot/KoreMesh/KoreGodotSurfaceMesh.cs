// KoreGodotSurfaceMesh : Class to take a KoreCommon/KoreMeshData and create a Godot SurfaceMesh from it.
// - Will use the vertices/triangles list and the Godot SurfaceTool.

using KoreCommon;

using Godot;

public partial class KoreGodotSurfaceMesh : Node3D
{
    private MeshInstance3D _meshInstance;
    private SurfaceTool _surfaceTool;
    private bool _meshNeedsUpdate = false;

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Create a MeshInstance3D to hold the generated line mesh
        _meshInstance = new MeshInstance3D();
        AddChild(_meshInstance);

        // Initialize the SurfaceTool
        _surfaceTool = new SurfaceTool();

        // Apply the shared Vertex Color Material to the MeshInstance3D
        _meshInstance.MaterialOverride = GetSharedVertexColorMaterial();

        // Debug Call: Create a cube with colored edges
        //CreateCube();
    }

    public override void _Process(double delta)
    {
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Mesh
    // --------------------------------------------------------------------------------------------

    private void UpdateMesh(KoreMeshData newMeshData)
    {
        _surfaceTool.Clear();
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        // Traverse the list using Id, so we have the index to look stuff up.
        int numLines = newMeshData.Lines.Count;
        for (int i = 0; i < numLines; i++)
        {
            // Get the line details
            var line = newMeshData.Lines[i];
            int pointAId = line.A;
            int pointBId = line.B;
            KoreXYZVector pointA = newMeshData.Vertices[pointAId];
            KoreXYZVector pointB = newMeshData.Vertices[pointBId];
            KoreMeshLineColour lineColour = newMeshData.LineColors[i];

            // Convert the line details to Godot terms
            Vector3 godotPosA = KoreConvPos.ToGodotVector3(pointA);
            Vector3 godotPosB = KoreConvPos.ToGodotVector3(pointB);
            Color colStart = KoreConvColor.ToGodotColor(lineColour.StartColor);
            Color colEnd = KoreConvColor.ToGodotColor(lineColour.EndColor);

            // Add the vertices to the SurfaceTool
            _surfaceTool.SetColor(colStart);
            _surfaceTool.AddVertex(godotPosA);
            _surfaceTool.SetColor(colEnd);
            _surfaceTool.AddVertex(godotPosB);
        }

        // Commit the mesh and assign it to the MeshInstance3D
        Mesh mesh = _surfaceTool.Commit();
        _meshInstance.Mesh = mesh;

        _meshNeedsUpdate = false;
    }

}