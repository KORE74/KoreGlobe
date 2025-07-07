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
//     // MARK: Elevation Methods
//     // --------------------------------------------------------------------------------------------

//     private void LoadTileEle()
//     {
//         int resX = TileSizePointsPerLvl[TileCode.MapLvl];
//         int resY = TileSizePointsPerLvl[TileCode.MapLvl];

//         GloLLPoint tileCenter = TileCode.LLBox.CenterPoint;
//         int tileResLat = TileSizePointsPerLvl[TileCode.MapLvl];
//         int tileResLon = GloElevationUtils.LonResForLat(tileResLat, tileCenter.LatRads);

//         GloFloat2DArray asciiArcArray = GloElevationPatchIO.LoadFromArcASIIGridFile(Filepaths.EleFilepath);

//         // If the max val is less than 0, create a default 10x10 flat tile
//         if (asciiArcArray.MaxVal() <= 0.5)
//         {
//             TileEleData = new GloFloat2DArray(10, 10);
//         }
//         else
//         {
//             GloFloat2DArray croppedArray  = GloFloat2DArrayOperations.CropToRange(asciiArcArray, new GloFloatRange(0f, 50000f));

//             //CreateDefaultChildEleData(croppedArray);
//             //GloFloat2DArray croppedArraySubSample = croppedArray.GetInterpolatedGrid(resX, resY);

//             TileEleData = croppedArray.GetInterpolatedGrid(tileResLon, tileResLat);
//         }
//     }

//     // --------------------------------------------------------------------------------------------

//     private void LoadTileEleArr()
//     {
//         GloElevationTile? eleTile = GloElevationTileIO.ReadFromTextFile(Filepaths.EleArrFilepath);

//         if (eleTile != null)
//         {
//             TileEleData = eleTile.ElevationData;

//             // if we have a subsurface tile, set it to a lower resolution
//             if (TileEleData.MaxVal() <= 0)
//                 TileEleData = new GloFloat2DArray(10, 10);

//             // Write the simplified data back to the file, to be faster next time.
//             eleTile.ElevationData = TileEleData;
//             GloElevationTileIO.WriteToTextFile(eleTile, Filepaths.EleArrFilepath);
//         }
//         else
//         {
//             GloCentralLog.AddEntry($"Failed to load: {Filepaths.EleArrFilepath}");
//         }

//         TileEleData = GloFloat2DArrayOperations.CropToRange(TileEleData, new GloFloatRange(0f, 10000f));
//     }

//     // --------------------------------------------------------------------------------------------

//     private void SubsampleParentTileEle()
//     {
//         GloLLPoint tileCenter = TileCode.LLBox.CenterPoint;
//         int tileResLat = TileSizePointsPerLvl[TileCode.MapLvl];
//         int tileResLon = GloElevationUtils.LonResForLat(tileResLat, tileCenter.LatRads);

//         // Use the latitude resolution and latitude to figure out a longitude resolution the gives the "most square" tiles possible.
//         // int tileResLon = GloElevationUtils.LonResForLat(tileResLat, lLBox.CenterPoint.LatDegs);

//         if (ParentTile != null)
//         {
//             Glo2DGridPos tileGridPos = TileCode.GridPos;

//             // Copy the parent's elevation data - regardless of its resolution, to pass that along to the child tiles
//             //GloFloat2DArray RawParentTileEleData = ParentTile.ChildEleData[tileGridPos.PosX, tileGridPos.PosY];

//             TileEleData = ParentTile.TileEleData.GetInterpolatedSubgrid(tileGridPos, tileResLon, tileResLat);
//         }
//         else
//         {
//             TileEleData = new GloFloat2DArray(tileResLon, tileResLat);
//         }

//         // Shouldn't need either of these two clauses, but to protect us against over-interpolating of edge cases.
//         if (TileEleData == null)
//         {
//             GloCentralLog.AddEntry($"SubsampleParentTileEle: Parent Tile Failure: {TileCode.ToString()}");
//         }
//         else if (TileEleData.Width < 10 || TileEleData.Height < 10)
//         {
//             GloCentralLog.AddEntry($"SubsampleParentTileEle: Parent Tile Failure: {TileCode.ToString()} {TileEleData.Width}x{TileEleData.Height}");
//             TileEleData = new GloFloat2DArray(10, 10);
//         }
//     }

//     // --------------------------------------------------------------------------------------------
//     // MARK: Ele
//     // --------------------------------------------------------------------------------------------

//     public float GetLocalElevation(GloLLPoint pos)
//     {
//         GloLLBox llBounds = TileCode.LLBox;
//         if (!llBounds.Contains(pos))
//             return GloElevationUtils.InvalidEle;

//         (float latFrac, float lonFrac) = llBounds.GetLatLonFraction(pos);

//         return TileEleData.InterpolatedValue(latFrac, lonFrac);
//     }

//     public float GetElevation(GloLLPoint pos)
//     {
//         GloLLBox llBounds = TileCode.LLBox;
//         if (!llBounds.Contains(pos))
//             return GloElevationUtils.InvalidEle;

//         // If we have child tiles all constructed, the look one up and return its elevation
//         if (false)
//         {

//         }

//         return GetLocalElevation(pos);
//     }

// }
