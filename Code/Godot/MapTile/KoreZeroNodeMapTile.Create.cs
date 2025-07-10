using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Godot;

#nullable enable

// ZeroNode map tile:
// - A tile placed at an offset from the zeronode.
// - Orientation is zero, with the onus on the central point to angle it according to its LL.

public partial class KoreZeroNodeMapTile : Node3D
{
    // --------------------------------------------------------------------------------------------
    // MARK: Background
    // --------------------------------------------------------------------------------------------

    // Create rules:
    // - Need to have the elevation file to gate the creation of a child tile, or it blocks the
    //   creation of the while set. No more exprapolation of elevation data.
    // - Will use the tile's own image, the parent tile's image if it exists, or a default image if it doesn't.

    private async Task BackgroundTileCreation(KoreMapTileCode tileCode)
    {
        // GD.Print($"Starting Create: {tileCode}");
        try
        {
            // Starting: Set the flags that will be used later to determine activity around the tile while we construct it.
            ConstructionComplete = false;
            ActiveVisibility     = false;

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // ----------------------------
            // File IO and Tile center

            // Setup some basic elements of the tile ahead of the main elevation and image loading.
            Filepaths = new KoreMapTileFilepaths(TileCode); // Figure out the file paths for the tile

            // Setup the tile center shortcuts
            RwTileLLBox     = KoreMapTileCode.LLBoxForCode(TileCode);
            RwTileCenterLLA = new KoreLLAPoint(RwTileLLBox.CenterPoint);
            RwTileCenterXYZ = RwTileCenterLLA.ToXYZ();

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // ----------------------------
            // Default Materials

            // Default everything, in case we fall through the logic, the objects are not null
            UVBox        = new KoreUVBoxDropEdgeTile(KoreUVBoxDropEdgeTile.UVTopLeft, KoreUVBoxDropEdgeTile.UVBottomRight);
            TileMaterial = KoreMaterialFactory.SimpleColoredMaterial(new Color(0.5f, 0.5f, 0f, 1f));
            TileEleData  = new KoreFloat2DArray(10, 10);

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // ----------------------------
            // Image

            // Source the tile image
            SourceTileImage();

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            // ----------------------------
            // Elevation

            if (Filepaths.EleArrFileExists)
            {
                LoadTileEleArr();
            }

            // Pause the thread, being a good citizen with lots of tasks around.
            await Task.Yield();

            CreateMeshPoints();

            BackgroundConstructionComplete = true;
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        // GD.Print($"Ending Create: {tileCode}");
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Main Thread
    // --------------------------------------------------------------------------------------------

    // A main thread function that is called a few times to complete the tile creation in stages.
    // - Grabs a limited ActionCounter/Token to throttle the workload.

    private void MainThreadFinalizeCreation()
    {
        //GD.Print($"MainThreadFinalizeCreation: {TileCode}");

        switch (ConstructionStage)
        {
            case 0:
                if (GrabbedActionCounter())
                {
                    CreateMeshTileSurface();
                    ConstructionStage = 1;
                }
                break;

            case 1:
                if (GrabbedActionCounter())
                {
                    CreateMeshWireFrame();
                    ConstructionStage = 2;
                }
                break;

            case 2:
                if (GrabbedActionCounter())
                {
                    // Create the label
                    LabelTile(TileCode);
                    ConstructionStage = 3;
                }
                break;

            default:
                ConstructionComplete = true;

                LocateTile();

                if (TileCode.MapLvl == 0)
                {
                    SetVisibility(true);
                    ActiveVisibility = true;

                    // Add a delay so we don't look to assess visibility of all tiles immediately.
                    UIUpdateTimer = KoreCentralTime.RuntimeSecs + (RandomLoopList.GetNext() / 2f);
                }

                break;
        }
    }

    // --------------------------------------------------------------------------------------------

    private void AddDebugOriginSphere()
    {
        float sphereRad = 1f / (float)((TileCode.MapLvl * 4) + 1);
        var sphereInstance = new MeshInstance3D { Name = "TestSphere", Mesh = new SphereMesh { Radius = sphereRad, Height = sphereRad*2f } };
        AddChild(sphereInstance);

        GEElements.Add(sphereInstance);
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Image
    // --------------------------------------------------------------------------------------------

    // We either load an image for this tile, or take the parent tile material (or a default material on error).

    private void SourceTileImage()
    {
        KoreTextureLoader? TL = KoreTextureLoader.Instance;

        // Load the image if we have it, or take the parent file if it exists, or leave the image blank.
        if (Filepaths.WebpFileExists)
        {
            TileMaterial = TL.CreateDirectWebpMaterial(Filepaths.WebpFilepath);
            UVBox        = KoreUVBoxDropEdgeTile.FullImage();
            TileOwnsTexture = true; // We own the texture, so we can delete it when done.
        }
        else if (Filepaths.ImageFileExists)
        {
            // Convert the image (typically from an import operation).
            KoreWebpConverter.CompressTexture(Filepaths.ImageFilepath, Filepaths.WebpFilepath);

            // repeat the Webp import process
            TileMaterial = TL.CreateDirectWebpMaterial(Filepaths.WebpFilepath);
            UVBox        = KoreUVBoxDropEdgeTile.FullImage();
            TileOwnsTexture = true; // We own the texture, so we can delete it when done.
        }
        else if (ParentTile != null)
        {
            TileMaterial = ParentTile!.TileMaterial;

            // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
            UVBox = new KoreUVBoxDropEdgeTile(ParentTile!.UVBox, TileCode.GridPos);
        }

        // If we still don't have the image, we'll setup a default Material and UVBox.
        if (TileMaterial == null)
        {
            TileMaterial = KoreMaterialFactory.SimpleColoredMaterial(new Color(1.0f, 0.0f, 1.0f, 1.0f));
            UVBox        = KoreUVBoxDropEdgeTile.FullImage();
        }

        // GD.Print($"TileCode: {TileCode} // UVBox: {UVBox.TopLeft} {UVBox.BottomRight}");

        // Turn the UVBox into a UV Ranges
        UVx = UVBox.UVRangeX;
        UVy = UVBox.UVRangeY;
    }


    // --------------------------------------------------------------------------------------------
    // MARK: Label
    // --------------------------------------------------------------------------------------------

    // Add a label to the middle of the tile, oriented to be flat to the surface

    private void LabelTile(KoreMapTileCode tileCode)
    {
        // Get the tile center forthe label position
        KoreLLPoint posLL = new KoreLLPoint(RwTileCenterLLA);

        // Decimal points on the lat/long per tile level. Avoid superfluous 0's on the end
        string format = "F" + dpPerLvl[TileCode.MapLvl];

        // Create the tile code string
        string tileCodeBoxStr =
            $"...[{RwTileLLBox.MaxLatDegs.ToString(format)}, {RwTileLLBox.MaxLonDegs.ToString(format)}]\n" +
            $"[{RwTileLLBox.MinLatDegs.ToString(format)}, {RwTileLLBox.MinLonDegs.ToString(format)}]...";

        // Determine the size and create the label
        float KPixelSize = LabelSizePerLvl[tileCode.MapLvl];
        TileCodeLabel = KoreLabel3DFactory.CreateLabel($"{tileCode.ToString()}\n{tileCodeBoxStr}", KPixelSize);

        AddChild(TileCodeLabel);
        TileCodeLabel.Visible = false;

        // Determine the GameEngine positions and orientation
        Godot.Vector3 v3VectN     = (TileLabelOffsetN     - TileLabelOffset).Normalized();
        Godot.Vector3 v3VectBelow = (TileLabelOffsetBelow - TileLabelOffset).Normalized();

        TileCodeLabel.Position = TileLabelOffset;
        TileCodeLabel.LookAt(v3VectN, v3VectBelow);

        // rotate to consider the lat long of the tile
        float rotAz = (float)(RwTileCenterLLA.LonRads) + (float)KoreValueUtils.DegsToRads(90);
        float rotEl = -1f * (float)(RwTileCenterLLA.LatRads); // We created the tile with relative elevation, so apply the absolute value to orient it to its latitude.
        TileCodeLabel.Rotation  = new Vector3(rotEl, rotAz, 0);
    }


}