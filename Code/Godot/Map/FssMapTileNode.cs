using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

// Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

public partial class FssMapTileNode : Node3D
{
    public StandardMaterial3D _material;
    public ArrayMesh TileMeshData;
    public ImageTexture TerrainTexture;

    // Construction Flags
    private bool ConstructionComplete = false;
    private bool MeshDone = false;
    private bool MeshInstatiated = false;
    private bool ImageDone = false;

    // Property to check if the material loading is done
    public bool IsDone => ConstructionComplete;

    private bool OneShotFlag = false;

    // Map Tile Readonly values
    private FssMapTileCode       TileCode;
    private FssXYZPoint          RwTileCenterXYZ;
    private FssTileNodeFilepaths Filepaths;

    private float UIUpdateTimer = 0.0f;

    // Property to get the loaded texture
    public StandardMaterial3D LoadedMaterial => _material;

    List<FssMapTileNode> ChildTiles = new();

    private MeshInstance3D MeshInstance  = new MeshInstance3D();
    private MeshInstance3D MeshInstanceW = new MeshInstance3D();

    public bool IntendedVisibility = false;

    // --------------------------------------------------------------------------------------------

    public static readonly int[] TileSizePointsPerLvl = { 30, 50, 100, 300, 500 };

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

        FssCentralLog.AddEntry($"Creating FssMapTileNode for {tileCode}");
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
        FssTextureLoader? TL = FssTextureLoader.GetGlobal();
        if ((TL != null) && (Filepaths.ImageFileExists))
            TL.QueueTexture(Filepaths.ImageFilepath);

        LabelTile(TileCode);
    }

    // --------------------------------------------------------------------------------------------

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!ConstructionComplete)
        {
            if ((MeshDone) && (!MeshInstatiated))
            {
                InstatiateMesh();
                MeshInstatiated = true;
            }

            if (MeshInstatiated)
            {
                FssTextureLoader? TL = FssTextureLoader.GetGlobal();
                if ((TL != null) && (Filepaths.ImageFileExists))
                {
                    if (TL.IsTextureLoaded(Filepaths.ImageFilepath))
                    {
                        ApplyImageMaterial();
                        ImageDone = true;
                        FssCentralLog.AddEntry($"Texture loaded: {Filepaths.ImageFilepath}");
                    }

                    if (ImageDone)
                    {
                        ConstructionComplete = true;
                    }
                }
            }
        }

        if (ConstructionComplete && !OneShotFlag)
        {
            if ((TileCode.ToString() == "BF") || (TileCode.ToString() == "BG"))
            {
                FssCentralLog.AddEntry("Creating subtiles for BF/BG");
                CreateSubtileNodes();
            }
            OneShotFlag = true;
        }

        if (UIUpdateTimer < FssCoreTime.RuntimeSecs)
        {
            UIUpdateTimer = FssCoreTime.RuntimeSecs + 1f;

            if ((TileCode.ToString() == "BF") || (TileCode.ToString() == "BG"))
            {
                UpdateVisbilityRules();
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Load Assets
    // --------------------------------------------------------------------------------------------

    private async void LoadTileEle(FssMapTileCode tileCode)
    {
        string tileCodeName = tileCode.ToString();

        bool loadEle  = Filepaths.EleFileExists;
        bool loadMesh = false; // Filepaths.MeshFileExists;
        bool saveMesh = !loadMesh;

        // Run file loading and processing on a background thread
        if (loadEle || loadMesh)
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

                // Load the elevation data
                FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(Filepaths.EleFilepath);
                FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 50000f));
                FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(res, res);

                FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);

                // Create the mesh
                meshBuilder.AddSurface(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssZeroOffset.GeEarthRadius, 0.000006f,
                    croppedArraySubSample
                );
                meshBuilder.AddSurfaceWedgeSides(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssZeroOffset.GeEarthRadius, 0.000006f, (float)(FssZeroOffset.GeEarthRadius * 0.95),
                    croppedArraySubSample
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

    // --------------------------------------------------------------------------------------------

    private void InstatiateMesh()
    {
        MeshInstance = new MeshInstance3D { Name = $"{TileCode.ToString()} mesh" };
        MeshInstance.Mesh = TileMeshData;
        MeshInstance.Visible = IntendedVisibility;
        AddChild(MeshInstance);

        MeshInstanceW = new MeshInstance3D { Name = $"{TileCode.ToString()} wire" };
        MeshInstanceW.Mesh = TileMeshData;
        MeshInstanceW.MaterialOverride = FssMaterialFactory.WireframeMaterial(new Color(0f, 0f, 0f, 0.3f));
        AddChild(MeshInstanceW);
    }

    // --------------------------------------------------------------------------------------------

    private void ApplyImageMaterial()
    {
        FssTextureLoader? TL = FssTextureLoader.GetGlobal();
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
        float KPixelSize = 0.0033f;
        Label3D label = FssLabel3DFactory.CreateLabel($"{tileCode.ToString()}", KPixelSize);

        FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);
        FssLLPoint posLL = tileBounds.CenterPoint;

        float labelGap = 0.2f;

        // Determine the positions and orientation
        FssLLAPoint pos  = new FssLLAPoint() { LatDegs = posLL.LatDegs,        LonDegs = posLL.LonDegs, RadiusM = FssZeroOffset.GeEarthRadius + labelGap};
        FssLLAPoint posN = new FssLLAPoint() { LatDegs = posLL.LatDegs + 0.01, LonDegs = posLL.LonDegs, RadiusM = FssZeroOffset.GeEarthRadius + labelGap};

        Godot.Vector3 v3Pos   = FssGeoConvOperations.RwToGe(pos);
        Godot.Vector3 v3PosN  = FssGeoConvOperations.RwToGe(posN);
        Godot.Vector3 v3VectN = v3PosN - v3Pos;

        AddChild(label);

        label.Position = v3Pos;
        label.LookAt(GlobalTransform.Origin, v3VectN);
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
            AddChild(childTile);

            ChildTiles.Add(childTile);
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
        if (MeshInstance != null)
            MeshInstance.Visible = visible;

        if (MeshInstanceW != null)
            MeshInstanceW.Visible = visible;
    }

    private void SetChildrenVisibility(bool visible)
    {
        SetVisibility(!visible);
        foreach (FssMapTileNode currTile in ChildTiles)
        {
            currTile.SetVisibility(visible);

        }
    }

    private void UpdateVisbilityRules()
    {
        // determine distance from the global focus point.
        // if the distance is less than a certain value, set the visibility to true
        // if the distance is greater than a certain value, set the visibility to false
        // if the distance is short, endeavour to create aand load child tiles.
        // if greater than a larger value, delete any child nodes to free resources.


        float distanceFraction = (float)( FssMapManager.LoadRefXYZ.DistanceTo(RwTileCenterXYZ) / FssPosConsts.EarthRadiusM );

        GD.Print($"Tile:{Name} Distance:{distanceFraction}");



        // Distance judged in multiples of radius, to accomodate smaller worlds while debugging

        float[] DisplayTileForLvl      = { 1f,   0.5f, 0.3f, 0.1f,    0.001f };
        float[] CreateChildTilesForLvl = { 0.5f, 0.3f, 0.1f, 0.001f , 0.00001f};
        float[] DeleteChildTilesForLvl = { 0.8f, 0.6f, 0.2f, 0.002f , 0.00002f};

        bool shouldDisplayChildTiles = distanceFraction < DisplayTileForLvl[TileCode.MapLvl];
        bool shouldCreateChildTiles  = distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl];
        bool shouldDeleteChildTiles  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];
        bool childTilesExist         = DoChildTilesExist();
        bool childTilesLoaded        = AreChildTilesLoaded();

        //GD.Print($"Tile: {TileCode} Distance: {distanceFraction} Display: {shouldDisplayChildTiles} Create: {shouldCreateChildTiles} Delete: {shouldDeleteChildTiles}");


        if (!childTilesExist && shouldCreateChildTiles)
        {
            CreateSubtileNodes();
        }

        if (childTilesExist)
        {
            if (shouldDisplayChildTiles && childTilesLoaded)
            {
                SetChildrenVisibility(true);
            }
            else
            {
                SetChildrenVisibility(false);
            }
        }
    }

}
