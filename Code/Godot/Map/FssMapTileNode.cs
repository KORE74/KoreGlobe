using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

#nullable enable

// Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

public partial class FssMapTileNode : Node3D
{
    public StandardMaterial3D?  TileMaterial = null;
    public ArrayMesh            TileMeshData;
    //public ImageTexture         TerrainTexture;
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

    // Timer for UI updates. Has some minor randomisation applied to even out the load.
    private float UIUpdateTimer = 0.0f;

    // Property to get the loaded texture
    public FssMapTileNode? ParentTile = null;
    List<FssMapTileNode>  ChildTiles = new();

    private MeshInstance3D MeshInstance  = new MeshInstance3D();
    private MeshInstance3D MeshInstanceW = new MeshInstance3D();
    private Label3D TileCodeLabel;

    // Flag set when the tile (or its children) should be visible. Gates the main visibility processing.
    public bool ActiveVisibility              = false;

    // Record the states we assign, so we can restict  actions to just changes.
    public bool VisibleState                  = false;
    public bool ChildrenVisibleState          = false;
    public bool ChildrenActiveState           = false;

    public FssRandomLoopList RandomLoopList = new FssRandomLoopList(30, 0.12f, 0.15f);

    // --------------------------------------------------------------------------------------------

    public static readonly int[]   TileSizePointsPerLvl   = { 15,   20,     20,     20,      20,         20 };
    public static readonly float[] LabelSizePerLvl        = { 0.1f, 0.005f, 0.002f, 0.0002f, 0.00003f,   0.000005f };

    public static readonly float[] childTileDisplayForLvl = { 0.8f, 0.15f,  0.04f,  0.0045f, 0.00120f, 0.0000005f};
    public static readonly float[] CreateChildTilesForLvl = { 1.0f, 0.25f,  0.08f,  0.0050f, 0.00140f, 0.0000005f};
    public static readonly float[] DeleteChildTilesForLvl = { 1.2f, 0.40f,  0.16f,  0.0080f, 0.00180f, 0.0000005f};

    // --------------------------------------------------------------------------------------------
    // MARK: Constructor
    // --------------------------------------------------------------------------------------------

    // Constructor to initialize the texture path and start loading
    public FssMapTileNode(FssMapTileCode tileCode)
    {
        // Set the core Tilecode and node name.
        TileCode = tileCode;
        Name     = tileCode.ToString();

        // Fire off the fully background task of creating/loading the mesh
        Task.Run(() => BackgroundTileCreation(tileCode));
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Node Routines
    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
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

            if (TileCode.ToString() == "BF_CF_AA")
            {
                // string x = (TileMaterial == null) ? "null" : "not null";

                // GD.Print($"Tile:{TileCode} Filepaths:{Filepaths.ImageFilepath} ImageDone:{ImageDone} MeshDone:{MeshDone} MeshInstatiated:{MeshInstatiated} ConstructionComplete:{ConstructionComplete}");
                // if (ParentTile != null) GD.Print($"Parent:{ParentTile.TileCode}");
                // GD.Print($"UVBox:{UVBox}");
                // GD.Print($"EleData:{TileEleData.sizeStr()}");
                // GD.Print($"TileMaterial:{x}");
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

    private async void BackgroundTileCreation(FssMapTileCode tileCode)
    {
        // Starting: Set the flags that will be used later to determine activity around the tile wheil we construct it.
        ConstructionComplete = false;
        ActiveVisibility     = false;

        //etup some basic elements of the tile ahead of the mail elevation and image loading.
        SetupTileCenterXYZ();
        Filepaths = new FssTileNodeFilepaths(TileCode); // Figure out the file paths for the tile

        // Pause the thread, being a good citizen with lots of tasks around.
        await Task.Yield();



        // Load the image data - which will determine the UVBox it will need.
        // - 1 - we find the image and load it.
        // - 2 - We copy the parent tile image and UVBox, and subsample them.
        if (Filepaths.ImageFileExists)
            LoadTileImage();
        else
            SubsampleParentTileImage();

        // Pause the thread, being a good citizen with lots of tasks around.
        await Task.Yield();



        // if (Filepaths.MeshFileExists)
        // {
        //     // Load the mesh data
        //     FssMeshBuilder meshBuilder = new();
        //     FssMeshData meshData = FssMeshDataIO.ReadMeshFromFile(Filepaths.MeshFilepath);
        //     meshBuilder.meshData = meshData;
        //     TileMeshData = meshBuilder.BuildWithUV(TileCode.ToString());
        //     MeshDone = true;
        // }
        // else
        // {


            if (Filepaths.EleFileExists)
                LoadTileEle();
            else
                SubsampleParentTileEle(); // Also handles the lack of a parent



        // }



        // Pause the thread, being a good citizen with lots of tasks around.
        await Task.Yield();


        CreateMesh(); // Take the Tile position data, the ele data, and the UV box, and draw the mesh.
        InstatiateMesh(); // Create the node objects - kjust not add them to the tree yet.



        // Pause the thread, being a good citizen with lots of tasks around.
        await Task.Yield();




        CallDeferred(nameof(MainThreadFinalizeCreation));
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

        ApplyImageMaterial();

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
            //FssCentralLog.AddEntry($"Creating subtile: {currTileCode}");

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

    // Applying each stage of the visibility rules to the tile and its children, split out for clarity
    // and to ease debugging and development steps.

    // Each function checks the current state, and if it is not the desired state, sets it.

    private void SetVisibility(bool visible)
    {
        if (VisibleState != visible)
        {
            VisibleState = visible;
            //GD.Print($"Setting visibility for {TileCode} to {visible}");

            bool showDebug  = FssMapManager.ShowDebug  && visible;

            if (MeshInstance != null)  MeshInstance.Visible  = visible;
            if (MeshInstanceW != null) MeshInstanceW.Visible = showDebug;
            if (TileCodeLabel != null) TileCodeLabel.Visible = showDebug;
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

    // --------------------------------------------------------------------------------------------

    private void UpdateVisbilityRules()
    {
        // Lvl0 tiles are always marked as active, so we have a starting point for the "towers of hanoi" tree of applying visibility rules.
        if (TileCode.MapLvl == 0) ActiveVisibility = true;

        if (ActiveVisibility)
        {
            // To allow for different game-engine deisplay radii, we do everything in terms of a fraction of the displayed Earth's radius.
            float distanceFraction = (float)( FssMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ) / FssPosConsts.EarthRadiusM );

            int maxMapLvl = FssMapManager.CurrMaxMapLvl;

            // The logic could get complex, so factored it all out into a set of statement flags.
            bool shouldDisplayChildTiles = distanceFraction < childTileDisplayForLvl[TileCode.MapLvl];
            bool shouldCreateChildTiles  = (distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl]) && (TileCode.MapLvl < maxMapLvl);
            bool shouldDeleteChildTiles  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];
            bool childTilesExist         = DoChildTilesExist();
            bool childTilesLoaded        = AreChildTilesLoaded();

            bool mapLibFolderExists      = Directory.Exists(FssMapManager.MapRootPath);

            // If we don't have any map tiles to read, just bail on setting any visibility.
            if (!mapLibFolderExists)
                return;

            // shouldCreateChildTiles = false; // Debug

            // If we should create child tiles, and they don't exist, create them.
            if (shouldCreateChildTiles && !childTilesExist)
            {
                CreateSubtileNodes();
                SetChildrenVisibility(false);
                // GD.Print($"Created subtiles for {TileCode}");
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
                        // GD.Print($"Deleted subtiles for {TileCode}");
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

    // --------------------------------------------------------------------------------------------
    // MARK: Private - Helper methods
    // --------------------------------------------------------------------------------------------

    private void SetupTileCenterXYZ()
    {
        // Setup a few readonly values that help control visibility
        FssLLBox llBoxForCode = FssMapTileCode.LLBoxForCode(TileCode);

        FssLLPoint  tileBoxLLCentre  = llBoxForCode.CenterPoint;
        FssLLAPoint tileBoxLLACentre = new FssLLAPoint() { LatDegs = tileBoxLLCentre.LatDegs, LonDegs = tileBoxLLCentre.LonDegs, AltMslM = 0 };
        RwTileCenterXYZ = tileBoxLLACentre.ToXYZ();
    }

    private void LoadTileImage()
    {
        FssTextureLoader? TL = FssTextureLoader.Instance;
        if (TL != null)
        {
            ImageTexture? tex = TL.LoadTextureDirect(Filepaths.ImageFilepath);

            if (tex != null)
            {
                TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

                if (TileMaterial != null)
                {
                    ImageDone = true;
                }
            }
        }

        // Setup the UV Box - new image, so a full 0,0 -> 1,1 range
        UVBox = new FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile.UVTopLeft, FssUvBoxDropEdgeTile.UVBottomRight);
    }

    private void SubsampleParentTileImage()
    {
        if (ParentTile != null)
        {
            FssTextureLoader? TL = FssTextureLoader.Instance;

            Filepaths.ImageFilepath = ParentTile.Filepaths.ImageFilepath;
            Filepaths.ImageFileExists = ParentTile.Filepaths.ImageFileExists;

            TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

            if (TileMaterial != null)
                ImageDone = true;

            // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
            // Get the grid position of this tile in its parent (eg [1x,2y] in a 5x5 grid).
            UVBox = new FssUvBoxDropEdgeTile(ParentTile.UVBox, TileCode.GridPos);

        }
        else
        {
            // Subsmapling, but no parent. Setup a default.
            UVBox = FssUvBoxDropEdgeTile.Default(TileSizePointsPerLvl[TileCode.MapLvl], TileSizePointsPerLvl[TileCode.MapLvl]);
        }
    }

    // --------------------------------------------------------------------------------------------

    private void LoadTileEle()
    {
        int resX = TileSizePointsPerLvl[TileCode.MapLvl];
        int resY = TileSizePointsPerLvl[TileCode.MapLvl];

        FssFloat2DArray asciiArcArray = FssFloat2DArrayIO.LoadFromArcASIIGridFile(Filepaths.EleFilepath);
        FssFloat2DArray croppedArray  = FssFloat2DArrayOperations.CropToRange(asciiArcArray, new FssFloatRange(0f, 50000f));
        FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(resX, resY);

        TileEleData = croppedArraySubSample;

        // Speculatively create the child tile subsampled data before we lose the read file.
        // Get the grid size for the child tiles

        if (TileCode.MapLvl < FssMapTileCode.MaxMapLvl)
        {
            int horizChildNumTiles = FssMapTileCode.NumTilesVertPerLvl[TileCode.MapLvl + 1];
            int vertChildNumTiles  = FssMapTileCode.NumTilesHorizPerLvl[TileCode.MapLvl + 1];

            int horizChildTileRes = TileSizePointsPerLvl[TileCode.MapLvl + 1];
            int vertChildTileRes  = TileSizePointsPerLvl[TileCode.MapLvl + 1];

            ChildEleData = asciiArcArray.GetInterpolatedSubGridCellWithOverlap(horizChildNumTiles, vertChildNumTiles, horizChildTileRes, vertChildTileRes);
        }
    }

    private void SubsampleParentTileEle()
    {
        if (ParentTile != null)
        {
            Fss2DGridPos tileGridPos = TileCode.GridPos;

            // Copy the parent's elevation data - regardless of its resolution, to pass that along to the child tiles
            FssFloat2DArray RawParentTileEleData = ParentTile.ChildEleData[tileGridPos.PosX, tileGridPos.PosY];

            // Create a subsampled version of the parent tile's elevation data
            TileEleData = RawParentTileEleData.GetInterpolatedGrid(TileSizePointsPerLvl[TileCode.MapLvl], TileSizePointsPerLvl[TileCode.MapLvl]);

            if (TileCode.MapLvl < FssMapTileCode.MaxMapLvl)
            {
                int horizChildNumTiles = FssMapTileCode.NumTilesVertPerLvl[TileCode.MapLvl + 1];
                int vertChildNumTiles  = FssMapTileCode.NumTilesHorizPerLvl[TileCode.MapLvl + 1];

                int horizChildTileRes = TileSizePointsPerLvl[TileCode.MapLvl + 1];
                int vertChildTileRes  = TileSizePointsPerLvl[TileCode.MapLvl + 1];

                ChildEleData = RawParentTileEleData.GetInterpolatedSubGridCellWithOverlap(horizChildNumTiles, vertChildNumTiles, 100, 100);
            }
        }
        else
        {
            TileEleData = new FssFloat2DArray(TileSizePointsPerLvl[TileCode.MapLvl], TileSizePointsPerLvl[TileCode.MapLvl]);
        }
    }

    private void CreateMesh()
    {
        // Pre-requisites:
        // - TileCode
        // - TileEleData
        // - UVBox
        // - TileMaterial

        FssMeshBuilder meshBuilder = new();

        FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(TileCode);

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

        TileMeshData = meshBuilder.BuildWithUV(TileCode.ToString());

        // bool saveMesh = true;
        // if (saveMesh)
        // {
        //     // Save the mesh to a file
        //     FssMeshDataIO.WriteMeshToFile(meshBuilder.meshData, Filepaths.MeshFilepath);
        //     FssCentralLog.AddEntry($"Saved mesh: {Filepaths.MeshFilepath}");
        // }
    }

}
