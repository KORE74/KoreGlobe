using System;
using System.Collections.Generic;
using System.IO;

// FssTileNodeFilepaths class to fugire out and store the filepaths of assets for a map tile.

public class FssTileNodeFilepaths
{
    public string EleFilepath { get; }
    public string MeshFilepath { get; }
    public string ImageFilepath { get; }

    public bool EleFileExists { get; } = false;
    public bool MeshFileExists  { get; } = false;
    public bool ImageFileExists{ get; } = false;

    public FssTileNodeFilepaths(FssMapTileCode tileCode)
    {
        // Get the tile code name
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        var config = FssCentralConfig.Instance;
        string externalRootPath = config.GetParameter("MapRootPath", "");
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        ImageFilepath = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");
        EleFilepath   = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.asc");
        MeshFilepath  = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Mesh_{tileCodeName}.mesh");


        // Check if the files exist
        EleFileExists = File.Exists(EleFilepath);
        MeshFileExists = File.Exists(MeshFilepath);
        ImageFileExists = File.Exists(ImageFilepath);

    }



}


