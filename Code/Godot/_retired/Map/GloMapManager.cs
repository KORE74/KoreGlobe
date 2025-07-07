// using Godot;
// using System;
// using System.IO;
// using System.Threading.Tasks;
// using System.Text;
// using System.Collections.Generic;
// using System.Collections.Concurrent;

// #nullable enable

// // Usage: GloTileInfo newTileInfo = new GloTileInfo() { TileCodeStruct = tilecodestr, TileLLBounds = llbox };

// // public struct GloTileInfo
// // {
// //     public GloMapTileCode TileCodeStruct;
// //     public GloLLBox TileLLBounds;
// // }

// public partial class GloMapManager : Node3D
// {
//     // Common map path point (save the config being queried excessively) // GloMapManager.MapRootPath
//     public static string MapRootPath = "";

//     // Load reference point - The position against which map tile load operations are judged
//     // GloMapManager.LoadRefLLA
//     public static GloLLAPoint LoadRefLLA = new GloLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
//     public static GloXYZPoint LoadRefXYZ => LoadRefLLA.ToXYZ();
//     public static bool ShowDebug = false; // GloMapManager.ShowDebug
//     public static int CurrMaxMapLvl = 1;

//     // Usage: GloMapManager.AllTileList.AddTile();
//     //        GloMapManager.AllTileList.RemoveTile();
//     //        GloMapManager.AllTileList.AllTilesList();
//     // public static GloMapTileList AllTileList = new ();

//     // Debug
//     private GloLLAPoint pos  = new GloLLAPoint() { LatDegs = 41, LonDegs = 6, AltMslM = 0 };
//     private GloCourse Course = new GloCourse()   { HeadingDegs = 90, SpeedMps = 1 };
//     // Node3D ModelNode         = null;
//     // Node3D ModelResourceNode = null;

//     // --------------------------------------------------------------------------------------------
//     // MARK: Node Functions
//     // --------------------------------------------------------------------------------------------

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         // Setup this node
//         Name = "MapManager";

//         LoadConfig();
//         // CreateDebugMarker();
//         // CreateLvl0Tiles();
//     }

//     // Called every frame. 'delta' is the elapsed time since the previous frame.
//     public override void _Process(double delta)
//     {
//         // Update this GE position, to keep up with the zero node movement


//     }



//     // --------------------------------------------------------------------------------------------
//     // MARK: Config Load
//     // --------------------------------------------------------------------------------------------

//     private static void LoadConfig()
//     {
//         // Read the debug flag from config
//         var config = GloCentralConfig.Instance;

//         // Read each of the params
//         ShowDebug     = config.GetParam<bool>("ShowDebug");
//         CurrMaxMapLvl = config.GetParam<int>("MaxMapLvl");
//         MapRootPath   = config.GetParam<string>("MapRootPath");

//         // Apply some validation to the max map level
//         CurrMaxMapLvl = GloValueUtils.Clamp(CurrMaxMapLvl, 0, GloMapTileCode.MaxMapLvl);

//         if (!Directory.Exists(GloMapManager.MapRootPath))
//            GloCentralLog.AddEntry("$GloMapManager.MapRootPath not found: {GloMapManager.MapRootPath}");
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Config Update
//     // --------------------------------------------------------------------------------------------

//     public static void SetDebug(bool debug)
//     {
//         ShowDebug = debug;

//         // Save the debug flag to config
//         var config = GloCentralConfig.Instance;
//         config.SetParam("ShowDebug", ShowDebug);
//     }

//     public static void SetMaxMapLvl(int maxMapLvl)
//     {
//         CurrMaxMapLvl = GloValueUtils.Clamp(maxMapLvl, 0, GloMapTileCode.MaxMapLvl);

//         // Save the max map level to config
//         var config = GloCentralConfig.Instance;
//         config.SetParam("MaxMapLvl", CurrMaxMapLvl);
//     }


//     // --------------------------------------------------------------------------------------------
//     // MARK: Tile Access
//     // --------------------------------------------------------------------------------------------

//     private void CreateLvl0Tiles()
//     {
//         GD.Print("MapManager: Creating Lvl0 Tiles");

//         // Create the level 0 tiles
//         for (int lonId = 0; lonId < 12; lonId++)
//         {
//             for (int latId = 0; latId < 6; latId++)
//             {
//                 GloMapTileCode tileCode = new GloMapTileCode(lonId, latId);

//                 //if (tileCode.TileCode != "BF")
//                 //{
//                     GloMapTileNode newChildTile = new GloMapTileNode(tileCode);
//                     AddChild(newChildTile);
//                 //}
//             }
//         }
//     }

//     private GloMapTileNode? GetLvl0Tile(GloMapTileCode tileCode)
//     {
//         // Get the name of the first lvl0
//         string tileName = tileCode.ToString();

//         GloMapTileCode? lvl0Code = GloMapTileCode.CodeToLvl(tileCode, 0);
//         if (lvl0Code == null) return null;

//         // Get the tile node for this tile code
//         return (GloMapTileNode)FindChild(lvl0Code.ToString());
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Elevation
//     // --------------------------------------------------------------------------------------------

//     // Get the elevation at a given LL point, as loaded by the map tiles.

//     public float GetElevation(GloLLPoint llPoint)
//     {
//         GloMapTileCode? tileCode = new GloMapTileCode(llPoint.LatDegs, llPoint.LonDegs, 0);

//         GloMapTileNode? lvl0Tile = GetLvl0Tile(tileCode);

//         if (lvl0Tile == null) return GloElevationUtils.InvalidEle;

//         return lvl0Tile.GetElevation(llPoint);
//     }


//     // --------------------------------------------------------------------------------------------
//     // MARK: Report
//     // --------------------------------------------------------------------------------------------

//     public string Report()
//     {
//         StringBuilder sb = new StringBuilder();

//         // List<GloMapTileNode> tList = GloMapManager.AllTileList.AllTilesList();

//         // float eleCount = 0f;
//         // // foreach (GloMapTileNode t in tList)
//         // // {
//         // //     eleCount += t.EleValCount;
//         // // }

//         // sb.AppendLine($"Map Manager Report: {eleCount:F2}");

//         return sb.ToString();
//     }

//     // private void CreateDebugMarker()
//     // {
//     //     float markerSize = 0.2f;

//     //     Node3D zeroNodeMarker = GloPrimitiveFactory.CreateSphereNode("Marker", new Vector3(0f, 0f, 0f), markerSize, GloColorUtil.Colors["Blue"], true);
//     //     AddChild(zeroNodeMarker);

//     //     zeroNodeMarker.AddChild( GloPrimitiveFactory.AxisMarkerNode(markerSize, markerSize/4) );
//     // }

// }
