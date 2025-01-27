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

    // Create rules:
    // - Need to have the elevation file to gate the creation of a child tile, or it blocks the
    //   creation of the while set. No more exprapolation of elevation data.
    // - Will use the tile's own image, the parent tile's image if it exists, or a default image if it doesn't.

    private async void BackgroundTileCreation(FssMapTileCode tileCode)
    {
        // GD.Print($"Starting Create: {tileCode}");
        try
        {
            // Starting: Set the flags that will be used later to determine activity around the tile while we construct it.
            ConstructionComplete = false;
            ActiveState          = false;
            MeshCreated          = false;

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // Setup some basic elements of the tile ahead of the main elevation and image loading.
            Filepaths = new FssMapTileFilepaths(TileCode); // Figure out the file paths for the tile

            // Load the elevation data
            //FssAppFactory.Instance.EleManager.RequestTile(tileCode);

            // Source the tile image
            SourceTileImage();
            SourceTileElevation();

            FssCentralLog.AddEntry($"Tile {TileCode} background stage 1.");

            // Yield until (ImageSourced && ElevationSourced)
            // while (!ImageSourced || !ElevationSourced)
            //     await Task.Delay(100);

            // Create the mesh
            //CreateMaterials();
            await Task.Yield(); // Yield, be a good citizen

            // Set the flag for the Process() function to pick up and conclude any
            // construction tasks on the main thread.
            BackgroundCreateCompleted = true;

            // Log the completion
            FssCentralLog.AddEntry($"Tile {TileCode} background creation complete.");
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        // GD.Print($"Ending Create: {tileCode}");
    }

    // --------------------------------------------------------------------------------------------

    public void ProgressCreation()
    {
        // GD.Print($"Progressing Create: {TileCode}");

        // Start the creation of the mesh
        CreateMesh();
        //AttachMesh();
        ConstructionComplete = true;

        AddDebugOriginSphere();
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
        List<double> lonZeroListRads = FssValueUtils.CreateRangeList(pointCountLon,  rwLLBox.HalfDeltaLonRads, -rwLLBox.HalfDeltaLonRads); // Relative azimuth
        List<double> latListRads     = FssValueUtils.CreateRangeList(pointCountLat,  rwLLBox.MaxLatRads,        rwLLBox.MinLatRads);
        Vector3[,]   v3Data          = new Vector3[pointCountLon, pointCountLat];

        for (int ix = 0; ix < pointCountLon; ix++)
        {
            for (int jy = 0; jy < pointCountLat; jy++)
            {
                // Find the Real-World (RW) position for each point in the mesh.
                double      lonRads       = lonZeroListRads[ix];
                double      latRads       = latListRads[jy];
                double      ele           = RwLLACenter.RadiusM + RwEleData[ix, jy];
                FssLLAPoint rwLLAPointPos = new FssLLAPoint() { LatRads = latRads, LonRads = lonRads, RadiusM = ele };
                FssXYZPoint rwXYZPointPos = rwLLAPointPos.ToXYZ();

                // Find the offset from the tile center.
                FssXYZPoint rwXYZCenterOffset = rwXYZZeroLonCenter.XYZTo(rwXYZPointPos);

                // Convert the Real-World position to the Game Engine position.
                v3Data[ix, jy] = new Vector3(
                    (float)(rwXYZCenterOffset.X * FssZeroOffset.RwToGeDistMultiplier),
                    (float)(rwXYZCenterOffset.Y * FssZeroOffset.RwToGeDistMultiplier),
                    (float)(rwXYZCenterOffset.Z * FssZeroOffset.RwToGeDistMultiplier));
            }
        }

        // Add the grid to the LineMesh
        LineMesh3D.AddSurface(v3Data, FssColorUtil.Colors["Red"], FssColorUtil.Colors["Yellow"]);

        // Create the game-engine mesh from the V3s
        FssMeshBuilder meshBuilder = new();
        meshBuilder.AddSurface(v3Data, UVx, UVy, false);

        // Build the mesh data and add it to the node
        TileMeshInstance        = new MeshInstance3D() { Name = "tileMesh" };
        TileMeshInstance.Mesh   = meshBuilder.BuildWithUV("Surface");
        //tileMeshInstance.MaterialOverride = FssMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
        TileMeshInstance.MaterialOverride = SurfaceMat;

        AddChild(TileMeshInstance);

        // Rotate each tile into its position - The zero node only translates, so this is fixed after creation
        float rotAz = (float)(RwLLACenter.LonRads); // We created the tile with relative azimuth, so apply the absolute value to orient it to its longitude.
        TileMeshInstance.Rotation = new Vector3(0, rotAz, 0);
        LineMesh3D.Rotation       = new Vector3(0, rotAz, 0);

        // Set the visibility false, later functions will assign the correct visibility rules.
        TileMeshInstance.Visible = false;
        LineMesh3D.Visible       = false;
        ActiveState              = false;
    }

    // Add a simple sphere marker for the tile center
    // Create a new SphereMesh instance with radius and height set

    private void AddDebugOriginSphere()
    {
        float sphereRad = 1f;
        var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f } };
        AddChild(sphereInstance);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Image
    // --------------------------------------------------------------------------------------------

    // We either load an image for this tile, or take the parent tile material (or a default material on error).

    private void SourceTileImage()
    {
        // Load the image if we have it, or take the parent file if it exists, or leave the image blank.
        if (Filepaths.ImageWebpFileExists)
        {
            SurfaceMat = FssGodotImageOperations.LoadToMaterial(Filepaths.ImageWebpFilepath);
            UVBox      = FssUVBoxDropEdgeTile.FullImage();
        }
        else if (ParentTile != null)
        {
            SurfaceMat = ParentTile!.SurfaceMat;

            // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
            UVBox = new FssUVBoxDropEdgeTile(ParentTile!.UVBox, TileCode.GridPos);
        }

        // If we still don't have the image, we'll setup a default Material and UVBox.
        if (SurfaceMat == null)
        {
            SurfaceMat = FssMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
            UVBox      = FssUVBoxDropEdgeTile.FullImage();
        }

        GD.Print($"TileCode: {TileCode} // UVBox: {UVBox.TopLeft} {UVBox.BottomRight}");

        // Turn the UVBox into a UV Ranges
        UVx = UVBox.UVXRange();
        UVy = UVBox.UVYRange();
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Elevation
    // --------------------------------------------------------------------------------------------

    private void SourceTileElevation()
    {
        RwEleData = new FssFloat2DArray(20, 20);
        //return;

        // // If we have the elevation data, we'll use it, otherwise we'll take the parent's data.
        // if (Filepaths.EleArrFileExists)
        // {
        //     // Load the elevation data
        //     RwEleData = FssFloat2DArrayIO.LoadFromCSVFile(Filepaths.EleArrFilepath);
        // }
        // else

        if (Filepaths.EleFileExists)
        {
            // Load the elevation data
            RwEleData = FssFloat2DArrayIO.LoadFromArcASIIGridFile(Filepaths.EleFilepath);
        }
        else
        {
            // for now, just create an ampty array
            CreationRejected = true;
        }
    }



}


