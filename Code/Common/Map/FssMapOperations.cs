using System;

public static class FssMapOperations
{
    public static void CreateBaseDirectories(string rootDir)
    {
        if (string.IsNullOrEmpty(rootDir))
            return;

        // Create the root directory
        if (!System.IO.Directory.Exists(rootDir))
            System.IO.Directory.CreateDirectory(rootDir);

        foreach (string currType in FssMap.ConstDataTypes)
        {
            string dataTypePath = System.IO.Path.Combine(rootDir, currType);
            if (!System.IO.Directory.Exists(dataTypePath))
                System.IO.Directory.CreateDirectory(dataTypePath);

            // Create the sub-directories
            foreach (string subLvl in FssMap.ConstPathPerLvl)
            {
                string lvlPath = System.IO.Path.Combine(dataTypePath, subLvl);
                if (!System.IO.Directory.Exists(lvlPath))
                    System.IO.Directory.CreateDirectory(lvlPath);
            }
        }
    }

    // --------------------------------------------------------------------------------------------

    public static void CreateTileEle(string rootDir, string tileCode, int lonRes)
    {
        // Create the tile directory
        string tileDir = System.IO.Path.Combine(rootDir, tileCode);
        if (!System.IO.Directory.Exists(tileDir))
            System.IO.Directory.CreateDirectory(tileDir);

        // Create the tile data directories
        foreach (string currType in FssMap.ConstDataTypes)
        {
            string dataTypePath = System.IO.Path.Combine(tileDir, currType);
            if (!System.IO.Directory.Exists(dataTypePath))
                System.IO.Directory.CreateDirectory(dataTypePath);
        }
    }

}

/*
public class FssMapOperations
{
    int MapLvlNum;
    int TileCountLong;
    int TileCountLat;

    FssMapTile[,] TileArray;

    public FssMapLayer(int mapLvlNum)
    {
        MapLvlNum     = mapLvlNum;

        TileCountLong = FssMapConsts.numTilesHorizPerLvl[MapLvlNum];
        TileCountLat  = FssMapConsts.numTilesVertPerLvl[MapLvlNum];
    }

    public void CreateTileArray()
    {
        double tileWidth = 360.0 / TileCountLong;
        double tileHeight = 180.0 / TileCountLat;

        TileArray = new FssMapTile[TileCountLong, TileCountLat];
        for (int i = 0; i < TileCountLong; i++)
        {
            for (int j = 0; j < TileCountLat; j++)
            {
                FssLLBox box = new FssLLBox()
                {
                    MinLatDegs = FssMapConsts.minLatDegs + j * tileHeight,
                    MinLonDegs = FssMapConsts.minLonDegs + i * tileWidth,
                    MaxLatDegs = FssMapConsts.minLatDegs + (j + 1) * tileHeight,
                    MaxLonDegs = FssMapConsts.minLonDegs + (i + 1) * tileWidth
                };
                TileArray[i, j] = new FssMapTile(box);
            }
        }
    }

    public void LoadTile(FssMapTileCode tileCode, string layerEleDir)
    {
        // Basic validation
        if (tileCode.MapLvl != MapLvlNum)
            return;

        // Turn the tile code string into a file path
        string tilePath = System.IO.Path.Combine(layerEleDir, tileCode.FullCode + ".ele");

    }

    public void SaveTile()
    {

    }

    public void ExportTile()
    {

    }

    public void SetValue(FssLL pos, float value)
    {

    }
}



*/