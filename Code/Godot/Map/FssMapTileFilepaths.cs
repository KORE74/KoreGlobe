using System;
using System.Collections.Generic;
using System.IO;

// FssMapTileFilepaths class to fugire out and store the filepaths of assets for a map tile.

public class FssMapTileFilepaths
{
    public string EleFilepath    { get; set; }
    public string EleArrFilepath { get; set; }
    public string MeshFilepath   { get; set; }
    public string ImageFilepath  { get; set; }
    public string KTX2Filepath   { get; set; }

    public bool EleFileExists    { get; set; } = false;
    public bool EleArrFileExists { get; set; } = false;
    public bool MeshFileExists   { get; set; } = false;
    public bool ImageFileExists  { get; set; } = false;
    public bool KTX2FileExists   { get; set; } = false;

    public FssMapTileFilepaths(FssMapTileCode tileCode)
    {
        // Get the tile code name
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        var config = FssCentralConfig.Instance;
        string externalRootPath = FssMapManager.MapRootPath;
        string externalMapLvlFilePath = FssFileOperations.JoinPaths(externalRootPath, FssMapTileCode.PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = FssFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        EleFilepath      = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.asc");
        EleArrFilepath   = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.arr");
        MeshFilepath     = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Mesh_{tileCodeName}.mesh");
        ImageFilepath    = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");
        KTX2Filepath     = FssFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.ktx2");

        // Check if the files exist
        EleFileExists    = File.Exists(EleFilepath);
        EleArrFileExists = File.Exists(EleArrFilepath);
        MeshFileExists   = File.Exists(MeshFilepath);
        ImageFileExists  = File.Exists(ImageFilepath);
        KTX2FileExists   = File.Exists(KTX2Filepath);
    }
}


