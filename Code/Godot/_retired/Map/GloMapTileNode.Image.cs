// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;

// using Godot;

// #nullable enable

// // Note that map tile nodes always hang off the EarthCoreNode parent, they never use the ZeroPoint offset.

// public partial class GloMapTileNode : Node3D
// {

//     // --------------------------------------------------------------------------------------------
//     // MARK: Image Methods
//     // --------------------------------------------------------------------------------------------

//     private void LoadWebp()
//     {
//         GloTextureLoader? TL = GloTextureLoader.Instance;

//         TileMaterial = TL.CreateDirectWebpMaterial(Filepaths.WebpFilepath);

//         if (TileMaterial == null)
//         {
//             GloCentralLog.AddEntry($"Failed to create material: {TileCode.ToString()} // {Filepaths.WebpFilepath}");


//         }
//         else
//         {
//             ImageDone = true;
//         }

//         // Setup the UV Box - new image, so a full 0,0 -> 1,1 range
//         UVBox = new GloUVBoxDropEdgeTile(GloUVBoxDropEdgeTile.UVTopLeft, GloUVBoxDropEdgeTile.UVBottomRight);
//     }

//     private void SubsampleParentTileImage()
//     {
//         if (ParentTile != null)
//         {
//             if (ParentTile.TileMaterial != null)
//                 TileMaterial = ParentTile.TileMaterial;

//             // Setup the UV Box - Sourced from the parent (which may already be subsampled), we subsample for this tile's range
//             // Get the grid position of this tile in its parent (eg [1x,2y] in a 5x5 grid).
//             UVBox = new GloUVBoxDropEdgeTile(ParentTile.UVBox, TileCode.GridPos);
//         }
//     }

// }
