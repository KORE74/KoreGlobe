using System;
using System.Collections.Generic;
using System.IO;

// FssMapTileFilepaths class to fugire out and store the filepaths of assets for a map tile.

public class FssMapTileFilepaths
{
    public string EleFilepath       { get; set; }
    public string EleArrFilepath    { get; set; }
    public string ImagePngFilepath  { get; set; }
    public string ImageWebpFilepath { get; set; }

    public bool EleFileExists       { get; set; } = false;
    public bool EleArrFileExists    { get; set; } = false;
    public bool ImagePngFileExists  { get; set; } = false;
    public bool ImageWebpFileExists { get; set; } = false;


    public FssMapTileFilepaths(FssMapTileCode tileCode)
    {
        string externalRootPath = FssFileOperations.StandardizePath( FssCentralConfig.Instance.GetParam<string>("MapRootPath") );

        // Get the tile code name
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        EleFilepath         = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.asc");
        EleArrFilepath      = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.arr");
        ImagePngFilepath    = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");
        ImageWebpFilepath   = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.webp");

        // Check if the files exist
        EleFileExists       = File.Exists(EleFilepath);
        EleArrFileExists    = File.Exists(EleArrFilepath);
        ImagePngFileExists  = File.Exists(ImagePngFilepath);
        ImageWebpFileExists = File.Exists(ImageWebpFilepath);
    }
}


