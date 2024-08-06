using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Godot;

public partial class FssMapTileNode : Node3D
{
    private string _imagePath;
    private bool _isDone;
    StandardMaterial3D _material;
    public ArrayMesh meshData;

    private bool MeshDone = false;

    // Property to check if the material loading is done
    public bool IsDone => _isDone;

    private bool OneShotFlag = false;

    FssMapTileCode TileCode;

    private float UIUpdateTimer = 0.0f;

    // Property to get the loaded texture
    public StandardMaterial3D LoadedMaterial => _material;

    List<FssMapTileNode> ChildTiles = new();

    private MeshInstance3D MeshInstance  = new MeshInstance3D();
    private MeshInstance3D MeshInstanceW = new MeshInstance3D();

    public bool IntendedVisibility = false;
    FssXYZPoint RwTileCenterXYZ = new FssXYZPoint(0, 0, 0);

    // --------------------------------------------------------------------------------------------

    // Constructor to initialize the texture path and start loading
    public FssMapTileNode(FssMapTileCode tileCode)
    {
        // _imagePath = imagePath;
        // _isDone = false;
        // StartLoading();
        TileCode = tileCode;

        Name = tileCode.ToString();

        RwTileCenterXYZ = FssMapTileCode.LLBoxForCode(tileCode).CenterPoint().ToXYZ();

        FssCentralLog.AddEntry($"Creating FssMapTileNode for {tileCode}");
    }

    // --------------------------------------------------------------------------------------------

    public override void _Ready()
    {
        Task.Run(() => LoadTileEle(TileCode));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (MeshDone && OneShotFlag == false)
        {
            LoadMaterial(TileCode);

            LabelTile(TileCode);

            // AddChild( new MeshInstance3D() {
            //     Name             = "MapTileMesh",
            //     Mesh             = meshData,
            //     MaterialOverride = _material
            // });

            // AddChild( new MeshInstance3D() {
            //     Name             = "MapTileMeshW",
            //     Mesh             = meshData,
            //     MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial()
            // });

            OneShotFlag = true;

            GD.Print("=========> MapTileMesh added to FssMapTileNode");

            if (TileCode.ToString() == "BG")
            {
                FssCentralLog.AddEntry("Creating subtiles for BG");
                CreateSubtileNodes();
            }
        }


        if (UIUpdateTimer < FssCoreTime.RuntimeSecs)
        {
            UIUpdateTimer = FssCoreTime.RuntimeSecs + 1f;

            if (TileCode.ToString() == "BG")
            {
                UpdateVisbilityRules();
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    // Async

    // // Method to start the texture loading process
    // private async void StartLoading()
    // {
    //     var image = await LoadImageAsync(_imagePath);
    //     CallDeferred(nameof(GetMaterial), image);

    // }

    // // Asynchronous method to load the image
    // private async Task<Image> LoadImageAsync(string path)
    // {
    //     return await Task.Run(() => {
    //         // Load the image from the file system
    //         Image image = new Image();
    //         image.Load(path);
    //         return image;
    //     });
    // }

    // --------------------------------------------------------------------------------------------

    // Image / texture / material functions

    // // Method to create a material with the loaded texture
    // private void GetMaterial(Image image)
    // {
    //     _material = new StandardMaterial3D();
    //     _material.AlbedoTexture = ImageTexture.CreateFromImage(image);

    //     // Only set the done flag AFTER the material is created
    //     _isDone = true;

    // }

    // --------------------------------------------------------------------------------------------
    // MARK: Load Assets
    // --------------------------------------------------------------------------------------------

    private async void LoadTileEle(FssMapTileCode tileCode)
    {
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        string imageFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");
        string eleFilePath   = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.asc");
        string meshFilePath  = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Mesh_{tileCodeName}.mesh");

        FssCentralLog.AddEntry($"Looking for elevation file: {eleFilePath}");
        FssCentralLog.AddEntry($"Looking for mesh file: {meshFilePath}");

        eleFilePath   = FssGodotFileUtil.GetActualPath(eleFilePath);
        meshFilePath  = FssGodotFileUtil.GetActualPath(meshFilePath);

        bool loadEle  = File.Exists(eleFilePath);
        bool loadMesh = File.Exists(meshFilePath);
        bool saveMesh = !loadMesh;

        // Run file loading and processing on a background thread
        if (loadEle || loadMesh)
        {
            FssMeshBuilder meshBuilder = new();
            if (loadMesh)
            {
                meshBuilder.meshData = FssMeshDataIO.ReadMeshFromFile(meshFilePath);
                FssCentralLog.AddEntry($"Loaded mesh: {meshFilePath}");
            }
            else
            {
                // Load the elevation data
                FssFloat2DArray asciiArcArry = FssFloat2DArrayIO.LoadFromArcASIIGridFile(eleFilePath);
                FssFloat2DArray croppedArray = FssFloat2DArrayOperations.CropToRange(asciiArcArry, new FssFloatRange(0f, 50000f));
                FssFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(300, 300);

                FssLLBox tileBounds = FssMapTileCode.LLBoxForCode(tileCode);

                // Create the mesh
                meshBuilder.AddSurface(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssZeroOffset.EarthRadiusM, 0.000006f,
                    croppedArraySubSample
                );
                meshBuilder.AddSurfaceWedgeSides(
                    (float)tileBounds.MinLonDegs, (float)tileBounds.MaxLonDegs,
                    (float)tileBounds.MinLatDegs, (float)tileBounds.MaxLatDegs,
                    (float)FssZeroOffset.EarthRadiusM, 0.000006f, 4.5f,
                    croppedArraySubSample
                ); //bool flipTriangles = false)
            }
            meshData = meshBuilder.BuildWithUV(tileCodeName);
            MeshDone = true;

            if (saveMesh)
            {
                // Save the mesh to a file
                FssMeshDataIO.WriteMeshToFile(meshBuilder.meshData, meshFilePath);
                FssCentralLog.AddEntry($"Saved mesh: {meshFilePath}");
            }

            //CallDeferred(nameof(LoadMaterial), tileCode);
        }
    }

    private void LoadMaterial(FssMapTileCode tileCode)
    {
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        string imageFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");

        imageFilePath  = FssGodotFileUtil.GetActualPath(imageFilePath);

        FssCentralLog.AddEntry($"Looking for image file: {imageFilePath}");

        // Check if the image file exists
        if (!File.Exists(imageFilePath))
        {
            FssCentralLog.AddEntry($"Failed to location map tile image: {imageFilePath}");
            return;
        }

        var image = new Image();
        //var err = image.Load(imageFilePath);
        var err = image.Load(imageFilePath);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load image: {imageFilePath}");
            return;
        }
        var texture = ImageTexture.CreateFromImage(image);
        var material = new StandardMaterial3D
        {
            AlbedoTexture = texture,
            ShadingMode = StandardMaterial3D.ShadingModeEnum.Unshaded
        };

        // Add the mesh instances to the current Node3D
        // MeshInstanceW = new MeshInstance3D { Name = $"{tileCodeName} wire" };
        // MeshInstanceW.Mesh = meshData;
        // MeshInstanceW.MaterialOverride = FssMaterialFactory.WireframeWhiteMaterial();
        // AddChild(MeshInstanceW);

        MeshInstance = new MeshInstance3D { Name = $"{tileCodeName} image" };
        MeshInstance.Mesh = meshData;
        MeshInstance.MaterialOverride = material;
        MeshInstance.Visible = IntendedVisibility;
        AddChild(MeshInstance);

        // Set the done flag
        _isDone = true;
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

        FssLLAPoint pos = new FssLLAPoint() {
            LatDegs = tileBounds.MidLatDegs,
            LonDegs = tileBounds.MidLonDegs,
            RadiusM = FssZeroOffset.EarthRadiusM + 0.1 };
        FssPosV3 posV3 = FssGeoConvOperations.RwToGeStruct(pos);

        AddChild(label);

        label.Position = posV3.Pos;
        label.LookAt(GlobalTransform.Origin, posV3.VecNorth);
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

        float distanceFraction = 0.1f;//(float)( RwTileCenterXYZ.DistanceTo(FssEarthCore.RwFocusXYZ) / FssEarthCore.EarthRadiusM );



        // Distance judged in multiples of radius, to accomodate smaller worlds while debugging

        float[] DisplayTileForLvl      = { 1f,   0.5f, 0.3f, 0.1f,    0.001f };
        float[] CreateChildTilesForLvl = { 0.5f, 0.3f, 0.1f, 0.001f , 0.00001f};
        float[] DeleteChildTilesForLvl = { 0.8f, 0.6f, 0.2f, 0.002f , 0.00002f};

        bool shouldDisplayChildTiles = distanceFraction < DisplayTileForLvl[TileCode.MapLvl];
        bool shouldCreateChildTiles  = distanceFraction < CreateChildTilesForLvl[TileCode.MapLvl];
        bool shouldDeleteChildTiles  = distanceFraction > DeleteChildTilesForLvl[TileCode.MapLvl];
        bool childTilesExist         = DoChildTilesExist();
        bool childTilesLoaded        = AreChildTilesLoaded();

        GD.Print($"Tile: {TileCode} Distance: {distanceFraction} Display: {shouldDisplayChildTiles} Create: {shouldCreateChildTiles} Delete: {shouldDeleteChildTiles}");


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
