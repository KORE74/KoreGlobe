

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Godot;

// Essential information around the tile image and potential sub-area within it.
// Logic:
// - Collection of time expiring images
// - Collection of tile inforation linking to an image and a UVBox.
// - Access always checks that the image has been maintained, prunes tile info when image not found.

public class TileImageInfo
{
    public FssMapTileCode TileCode;
    public Texture2D      TileImage;
    public FssUVBox       UVBox;
}

public partial class FssTileImageManager : Node
{
    // Cache to hold the loading tasks, to avoid duplicating a running task
    private ConcurrentDictionary<string, Task> LoadingTasks = new ConcurrentDictionary<string, Task>();

    // List of the native images
    private ConcurrentDictionary<string, Texture> TextureCache = new();

    // Time for textures to keep alive (in seconds).
    private const int KeepAliveTime = 10;
    private ConcurrentDictionary<string, int> LastAccessTimeList = new ConcurrentDictionary<string, int>();

    // List of the tile information that consumes the images
    private ConcurrentDictionary<FssMapTileCode, TileImageInfo> tileCache = new();


    public void RequestTileImage(FssMapTileCode tilecode)
    {
        // Get the filepaths
        FssMapTileFilepaths filepaths = new FssMapTileFilepaths(tilecode);

        // If the actual image exists, we don't need to search for a parent tile image etc
        if (filepaths.ImageFileExists)
        {
            // if the image is already loaded
            if (HasImage(filepaths.ImageFilepath))
            {

            }
        }
        // if the file exists, check its loaded.


    }

    // --------------------------------------------------------------------------------------------
    // MARK: Image Management
    // --------------------------------------------------------------------------------------------

    // Checks if the image has been loaded and is available in the cache.
    public bool HasImage(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessTimeList[path] = FssCentralTime.RuntimeIntSecs;
            return true;
        }
        return false;
    }

    // Keeps a texture alive by updating its last access time.
    public void KeepAlive(string path)
    {
        if (TextureCache.ContainsKey(path))
        {
            LastAccessTimeList[path] = FssCentralTime.RuntimeIntSecs;
        }
    }

    // --------------------------------------------------------------------------------------------
    // MARK: Expired Tile Deletion
    // --------------------------------------------------------------------------------------------


}