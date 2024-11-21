using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

// Overarching class to manage elevation activities, including background tasks around the loading and creating.

public class FssElevationManager
{
    // Consume general Arc ASCII grid files and output elevations for a lat/lon.
    private FssElevationPrepSystem ElePrep = new();

    // Hold map tiles for use in display. Load/save tiles for caching work.
    private FssElevationTileSystem EleTiles = new();

    // Semaphore to limit the number of concurrent tasks.
    private static readonly SemaphoreSlim semaphore = new(10); // Adjust the number as needed.

    // --------------------------------------------------------------------------------------------
    // MARK: Prep
    // --------------------------------------------------------------------------------------------

    public void LoadArcASCIIGridFile(string filename, FssLLBox llBox)
    {
        Task.Run(async() =>
        {
            await semaphore.WaitAsync(); // Wait for an available slot
            try
            {
                FssElevationPrepTile? newTile = ElePrep.ArcASCIIToTile(filename, llBox);

                if (newTile != null)
                {
                    FssCentralLog.AddEntry($"Failed to load Arc ASCII Grid: {filename} // {llBox}");
                }
            }
            finally
            {
                semaphore.Release(); // Release the slot
            }
        });
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Tiles
    // --------------------------------------------------------------------------------------------

    // Non-blocking call to start the tile preparation activities, that will require nested looping through
    // a large number of LL pairs.
    public void PrepTile(FssMapTileCode tileCode, bool saveFile)
    {
        Task.Run(async() =>
        {
            //await semaphore.WaitAsync(); // Wait for an available slot
            try
            {
                // Setup the tile defining values
                FssLLBox llBox      = tileCode.LLBox;
                int      tileResLat = 100;
                int      tileResLon = FssElevationPrepTile.GetLongitudeResolution(tileResLat, llBox.CenterPoint.LatDegs);

                // Big operation: get the 2D array of elevations, which itself may require interpolation across nested arrays.
                FssFloat2DArray eleData = FssElevationTileSystem.PrepTileData(ElePrep, llBox, tileResLat, tileResLon);

                // Create the new tile and add it to the collection
                FssElevationTile newTile = new() {
                    TileCode      = tileCode,
                    ElevationData = eleData,
                    LLBox         = llBox
                };

                if (saveFile)
                {
                    FssMapTileFilepaths filepaths = new(tileCode);
                    try
                    {
                        FssElevationTileIO.WriteToTextFile(newTile, filepaths.EleArrFilepath);
                    }
                    catch (Exception ex)
                    {
                        FssCentralLog.AddEntry($"Failed to write: {filepaths.EleArrFilepath}. Exception: {ex.Message}");
                    }
                }

                EleTiles.AddTile(newTile);
            }
            catch(Exception ex)
            {
                FssCentralLog.AddEntry($"{ex.Message}");
            }
            //finally
            //{
                //semaphore.Release(); // Release the slot
            //}
        });
    }

    // --------------------------------------------------------------------------------------------

    public void LoadTile(FssMapTileCode tileCode)
    {
        Task.Run(async() =>
        {
            await semaphore.WaitAsync(); // Wait for an available slot
            try
            {
                // Avoid the workload if tile already exists
                if (HasTile(tileCode))
                    return;

                // Get the filepaths
                FssMapTileFilepaths filepaths = new FssMapTileFilepaths(tileCode);

                LoadTileFile(filepaths.EleArrFilepath);
            }
            finally
            {
                semaphore.Release(); // Release the slot
            }
        });
    }

    private void LoadTileFile(string filepath)
    {
        try
        {
            FssElevationTile? newTile = FssElevationTileIO.ReadFromTextFile(filepath);
            if (newTile != null)
                EleTiles.AddTile(newTile!);
            else
                FssCentralLog.AddEntry($"Failed to load file: {filepath}");
        }
        catch (Exception ex)
        {
            FssCentralLog.AddEntry($"EXCEPTION: Failed to load file: {filepath}. Exception: {ex.Message}");
        }
    }

    public bool             HasTile(FssMapTileCode tileCode)    => EleTiles.HasTile(tileCode.TileCode);
    public FssElevationTile GetTile(FssMapTileCode tileCode)    => EleTiles.GetTile(tileCode.TileCode); // Returns a default tile when not found, use HasTile first
    public void             DeleteTile(FssMapTileCode tileCode) => EleTiles.DeleteTile(tileCode.TileCode);
    public void             DeleteAllTiles()                    => EleTiles.DeleteAllTiles();

    // --------------------------------------------------------------------------------------------
    // MARK: High-Level task
    // --------------------------------------------------------------------------------------------

    // Function called by a tile that wants the elevation tile for it, and doesn't care about the complexity required to acheive that.
    // This function exists to perform whater tasks are required to supply that data, through the HasTile() and GetTile() calls.

    public void RequestTile(FssMapTileCode tileCode)
    {
        Task.Run(async() =>
        {
            await semaphore.WaitAsync(); // Wait for an available slot
            try
            {
                // Avoid the workload if tile already exists
                if (HasTile(tileCode))
                    return;

                // Get the filepaths
                FssMapTileFilepaths filepaths = new FssMapTileFilepaths(tileCode);

                // If a saved tile exists, load that.
                if (filepaths.EleArrFileExists)
                {
                    LoadTileFile(filepaths.EleArrFilepath);
                    return;
                }

                // No pre-existing tile, so get one from the Prep area
                PrepTile(tileCode, true);
            }
            finally
            {
                semaphore.Release(); // Release the slot
            }
        });
    }

}

