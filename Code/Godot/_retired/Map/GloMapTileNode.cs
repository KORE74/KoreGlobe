// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;

// using Godot;

// #nullable enable

// // Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

// public partial class GloMapTileNode : Node3D
// {
//     public StandardMaterial3D?  TileMaterial = null;
//     public ArrayMesh            TileMeshData;
//     public GloUVBoxDropEdgeTile UVBox;

//     // Construction Flags
//     private bool ChildTileDataAvailable = false;
//     private bool ConstructionComplete = false;
//     private bool MeshDone             = false;
//     private bool MeshInstatiated      = false;
//     private bool ImageDone            = false;

//     // Property to check if the material loading is done
//     public bool IsDone => ConstructionComplete;

//     // Map Tile Readable values
//     public  GloMapTileCode       TileCode;
//     private GloLLAPoint          RwTileCenterLLA;
//     private GloXYZPoint          RwTileCenterXYZ;
//     public  GloMapTileFilepaths  Filepaths;
//     public  GloFloat2DArray      TileEleData;

//     //private string KeepAliveTexFilename = "";

//     // Timer for UI updates. Has some minor randomisation applied to even out the load.
//     private float UIUpdateTimer = 0.0f;

//     // Property to get the loaded texture
//     public GloMapTileNode? ParentTile = null;
//     List<GloMapTileNode>   ChildTiles = new();

//     private MeshInstance3D MeshInstance  = new();
//     private MeshInstance3D MeshInstanceW = new();
//     private Label3D        TileCodeLabel;

//     // Flag set when the tile (or its children) should be visible. Gates the main visibility processing.
//     public bool ActiveVisibility              = false;

//     // Record the states we assign, so we can restict  actions to just changes.
//     public bool VisibleState                  = false;
//     public bool ChildrenVisibleState          = false;
//     public bool ChildrenActiveState           = false;

//     private bool ChildConstructionComplete    = false;

//     public GloRandomLoopList RandomLoopList = new GloRandomLoopList(30, 0.1f, 0.3f);

//     // --------------------------------------------------------------------------------------------

//     public static readonly int[]   TileSizePointsPerLvl    = [ 40,    40,     40,     40,      40,         40 ];
//     public static readonly float[] LabelSizePerLvl         = [ 0.70f, 0.10f,  0.030f, 0.0012f, 0.00013f,   0.000015f ];

//     public static readonly float[] ChildTileDisplayForLvl  = [ 0.8f,  0.10f,  0.02f,  0.0045f, 0.00120f, 0.00000050f ];
//     public static readonly float[] CreateChildTilesForLvl  = [ 1.0f,  0.15f,  0.04f,  0.0050f, 0.00140f, 0.00000055f ];
//     public static readonly float[] DeleteChildTilesForLvl  = [ 1.2f,  0.25f,  0.80f,  0.0080f, 0.00180f, 0.00000070f ];

//     public static readonly float[] ChildTileDisplayMForLvl = [ 500000f,  100000f,  20000f,  5000f, 1000f, 100f];
//     public static readonly float[] CreateChildTilesMForLvl = [ 600000f,  200000f,  25000f,  7000f, 2000f, 200f];
//     public static readonly float[] DeleteChildTilesMForLvl = [ 700000f,  300000f,  30000f,  9000f, 3000f, 300f];

//     // --------------------------------------------------------------------------------------------
//     // MARK: Constructor
//     // --------------------------------------------------------------------------------------------

//     // Constructor to initialize the texture path and start loading
//     public GloMapTileNode(GloMapTileCode tileCode)
//     {
//         // Set the core Tilecode and node name.
//         TileCode = tileCode;
//         Name     = tileCode.ToString();

//         // Default the state flags
//         ConstructionComplete = false;
//         ActiveVisibility     = false;

//         // Fire off the fully background task of creating/loading the mesh
//         Task.Run(() => BackgroundTileCreation(tileCode));
//         Task.Run(() => DetermineChildTileAvailability(tileCode));
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Node Routines
//     // --------------------------------------------------------------------------------------------

//     public override void _Ready()
//     {
//     }

//     // --------------------------------------------------------------------------------------------

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//         // Slow the tile processing down to a random 10hz
//         if (UIUpdateTimer < GloCentralTime.RuntimeSecs)
//         {
//             UIUpdateTimer = GloCentralTime.RuntimeSecs + RandomLoopList.GetNext();

//             DetermineChildConstructionComplete();

//             if (ConstructionComplete)
//             {
//                 if (TileCode.MapLvl == 0)
//                     ActiveVisibility = true;

//                 if (ActiveVisibility)
//                 {
//                     try
//                     {
//                         UpdateVisbilityRules();
//                     }
//                     catch (Exception e)
//                     {
//                         GD.PrintErr($"UpdateVisbilityRules: {TileCode.ToString()} {e.Message}");
//                     }
//                 }

//                 // bool showDebug = GloMapManager.ShowDebug && VisibleState;
//                 // if (TileCodeLabel != null)
//                 //     TileCodeLabel.Visible = showDebug;

//                 // if (MeshInstanceW != null)
//                 //     MeshInstanceW.Visible = showDebug;

//                 // if (!string.IsNullOrEmpty(KeepAliveTexFilename))
//                 //     GloGodotFactory.Instance.ImageManager.KeepAlive(KeepAliveTexFilename);
//             }
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Create Process
//     // --------------------------------------------------------------------------------------------

//     // Creation process for this tile:
//     // - Background thread fucntino to load the image and elevation assets.
//     // - Main thread function to create the mesh and add it to the tree etc.

//     private async void BackgroundTileCreation(GloMapTileCode tileCode)
//     {
//         // Pause the thread, being a good citizen with lots of tasks around.
//         await Task.Yield();

//         // Starting: Set the flags that will be used later to determine activity around the tile while we construct it.
//         ConstructionComplete = false;
//         ActiveVisibility     = false;

//         // Setup the tile center shortcuts
//         GloLLBox llBoxForCode = GloMapTileCode.LLBoxForCode(TileCode);
//         RwTileCenterLLA = new GloLLAPoint(llBoxForCode.CenterPoint);
//         RwTileCenterXYZ = RwTileCenterLLA.ToXYZ();

//         // Setup some basic elements of the tile ahead of the mail elevation and image loading.
//         Filepaths = new GloMapTileFilepaths(TileCode); // Figure out the file paths for the tile

//         // Pause the thread, being a good citizen with lots of tasks around.
//         await Task.Yield();

//         // ----------------------------
//         // Default

//         // Default everything, in case we fall through the logic, the objects are not null
//         UVBox        = new GloUVBoxDropEdgeTile(GloUVBoxDropEdgeTile.UVTopLeft, GloUVBoxDropEdgeTile.UVBottomRight);
//         TileMaterial = GloMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0f, 1f));
//         TileEleData  = new GloFloat2DArray(10, 10);

//         // ----------------------------
//         // Image

//         // Simple case, we have a WebP image, load it.
//         if (Filepaths.WebpFileExists)
//         {
//             LoadWebp();
//         }
//         // We don't have a WebP, but we do see a PNG (most likely import format). Convert it to WebP. Open it.
//         else if (Filepaths.ImageFileExists)
//         {
//             if (!Filepaths.WebpFileExists)
//                 GloWebpCompressor.CompressTexture(Filepaths.ImageFilepath, Filepaths.WebpFilepath);

//             // Update the filepaths to reflect the new WebP file, try to open it.
//             Filepaths = new GloMapTileFilepaths(TileCode);
//             if (Filepaths.WebpFileExists)
//                 LoadWebp();
//         }
//         else
//         {
//             // Else, we have no image, use the parent tile if we see one.
//             if (ParentTile != null)
//             {
//                 if (ParentTile.TileMaterial != null)
//                     TileMaterial = ParentTile.TileMaterial;

//                 // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range.
//                 // Get the grid position of this tile in its parent (eg [1x,2y] in a 5x5 grid).
//                 UVBox = new GloUVBoxDropEdgeTile(ParentTile.UVBox, TileCode.GridPos);
//             }
//         }

//         // Pause the thread, being a good citizen with lots of tasks around.
//         await Task.Yield();

//         // ----------------------------
//         // Elevation

//         if (Filepaths.EleArrFileExists)
//         {
//             LoadTileEleArr();
//         }
//         else
//         {
//             // Shouldn't be called, due to file presence checks earlier, but is a robustness case to ensure teh data is at least something.
//             SubsampleParentTileEle();
//         }

//         // Pause the thread, being a good citizen with lots of tasks around.
//         await Task.Yield();

//         // ----------------------------
//         // Mesh

//         CreateMesh(); // Take the Tile position data, the ele data, and the UV box, and draw the mesh.
//         // InstatiateMesh(); // Create the node objects - just not add them to the tree yet.

//         // // Pause the thread, being a good citizen with lots of tasks around.
//         // await Task.Yield();

//         CallDeferred(nameof(MainThreadFinalizeCreation));
//     }

//     private void MainThreadFinalizeCreation()
//     {
//         //CreateMesh(); // Take the Tile position data, the ele data, and the UV box, and draw the mesh.
//         InstatiateMesh(); // Create the node objects - just not add them to the tree yet.

//         // Add the mesh to the tree
//         AddMeshToTree();

//         // Set the construction flag
//         ConstructionComplete = true;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Load Label
//     // --------------------------------------------------------------------------------------------

//     // Add a label to the middle of the tile, oriented to be flat to the surface

//     private void LabelTile(GloMapTileCode tileCode)
//     {
//         GloLLBox tileBounds = tileCode.LLBox;
//         GloLLPoint posLL = tileBounds.CenterPoint;

//         string tileCodeBoxStr;
//         if      (TileCode.MapLvl <= 2) tileCodeBoxStr = $"...[{tileBounds.MaxLatDegs:F0}, {tileBounds.MaxLonDegs:F0}]\n[{tileBounds.MinLatDegs:F0}, {tileBounds.MinLonDegs:F0}]...";
//         else if (TileCode.MapLvl < 4)  tileCodeBoxStr = $"...[{tileBounds.MaxLatDegs:F2}, {tileBounds.MaxLonDegs:F2}]\n[{tileBounds.MinLatDegs:F2}, {tileBounds.MinLonDegs:F2}]...";
//         else                           tileCodeBoxStr = $"...[{tileBounds.MaxLatDegs:F4}, {tileBounds.MaxLonDegs:F4}]\n[{tileBounds.MinLatDegs:F4}, {tileBounds.MinLonDegs:F4}]...";

//         float KPixelSize = LabelSizePerLvl[tileCode.MapLvl];
//         TileCodeLabel = GloLabel3DFactory.CreateLabel($"{tileCode.ToString()}\n{tileCodeBoxStr}", KPixelSize);
//         TileCodeLabel.Name = $"Label:{tileCode.ToString()}";


//         // Determine the positions and orientation
//         GloLLAPoint pos  = new GloLLAPoint() { LatDegs = posLL.LatDegs,        LonDegs = posLL.LonDegs, AltMslM = 2000};
//         GloLLAPoint posN = new GloLLAPoint() { LatDegs = posLL.LatDegs + 0.01, LonDegs = posLL.LonDegs, AltMslM = 2000};

//         Godot.Vector3 v3Pos   = GloGeoConvOperations.RwToGe(pos);
//         Godot.Vector3 v3PosN  = GloGeoConvOperations.RwToGe(posN);
//         Godot.Vector3 v3VectN = (v3PosN - v3Pos).Normalized();

//         TileCodeLabel.Visible = false;
//         AddChild(TileCodeLabel);

//         TileCodeLabel.Position = v3Pos;
//         TileCodeLabel.LookAt(GlobalTransform.Origin, v3VectN);
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Load Subtiles
//     // --------------------------------------------------------------------------------------------

//     private void CreateSubtileNodes()
//     {
//         if (!TileCode.IsValid())
//         {
//             GloCentralLog.AddEntry($"Invalid tile code: {TileCode}");
//             return;
//         }

//         // Compile the list of child node names.
//         List<GloMapTileCode> childTileCodes = TileCode.ChildCodesList();

//         if ((childTileCodes == null) || (childTileCodes.Count == 0))
//         {
//             GloCentralLog.AddEntry($"No child tile codes for: {TileCode}");
//             return;
//         }

//         if (!IsInsideTree())
//         {
//             GloCentralLog.AddEntry($"Node not in tree: {TileCode}");
//             return;
//         }

//         // Loop through the list of child node names, and create a new node for each one, if it does not exist.
//         foreach (GloMapTileCode currTileCode in childTileCodes)
//         {
//             //GloCentralLog.AddEntry($"Creating subtile: {currTileCode}");

//             string tileName = currTileCode.ToString();

//             // Check if the node already exists
//             if (HasNode(tileName)) continue;

//             // Create a new node
//             GloMapTileNode childTile   = new GloMapTileNode(currTileCode);
//             childTile.ParentTile       = this;
//             childTile.ActiveVisibility = false;

//             AddChild(childTile);
//             ChildTiles.Add(childTile);
//             // GloMapManager.AllTileList.AddTile(childTile);

//             childTile.UpdateVisbilityRules();
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Delete Subtiles
//     // --------------------------------------------------------------------------------------------

//     // Cascades - so we don't get an inconsistency in the tree on a mid-tree node getting serviced before the leaf node.
//     void DeleteSubtileNodes()
//     {
//         // Assume visibility is already set to false
//         foreach (GloMapTileNode currTile in ChildTiles)
//         {
//             currTile.DeleteSubtileNodes();
//             currTile.QueueFree();
//             // GloMapManager.AllTileList.AddTile(currTile);
//         }
//         ChildTiles.Clear();
//         ChildConstructionComplete = false;
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Visibility
//     // --------------------------------------------------------------------------------------------

//     // Applying each stage of the visibility rules to the tile and its children, split out for clarity
//     // and to ease debugging and development steps.

//     // Each function checks the current state, and if it is not the desired state, sets it.

//     private void SetVisibility(bool visible)
//     {
//         if (VisibleState != visible)
//         {
//             VisibleState = visible;
//             //GD.Print($"Setting visibility for {TileCode} to {visible}");

//             bool showDebug  = visible && GloGodotFactory.Instance.UIState.ShowTileInfo;


//             if (MeshInstance  != null) MeshInstance.Visible  = visible;
//             if (MeshInstanceW != null) MeshInstanceW.Visible = showDebug;
//             if (TileCodeLabel != null) TileCodeLabel.Visible = showDebug;
//         }
//     }

//     private void SetChildrenVisibility(bool visible)
//     {
//         //GD.Print($"{TileCode} - {VisibleState} {visible}");

//         foreach (GloMapTileNode currTile in ChildTiles)
//         {
//             currTile.SetVisibility(visible);

//             // cascade if deactivating children
//             if (!visible)
//                 currTile.SetChildrenVisibility(false);
//         }
//     }

//     private void SetChildrenActive(bool active)
//     {
//         foreach (GloMapTileNode currTile in ChildTiles)
//         {
//             currTile.ActiveVisibility = active;

//             // cascade if deactivating children
//             if (!active)
//                 currTile.SetChildrenActive(false);
//         }
//     }

//     // --------------------------------------------------------------------------------------------

//     // ActiveVisibility: Means active within the "Towers of Hanoi" tree of tiles, but not necessarily
//     //                   visible, its children may be visible. This flag means its assessed

//     private void UpdateVisbilityRules()
//     {
//         // Don't entertain the idea of displaying any elements until the construction is complete and all the nodes and meshes are in place.
//         if (!ConstructionComplete) return;

//         // Lvl0 tiles are always marked as active, so we have a starting point for the "towers of hanoi" tree of applying visibility rules.
//         if (TileCode.MapLvl == 0) ActiveVisibility = true;

//         if (ActiveVisibility)
//         {
//             // To allow for different game-engine display radii, we do everything in terms of a fraction of the displayed Earth's radius.
//             float distanceToHorizonM    = (float)(GloWorldOperations.DistanceToHorizonM(GloMapManager.LoadRefLLA.AltMslM));
//             float distanceToTileCenterM = (float)(GloMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ));
//             float distanceFraction      = (float)(distanceToTileCenterM / GloWorldConsts.EarthRadiusM);

//             bool tileOverHorizon = distanceToTileCenterM > (distanceToHorizonM * 1.5);

//             int maxMapLvl = GloMapManager.CurrMaxMapLvl;

//             // The logic could get complex, so factored it all out into a set of statement flags.
//             bool withinChildDisplayDistance = distanceFraction < ChildTileDisplayForLvl[TileCode.MapLvl];
//             bool withinChildCreateDistance  = (distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl]);
//             bool beyondChildDeleteDistance  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];

//             bool childTilesExist            = DoChildTilesExist();
//             bool childTilesLoaded           = AreChildTilesLoaded();

//             bool shouldCreateChildTiles     = withinChildCreateDistance && ConstructionComplete && !childTilesExist && (TileCode.MapLvl < maxMapLvl) && ChildTileDataAvailable;
//             bool shouldDisplayChildTiles    = withinChildDisplayDistance && childTilesLoaded;
//             bool shouldDeleteChildTiles     = beyondChildDeleteDistance;

//             float tileCenterDistanceM = (float)(GloMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ));
//             bool withinChildDisplayDistanceM = tileCenterDistanceM < ChildTileDisplayMForLvl[TileCode.MapLvl];
//             bool withinChildCreateDistanceM  = tileCenterDistanceM < CreateChildTilesMForLvl[TileCode.MapLvl];
//             bool beyondChildDeleteDistanceM  = tileCenterDistanceM > DeleteChildTilesMForLvl[TileCode.MapLvl];

//             // bool shouldDisplayChildTilesByDistance = distanceToTileCenterM < ChildTileDisplayMForLvl[TileCode.MapLvl];

//             // If we don't have any map tiles to read, just bail on setting any visibility.
//             bool mapLibFolderExists = Directory.Exists(GloMapManager.MapRootPath);
//             if (!mapLibFolderExists)
//                 return;

//             // shouldCreateChildTiles = false; // Debug

//             // If we should create child tiles, and they don't exist, create them, kick off the background processing
//             if (shouldCreateChildTiles)
//                 CreateSubtileNodes();

//             // If we should display child tiles, then make ourselves invisible, and make the children visible.
//             if (shouldDisplayChildTiles)
//             {
//                 if (!ChildrenVisibleState)
//                 {
//                     ChildrenVisibleState = true;
//                     SetChildrenActive(true);
//                     SetChildrenVisibility(true);
//                     SetVisibility(false);
//                 }
//             }
//             else
//             {
//                 if (ChildrenVisibleState)
//                 {
//                     ChildrenVisibleState = false;
//                     SetChildrenActive(false);
//                     SetChildrenVisibility(false);
//                     SetVisibility(true);
//                 }
//                 if (shouldDeleteChildTiles)
//                     DeleteSubtileNodes();
//             }
//         }
//         else
//         {
//             SetVisibility(false);
//             //SetChildrenVisibility(false);
//             SetChildrenActive(false);
//         }
//     }
// }
