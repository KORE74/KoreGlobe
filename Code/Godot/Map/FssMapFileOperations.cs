using System;

public static class FssMapFileOperations
{


    string PathForMapLvl(int lvl)
    {
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        return externalMapLvlFilePath;
    }


    private void CreateMapLibaryDirectories()
    {
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");

        for (int currLvl = 0; currLvl < 3; currLvl++)
        {
            string externalMapLvlFilePath = Path.Combine(externalRootPath, FssMapTileCode.PathPerLvl[currLvl]);

            // Create any top level dirs that don't exist
            if (!Directory.Exists(externalMapLvlFilePath))
                Directory.CreateDirectory(externalMapLvlFilePath);

            // Create the Lvl1 subdirectories
            if (currLvl == 1)
            {
                int numTilesHoriz = FssMapTileCode.NumTilesHorizPerLvl[currLvl];
                int numTilesVert  = FssMapTileCode.NumTilesVertPerLvl[currLvl];

                for (int lonId = 0; lonId < numTilesHoriz; lonId++)
                {
                    for (int latId = 0; latId < numTilesVert; latId++)
                    {
                        string tileCode = FssMapTileCode.CodeForIndex(latId, lonId);
                        string tileDir  = Path.Combine(externalMapLvlFilePath, tileCode);

                        // Create any top level dirs that don't exist
                        if (!Directory.Exists(tileDir))
                            Directory.CreateDirectory(tileDir);
                    }
                }
            }
        }


    }

}