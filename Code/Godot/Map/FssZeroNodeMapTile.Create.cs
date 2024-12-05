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
    // --------------------------------------------------------------------------------------------
    // MARK: Main Create
    // --------------------------------------------------------------------------------------------

    private async void BackgroundTileCreation(FssMapTileCode tileCode)
    {
        try
        {
            // Starting: Set the flags that will be used later to determine activity around the tile while we construct it.
            ConstructionComplete = false;
            ActiveVisibility     = false;
            MeshCreated          = false;

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // Setup some basic elements of the tile ahead of the main elevation and image loading.
            Filepaths = new FssMapTileFilepaths(TileCode); // Figure out the file paths for the tile

            // Load the elevation data
            FssAppFactory.Instance.EleManager.RequestTile(tileCode);

            // Source the tile image
            SourceTileImage();

            // Yield until (ImageSourced && ElevationSourced)
            while (!ImageSourced || !ElevationSourced)
                await Task.Delay(100);

            // Create the mesh
            CreateMaterials();
            await Task.Yield(); // Yield, be a good citizen
            CreateMesh();
            await Task.Yield(); // Yield, be a good citizen

            // Set the flag for the Process() function to pick up and conclude any
            // construction tasks on the main thread.
            MeshCreated = true;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    // --------------------------------------------------------------------------------------------

    public void ProgressCreation()
    {
        // Start the creation of the mesh
        if (MeshCreated)
        {
            AttachMesh();
            ConstructionComplete = true;
        }
    }

    // --------------------------------------------------------------------------------------------

    public void CreateMaterials()
    {
        // Create the material for the mesh.
        WireColor  = FssColorUtil.Colors["Red"]; //StringToColor("default");
        SurfaceMat = FssMaterialFactory.TransparentColoredMaterial(WireColor);
    }

    // Inputs being the RwAzElBox and RwEleData, we create a mesh that represents the tile.
    // Context being the RwtoGe scaling factor.

    public void CreateMesh()
    {
        LineMesh3D = new() { Name = "LineMesh" };
        AddChild(LineMesh3D);

        // Setup the center position for the mesh and later positioning
        FssLLBox rwLLBox = TileCode.LLBox;
        //RwLLACenter = new FssLLAPoint() { LatDegs = rwLLBox.MidLatDegs, LonDegs = rwLLBox.MinLonDegs, RadiusM = FssPosConsts.EarthRadiusM};

        // Define zero longitude center, so we can create the tile from relative (not absolute) angles and
        // more intuitively rotate the tile.
        FssLLAPoint rwLLAZeroLonCenter = new FssLLAPoint() {
            LatDegs = RwLLACenter.LatDegs,
            LonDegs = 0,
            RadiusM = FssPosConsts.EarthRadiusM};
        FssXYZPoint rwXYZZeroLonCenter = rwLLAZeroLonCenter.ToXYZ();

        // Setup the loop control values
        int          pointCountLon   = RwEleData.Width;
        int          pointCountLat   = RwEleData.Height;
        List<double> lonZeroListRads = FssValueUtils.CreateRangeList(pointCountLon, -rwLLBox.HalfDeltaLonRads, rwLLBox.HalfDeltaLonRads); // Relative azimuth
        List<double> latListRads     = FssValueUtils.CreateRangeList(pointCountLat,  rwLLBox.MinLatRads,       rwLLBox.MaxLatRads);
        Vector3[,]   v3Data          = new Vector3[pointCountLon, pointCountLat];

        for (int i = 0; i < pointCountLon; i++)
        {
            for (int j = 0; j < pointCountLat; j++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double      lonRads       = lonZeroListRads[i];
                double      latRads       = latListRads[j];
                double      ele           = RwLLACenter.RadiusM + RwEleData[i, j];
                FssLLAPoint rwLLAPointPos = new FssLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = ele };
                FssXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // Find the offset from the tile center.
                FssXYZPoint rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // Convert the Real-World position to the Game Engine position.
                v3Data[i, j] = new Vector3(
                    (float)(rwXYZCenterOffset.X * FssZeroOffset.RwToGeDistMultiplier),
                    (float)(rwXYZCenterOffset.Y * FssZeroOffset.RwToGeDistMultiplier),
                    (float)(rwXYZCenterOffset.Z * FssZeroOffset.RwToGeDistMultiplier));
            }
        }

        // Add the grid to the LineMesh
        LineMesh3D.AddSurface(v3Data, FssColorUtil.Colors["Red"]);

        // Create the game-engine mesh from the V3s
        FssMeshBuilder meshBuilder = new();
        meshBuilder.AddSurface(v3Data, UVx, UVy, false);

        // Build the mesh data and add it to the node
        MeshInstance3D tileMeshInstance   = new MeshInstance3D() { Name = "tileMesh" };
        tileMeshInstance.Mesh             = meshBuilder.BuildWithUV("Surface");
        tileMeshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        //tileMeshInstance.MaterialOverride = FssMaterialFactory.WaterMaterial();

        AddChild(tileMeshInstance);

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwLLACenter.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.
        tileMeshInstance.Rotation = new Vector3(0, rotAz, 0);
        LineMesh3D.Rotation       = new Vector3(0, rotAz, 0);

        // Add a simple sphere marker for the tile center
        // Create a new SphereMesh instance with radius and height set
        {
            float sphereRad = 10f;
            var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f } };
            AddChild(sphereInstance);
        }

    }

    // --------------------------------------------------------------------------------------------

    // Creating the mesh can be done in a bckground thread, but it needs to be on the main thread
    // when we attach it to the scene tree.
    private void AttachMesh()
    {

    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation
    // --------------------------------------------------------------------------------------------

    private void SourceTileElevation()
    {
        // Load the elevation data
        FssAppFactory.Instance.EleManager.RequestTile(TileCode);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Image
    // --------------------------------------------------------------------------------------------

    private void SourceTileImage()
    {
        // Request the image to be loaded in the background - non-blocking.
        // RegisteredTextureName

        // FssTextureLoader? TL = FssTextureLoader.Instance;
        // if (TL != null)
        // {
        //     ImageTexture? tex = TL.LoadTextureDirect(Filepaths.ImageFilepath);

        //     if (tex != null)
        //     {
        //         TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

        //         if (TileMaterial != null)
        //         {
        //             ImageDone = true;
        //         }
        //     }
        // }

        // // Setup the UV Box - new image, so a full 0,0 -> 1,1 range
        // UVBox = new FssUvBoxDropEdgeTile(FssUvBoxDropEdgeTile.UVTopLeft, FssUvBoxDropEdgeTile.UVBottomRight);
    }


    private void SubsampleParentTileImage()
    {
        // if (ParentTile != null)
        // {
        //     FssTextureLoader? TL = FssTextureLoader.Instance;

        //     Filepaths.ImageFilepath   = ParentTile.Filepaths.ImageFilepath;
        //     Filepaths.ImageFileExists = ParentTile.Filepaths.ImageFileExists;

        //     TileMaterial = TL.GetMaterialWithTexture(Filepaths.ImageFilepath);

        //     if (TileMaterial != null)
        //         ImageDone = true;

        //     // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
        //     // Get the grid position of this tile in its parent (eg [1x,2y] in a 5x5 grid).
        //     UVBox = new FssUvBoxDropEdgeTile(ParentTile.UVBox, TileCode.GridPos);

        // }
        // else
        // {
        //     // Subsmapling, but no parent. Setup a default.
        //     UVBox = FssUvBoxDropEdgeTile.Default(TileSizePointsPerLvl[TileCode.MapLvl], TileSizePointsPerLvl[TileCode.MapLvl]);
        // }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Create Elevation
    // --------------------------------------------------------------------------------------------




}


