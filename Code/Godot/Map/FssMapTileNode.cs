using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

// Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

public partial class FssMapTileNode : Node3D
{
    public StandardMaterial3D   _material;
    public ArrayMesh            TileMeshData;
    public ImageTexture         TerrainTexture;
    public FssUvBoxDropEdgeTile UVBox;

    public bool ChildEleSubampled = false;
    public FssFloat2DArray[,] ChildEleData;

    // Construction Flags
    private bool ConstructionComplete = false;
    private bool MeshDone             = false;
    private bool MeshInstatiated      = false;
    private bool ImageDone            = false;

    // Property to check if the material loading is done
    public bool IsDone => ConstructionComplete;

    // Map Tile Readable values
    public  FssMapTileCode       TileCode;
    private FssXYZPoint          RwTileCenterXYZ;
    public  FssTileNodeFilepaths Filepaths;
    public  FssFloat2DArray      TileEleData;

    private float UIUpdateTimer = 0.0f;

    // Property to get the loaded texture
    public StandardMaterial3D LoadedMaterial => _material;

    public FssMapTileNode ParentTile = null;
    List<FssMapTileNode> ChildTiles = new();

    private MeshInstance3D MeshInstance  = new MeshInstance3D();
    private MeshInstance3D MeshInstanceW = new MeshInstance3D();
    private Label3D TileCodeLabel;

    public bool ActiveVisibility           = false;

    // Record the states we assign, so we can restict  actions to just changes.
    public bool VisibleState                  = false;
    public bool ChildrenVisibleState          = false;
    public bool ChildrenActiveState           = false;

    public FssRandomLoopList RandomLoopList = new FssRandomLoopList(30, 0.12f, 0.15f);

    // --------------------------------------------------------------------------------------------

    public static readonly int[]   TileSizePointsPerLvl = { 15, 20, 20, 20, 20 };
    public static readonly float[] LabelSizePerLvl      = { 0.1f, 0.005f, 0.002f, 0.0002f, 0.00003f };

    public static readonly float[] childTileDisplayForLvl = { 0.8f, 0.15f, 0.04f, 0.0025f, 0.0000005f };
    public static readonly float[] CreateChildTilesForLvl = { 1.0f, 0.25f, 0.08f, 0.0050f, 0.0000010f};
    public static readonly float[] DeleteChildTilesForLvl = { 1.2f, 0.40f, 0.16f, 0.0100f, 0.0000015f};

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    // Constructor to initialize the texture path and start loading
    public FssMapTileNode(FssMapTileCode tileCode)
    {
        TileCode = tileCode;

        // Set the node name
        Name = tileCode.ToString();

        // Setup a few readonly values that help control visibility
        FssLLBox llBoxForCode = FssMapTileCode.LLBoxForCode(tileCode);

        FssLLPoint  tileBoxLLCentre  = llBoxForCode.CenterPoint;
        FssLLAPoint tileBoxLLACentre = new FssLLAPoint() { LatDegs = tileBoxLLCentre.LatDegs, LonDegs = tileBoxLLCentre.LonDegs, AltMslM = 0 };
        RwTileCenterXYZ = tileBoxLLACentre.ToXYZ();

        Task.Run(() => BackgroundTileCreation(tileCode));

        // FssCentralLog.AddEntry($"Creating FssMapTileNode for {tileCode}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node Routines
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        // Figure out the file paths for the tile
        Filepaths = new FssTileNodeFilepaths(TileCode);

        // Fire off the fully background task of creating/loading the mesh
        Task.Run(() => LoadTileEle(TileCode));

        // Get the global texture loader instance
        FssTextureLoader? TL = FssTextureLoader.Instance;
        if (TL != null)
        {
            if (Filepaths.ImageFileExists)
            {
                TL.QueueTexture(Filepaths.ImageFilepath);
            }

        }

        LabelTile(TileCode);
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Slow the tile processing down to a random 10hz
        if (UIUpdateTimer < FssCoreTime.RuntimeSecs)
        {
            UIUpdateTimer = FssCoreTime.RuntimeSecs + RandomLoopList.GetNext();

            if (!ConstructionComplete)
            {
                if ((MeshDone) && (!MeshInstatiated))
                {
                    InstatiateMesh();
                    MeshInstatiated = true;
                    return;
                }

                if (MeshInstatiated)
                {
                    FssTextureLoader? TL = FssTextureLoader.Instance;
                    if ((TL != null) && (Filepaths.ImageFileExists))
                    {
                        if (TL.IsTextureLoaded(Filepaths.ImageFilepath))
                        {
                            ApplyImageMaterial();
                            ImageDone = true;
                            ConstructionComplete = true;
                            FssCentralLog.AddEntry($"Texture loaded: {Filepaths.ImageFilepath}");
                        }
                    }
                }
            }


            if (ConstructionComplete)
            {
                if (TileCode.MapLvl == 0) ActiveVisibility = true;

                if (ActiveVisibility) UpdateVisbilityRules();
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load Assets
    // --------------------------------------------------------------------------------------------

    // Top level async function to co-oridnate the loading of the tile assets
    // Single-thread critical areas will be in factored out functions that are CallDeferred().
    // Note that CallDeferred() is not a blocking call, it will be executed on the next frame.

    private void BackgroundTileCreation(FssMapTileCode tileCode)
    {
        // Figure out the file paths for the tile
        Filepaths = new FssTileNodeFilepaths(TileCode);

        // Load the elevation data / UV-Box / Mesh
        LoadTileEle(tileCode);

        InstatiateMesh();

        // Load the image data
        FssTextureLoader? TL = FssTextureLoader.Instance;
        if (TL != null)
        {
            if (Filepaths.ImageFileExists)
            {
                TL.QueueTexture(Filepaths.ImageFilepath);
            }

            ImageTexture? tex = TL.LoadTextureDirect(Filepaths.ImageFilepath);
            if (tex != null)
            {
                Material? mat = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);
                if (mat != null)
                {
                    MeshInstance.MaterialOverride = mat;
                }
            }

        }

        CallDeferred(nameof(MainThreadFinalizeCreation));

    }

    private void LoadTileEle(FssMapTileCode tileCode)
    {
        string tileCodeName = tileCode.ToString();

        bool loadEle  = Filepaths.EleFileExists;
        bool loadMesh = false; //Filepaths.MeshFileExists;
        bool saveMesh = false; //!loadMesh;

        // Run file loading and processing on a background thread
        //if (loadEle || loadMesh)
        {
            FssMeshBuilder meshBuilder = new();
            if (loadMesh)
            {
                meshBuilder.meshData = FssMeshDataIO.ReadMeshFromFile(Filepaths.MeshFilepath);
                FssCentralLog.AddEntry($"Loaded mesh: {Filepaths.MeshFilepath}");
            }
            else
            {
                int res = TileSizePointsPerLvl[TileCode.MapLvl];

                // --------------------------------------
                // Loading or sourcing the elevation data

                if (Filepaths.EleFileExists)
                {
                    // Load the elevation data
                    FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(Filepaths.EleFilepath);
                    FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 50000f));
                    FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(res, res);

                    TileEleData = croppedArraySubSample;

                    // Speculatively create the child tile subsampled data before we lose the read file.



                    //UVBox = new FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile.UVTopLeft, FssUvBoxDropEdgeTile.UVBottomRight, res, res);
                    //GD.Print($"{tileCode} - Clean UVBox - {UVBox}");
                }
                else
                {
                    // Else, no elevation exists, subsample the parent tile elevation data and UV-Box.
                    if (ParentTile != null)
                    {
                        TileEleData = ParentTile.TileEleData.GetInterpolatedSubgrid(tileCode.GridPos, res, res);
                        saveMesh = false;

                        //UVBox = new FssUvBoxDropEdgeTile(ParentTile.UVBox, res, res, tileCode.GridPos);
                        //GD.Print($"{tileCode} - Subsampled UVBox - {UVBox} // ParentTile.UVBox{ParentTile.UVBox} // tileCode.GridPos: {tileCode.GridPos}");
                    }
                    // Else no parent, create a flat tile
                    else
                    {
                        TileEleData = new FssFloat2DArray(res, res);
                        saveMesh = false;

                        //UVBox = FssUvBoxDropEdgeTile.Default(res, res);
                        //GD.Print($"{tileCode} - Zero UVBox - {UVBox}");
                    }
                }

                // --------------------------------------

                int widthRes = TileEleData.Width;
                int heightRes = TileEleData.Height;

                if (Filepaths.ImageFileExists)
                {
                    UVBox = new FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile.UVTopLeft, FssUvBoxDropEdgeTile.UVBottomRight, widthRes, heightRes);
                    //GD.Print($"{tileCode} - Clean UVBox - {UVBox}");
                }
                else
                {
                    if (ParentTile != null)
                    {
                        Filepaths.ImageFilepath   = ParentTile.Filepaths.ImageFilepath;
                        Filepaths.ImageFileExists = ParentTile.Filepaths.ImageFileExists;

                        UVBox = new FssUvBoxDropEdgeTile(ParentTile.UVBox, widthRes, heightRes, tileCode.GridPos);
                        //GD.Print($"{tileCode} - Subsampled UVBox - {UVBox} // ParentTile.UVBox{ParentTile.UVBox} // tileCode.GridPos: {tileCode.GridPos}");
                    }
                    else
                    {
                        UVBox = FssUvBoxDropEdgeTile.Default(widthRes, heightRes);
                        //GD.Print($"{tileCode} - Zero UVBox - {UVBox}");
                    }
                }

                // --------------------------------------

                FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);

                // Create the mesh
                meshBuilder.AddSurfaceWithUVBox(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssPosConsts.EarthRadiusM,
                    TileEleData, UVBox
                );
                meshBuilder.AddSurfaceWedgeSides(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssPosConsts.EarthRadiusM, (float)FssPosConsts.EarthRadiusM * 0.9f,
                    TileEleData
                ); //bool flipTriangles = false)
            }
            TileMeshData = meshBuilder.BuildWithUV(tileCodeName);
            MeshDone = true;

            if (saveMesh)
            {
                // Save the mesh to a file
                FssMeshDataIO.WriteMeshToFile(meshBuilder.meshData, Filepaths.MeshFilepath);
                FssCentralLog.AddEntry($"Saved mesh: {Filepaths.MeshFilepath}");
            }
        }
    }

    private void MainThreadFinalizeCreation()
    {
        // Add the mesh to the tree
        AddMeshToTree();

        // Set the visibility rules
        UpdateVisbilityRules();

        // Set the construction flag
        ConstructionComplete = true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Final Load instantiation steps
    // --------------------------------------------------------------------------------------------

    private void InstatiateMesh()
    {
        MeshInstance = new MeshInstance3D { Name = $"{TileCode.ToString()} mesh" };
        MeshInstance.Mesh = TileMeshData;
        //AddChild(MeshInstance);

        MeshInstanceW = new MeshInstance3D { Name = $"{TileCode.ToString()} wire" };
        MeshInstanceW.Mesh = TileMeshData;
        MeshInstanceW.MaterialOverride = FssMaterialFactory.WireframeMaterial(new Color(0f, 0f, 0f, 0.3f));
        //AddChild(MeshInstanceW);

        // Will be made visible when the texture is loaded
        MeshInstance.Visible  = false;
        MeshInstanceW.Visible = false;
    }

    private void AddMeshToTree()
    {
        AddChild(MeshInstance);
        AddChild(MeshInstanceW);
    }

    // --------------------------------------------------------------------------------------------

    private void ApplyImageMaterial()
    {
        FssTextureLoader? TL = FssTextureLoader.Instance;
        if (TL != null)
        {
            if (TL.IsTextureLoaded(Filepaths.ImageFilepath))
            {
                Material? mat = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);
                if (mat != null)
                {
                    MeshInstance.MaterialOverride = mat;
                }
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load Label
    // --------------------------------------------------------------------------------------------

    // Add a label to the middle of the tile, oriented to be flat to the surface

    private void LabelTile(FssMapTileCode tileCode)
    {
        float KPixelSize = LabelSizePerLvl[tileCode.MapLvl];
        TileCodeLabel = FssLabel3DFactory.CreateLabel($"{tileCode.ToString()}", KPixelSize);

        FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);
        FssLLPoint posLL = tileBounds.CenterPoint;

        // Determine the positions and orientation
        FssLLAPoint pos  = new FssLLAPoint() { LatDegs = posLL.LatDegs,        LonDegs = posLL.LonDegs, AltMslM = 2000};
        FssLLAPoint posN = new FssLLAPoint() { LatDegs = posLL.LatDegs + 0.01, LonDegs = posLL.LonDegs, AltMslM = 2000};

        Godot.Vector3 v3Pos   = FssGeoConvOperations.RwToGe(pos);
        Godot.Vector3 v3PosN  = FssGeoConvOperations.RwToGe(posN);
        Godot.Vector3 v3VectN = (v3PosN - v3Pos).Normalized();

        TileCodeLabel.Visible = false;
        AddChild(TileCodeLabel);

        TileCodeLabel.Position = v3Pos;
        TileCodeLabel.LookAt(GlobalTransform.Origin, v3VectN);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load Subtiles
    // --------------------------------------------------------------------------------------------

    private void CreateSubtileNodes()
    {
        // Compile the list of child node names.
        List<FssMapTileCode> childTileCodes = TileCode.ChildCodesList();

        // Loop through the list of child node names, and create a new node for each one, if it does not exist.
        foreach (FssMapTileCode currTileCode in childTileCodes)
        {
            FssCentralLog.AddEntry($"Creating subtile: {currTileCode}");

            string tileName = currTileCode.ToString();

            // Check if the node already exists
            if (HasNode(tileName)) continue;

            // Create a new node
            FssMapTileNode childTile = new FssMapTileNode(currTileCode);
            childTile.ParentTile = this;
            childTile.ActiveVisibility = false;
            AddChild(childTile);

            ChildTiles.Add(childTile);

            //childTile.UpdateVisbilityRules();
        }
    }

    private bool DoChildTilesExist()
    {
        return ChildTiles.Count > 0;
    }

    // Get the list of child tile names, then loop through, finding these nodes, and querying their IsDone property
    private bool AreChildTilesLoaded()
    {
        // return false if there are no child tiles
        if (ChildTiles.Count == 0)
            return false;

        // Loop through the list of child node names, and query the IsDone property of each node.
        foreach (FssMapTileNode currTile in ChildTiles)
        {
            // Query the IsDone property of the node
            if (!currTile.IsDone)
            {
                return false;
            }
        }

        // Return true, we've not found any false criteria in the search
        return true;
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Delete Subtiles
    // --------------------------------------------------------------------------------------------

    void DeleteSubtileNodes()
    {
        // Assume visibility is already set to false
        foreach (FssMapTileNode currTile in ChildTiles)
        {
            currTile.QueueFree();
        }
        ChildTiles.Clear();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Visibility
    // --------------------------------------------------------------------------------------------

    private void SetVisibility(bool visible)
    {
        if (VisibleState != visible)
        {
            VisibleState = visible;
            //GD.Print($"Setting visibility for {TileCode} to {visible}");

            if (MeshInstance != null)  MeshInstance.Visible  = visible;
            if (MeshInstanceW != null) MeshInstanceW.Visible = visible;
            if (TileCodeLabel != null) TileCodeLabel.Visible = visible;
        }
    }

    private void SetChildrenVisibility(bool visible)
    {
        if (ChildrenVisibleState != visible)
        {
            ChildrenVisibleState = visible;

            //GD.Print($"Setting children visibility for {TileCode} to {visible}");

            foreach (FssMapTileNode currTile in ChildTiles)
            {
                currTile.SetVisibility(visible);
            }
        }
    }

    private void SetChildrenActive(bool active)
    {
        if (ChildrenActiveState != active)
        {
            ChildrenActiveState = active;
            //GD.Print($"Setting children active for {TileCode} to {active}");

            foreach (FssMapTileNode currTile in ChildTiles)
            {
                currTile.ActiveVisibility = active;
            }
        }
    }

    private void UpdateVisbilityRules()
    {
        // determine distance from the global focus point.
        // if the distance is less than a certain value, set the visibility to true
        // if the distance is greater than a certain value, set the visibility to false
        // if the distance is short, endeavour to create aand load child tiles.
        // if greater than a larger value, delete any child nodes to free resources.




        // Determine the "ActiveVisibility" for every tile for its level.
        // without ActiveVisibility, the update visbility rules will not be applied and the tile should not be displayed.
        // Tile with ActiveVisibility may be visible, or their children may be visisble.

        // IntendedVisibility: A flag set by a parent tile to indicate that the tile (or its children) should be visible.

        if (TileCode.MapLvl == 0) ActiveVisibility = true;

        if (ActiveVisibility)
        {
            float distanceFraction = (float)( FssMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ) / FssPosConsts.EarthRadiusM );

            //GD.Print($"Tile:{Name} Distance:{distanceFraction:0.00}");

            // Distances judged in multiples of radius, to accomodate smaller worlds (GE Radius)
            //float[] DisplayTileForLvl      = { 1f,   0.1f, 0.05f, 0.025f, 0.0005f };

            bool shouldDisplayChildTiles = distanceFraction < childTileDisplayForLvl[TileCode.MapLvl];
            bool shouldCreateChildTiles  = (distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl]) && (TileCode.MapLvl < FssMapTileCode.MaxMapLvl);
            bool shouldDeleteChildTiles  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];
            bool childTilesExist         = DoChildTilesExist();
            bool childTilesLoaded        = AreChildTilesLoaded();

            // If we should create child tiles, and they don't exist, create them.
            if (shouldCreateChildTiles && !childTilesExist)
            {
                CreateSubtileNodes();
                SetChildrenVisibility(false);
                GD.Print($"Created subtiles for {TileCode}");
            }

            // If the child tiles exist, and they are loaded, and we should display them, set their visibility to true.
            if (childTilesLoaded)
            {
                if (shouldDisplayChildTiles)
                {
                    SetChildrenActive(true);
                    SetChildrenVisibility(true);
                    SetVisibility(false);
                }
                else
                {
                    SetChildrenActive(false);
                    SetChildrenVisibility(false);
                    SetVisibility(true);

                    if (shouldDeleteChildTiles)
                    {
                        DeleteSubtileNodes();
                        GD.Print($"Deleted subtiles for {TileCode}");
                    }
                }
            }
            else
            {
                SetChildrenActive(false);
                SetVisibility(true); // no children ready, show the parent
            }
        }
        else
        {
            SetVisibility(false);
            SetChildrenVisibility(false);
            SetChildrenActive(false);
        }
    }
}
