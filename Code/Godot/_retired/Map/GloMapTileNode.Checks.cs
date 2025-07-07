// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;

// using Godot;

// #nullable enable

// // Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

// public partial class GloMapTileNode : Node3D
// {
//     private bool DoChildTilesExist()
//     {
//         return ChildTiles.Count > 0;
//     }

//     // --------------------------------------------------------------------------------------------

//     // Get the list of child tile names, then loop through, finding these nodes, and querying their IsDone property
//     private bool AreChildTilesLoaded()
//     {
//         // return false if there are no child tiles
//         if (ChildTiles.Count == 0)
//             return false;

//         // Loop through the list of child node names, and query the IsDone property of each node.
//         foreach (GloMapTileNode currTile in ChildTiles)
//         {
//             // Query the IsDone property of the node
//             if (!currTile.ConstructionComplete)
//                 return false;
//         }

//         // Return true, we've not found any false criteria in the search
//         return true;
//     }

//     // --------------------------------------------------------------------------------------------

//     private async void DetermineChildTileAvailability(GloMapTileCode tileCode)
//     {
//         // Pause the thread, being a good citizen with lots of tasks around.
//         await Task.Yield();

//         ChildTileDataAvailable = false;

//         // loop through all the child tiles for the input tile code, and ensure that they are all available

//         // Compile the list of child node names.
//         List<GloMapTileCode> childTileCodes = TileCode.ChildCodesList();

//         foreach (GloMapTileCode currChildCode in childTileCodes)
//         {
//             // Get the filenames for each child tile
//             GloMapTileFilepaths currChildFilepaths = new GloMapTileFilepaths(currChildCode); // Figure out the file paths for the tile

//             // Pause the thread, being a good citizen with lots of tasks around.
//             await Task.Yield();

//             // Check if the files exist
//             if (!currChildFilepaths.EleArrFileExists)
//             {
//                 ChildTileDataAvailable = false;
//                 return;
//             }
//         }

//         // No failure to find the elevation tile
//         ChildTileDataAvailable = true;
//     }

//     // --------------------------------------------------------------------------------------------

//     private void DetermineChildConstructionComplete()
//     {
//         if (ChildConstructionComplete)
//             return;

//         // loop through all the child tiles for the input tile code, and ensure that they are all available
//         if (ChildTiles.Count == 0)
//         {
//             ChildConstructionComplete = false;
//             return;
//         }

//         foreach (GloMapTileNode currChildNode in ChildTiles)
//         {
//             // Check if the files exist
//             if (!currChildNode.ConstructionComplete)
//             {
//                 ChildConstructionComplete = false;
//                 return;
//             }
//         }

//         // No failure to find the elevation tile
//         ChildConstructionComplete = true;
//     }

// }
