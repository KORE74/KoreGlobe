using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class FssZeroNodeMapTile : Node3D
{
    // Tile code
    public FssMapTileCode        TileCode   = FssMapTileCode.Zero;

    // Parent/child tiles relationships
    public FssZeroNodeMapTile?   ParentTile = null;
    public List<FssZeroNodeMapTile> ChildTiles = new List<FssZeroNodeMapTile>();

    // Real world elevation data, defining the tile's geometry
    public FssFloat2DArray       RwEleData  = new FssFloat2DArray();

    // Child accessible values
    public FssFloatRange         UVx   = FssFloatRange.ZeroToOne;
    public FssFloatRange         UVy   = FssFloatRange.ZeroToOne;
    public FssUVBox              UVBox = FssUVBox.Zero;

    // Working values
    private FssMapTileFilepaths  Filepaths;
    private readonly FssLLAPoint RwLLACenter = FssLLAPoint.Zero; // Shortcut from the tilecode center

    // One way or another, a tile will end up with an image filename, either from the Filename structure, a
    // parent tile of a default choice.
    public string                RegisteredTextureName = "";

    // Materials and Meshes
    private ArrayMesh            TileMeshData;
    private Color                WireColor;
    private StandardMaterial3D   SurfaceMat;
    private FssLineMesh3D        LineMesh3D;

    // Construction Flags
    private bool ConstructionComplete = false;
    private bool ImageSourced         = false; // Initial background task to source the image
    private bool ElevationSourced     = false; // Initial background task to source the elevation
    private bool MeshCreated          = false;
    private bool MeshInstatiated      = false;
    private bool ImageDone            = false;

    // Flag set when the tile (or its children) should be visible. Gates the main visibility processing.
    public bool  ActiveVisibility        = false;
    public float LatestPixelsPerTriangle = 1000f

    // Update timers
    private float CreateTaskUpdateTimerSecs = 1.0f;
    private float CreateTaskUpdateTimer     = 0.0f;

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public FssZeroNodeMapTile(FssMapTileCode tileCode)
    {
        // Set the core Tilecode and node name.
        TileCode    = tileCode;
        Name        = tileCode.ToString();
        RwLLACenter = tileCode.LLACenter;

        // Fire off the fully background task of creating/loading the tile elements asap.
        Task.Run(() => BackgroundTileCreation(tileCode));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node3D Functions
    // --------------------------------------------------------------------------------------------

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Update tile position in cycle when the ZeroNode location is updated
        if (FssZeroNode.ZeroNodeUpdateCycle && ConstructionComplete)
            LocateTile();

        if (CreateTaskUpdateTimer < FssCentralTime.RuntimeSecs)
        {
            CreateTaskUpdateTimer = FssCentralTime.RuntimeSecs + CreateTaskUpdateTimerSecs;

            if (!ConstructionComplete)
                ProgressCreation();
            else
                UpdateVisibility();
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Child Tiles
    // --------------------------------------------------------------------------------------------

    public void CreateChildTiles()
    {
        if (!TileCode.IsValid())
        {
            FssCentralLog.AddEntry("Invalid tile code for child tile creation.");
            return;
        }

        List<FssMapTileCode> childCodes = TileCode.GetChildTileCodes();

        foreach (FssMapTileCode currcode in childCodes)
            ChildTiles.Add(new FssZeroNodeMapTile(currcode));
    }

    public bool ChildTilesExist()
    {
        return ChildTiles.Count > 0;
    }

    public bool ChildTilesContructed()
    {
        if (!ChildTilesExist())
            return false;

        foreach (FssZeroNodeMapTile childTile in ChildTiles)
        {
            if (!childTile.ConstructionComplete)
                return false;
        }
    }

    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine.

    private void LocateTile()
    {
        Position = FssGeoConvOperations.RwToOffsetGe(RwLLACenter);
    }



}


