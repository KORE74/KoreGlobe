using System;

using Godot;

#nullable enable

public static class FssMapTileNodeOperations
{
    // Scavenge a texture from the parent map tile (for when we can't find our own) and subdivide it
    // for this tile.

    public static ImageTexture? ScavengeTextureFromParent(FssMapTileNode parentTileNode, FssMapTileCode childtileCode)
    {
        // First get the texture at all, from the parent node material
        ImageTexture parentTex = parentTileNode.TerrainTexture;

        if (parentTex == null)
            return null;

        // Now we need to subdivide the texture into the child tile
        // First, get the UV coordinates for the parent tile
        // public int HorizCount => NumTilesHorizPerLvl[MapLvl];
        // public int VertCount  => NumTilesVertPerLvl[MapLvl];

        // public int HorizIndex => CodeList[MapLvl].LonIndex;
        // public int VertIndex  => CodeList[MapLvl].LatIndex;



        return null;
    }

    // public static string PathForMapLvl(int lvl)
    // {
    //     var config = FssCentralConfig.Instance;
    //     string externalRootPath = config.GetParameter("MapRootPath", "");
        
    //     string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);

    //     if (lvl != 0)
    //         externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

    //     return externalMapLvlFilePath;
    // }

    // // Usage: FssMapFileOperations.CreateMapLibraryDirectories();
    // public static void CreateMapLibraryDirectories()
    // {
    //     var config = FssCentralConfig.Instance;
    //     string externalRootPath = config.GetParameter("MapRootPath", "");

    //     for (int currLvl = 0; currLvl < 3; currLvl++)
    //     {
    //         string externalMapLvlFilePath = Path.Combine(externalRootPath, FssMapTileCode.PathPerLvl[currLvl]);

    //         // Create any top level dirs that don't exist
    //         if (!Directory.Exists(externalMapLvlFilePath))
    //             Directory.CreateDirectory(externalMapLvlFilePath);

    //         // Create the Lvl1 subdirectories
    //         if (currLvl == 1)
    //         {
    //             int numTilesHoriz = FssMapTileCode.NumTilesHorizPerLvl[currLvl];
    //             int numTilesVert  = FssMapTileCode.NumTilesVertPerLvl[currLvl];

    //             for (int lonId = 0; lonId < numTilesHoriz; lonId++)
    //             {
    //                 for (int latId = 0; latId < numTilesVert; latId++)
    //                 {
    //                     string tileCode = FssMapTileCode.CodeForIndex(latId, lonId);
    //                     string tileDir  = Path.Combine(externalMapLvlFilePath, tileCode);

    //                     // Create any top level dirs that don't exist
    //                     if (!Directory.Exists(tileDir))
    //                         Directory.CreateDirectory(tileDir);
    //                 }
    //             }
    //         }
    //     }


    // }

}
