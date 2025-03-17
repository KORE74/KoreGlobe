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
    public FssZeroNodeMapTile?      ParentTile = null;
    public List<FssZeroNodeMapTile> ChildTiles = new List<FssZeroNodeMapTile>();

    // Real world elevation data, defining the tile's geometry
    public FssFloat2DArray       RwEleData  = new FssFloat2DArray();

    // UV Box - Child accessible values
    public FssFloatRange         UVx   = FssFloatRange.ZeroToOne;
    public FssFloatRange         UVy   = FssFloatRange.ZeroToOne;
    public FssUVBoxDropEdgeTile  UVBox = FssUVBoxDropEdgeTile.FullImage();

    // Working values
    private FssMapTileFilepaths  Filepaths;
    private readonly FssLLAPoint RwLLACenter = FssLLAPoint.Zero; // Shortcut from the tilecode center
    private readonly FssXYZPoint RwXYZCenter = FssXYZPoint.Zero; // Shortcut from the tilecode center

    // Materials and Meshes
    private ArrayMesh            TileMeshData;
    private Color                WireColor;
    private Material?            SurfaceMat;

    private FssLineMesh3D        LineMesh3D;
    private MeshInstance3D       TileMeshInstance;

    // Construction Flags
    private bool ConstructionComplete = false;
    private bool ImageSourced         = false; // Initial background task to source the image
    private bool ElevationSourced     = false; // Initial background task to source the elevation
    private bool MeshCreated          = false;
    private bool MeshInstatiated      = false;
    private bool ImageDone            = false;
    private bool BackgroundCreateCompleted = false;

    private bool CreationRejected = false;
    private bool ChildCreationRejected = false;

    // Flag set when the tile (or its children) should be visible. Gates the main visibility processing.
    public bool  ActiveState             = false;
    //public float LatestPixelsPerTriangle = 1000f;
    public bool  ChildrenSetVisible       = false; // flag to more check intended child visibility state

    // Update timers
    private float CreateTaskUpdateTimerSecs = 1.0f;
    private float CreateTaskUpdateTimer     = 0.0f;

    //                                                               Lvl0, Lvl1, Lvl2, Lvl3, Lvl4, Lvl5
    public static readonly double[] childTileDisplayDistKmForLvl = { 2500, 800,  100,  20,   5,    1};

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    public FssZeroNodeMapTile(FssMapTileCode tileCode)
    {
        // Set the core Tilecode and node name.
        TileCode    = tileCode;
        Name        = tileCode.ToString();

        // Populate the locatino shortcuts.
        RwLLACenter = new FssLLAPoint(tileCode.LLBox.CenterPoint);
        RwXYZCenter = RwLLACenter.ToXYZ();

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
        if ((BackgroundCreateCompleted) && (!ConstructionComplete))
        {
            // Create the mesh instance
            ProgressCreation();
        }

        // Update tile position in cycle when the ZeroNode location is updated
        // if (FssZeroNode.ZeroNodeUpdateCycle && ConstructionComplete)
        if (ConstructionComplete)
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

    public bool ChildTilesExist()
    {
        return ChildTiles.Count > 0;
    }

    public void CreateChildTiles()
    {
        if (!TileCode.IsValid())
        {
            FssCentralLog.AddEntry("Invalid tile code for child tile creation.");
            return;
        }

        List<FssMapTileCode> childCodes = TileCode.ChildCodesList();

        foreach (FssMapTileCode currcode in childCodes)
            ChildTiles.Add(new FssZeroNodeMapTile(currcode));

        foreach (var currChildTile in ChildTiles)
        {
            AddChild(currChildTile);
            currChildTile.ParentTile = this;
        }
    }

    public bool ChildTilesContructed()
    {
        if (ChildCreationRejected)
            return false;

        if (!ChildTilesExist())
            return false;

        foreach (FssZeroNodeMapTile childTile in ChildTiles)
        {
            if (childTile.CreationRejected)
            {
                ChildCreationRejected = true;
                return false;
            }
            if (!childTile.ConstructionComplete)
                return false;
        }
        return true;
    }

    // --------------------------------------------------------------------------------------------

    // Locate the tile in the game engine.

    private void LocateTile()
    {
        // Set the local position from the parent object
        Vector3 newPos = FssZeroOffsetOperations.RwToOffsetGe(RwLLACenter);

        // // Set the position from the global transform
        // GlobalTransform = new Transform(Basis.Identity, newPos);

        var transform    = GlobalTransform;
        transform.Origin = newPos;
        GlobalTransform  = transform;

    }

}


