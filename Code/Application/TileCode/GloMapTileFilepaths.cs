using System;
using System.Collections.Generic;
using System.IO;

// GloMapTileFilepaths class to fugire out and store the filepaths of assets for a map tile.

public class GloMapTileFilepaths
{
    public string EleFilepath      { get; set; }
    public string EleArrFilepath   { get; set; }
    public string MeshFilepath     { get; set; }
    public string ImageFilepath    { get; set; }
    public string WebpFilepath     { get; set; }
    //public string WebpAnalysisFilepath     { get; set; }

    public bool   EleFileExists    { get; set; } = false;
    public bool   EleArrFileExists { get; set; } = false;
    public bool   MeshFileExists   { get; set; } = false;
    public bool   ImageFileExists  { get; set; } = false;
    public bool   WebpFileExists   { get; set; } = false;

    public static readonly string[] PathPerLvl = { "Lvl0_30x30", "Lvl1_5x5", "Lvl2_1x1", "Lvl3_0p2x0p2", "Lvl4_0p04x0p04", "Lvl5_0p008x0p008"};

    public GloMapTileFilepaths(GloMapTileCode tileCode)
    {
        // Get the tile code name
        string tileCodeName = tileCode.ToString();

        // Setup the path to the map level directory
        var config = GloCentralConfig.Instance;
        string mapRoot = config.GetParam<string>("MapRootPath");

        string externalRootPath       = mapRoot;
        string externalMapLvlFilePath = GloFileOperations.JoinPaths(externalRootPath, PathPerLvl[tileCode.MapLvl]);
        if (tileCode.MapLvl != 0)
            externalMapLvlFilePath = GloFileOperations.JoinPaths(externalMapLvlFilePath, tileCode.ParentString());

        // Setup the file paths
        EleFilepath      = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.asc");
        EleArrFilepath   = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Ele_{tileCodeName}.arr");
        MeshFilepath     = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Mesh_{tileCodeName}.mesh");
        ImageFilepath    = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.png");
        WebpFilepath     = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}.webp");
        //WebpAnalysisFilepath     = GloFileOperations.JoinPaths(externalMapLvlFilePath, $"Sat_{tileCodeName}_check.webp");

        // Check if the files exist
        EleFileExists    = File.Exists(EleFilepath);
        EleArrFileExists = File.Exists(EleArrFilepath);
        MeshFileExists   = File.Exists(MeshFilepath);
        ImageFileExists  = File.Exists(ImageFilepath);
        WebpFileExists   = File.Exists(WebpFilepath);
    }

    public void InheritImageFilepaths(GloMapTileFilepaths parentFilepaths)
    {
        ImageFilepath    = parentFilepaths.ImageFilepath;
        WebpFilepath     = parentFilepaths.WebpFilepath;

        ImageFileExists  = parentFilepaths.ImageFileExists;
        WebpFileExists   = parentFilepaths.WebpFileExists;
    }

    public static string WorldTileFilepath()
    {
        var config = GloCentralConfig.Instance;
        string mapRoot = config.GetParam<string>("MapRootPath");

        return GloFileOperations.JoinPaths(mapRoot, "WorldTile.webp");
    }

}


